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
        private readonly EventQueuePublisher _publisher;
        private readonly ILogger<EventQueuePublisherService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventQueuePublisherService"/> class.
        /// </summary>
        /// <param name="publisher">The event queue publisher.</param>
        /// <param name="logger">The logger.</param>
        public EventQueuePublisherService(EventQueuePublisher publisher, ILogger<EventQueuePublisherService> logger)
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
            try
            {
                await _publisher.PublishPendingAsync(cancellationToken)
                                .ConfigureAwait(false);
            }
            catch (TaskCanceledException) {}
            catch (OperationCanceledException) {}
            catch (Exception error)
            {
                _logger.PublishingQueuedEventsFailed(error);
            }
        }

        /// <summary>
        /// The scoped <see cref="BackgroundService"/> used to publish queued events.
        /// </summary>
        public class Scoped : BackgroundService
        {
            private readonly IServiceProvider _serviceProvider;

            /// <summary>
            /// Initializes a new instance of the <see cref="BackgroundService"/> class.
            /// </summary>
            /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to resolve an <see cref="EventQueuePublisher"/>.</param>
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
                    using IServiceScope serviceScope = _serviceProvider.CreateScope();
                    IServiceProvider serviceProvider = serviceScope.ServiceProvider;

                    using var publisher = new EventQueuePublisherService(
                        serviceProvider.GetRequiredService<EventQueuePublisher>(),
                        serviceProvider.GetRequiredService<ILogger<EventQueuePublisherService>>());

                    await publisher.PublishAsync(cancellationToken);
                }
            }
        }
    }
}