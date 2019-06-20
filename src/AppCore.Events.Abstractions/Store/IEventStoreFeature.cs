// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

namespace AppCore.Events.Store
{
    public interface IEventStoreFeature
    {
        IEventStore Store { get; }

        bool IsPersisted { get; }
    }
}
