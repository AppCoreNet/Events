// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;

namespace AppCore.Events.Storage
{
    /// <inheritdoc />
    public class EventStorePublisherOffset : IEventStorePublisherOffset
    {
        private static readonly Task _completedTask = Task.FromResult(true);
        private EventOffset _nextOffset = EventOffset.Next;

        /// <inheritdoc />
        public ValueTask<EventOffset> GetNextOffset(CancellationToken cancellationToken = default)
        {
            return new ValueTask<EventOffset>(_nextOffset);
        }

        /// <inheritdoc />
        public Task CommitOffset(long offset, CancellationToken cancellationToken = default)
        {
            Ensure.Arg.InRange(offset, 0, long.MaxValue, nameof(offset));
            _nextOffset = offset + 1;
            return _completedTask;
        }
    }
}