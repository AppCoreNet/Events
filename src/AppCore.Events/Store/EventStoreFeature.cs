// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using AppCore.Diagnostics;

namespace AppCore.Events.Store
{
    /// <inheritdoc />
    public class EventStoreFeature : IEventStoreFeature
    {
        /// <inheritdoc />
        public IEventStore Store { get; }

        /// <inheritdoc />
        public bool IsPersisted { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreFeature"/> class.
        /// </summary>
        /// <param name="store">The <see cref="IEventStore"/> of the event.</param>
        /// <param name="isPersisted"><c>true</c> if event is read from a store.</param>
        public EventStoreFeature(IEventStore store, bool isPersisted)
        {
            Ensure.Arg.NotNull(store, nameof(store));

            Store = store;
            IsPersisted = isPersisted;
        }
    }
}