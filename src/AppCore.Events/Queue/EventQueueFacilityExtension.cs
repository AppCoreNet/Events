// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.DependencyInjection;
using AppCore.DependencyInjection.Facilities;
using AppCore.Events.Queue;

// ReSharper disable once CheckNamespace
namespace AppCore.Events
{
    /// <summary>
    /// Provides event queuing extension for the <see cref="EventsFacility"/>.
    /// </summary>
    public class EventQueueFacilityExtension : FacilityExtension
    {
        /// <inheritdoc />
        protected override void Build(IComponentRegistry registry)
        {
            base.Build(registry);

            ComponentLifetime lifetime = ((EventsFacility) Facility).Lifetime;

            registry.TryAdd(ComponentRegistration.Create<EventQueuePublisher, EventQueuePublisher>(lifetime));

            if (lifetime == ComponentLifetime.Singleton)
            {
                registry.AddHosting(h => h.WithBackgroundService<EventQueuePublisherService>());
            }
            else
            {
                registry.AddHosting(h => h.WithBackgroundService<EventQueuePublisherService.Scoped>());
            }
        }
    }
}
