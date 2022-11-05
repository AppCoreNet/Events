// Licensed under the MIT License.
// Copyright (c) 2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.EventModel.Pipeline;

/// <summary>
/// Represents an event pipeline.
/// </summary>
public interface IEventPipeline
{
    /// <summary>
    /// Processes an event.
    /// </summary>
    /// <param name="context">The event context.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous event operation.</returns>
    Task ProcessAsync(IEventContext context, CancellationToken cancellationToken);
}

/// <inheritdoc />
public interface IEventPipeline<in TEvent> : IEventPipeline
    where TEvent : IEvent
{
    /// <summary>
    /// Processes an event.
    /// </summary>
    /// <param name="context">The event context.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous event operation.</returns>
    Task ProcessAsync(IEventContext<TEvent> context, CancellationToken cancellationToken);
}