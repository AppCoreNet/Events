// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

namespace AppCore.Events.Pipeline
{
    /// <summary>
    /// Represents a type which instantiates <see cref="IEventContext"/> objects.
    /// </summary>
    public interface IEventContextFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IEventContext"/> for the specified <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>The <see cref="IEventContext"/>.</returns>
        IEventContext CreateContext(IEvent @event);
    }
}
