// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

namespace AppCore.Events.Store
{
    /// <summary>
    /// Provides event store related feature to an <see cref="IEventContext"/>.
    /// </summary>
    public interface IEventStoreFeature
    {
        /// <summary>
        /// Gets the <see cref="IEventStore"/> where the event was read from.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c> if the event is not persisted.
        /// </remarks>
        IEventStore Store { get; }

        /// <summary>
        /// Gets a value indicating whether the event was read from the store.
        /// </summary>
        bool IsPersisted { get; }
    }
}
