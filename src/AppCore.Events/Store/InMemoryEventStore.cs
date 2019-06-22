// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;

namespace AppCore.Events.Store
{
    /// <summary>
    /// Provides an event store which stores events in-memory.
    /// </summary>
    public class InMemoryEventStore : ICommittableEventStore
    {
        private static readonly Task _completedTask = Task.FromResult(true);

        private readonly IEventContextFactory _contextFactory;

        private readonly ConcurrentDictionary<string, EventDataStream> _streams =
            new ConcurrentDictionary<string, EventDataStream>();

        private class EventData
        {
            public int Offset { get; }

            public EventDescriptor Descriptor { get; }

            public IEvent Event { get; }

            public EventData(int offset, EventDescriptor descriptor, IEvent @event)
            {
                Offset = offset;
                Descriptor = descriptor;
                Event = @event;
            }
        }

        private class EventDataStream
        {
            private readonly IEventStore _store;
            private readonly IEventContextFactory _contextFactory;
            private readonly object _syncObject = new object();
            private readonly List<EventData> _events = new List<EventData>();
            private readonly Queue<TaskCompletionSource<bool>> _taskCompletionSources =
                new Queue<TaskCompletionSource<bool>>();
            private int _nextOffset;
            private int _committedOffset = -1;

            public EventDataStream(IEventStore store, IEventContextFactory contextFactory)
            {
                _store = store;
                _contextFactory = contextFactory;
            }

            public Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken)
            {
                lock (_syncObject)
                {
                    _events.AddRange(events.Select(e => new EventData(_nextOffset++, e.EventDescriptor, e.Event)));

                    // notify all readers
                    TaskCompletionSource<bool>[] sources = _taskCompletionSources.ToArray(); 
                    _taskCompletionSources.Clear();
                    foreach (TaskCompletionSource<bool> source in sources)
                        source.TrySetResult(true);
                }

                return _completedTask;
            }

            public async Task<IEnumerable<IEventContext>> ReadAsync(long offset, int maxCount, TimeSpan timeout, CancellationToken cancellationToken)
            {
                CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(timeout);

                do
                {
                    TaskCompletionSource<bool> taskCompletionSource;
                    lock (_syncObject)
                    {
                        if (offset == -2)
                            offset = _committedOffset + 1;

                        EventData lastEvent = _events.LastOrDefault();
                        if (lastEvent != null && (offset == -1 || lastEvent.Offset >= offset))
                        {
                            EventData firstEvent = _events.First();
                            if (offset == -1)
                                offset = firstEvent.Offset;

                            int startIndex = (int) offset - firstEvent.Offset;
                            int count = Math.Min(maxCount, _events.Count - startIndex);

                            var result = new List<IEventContext>(count);
                            while (count-- > 0)
                            {
                                EventData currentEvent = _events[startIndex++];
                                IEventContext currentEventContext = _contextFactory.CreateContext(
                                    currentEvent.Descriptor,
                                    currentEvent.Event);

                                currentEventContext.AddFeature<IEventStoreFeature>(
                                    new EventStoreFeature(_store, currentEvent.Offset));

                                result.Add(currentEventContext);
                            }

                            return result;
                        }

                        taskCompletionSource = new TaskCompletionSource<bool>();
                        _taskCompletionSources.Enqueue(taskCompletionSource);
                    }

                    // wait for next events to appear or elapsed timeout
                    using (cts.Token.Register(() => taskCompletionSource.TrySetResult(false)))
                    {
                        if (!await taskCompletionSource.Task.ConfigureAwait(false))
                            return Enumerable.Empty<IEventContext>();
                    }

                } while (true);
            }

            public Task CommitAsync(long offset, CancellationToken cancellationToken)
            {
                lock (_syncObject)
                {
                    _committedOffset = (int) offset;
                }

                return _completedTask;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryEventStore"/> class.
        /// </summary>
        /// <param name="contextFactory">The factory used to create instances of <see cref="IEventContext"/>.</param>
        public InMemoryEventStore(IEventContextFactory contextFactory)
        {
            Ensure.Arg.NotNull(contextFactory, nameof(contextFactory));
            _contextFactory = contextFactory;
        }

        private EventDataStream GetStream(string streamName)
        {
            if (streamName == null)
                streamName = string.Empty;

            return _streams.GetOrAdd(streamName, t => new EventDataStream(this, _contextFactory));
        }

        /// <inheritdoc />
        public async Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken)
        {
            Ensure.Arg.NotNull(events, nameof(events));

            var eventsByStream =
                events.GroupBy(
                          e => e.EventDescriptor.GetMetadata<string>(
                              EventStoreMetadataKeys.StreamNameMetadataKey,
                              null))
                      .Select(
                          group => new
                          {
                              Stream = GetStream(group.Key),
                              Events = (IEnumerable<IEventContext>) group
                          });

            foreach (var eventsToStream in eventsByStream)
            {
                await eventsToStream.Stream.WriteAsync(eventsToStream.Events, cancellationToken)
                                    .ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IEventContext>> ReadAsync(
            string streamName,
            long offset,
            int maxCount,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            Ensure.Arg.InRange(offset, -2, long.MaxValue, nameof(offset));
            Ensure.Arg.InRange(maxCount, 0, int.MaxValue, nameof(maxCount));

            EventDataStream queue = GetStream(streamName);
            return await queue.ReadAsync(offset, maxCount, timeout, cancellationToken)
                              .ConfigureAwait(false);
        }

        public async Task CommitAsync(string streamName, long offset, CancellationToken cancellationToken)
        {
            Ensure.Arg.InRange(offset, 0, long.MaxValue, nameof(offset));

            EventDataStream queue = GetStream(streamName);
            await queue.CommitAsync(offset, cancellationToken)
                       .ConfigureAwait(false);
        }
    }
}