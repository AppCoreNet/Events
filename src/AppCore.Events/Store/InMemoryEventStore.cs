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
    public class InMemoryEventStore : IEventStore
    {
        private static readonly Task _completedTask = Task.FromResult(true);

        private readonly IEventContextFactory _contextFactory;

        private readonly ConcurrentDictionary<string, EventDataStream> _streams =
            new ConcurrentDictionary<string, EventDataStream>();

        private class EventData
        {
            public int SequenceNo { get; }

            public EventDescriptor Descriptor { get; }

            public IEvent Event { get; }

            public EventData(int sequenceNo, EventDescriptor descriptor, IEvent @event)
            {
                SequenceNo = sequenceNo;
                Descriptor = descriptor;
                Event = @event;
            }
        }

        private class EventDataStream
        {
            private readonly IEventContextFactory _contextFactory;
            private readonly object _syncObject = new object();
            private readonly List<EventData> _events = new List<EventData>();
            private readonly Queue<TaskCompletionSource<bool>> _taskCompletionSources =
                new Queue<TaskCompletionSource<bool>>();
            private int _nextSequenceNo;
            private readonly EventStoreFeature _eventStoreFeature;

            public EventDataStream(IEventStore store, IEventContextFactory contextFactory)
            {
                _contextFactory = contextFactory;
                _eventStoreFeature = new EventStoreFeature(store, true);
            }

            public Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken)
            {
                lock (_syncObject)
                {
                    _events.AddRange(events.Select(e => new EventData(_nextSequenceNo++, e.EventDescriptor, e.Event)));

                    // notify all readers
                    TaskCompletionSource<bool>[] sources = _taskCompletionSources.ToArray(); 
                    _taskCompletionSources.Clear();
                    foreach (TaskCompletionSource<bool> source in sources)
                        source.TrySetResult(true);
                }

                return _completedTask;
            }

            public async Task<IEnumerable<IEventContext>> ReadAsync(long startSequenceNo, int maxCount, TimeSpan timeout, CancellationToken cancellationToken)
            {
                CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(timeout);

                do
                {
                    TaskCompletionSource<bool> taskCompletionSource;
                    lock (_syncObject)
                    {
                        EventData lastEvent = _events.LastOrDefault();
                        if (lastEvent?.SequenceNo >= startSequenceNo)
                        {
                            EventData firstEvent = _events.First();
                            int startIndex = (int) startSequenceNo - firstEvent.SequenceNo;
                            int count = Math.Min(maxCount, _events.Count - startIndex);

                            var result = new List<IEventContext>(count);
                            while (count-- > 0)
                            {
                                EventData currentEvent = _events[startIndex++];
                                IEventContext currentEventContext = _contextFactory.CreateContext(
                                    currentEvent.Descriptor,
                                    currentEvent.Event);

                                currentEventContext.AddFeature<IEventStoreFeature>(_eventStoreFeature);
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

        private EventDataStream GetStream(string topic)
        {
            if (topic == null)
                topic = string.Empty;

            return _streams.GetOrAdd(topic, t => new EventDataStream(this, _contextFactory));
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
            long startSequenceNo,
            int maxCount,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            Ensure.Arg.InRange(startSequenceNo, 0, long.MaxValue, nameof(startSequenceNo));
            Ensure.Arg.InRange(maxCount, 0, int.MaxValue, nameof(maxCount));

            EventDataStream queue = GetStream(streamName);

            return await queue.ReadAsync(startSequenceNo, maxCount, timeout, cancellationToken)
                              .ConfigureAwait(false);
        }
    }
}