// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Generic;
using AppCore.Diagnostics;

namespace AppCore.EventModel.Metadata;

/// <summary>
/// Describes an event type.
/// </summary>
public class EventDescriptor
{
    /// <summary>
    /// Gets the type of the event.
    /// </summary>
    public Type EventType { get; }

    /// <summary>
    /// Gets the metadata of the event type.
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventDescriptor"/>.
    /// </summary>
    /// <param name="eventType">The type of the event.</param>
    /// <param name="metadata">The event type metadata.</param>
    public EventDescriptor(Type eventType, IReadOnlyDictionary<string, object> metadata)
    {
        Ensure.Arg.NotNull(eventType);
        Ensure.Arg.OfType<IEvent>(eventType);
        Ensure.Arg.NotNull(metadata);

        EventType = eventType;
        Metadata = metadata;
    }
}