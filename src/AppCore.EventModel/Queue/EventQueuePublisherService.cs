// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Threading;
using System.Threading.Tasks;
using AppCore.DependencyInjection;
using AppCore.Diagnostics;
using AppCore.Hosting;
using AppCore.Logging;

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
        protected override async Task RunAsync(CancellationToken cancellationToken)
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
            private readonly IContainer _container;

            /// <summary>
            /// Initializes a new instance of the <see cref="BackgroundService"/> class.
            /// </summary>
            /// <param name="container">The <see cref="IContainer"/> used to resolve an <see cref="EventQueuePublisher"/>.</param>
            public Scoped(IContainer container)
            {
                Ensure.Arg.NotNull(container, nameof(container));
                _container = container;
            }

            /// <summary>
            /// Publishes events until canceled.
            /// </summary>
            /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
            /// <returns>A task that represents the asynchronous operation.</returns>
            protected override async Task RunAsync(CancellationToken cancellationToken)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    using IContainerScope scope = _container.CreateScope();
                    IContainer container = scope.Container;

                    using var publisher = new EventQueuePublisherService(
                        container.Resolve<EventQueuePublisher>(),
                        container.Resolve<ILogger<EventQueuePublisherService>>());

                    await publisher.PublishAsync(cancellationToken);
                }
            }
        }
    }
}