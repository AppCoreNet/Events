// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.EventModel.Pipeline;

/// <summary>
/// Represents a behavior for the event pipeline.
/// </summary>
/// <typeparam name="TEvent">The type of the event that is notified.</typeparam>
public interface IEventPipelineBehavior<TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Handles the specified event.
    /// </summary>
    /// <param name="context">The event context.</param>
    /// <param name="next">The behavior which should be invoked next.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous command operation.</returns>
    Task HandleAsync(
        IEventContext<TEvent> context,
        EventPipelineDelegate<TEvent> next,
        CancellationToken cancellationToken);
}