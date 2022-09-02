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
/// Represents a <see cref="BackgroundService"/> which publishes events.
/// </summary>
public class EventStorePublisherService : BackgroundService
{
    private readonly IEventStorePublisher _publisher;
    private readonly ILogger<EventStorePublisherService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStorePublisherService"/> class.
    /// </summary>
    /// <param name="publisher">The <see cref="IEventStorePublisher"/> used to publish events.</param>
    /// <param name="logger">The <see cref="ILogger{TCategory}"/>.</param>
    public EventStorePublisherService(IEventStorePublisher publisher, ILogger<EventStorePublisherService> logger)
    {
        Ensure.Arg.NotNull(publisher, nameof(publisher));
        Ensure.Arg.NotNull(logger, nameof(logger));

        _publisher = publisher;
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
            await _publisher.PublishPendingAsync(cancellationToken)
                            .ConfigureAwait(false);
        }
        catch (TaskCanceledException) {}
        catch (OperationCanceledException) {}
        catch (Exception error)
        {
            _logger.PublishingStoredEventsFailed(error);
        }
    }

    /// <summary>
    /// Represents a <see cref="BackgroundService"/> which publishes events using a service scope.
    /// </summary>
    public class Scoped : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundService"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to resolve an <see cref="IEventStorePublisher"/>.</param>
        public Scoped(IServiceProvider serviceProvider)
        {
            Ensure.Arg.NotNull(serviceProvider, nameof(serviceProvider));
            _serviceProvider = serviceProvider;
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
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    IServiceProvider container = scope.ServiceProvider;

                    using (var publisherTask = new EventStorePublisherService(
                               container.GetRequiredService<IEventStorePublisher>(),
                               container.GetRequiredService<ILogger<EventStorePublisherService>>()))
                    {
                        await publisherTask.PublishAsync(cancellationToken);
                    }
                }
            }
        }
    }
}