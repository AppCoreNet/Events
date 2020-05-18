// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using AppCore.DependencyInjection;
using AppCore.DependencyInjection.Facilities;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;

namespace AppCore.Events.Storage
{
    /// <summary>
    /// Represents extension for the <see cref="IEventsFacility"/> which registers event store behavior.
    /// </summary>
    public class EventStoreExtension : FacilityExtension<IEventsFacility>
    {
        /// <summary>
        /// Gets or sets a value indicating whether to register <see cref="IEventStorePublisher"/> as a background
        /// service.
        /// </summary>
        public bool RegisterBackgroundPublisher { get; set; }

        /// <summary>
        /// Gets a list of additional registration callbacks.
        /// </summary>
        public IList<Action<IComponentRegistry, IEventsFacility>> RegistrationCallbacks { get; } =
            new List<Action<IComponentRegistry, IEventsFacility>>();

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

            if (RegisterBackgroundPublisher)
            {
                registry.Register<IEventStorePublisherOffset>()
                        .Add<EventStorePublisherOffset>()
                        .IfNoneRegistered()
                        .PerContainer();

                registry.Register<IEventStorePublisher>()
                        .Add<EventStorePublisher>()
                        .IfNoneRegistered()
                        .WithLifetime(facility.Lifetime);

                if (facility.Lifetime == ComponentLifetime.Singleton)
                {
                    registry.RegisterFacility<HostingFacility>()
                            .UseBackgroundServices(
                                r => r.Add<EventStorePublisherService>()
                                      .IfNotRegistered()
                                      .PerContainer());
                }
                else
                {
                    registry.RegisterFacility<HostingFacility>()
                            .UseBackgroundServices(
                                r => r.Add<EventStorePublisherService.Scoped>()
                                      .IfNotRegistered()
                                      .WithLifetime(facility.Lifetime));
                }
            }

            foreach (Action<IComponentRegistry, IEventsFacility> registrationCallback in RegistrationCallbacks)
            {
                registrationCallback(registry, facility);
            }
        }
    }
}