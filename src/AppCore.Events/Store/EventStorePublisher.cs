// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.DependencyInjection;
using AppCore.Diagnostics;
using AppCore.Events.Pipeline;

namespace AppCore.Events.Store
{
    /// <inheritdoc />
    public class EventStorePublisher : IEventStorePublisher
    {
        private readonly IEventStore _store;
        private readonly IEventStorePublisherOffset _storeOffset;
        private readonly EventPipelineResolver _pipelineResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStorePublisher"/> class.
        /// </summary>
        /// <param name="store">The event store to use.</param>
        /// <param name="storeOffset">Used to load/save the current event offset.</param>
        /// <param name="container">The <see cref="IContainer"/> used to resolve <see cref="IEventPipeline"/>'s.</param>
        public EventStorePublisher(IEventStore store, IEventStorePublisherOffset storeOffset, IContainer container)
        {
            Ensure.Arg.NotNull(store, nameof(store));
            Ensure.Arg.NotNull(storeOffset, nameof(storeOffset));
            Ensure.Arg.NotNull(container, nameof(container));

            _store = store;
            _storeOffset = storeOffset;
            _pipelineResolver = new EventPipelineResolver(container);
        }

        private IEventPipeline ResolvePipeline(IEventContext eventContext)
        {
            Type eventType = eventContext.EventDescriptor.EventType;
            return _pipelineResolver.Resolve(eventType);
        }

        /// <inheritdoc />
        public async Task PublishPendingAsync(CancellationToken cancellationToken)
        {
            string streamName = string.Empty;

            EventOffset offset = await _storeOffset.GetNextOffset(cancellationToken)
                                                   .ConfigureAwait(false);

            IEnumerable<IEventContext> events = await _store.ReadAsync(
                streamName,
                offset,
                64,
                Timeout.InfiniteTimeSpan,
                cancellationToken);

            long lastOffset = -1;
            foreach (IEventContext eventContext in events)
            {
                IEventPipeline pipeline = ResolvePipeline(eventContext);
                await pipeline.PublishAsync(eventContext, cancellationToken)
                              .ConfigureAwait(false);

                lastOffset = eventContext.GetEventStoreOffset();
            }

            if (lastOffset != -1)
            {
                if (_store is ICommittableEventStore committableStore)
                {
                    await committableStore.CommitAsync(streamName, lastOffset, cancellationToken)
                                          .ConfigureAwait(false);
                }

                await _storeOffset.CommitOffset(lastOffset, cancellationToken)
                                  .ConfigureAwait(false);
            }
        }
    }
}
