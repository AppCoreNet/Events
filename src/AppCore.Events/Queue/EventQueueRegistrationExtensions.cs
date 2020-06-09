// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events;
using AppCore.Events.Queue;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register an event queue.
    /// </summary>
    public static class EventQueueRegistrationExtensions
    {
        /// <summary>
        /// Registers event store behavior.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityExtensionBuilder{TFacility,TExtension}"/>.</param>
        /// <param name="configure">The delegate which is invoked to configure the extension.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IFacilityBuilder<IEventsFacility> AddQueue(
            this IFacilityBuilder<IEventsFacility> builder,
            Action<IFacilityExtensionBuilder<IEventsFacility, IEventQueueExtension>> configure = null)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));
            return builder.Add<EventQueueExtension>(configure);
        }

        /// <summary>
        /// Registers in-memory event queue.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityExtensionBuilder{TFacility,TExtension}"/>.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IFacilityExtensionBuilder<IEventsFacility, IEventQueueExtension> WithInMemoryQueue(
            this IFacilityExtensionBuilder<IEventsFacility, IEventQueueExtension> builder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));

            builder.Configure(
                (f, e) => e.RegistrationCallbacks.Add(
                    (r, f2) =>
                    {
                        r.Register<IEventQueue>()
                         .Add<InMemoryEventQueue>()
                         .IfNoneRegistered()
                         .PerContainer();
                    }));

            return builder;
        }
    }
}