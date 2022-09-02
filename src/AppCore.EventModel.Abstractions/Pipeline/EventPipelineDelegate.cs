// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.EventModel.Pipeline;

/// <summary>
/// Represents a delegate in the event pipeline.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
/// <param name="event">The event.</param>
/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
/// <returns>A task that represents the asynchronous event operation.</returns>
public delegate Task EventPipelineDelegate<in TEvent>(
    IEventContext<TEvent> @event,
    CancellationToken cancellationToken)
    where TEvent : IEvent;