// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Generic;
using AppCore.EventModel.Metadata;

namespace AppCore.EventModel;

/// <summary>
/// Represents the context when processing events.
/// </summary>
public interface IEventContext
{
    /// <summary>
    /// Gets the <see cref="EventDescriptor"/> of the event.
    /// </summary>
    EventDescriptor EventDescriptor { get; }

    /// <summary>
    /// Gets the event.
    /// </summary>
    IEvent Event { get; }

    /// <summary>
    /// Gets a <see cref="IDictionary{TKey,TValue}"/> of context items.
    /// </summary>
    IDictionary<object, object> Items { get; }

    /// <summary>
    /// Gets a <see cref="IDictionary{TKey,TValue}"/> of context features.
    /// </summary>
    IDictionary<Type, object> Features { get; }
}

/// <summary>
/// Represents typed context when processing events.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public interface IEventContext<out TEvent> : IEventContext
    where TEvent : IEvent
{
    /// <summary>
    /// Gets the event.
    /// </summary>
    new TEvent Event { get; }
}