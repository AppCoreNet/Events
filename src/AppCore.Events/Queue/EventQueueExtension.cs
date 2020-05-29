// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Collections.Generic;
using AppCore.DependencyInjection;
using AppCore.DependencyInjection.Facilities;

namespace AppCore.Events.Queue
{
    /// <summary>
    /// Provides event queuing extension for the <see cref="IEventsFacility"/>.
    /// </summary>
    public class EventQueueExtension : FacilityExtension<IEventsFacility>, IEventQueueExtension
    {
        /// <inheritdoc />
        public IList<Action<IComponentRegistry, IEventsFacility>> RegistrationCallbacks { get; } =
            new List<Action<IComponentRegistry, IEventsFacility>>();

        /// <inheritdoc />
        protected override void RegisterComponents(IComponentRegistry registry, IEventsFacility facility)
        {
            registry.Register<EventQueuePublisher>()
                    .Add<EventQueuePublisher>()
                    .IfNoneRegistered()
                    .WithLifetime(facility.Lifetime);

            if (facility.Lifetime == ComponentLifetime.Singleton)
            {
                registry.RegisterFacility<HostingFacility>()
                        .UseBackgroundServices(
                            r => r.Add<EventQueuePublisherService>()
                                  .IfNotRegistered()
                                  .PerContainer());
            }
            else
            {
                registry.RegisterFacility<HostingFacility>()
                        .UseBackgroundServices(
                            r => r.Add<EventQueuePublisherService.Scoped>()
                                  .IfNotRegistered()
                                  .WithLifetime(facility.Lifetime));
            }

            foreach (Action<IComponentRegistry, IEventsFacility> registrationCallback in RegistrationCallbacks)
            {
                registrationCallback(registry, facility);
            }
        }
    }
}
