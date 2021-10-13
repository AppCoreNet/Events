// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.EventModel.Store
{
    /// <summary>
    /// Represents an event store which supports persistent subscriptions.
    /// </summary>
    public interface ICommittableEventStore : IEventStore
    {
        /// <summary>
        /// Commits the specified offset for the stream.
        /// </summary>
        /// <param name="streamName">The name of the event stream.</param>
        /// <param name="offset">The offset to commit.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous commit operation.</returns>
        Task CommitAsync(string streamName, long offset, CancellationToken cancellationToken);
    }
}