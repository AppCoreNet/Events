// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.EventModel;

/// <summary>
/// Provides a handler for events.
/// </summary>
/// <typeparam name="TEvent">The type of the event that is handled.</typeparam>
public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Handles the event.
    /// </summary>
    /// <param name="context">The context of the event that should be handled.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous event operation.</returns>
    Task HandleAsync(IEventContext<TEvent> context, CancellationToken cancellationToken);
}