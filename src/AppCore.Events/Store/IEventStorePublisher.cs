// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Events.Store
{
    /// <summary>
    /// Represents a type which publishes events from the store.
    /// </summary>
    public interface IEventStorePublisher
    {
        Task PublishPendingAsync(CancellationToken cancellationToken);
    }
}