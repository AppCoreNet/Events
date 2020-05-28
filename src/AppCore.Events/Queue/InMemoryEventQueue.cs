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
    public class InMemoryEventQueue
    {
        private const int DefaultCapacity = 1024;
        private const int DefaultMaxEventsToRead = 64;

        private readonly Channel<IEventContext> _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryEventQueue"/> class.
        /// </summary>
        public InMemoryEventQueue()
        {
            _channel = Channel.CreateBounded<IEventContext>(new BoundedChannelOptions(DefaultCapacity));
        }

        /// <summary>
        /// Reads events from the queue.
        /// </summary>
        /// <param name="timeout">The timeout for the read operation.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task<IReadOnlyCollection<IEventContext>> ReadAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var result = new List<IEventContext>();

            using var cts = new CancellationTokenSource(timeout);
            using var cts2 = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

            ChannelReader<IEventContext> reader = _channel.Reader;
            bool eventAvailable = await reader.WaitToReadAsync(cts2.Token)
                                              .ConfigureAwait(false);

            if (!eventAvailable)
                return result;

            while (result.Count < DefaultMaxEventsToRead
                   && reader.TryRead(out IEventContext @event))
            {
                result.Add(@event);
            }

            return result;
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
