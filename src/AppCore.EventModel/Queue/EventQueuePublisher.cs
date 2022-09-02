// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using AppCore.EventModel.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppCore.EventModel.Queue;

/// <summary>
/// Publishes events by reading from the queue and processing the pipeline.
/// </summary>
public class EventQueuePublisher
{
    private readonly IEventQueue _queue;
    private readonly IEventPipelineResolver _pipelineResolver;
    private readonly IOptionsMonitor<EventQueueOptions> _optionsMonitor;
    private readonly ILogger<EventQueuePublisher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventQueuePublisher"/> class.
    /// </summary>
    /// <param name="queue">The event queue.</param>
    /// <param name="pipelineResolver">The event pipeline resolver.</param>
    /// <param name="optionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> of <see cref="EventQueueOptions"/>.</param>
    /// <param name="logger">The logger.</param>
    public EventQueuePublisher(
        IEventQueue queue,
        IEventPipelineResolver pipelineResolver,
        IOptionsMonitor<EventQueueOptions> optionsMonitor,
        ILogger<EventQueuePublisher> logger)
    {
        Ensure.Arg.NotNull(queue);
        Ensure.Arg.NotNull(pipelineResolver);
        Ensure.Arg.NotNull(optionsMonitor);
        Ensure.Arg.NotNull(logger);

        _queue = queue;
        _pipelineResolver = pipelineResolver;
        _optionsMonitor = optionsMonitor;
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

        EventQueueOptions options = _optionsMonitor.CurrentValue;

        IReadOnlyCollection<IEventContext> events =
            await _queue.ReadAsync(options.BatchSize, cancellationToken)
                        .ConfigureAwait(false);

        _logger.PublishingEvents(events.Count);

        IEventContext? lastEvent = null;
        int eventCount = 0;

        try
        {
            foreach (IEventContext @event in events)
            {
                IEventPipeline pipeline = _pipelineResolver.Resolve(@event.EventDescriptor.EventType);
                await pipeline.ProcessAsync(@event, cancellationToken)
                              .ConfigureAwait(false);

                lastEvent = @event;
                ++eventCount;
            }
        }
        finally
        {
            _logger.PublishedEvents(eventCount);

            if (lastEvent != null)
            {
                await _queue.CommitReadAsync(lastEvent, cancellationToken)
                            .ConfigureAwait(false);
            }
        }
    }
}