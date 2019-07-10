// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events;
using AppCore.Events.Store;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register an event store.
    /// </summary>
    public static class EventStoreRegistrationExtensions
    {
        /// <summary>
        /// Registers event store behavior.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityExtensionBuilder{TFacility,TExtension}"/>.</param>
        /// <param name="configure">The delegate which is invoked to configure the extension.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IFacilityBuilder<IEventsFacility> UseEventStore(
            this IFacilityBuilder<IEventsFacility> builder,
            Action<IFacilityExtensionBuilder<IEventsFacility, EventStoreExtension>> configure = null)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));
            return builder.AddExtension(configure);
        }

        /// <summary>
        /// Registers background task for publishing pending events from the store.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityExtensionBuilder{TFacility,TExtension}"/>.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IFacilityExtensionBuilder<IEventsFacility, EventStoreExtension> WithBackgroundPublisher(
            this IFacilityExtensionBuilder<IEventsFacility, EventStoreExtension> builder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));
            builder.Extension.RegisterBackgroundPublisher = true;
            return builder;
        }

        /// <summary>
        /// Registers in-memory event store.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityExtensionBuilder{TFacility,TExtension}"/>.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IFacilityExtensionBuilder<IEventsFacility, EventStoreExtension> WithInMemoryStore(
            this IFacilityExtensionBuilder<IEventsFacility, EventStoreExtension> builder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));

            builder.Extension.RegistrationCallbacks.Add(
                (r, f) => r.Register<IEventStore>()
                           .Add<InMemoryEventStore>()
                           .IfNoneRegistered()
                           .PerContainer());

            return builder;
        }
    }
}