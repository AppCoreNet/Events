// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;
using AppCore.DependencyInjection;
using AppCore.Diagnostics;

namespace AppCore.Events.Storage
{
    public class ScopedEventStorePublisherTask : AsyncBackgroundTask
    {
        private readonly IContainer _container;

        public ScopedEventStorePublisherTask(IContainer container)
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
                    var dispatcher = scope.Container.Resolve<IEventStorePublisher>();
                    await dispatcher.PublishPendingAsync(cancellationToken);
                }
            }
        }
    }
}