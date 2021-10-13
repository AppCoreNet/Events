// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;
using AppCore.EventModel.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Represents extension for the <see cref="EventModelFacility"/> which registers event store behavior.
    /// </summary>
    public class EventStoreFacilityExtension : FacilityExtension
    {
        private bool _registerBackgroundPublisher;

        /// <summary>
        /// Sets a value indicating whether to register background publishes.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public EventStoreFacilityExtension WithBackgroundPublisher(bool value = true)
        {
            _registerBackgroundPublisher = value;
            return this;
        }

        /// <summary>
        /// Registers in-memory event queue.
        /// </summary>
        /// <returns>The facility to allow chaining.</returns>
        public EventStoreFacilityExtension WithInMemoryQueue()
        {
            ConfigureServices(services => { services.TryAddSingleton<IEventStore, InMemoryEventStore>(); });
            return this;
        }

        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection registry)
        {
            base.ConfigureServices(registry);

            ServiceLifetime lifetime = ((EventModelFacility) Facility).Lifetime;

            registry.TryAddEnumerable(
                new[]
                {
                    ServiceDescriptor.Singleton<IEventMetadataProvider, EventStoreMetadataProvider>(),
                    ServiceDescriptor.Describe(
                        typeof(IEventPipelineBehavior<>),
                        typeof(EventStoreBehavior<>),
                        lifetime)
                });

            if (_registerBackgroundPublisher)
            {
                registry.TryAdd(
                    new[]
                    {
                        ServiceDescriptor.Scoped<IEventStorePublisherOffset, EventStorePublisherOffset>(),
                        ServiceDescriptor.Describe(typeof(IEventStorePublisher), typeof(EventStorePublisher), lifetime)
                    });

                if (lifetime == ServiceLifetime.Singleton)
                {
                    registry.AddHostedService<EventStorePublisherService>();
                }
                else
                {
                    registry.AddHostedService<EventStorePublisherService.Scoped>();
                }
            }
        }
    }
}