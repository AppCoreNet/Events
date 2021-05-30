// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.EventModel.Store
{
    /// <summary>
    /// Stores the offset of the last event processed by <see cref="IEventStorePublisher"/>.
    /// </summary>
    public interface IEventStorePublisherOffset
    {
        /// <summary>
        /// Gets the next offset of the next event to read from the <see cref="IEventStore"/>.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask<EventOffset> GetNextOffset(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the offset of the last processed event.
        /// </summary>
        /// <param name="offset">The offset of the event.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CommitOffset(long offset, CancellationToken cancellationToken = default);
    }
}