// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.EventModel.Store
{
    /// <summary>
    /// Represents a type which publishes events from the store.
    /// </summary>
    public interface IEventStorePublisher
    {
        /// <summary>
        /// Published pending events from the store.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous publish operation.</returns>
        Task PublishPendingAsync(CancellationToken cancellationToken);
    }
}