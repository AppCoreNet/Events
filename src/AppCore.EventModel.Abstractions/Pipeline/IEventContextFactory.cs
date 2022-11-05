// Licensed under the MIT License.
// Copyright (c) 2019 the AppCore .NET project.

using AppCore.EventModel.Metadata;

namespace AppCore.EventModel.Pipeline;

/// <summary>
/// Represents a type which instantiates <see cref="IEventContext"/> objects.
/// </summary>
public interface IEventContextFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IEventContext"/> for the specified <paramref name="event"/>.
    /// </summary>
    /// <param name="descriptor">The descriptor of the event.</param>
    /// <param name="event">The event.</param>
    /// <returns>The <see cref="IEventContext"/>.</returns>
    IEventContext CreateContext(EventDescriptor descriptor, IEvent @event);
}