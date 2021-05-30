// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using AppCore.EventModel.Pipeline;
using AppCore.Logging;

namespace AppCore.EventModel.Store
{
    /// <inheritdoc />
    public class EventStorePublisher : IEventStorePublisher
    {
        private readonly IEventStore _store;
        private readonly IEventStorePublisherOffset _storeOffset;
        private readonly ILogger<EventStorePublisher> _logger;
        private readonly IEventPipelineResolver _pipelineResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStorePublisher"/> class.
        /// </summary>
        /// <param name="store">The event store to use.</param>
        /// <param name="storeOffset">Used to load/save the current event offset.</param>
        /// <param name="pipelineResolver">The <see cref="IEventPipelineResolver"/> used to resolve <see cref="IEventPipeline"/>'s.</param>
        /// <param name="logger">The <see cref="ILogger{TCategory}"/>.</param>
        public EventStorePublisher(
            IEventStore store,
            IEventStorePublisherOffset storeOffset,
            IEventPipelineResolver pipelineResolver,
            ILogger<EventStorePublisher> logger)
        {
            Ensure.Arg.NotNull(store, nameof(store));
            Ensure.Arg.NotNull(storeOffset, nameof(storeOffset));
            Ensure.Arg.NotNull(pipelineResolver, nameof(pipelineResolver));
            Ensure.Arg.NotNull(logger, nameof(logger));

            _store = store;
            _storeOffset = storeOffset;
            _logger = logger;
            _pipelineResolver = pipelineResolver;
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

            _logger.ReadingEvents(streamName, offset.Value);

            IReadOnlyCollection<IEventContext> events = await _store.ReadAsync(
                streamName,
                offset,
                64,
                Timeout.InfiniteTimeSpan,
                cancellationToken);

            if (events.Count > 0)
            {
                long firstOffset = events.First().GetEventStoreOffset();
                long lastOffset = events.Last().GetEventStoreOffset();

                _logger.PublishingEvents(events.Count, streamName, firstOffset, lastOffset);

                foreach (IEventContext eventContext in events)
                {
                    IEventPipeline pipeline = ResolvePipeline(eventContext);
                    await pipeline.ProcessAsync(eventContext, cancellationToken)
                                  .ConfigureAwait(false);
                }

                if (_store is ICommittableEventStore committableStore)
                {
                    await committableStore.CommitAsync(streamName, lastOffset, cancellationToken)
                                          .ConfigureAwait(false);
                }

                await _storeOffset.CommitOffset(lastOffset, cancellationToken)
                                  .ConfigureAwait(false);

                _logger.PublishedEvents(events.Count, streamName);
            }
        }
    }
}
