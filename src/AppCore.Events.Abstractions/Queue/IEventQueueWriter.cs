// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Events.Queue
{
    /// <summary>
    /// Represents a event queue writer.
    /// </summary>
    public interface IEventQueueWriter
    {
        /// <summary>
        /// Writes events to the queue.
        /// </summary>
        /// <param name="events">The <see cref="IEnumerable{T}"/> of events to enqueue.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken);
    }
}
