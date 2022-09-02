// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;

namespace AppCore.EventModel.Metadata;

/// <summary>
/// Represents a type which instantiates <see cref="EventDescriptor"/> objects.
/// </summary>
public interface IEventDescriptorFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="EventDescriptor"/> for the specified <paramref name="eventType"/>.
    /// </summary>
    /// <param name="eventType">The type of the event.</param>
    /// <returns>The <see cref="EventDescriptor"/>.</returns>
    EventDescriptor CreateDescriptor(Type eventType);
}