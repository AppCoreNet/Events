// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;

namespace AppCore.Events.Queue
{
    /// <summary>
    /// Provides a reader for the in-memory event queue.
    /// </summary>
    public class InMemoryEventQueueReader : IEventQueueReader
    {
        private readonly InMemoryEventQueue _queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryEventQueueReader"/> class.
        /// </summary>
        /// <param name="queue">The in-memory event queue.</param>
        public InMemoryEventQueueReader(InMemoryEventQueue queue)
        {
            Ensure.Arg.NotNull(queue, nameof(queue));
            _queue = queue;
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<IEventContext>> ReadAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return _queue.ReadAsync(timeout, cancellationToken);
        }
    }
}