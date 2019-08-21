// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Events.Store
{
    /// <summary>
    /// Stores the offset of the last event processed by <see cref="IEventStorePublisher"/>.
    /// </summary>
    public interface IEventStorePublisherOffset
    {
        ValueTask<EventOffset> GetNextOffset(CancellationToken cancellationToken = default);

        Task CommitOffset(long offset, CancellationToken cancellationToken = default);
    }
}