// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.DependencyInjection;
using AppCore.DependencyInjection.Facilities;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;
using AppCore.EventModel.Store;

// ReSharper disable once CheckNamespace
namespace AppCore.EventModel
{
    /// <summary>
    /// Represents extension for the <see cref="EventsFacility"/> which registers event store behavior.
    /// </summary>
    public class EventStoreExtension : FacilityExtension
    {
        private bool _registerBackgroundPublisher;

        /// <summary>
        /// Sets a value indicating whether to register background publishes.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public EventStoreExtension WithBackgroundPublisher(bool value = true)
        {
            _registerBackgroundPublisher = value;
            return this;
        }

        /// <inheritdoc />
        protected override void Build(IComponentRegistry registry)
        {
            base.Build(registry);

            ComponentLifetime lifetime = ((EventsFacility) Facility).Lifetime;

            registry.TryAddEnumerable(
                new[]
                {
                    ComponentRegistration.Singleton<IEventMetadataProvider, EventStoreMetadataProvider>(),
                    ComponentRegistration.Create(
                        typeof(IEventPipelineBehavior<>),
                        typeof(EventStoreBehavior<>),
                        lifetime)
                });

            if (_registerBackgroundPublisher)
            {
                registry.TryAdd(
                    new[]
                    {
                        ComponentRegistration.Scoped<IEventStorePublisherOffset, EventStorePublisherOffset>(),
                        ComponentRegistration.Create<IEventStorePublisher, EventStorePublisher>(lifetime)
                    });

                if (lifetime == ComponentLifetime.Singleton)
                {
                    registry.AddHosting(h => h.WithBackgroundService<EventStorePublisherService>());
                }
                else
                {
                    registry.AddHosting(h => h.WithBackgroundService<EventStorePublisherService.Scoped>());
                }
            }
        }
    }
}