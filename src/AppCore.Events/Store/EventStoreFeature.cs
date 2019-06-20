// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using AppCore.Diagnostics;

namespace AppCore.Events.Store
{
    public class EventStoreFeature : IEventStoreFeature
    {
        public IEventStore Store { get; }

        public bool IsPersisted { get; }

        public EventStoreFeature(IEventStore store, bool isPersisted)
        {
            Ensure.Arg.NotNull(store, nameof(store));

            Store = store;
            IsPersisted = isPersisted;
        }
    }
}