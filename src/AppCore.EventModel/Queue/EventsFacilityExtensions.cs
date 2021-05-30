// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.DependencyInjection;
using AppCore.Diagnostics;
using AppCore.EventModel.Queue;
using AppCore.Events;

// ReSharper disable once CheckNamespace
namespace AppCore.EventModel
{
    /// <summary>
    /// Provides extension methods to register an event queue.
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
            Action<EventQueueFacilityExtension> configure = null)
        {
            Ensure.Arg.NotNull(facility, nameof(facility));
            facility.AddExtension(configure);
            return facility;
        }

        /// <summary>
        /// Registers in-memory event queue.
        /// </summary>
        /// <param name="extension">The <see cref="EventQueueFacilityExtension"/>.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventQueueFacilityExtension WithInMemoryQueue(this EventQueueFacilityExtension extension)
        {
            Ensure.Arg.NotNull(extension, nameof(extension));

            extension.ConfigureRegistry(
                r =>
                {
                    r.TryAdd(ComponentRegistration.Singleton<IEventQueue, InMemoryEventQueue>());
                });

            return extension;
        }
    }
}