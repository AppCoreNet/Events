// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;

namespace AppCore.Events.Queue
{
    /// <summary>
    /// Provides a writer for the in-memory event queue.
    /// </summary>
    public class InMemoryEventQueueWriter : IEventQueueWriter
    {
        private readonly InMemoryEventQueue _queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryEventQueueWriter"/> class.
        /// </summary>
        /// <param name="queue">The in-memory event queue.</param>
        public InMemoryEventQueueWriter(InMemoryEventQueue queue)
        {
            Ensure.Arg.NotNull(queue, nameof(queue));
            _queue = queue;
        }

        /// <inheritdoc />
        public Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken)
        {
            return _queue.WriteAsync(events, cancellationToken);
        }
    }
}