// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events.Queue;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register an event queue.
    /// </summary>
    public static class QueueEventFacilityExtensions
    {
        /// <summary>
        /// Registers event store behavior.
        /// </summary>
        /// <param name="facility">The <see cref="EventsFacility"/>.</param>
        /// <param name="configure">The delegate which is invoked to configure the extension.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventsFacility UseQueuing(
            EventsFacility facility,
            Action<EventQueueExtension> configure = null)
        {
            Ensure.Arg.NotNull(facility, nameof(facility));
            facility.AddExtension(configure);
            return facility;
        }

        /// <summary>
        /// Registers in-memory event queue.
        /// </summary>
        /// <param name="extension">The <see cref="EventQueueExtension"/>.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventQueueExtension WithInMemoryQueue(this EventQueueExtension extension)
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