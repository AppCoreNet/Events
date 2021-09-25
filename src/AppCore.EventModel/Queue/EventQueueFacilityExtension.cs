// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.EventModel.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides event queuing extension for the <see cref="EventModelFacility"/>.
    /// </summary>
    public class EventQueueFacilityExtension : FacilityExtension
    {
        /// <summary>
        /// Registers in-memory event queue.
        /// </summary>
        /// <returns>The facility to allow chaining.</returns>
        public EventQueueFacilityExtension WithInMemoryQueue()
        {
            ConfigureServices(services => { services.TryAddSingleton<IEventQueue, InMemoryEventQueue>(); });
            return this;
        }

        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection registry)
        {
            base.ConfigureServices(registry);

            ServiceLifetime lifetime = ((EventModelFacility) Facility).Lifetime;

            registry.TryAdd(
                ServiceDescriptor.Describe(typeof(EventQueuePublisher), typeof(EventQueuePublisher), lifetime));

            if (lifetime == ServiceLifetime.Singleton)
            {
                registry.AddHostedService<EventQueuePublisherService>();
            }
            else
            {
                registry.AddHostedService<EventQueuePublisherService.Scoped>();
            }
        }
    }
}
