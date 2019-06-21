// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using AppCore.Diagnostics;

namespace AppCore.Events.Store
{
    /// <summary>
    /// Provides event store extension methods for <see cref="IEventContext"/>.
    /// </summary>
    public static class EventStoreContextExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the event is persisted.
        /// </summary>
        /// <param name="context">The <see cref="IEventContext"/>.</param>
        /// <returns><c>true</c> if the event is persisted; <c>false</c> otherwise.</returns>
        public static bool IsPersisted(this IEventContext context)
        {
            Ensure.Arg.NotNull(context, nameof(context));
            return context.TryGetFeature(out IEventStoreFeature feature) && feature.IsPersisted;
        }

        /// <summary>
        /// Gets the <see cref="IEventStore"/> where the event was read from.
        /// </summary>
        /// <param name="context">The <see cref="IEventContext"/>.</param>
        /// <returns>The <see cref="IEventStore"/>.</returns>
        public static IEventStore GetEventStore(this IEventContext context)
        {
            Ensure.Arg.NotNull(context, nameof(context));
            return context.GetFeature<IEventStoreFeature>()
                          .Store;
        }
    }
}