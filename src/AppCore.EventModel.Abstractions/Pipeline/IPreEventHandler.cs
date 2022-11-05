// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.EventModel.Pipeline;

/// <summary>
/// Represents a handler which is invoked before a event is handled.
/// </summary>
/// <typeparam name="TEvent">The type of the event that is processed.</typeparam>
public interface IPreEventHandler<in TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Invoked before the event is being handled.
    /// </summary>
    /// <param name="context">The context of the event that is about to be handled.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous command operation.</returns>
    Task OnHandlingAsync(IEventContext<TEvent> context, CancellationToken cancellationToken);
}