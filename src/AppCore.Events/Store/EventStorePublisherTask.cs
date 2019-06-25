// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;

namespace AppCore.Events.Store
{
    public class EventStorePublisherTask : AsyncBackgroundTask
    {
        private readonly IEventStorePublisher _publisher;

        public EventStorePublisherTask(IEventStorePublisher publisher)
        {
            Ensure.Arg.NotNull(publisher, nameof(publisher));
            _publisher = publisher;
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _publisher.PublishPendingAsync(cancellationToken)
                                .ConfigureAwait(false);
            }
        }
    }
}