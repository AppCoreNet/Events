// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppCore.EventModel.Queue
{
    /// <summary>
    /// The <see cref="BackgroundService"/> used to publish queued events.
    /// </summary>
    public class EventQueuePublisherService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EventQueuePublisherService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventQueuePublisherService"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory">The <see cref="IServiceScopeFactory"/>.</param>
        /// <param name="logger">The logger.</param>
        public EventQueuePublisherService(IServiceScopeFactory serviceScopeFactory, ILogger<EventQueuePublisherService> logger)
        {
            Ensure.Arg.NotNull(serviceScopeFactory);
            Ensure.Arg.NotNull(logger);

            _serviceScopeFactory = serviceScopeFactory;
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
            try
            {
                using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
                IServiceProvider serviceProvider = serviceScope.ServiceProvider;
                var publisher = serviceProvider.GetRequiredService<EventQueuePublisher>();
                await publisher.PublishPendingAsync(cancellationToken)
                               .ConfigureAwait(false);
            }
            catch (TaskCanceledException) {}
            catch (OperationCanceledException) {}
            catch (Exception error)
            {
                _logger.PublishingQueuedEventsFailed(error);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}