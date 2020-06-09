// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppCore.Diagnostics;

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

        /// <inheritdoc />
        public async Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken)
        {
            Ensure.Arg.NotNull(events, nameof(events));

            ChannelWriter<IEventContext> writer = _channel.Writer;
            foreach (IEventContext @event in events)
            {
                await writer.WriteAsync(@event, cancellationToken)
                            .ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<IEventContext>> ReadAsync(int maxEventsToRead, CancellationToken cancellationToken)
        {
            Ensure.Arg.InRange(maxEventsToRead, 1, int.MaxValue, nameof(maxEventsToRead));

            ChannelReader<IEventContext> reader = _channel.Reader;
            bool eventAvailable = await reader.WaitToReadAsync(cancellationToken)
                                              .ConfigureAwait(false);

            var result = new List<IEventContext>();
            if (eventAvailable)
            {
                while (result.Count < maxEventsToRead
                       && reader.TryRead(out IEventContext @event))
                {
                    result.Add(@event);
                }
            }

            return result.AsReadOnly();
        }

        /// <inheritdoc />
        public Task CommitReadAsync(IEventContext @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
