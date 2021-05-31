// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.Diagnostics;
using AppCore.EventModel;
using AppCore.EventModel.Store;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register an event store.
    /// </summary>
    public static class EventsFacilityExtensions
    {
        /// <summary>
        /// Registers event store behavior.
        /// </summary>
        /// <param name="facility">The <see cref="EventsFacility"/>.</param>
        /// <param name="configure">The delegate which is invoked to configure the extension.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventsFacility UseQueuing(
            EventsFacility facility,
            Action<EventStoreExtension> configure = null)
        {
            Ensure.Arg.NotNull(facility, nameof(facility));
            facility.AddExtension(configure);
            return facility;
        }

        /// <summary>
        /// Registers in-memory event queue.
        /// </summary>
        /// <param name="extension">The <see cref="EventStoreExtension"/>.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventStoreExtension WithInMemoryQueue(this EventStoreExtension extension)
        {
            Ensure.Arg.NotNull(extension, nameof(extension));

            extension.ConfigureRegistry(
                r =>
                {
                    r.TryAdd(ComponentRegistration.Singleton<IEventStore, InMemoryEventStore>());
                });

            return extension;
        }
    }
}