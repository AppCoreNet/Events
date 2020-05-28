// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Events.Queue
{
    /// <summary>
    /// Represents a event queue reader.
    /// </summary>
    public interface IEventQueueReader
    {
        /// <summary>
        /// Reads events from the queue.
        /// </summary>
        /// <param name="timeout">The timeout for the read operation.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        Task<IReadOnlyCollection<IEventContext>> ReadAsync(TimeSpan timeout, CancellationToken cancellationToken);
    }
}