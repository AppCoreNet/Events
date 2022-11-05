// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppCore.EventModel.Queue;

/// <summary>
/// The <see cref="BackgroundService"/> used to consume and publish queued events.
/// </summary>
public class EventQueueConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IOptionsMonitor<EventQueueOptions> _optionsMonitor;
    private readonly ILogger<EventQueueConsumerService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventQueueConsumerService"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory">The <see cref="IServiceScopeFactory"/>.</param>
    /// <param name="optionsMonitor">The <see cref="IOptionsMonitor{TOptions}"/> of <see cref="EventQueueOptions"/>.</param>
    /// <param name="logger">The logger.</param>
    public EventQueueConsumerService(
        IServiceScopeFactory serviceScopeFactory,
        IOptionsMonitor<EventQueueOptions> optionsMonitor,
        ILogger<EventQueueConsumerService> logger)
    {
        Ensure.Arg.NotNull(serviceScopeFactory);
        Ensure.Arg.NotNull(logger);

        _serviceScopeFactory = serviceScopeFactory;
        _optionsMonitor = optionsMonitor;
        _logger = logger;
    }

    /// <summary>
    /// Publishes events until canceled.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await PublishAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Publishes pending events.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual async Task PublishAsync(CancellationToken cancellationToken)
    {
        EventQueueOptions options = _optionsMonitor.CurrentValue;

        try
        {
            using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
            IServiceProvider serviceProvider = serviceScope.ServiceProvider;
            var publisher = serviceProvider.GetRequiredService<EventQueueConsumer>();
            await publisher.PublishPendingAsync(cancellationToken)
                           .ConfigureAwait(false);
        }
        catch (TaskCanceledException) {}
        catch (OperationCanceledException) {}
        catch (Exception error)
        {
            _logger.PublishingQueuedEventsFailed(error);
            await Task.Delay(options.RetryDelay, cancellationToken);
        }
    }
}