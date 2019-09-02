// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using AppCore.Diagnostics;

namespace AppCore.Events.Storage
{
    /// <summary>
    /// Provides event store extension methods for <see cref="IEventContext"/>.
    /// </summary>
    public static class EventStoreContextExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the event was read from a store.
        /// </summary>
        /// <param name="context">The <see cref="IEventContext"/>.</param>
        /// <returns><c>true</c> if the event was read from a store; <c>false</c> otherwise.</returns>
        public static bool IsFromEventStore(this IEventContext context)
        {
            Ensure.Arg.NotNull(context, nameof(context));
            return context.HasFeature<IEventStoreFeature>();
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

        /// <summary>
        /// Gets the offset of the event in the store.
        /// </summary>
        /// <param name="context">The <see cref="IEventContext"/>.</param>
        /// <returns>The offset of the event.</returns>
        public static long GetEventStoreOffset(this IEventContext context)
        {
            Ensure.Arg.NotNull(context, nameof(context));
            return context.GetFeature<IEventStoreFeature>()
                          .Offset;
        }
    }
}