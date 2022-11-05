// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.EventModel;

/// <summary>
/// Represents the event publisher.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event.
    /// </summary>
    /// <param name="event">The event to publish.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous event operation.</returns>
    Task PublishAsync(IEvent @event, CancellationToken cancellationToken);
}