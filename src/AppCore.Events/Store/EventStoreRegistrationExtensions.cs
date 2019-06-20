// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events;
using AppCore.Events.Store;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    public static class EventStoreRegistrationExtensions
    {
        public static IFacilityExtensionBuilder<IEventsFacility, EventStoreExtension> WithEventStore(
            this IFacilityBuilder<IEventsFacility> builder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));
            return builder.AddExtension<EventStoreExtension>();
        }

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