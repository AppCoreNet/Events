// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using AppCore.DependencyInjection;
using AppCore.DependencyInjection.Facilities;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;

namespace AppCore.Events.Store
{
    /// <summary>
    /// Represents extension for the <see cref="IEventsFacility"/> which registers event store behavior.
    /// </summary>
    public class EventStoreExtension : FacilityExtension<IEventsFacility>
    {
        /// <inheritdoc />
        protected override void RegisterComponents(IComponentRegistry registry, IEventsFacility facility)
        {
            registry.Register<IEventMetadataProvider>()
                    .Add<EventStoreMetadataProvider>()
                    .IfNotRegistered()
                    .PerContainer();

            registry.Register(typeof(IEventPipelineBehavior<>))
                    .Add(typeof(EventStoreBehavior<>))
                    .IfNotRegistered()
                    .WithLifetime(facility.Lifetime);
        }
    }
}