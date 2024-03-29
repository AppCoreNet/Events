﻿// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.EventModel.Store;

/// <summary>
/// Represents a store for events.
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// Writes events to the store.
    /// </summary>
    /// <param name="events">The <see cref="IEnumerable{T}"/> of events to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken);

    /// <summary>
    /// Reads events from the store.
    /// </summary>
    /// <remarks>
    /// The number of events read may be less than the requested count if fewer events are available.
    /// The operation waits until at least one event with the specified <paramref name="offset"/> is available
    /// or the timeout has elapsed.
    /// </remarks>
    /// <param name="streamName">The name of the event stream.</param>
    /// <param name="offset">The offset of the first event to read.</param>
    /// <param name="maxCount">The maximum number of events to read.</param>
    /// <param name="timeout">The timeout for the read operation.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    Task<IReadOnlyCollection<IEventContext>> ReadAsync(
        string streamName,
        EventOffset offset,
        int maxCount,
        TimeSpan timeout,
        CancellationToken cancellationToken);
}