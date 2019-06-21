// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

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
        /// <returns>The passed builder to allow chaining.</returns>
        public static IFacilityExtensionBuilder<IEventsFacility, EventStoreExtension> WithEventStore(
            this IFacilityBuilder<IEventsFacility> builder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));
            return builder.AddExtension<EventStoreExtension>();
        }

        /// <summary>
        /// Registers in-memory event store.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityExtensionBuilder{TFacility,TExtension}"/>.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IFacilityExtensionBuilder<IEventsFacility, EventStoreExtension> UseInMemory(
            this IFacilityExtensionBuilder<IEventsFacility, EventStoreExtension> builder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));
            builder.AddExtension(
                new RegistrationFacilityExtension<IEventsFacility, IEventStore>(
                    (r, f) => r.Add<InMemoryEventStore>()
                               .IfNoneRegistered()
                               .PerContainer()));

            return builder;
        }
    }
}