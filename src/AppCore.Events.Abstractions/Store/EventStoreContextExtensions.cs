// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using AppCore.Diagnostics;

namespace AppCore.Events.Store
{
    public static class EventStoreContextExtensions
    {
        public static bool IsPersisted(this IEventContext context)
        {
            Ensure.Arg.NotNull(context, nameof(context));
            return context.TryGetFeature(out IEventStoreFeature feature) && feature.IsPersisted;
        }
    }
}
