// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Events.Store
{
    /// <summary>
    /// Represents a store for events.
    /// </summary>
    public interface IEventStore
    {
        Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken);

        Task<IEnumerable<IEventContext>> ReadAsync(
            string streamName,
            long startSequenceNo,
            int maxCount,
            TimeSpan timeout,
            CancellationToken cancellationToken);
    }
}
