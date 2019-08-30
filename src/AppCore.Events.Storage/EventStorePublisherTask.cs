// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Threading;
using System.Threading.Tasks;
using AppCore.DependencyInjection;
using AppCore.Diagnostics;
using AppCore.Logging;

namespace AppCore.Events.Storage
{
    public class EventStorePublisherTask : AsyncBackgroundTask
    {
        private readonly IEventStorePublisher _publisher;
        private readonly ILogger<EventStorePublisherTask> _logger;

        public EventStorePublisherTask(IEventStorePublisher publisher, ILogger<EventStorePublisherTask> logger)
        {
            Ensure.Arg.NotNull(publisher, nameof(publisher));
            Ensure.Arg.NotNull(logger, nameof(logger));

            _publisher = publisher;
            _logger = logger;
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await PublishAsync(cancellationToken);
            }
        }

        protected virtual async Task PublishAsync(CancellationToken cancellationToken)
        {
            _logger.PublishingStoredEvents();

            try
            {
                await _publisher.PublishPendingAsync(cancellationToken)
                                .ConfigureAwait(false);
            }
            catch (Exception error)
            {
                _logger.PublishingStoredEventsFailed(error);
            }
        }

        public class Scoped : AsyncBackgroundTask
        {
            private readonly IContainer _container;

            public Scoped(IContainer container)
            {
                Ensure.Arg.NotNull(container, nameof(container));
                _container = container;
            }

            protected override async Task RunAsync(CancellationToken cancellationToken)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    using (IContainerScope scope = _container.CreateScope())
                    {
                        IContainer container = scope.Container;

                        var publisherTask = new EventStorePublisherTask(
                            container.Resolve<IEventStorePublisher>(),
                            container.Resolve<ILogger<EventStorePublisherTask>>());

                        await publisherTask.PublishAsync(cancellationToken);
                    }
                }
            }
        }
    }
}