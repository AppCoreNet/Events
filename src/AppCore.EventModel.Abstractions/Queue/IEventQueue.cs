// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.EventModel.Queue
{
    /// <summary>
    /// Represents an event queue.
    /// </summary>
    public interface IEventQueue
    {
        /// <summary>
        /// Writes an event to the queue.
        /// </summary>
        /// <param name="events">The event to enqueue.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads pending events from the queue.
        /// </summary>
        /// <param name="maxEventsToRead">The maximum number of events to read.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        Task<IReadOnlyCollection<IEventContext>> ReadAsync(int maxEventsToRead, CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the last processed read event.
        /// </summary>
        /// <param name="event">The event which has been processed.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        Task CommitReadAsync(IEventContext @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the event history from the queue.
        /// </summary>
        /// <param name="offset">The starting offset.</param>
        /// <param name="maxEventsToRead">The maximum number of events to read.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        Task<IReadOnlyCollection<IEventContext>> ReadHistoryAsync(
            long offset,
            int maxEventsToRead,
            CancellationToken cancellationToken = default);
    }
}