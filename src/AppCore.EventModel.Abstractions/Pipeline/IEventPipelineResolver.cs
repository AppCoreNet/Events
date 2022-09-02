// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;

namespace AppCore.EventModel.Pipeline;

/// <summary>
/// Represents a resolver for <see cref="IEventPipeline"/> instances.
/// </summary>
public interface IEventPipelineResolver
{
    /// <summary>
    /// Resolves an <see cref="IEventPipeline{TEvent}"/> for the specified <paramref name="eventType"/>.
    /// </summary>
    /// <param name="eventType">The type of the event.</param>
    /// <returns>The <see cref="IEventPipeline{TEvent}"/> instance.</returns>
    IEventPipeline Resolve(Type eventType);
}