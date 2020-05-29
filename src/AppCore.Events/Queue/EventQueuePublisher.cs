// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using AppCore.Events.Pipeline;
using AppCore.Logging;

namespace AppCore.Events.Queue
{
    /// <summary>
    /// Publishes events by reading from the queue and processing the pipeline.
    /// </summary>
    public class EventQueuePublisher
    {
        private readonly IEventQueue _queue;
        private readonly IEventPipelineResolver _pipelineResolver;
        private readonly ILogger<EventQueuePublisher> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventQueuePublisher"/> class.
        /// </summary>
        /// <param name="queue">The event queue.</param>
        /// <param name="pipelineResolver">The event pipeline resolver.</param>
        /// <param name="logger">The logger.</param>
        public EventQueuePublisher(
            IEventQueue queue,
            IEventPipelineResolver pipelineResolver,
            ILogger<EventQueuePublisher> logger)
        {
            Ensure.Arg.NotNull(queue, nameof(queue));
            Ensure.Arg.NotNull(pipelineResolver, nameof(pipelineResolver));
            Ensure.Arg.NotNull(logger, nameof(logger));

            _queue = queue;
            _pipelineResolver = pipelineResolver;
            _logger = logger;
        }

        /// <summary>
        /// Publishes queued events.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task PublishPendingAsync(CancellationToken cancellationToken)
        {
            _logger.DequeuingEvents();

            await _queue
                  .ReadAsync(
                      async (events, ct) =>
                      {
                          _logger.PublishingEvents(events.Count);

                          foreach (IEventContext @event in events)
                          {
                              IEventPipeline pipeline = _pipelineResolver.Resolve(@event.EventDescriptor.EventType);
                              await pipeline.ProcessAsync(@event, cancellationToken)
                                            .ConfigureAwait(false);
                          }

                          _logger.PublishedEvents(events.Count);
                      },
                      new EventQueueReadOptions(),
                      cancellationToken)
                  .ConfigureAwait(false);
        }
    }
}
