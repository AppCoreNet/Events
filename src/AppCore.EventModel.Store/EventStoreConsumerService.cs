// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppCore.EventModel.Store;

/// <summary>
/// Represents a <see cref="BackgroundService"/> which consumers events from the store and publishes them to the
/// event pipeline.
/// </summary>
public class EventStoreConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventStoreConsumerService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreConsumerService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to resolve the <see cref="IEventStoreConsumer"/>.</param>
    /// <param name="logger">The <see cref="ILogger{TCategory}"/>.</param>
    public EventStoreConsumerService(IServiceProvider serviceProvider, ILogger<EventStoreConsumerService> logger)
    {
        Ensure.Arg.NotNull(serviceProvider);
        Ensure.Arg.NotNull(logger);

        _serviceProvider = serviceProvider;
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
        _logger.PublishingStoredEvents();

        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IServiceProvider serviceProvider = scope.ServiceProvider;
            var publisher = serviceProvider.GetRequiredService<IEventStoreConsumer>();
            await publisher.PublishPendingAsync(cancellationToken)
                           .ConfigureAwait(false);
        }
        catch (TaskCanceledException) {}
        catch (OperationCanceledException) {}
        catch (Exception error)
        {
            _logger.PublishingStoredEventsFailed(error);
        }
    }
}