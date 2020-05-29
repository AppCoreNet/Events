// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AppCore.Events.Queue
{
    /// <summary>
    /// Provides in-memory event queue.
    /// </summary>
    public class InMemoryEventQueue : IEventQueue
    {
        private const int DefaultCapacity = 1024;
        private readonly Channel<IEventContext> _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryEventQueue"/> class.
        /// </summary>
        public InMemoryEventQueue()
        {
            _channel = Channel.CreateBounded<IEventContext>(
                new BoundedChannelOptions(DefaultCapacity) {SingleReader = true});
        }

        /// <summary>
        /// Reads events from the queue.
        /// </summary>
        /// <param name="callback">The callback which is invoked when events become available.</param>
        /// <param name="options">The read options.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task ReadAsync(
            Func<IReadOnlyCollection<IEventContext>, CancellationToken, Task> callback,
            EventQueueReadOptions options,
            CancellationToken cancellationToken)
        {
            var result = new List<IEventContext>();

            using CancellationTokenSource cts = options.Timeout == Timeout.InfiniteTimeSpan
                ? new CancellationTokenSource()
                : new CancellationTokenSource(options.Timeout);

            using var cts2 = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

            ChannelReader<IEventContext> reader = _channel.Reader;
            bool eventAvailable = await reader.WaitToReadAsync(cts2.Token)
                                              .ConfigureAwait(false);

            if (!eventAvailable)
                return;

            while (result.Count < options.MaxEventsToRead
                   && reader.TryRead(out IEventContext @event))
            {
                result.Add(@event);
            }

            await callback(result, cancellationToken);
        }

        /// <summary>
        /// Writes events to the queue.
        /// </summary>
        /// <param name="events">The <see cref="IEnumerable{T}"/> of events to enqueue.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken)
        {
            ChannelWriter<IEventContext> writer = _channel.Writer;

            foreach (IEventContext @event in events)
            {
                await writer.WriteAsync(@event, cancellationToken)
                            .ConfigureAwait(false);
            }
        }
    }
}
