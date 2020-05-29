// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Events.Queue
{
    /// <summary>
    /// Represents an event queue.
    /// </summary>
    public interface IEventQueue
    {
        /// <summary>
        /// Writes events to the queue.
        /// </summary>
        /// <param name="events">The <see cref="IEnumerable{T}"/> of events to enqueue.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken);

        /// <summary>
        /// Reads events from the queue.
        /// </summary>
        /// <param name="callback">The callback which is invoked when events become available.</param>
        /// <param name="options">The read options.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        Task ReadAsync(
            Func<IReadOnlyCollection<IEventContext>, CancellationToken, Task> callback,
            EventQueueReadOptions options,
            CancellationToken cancellationToken);
    }
}