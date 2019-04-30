// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using AppCore.DependencyInjection;
using AppCore.DependencyInjection.Facilities;
using AppCore.Events.Pipeline;

namespace AppCore.Events.Logging
{
    /// <summary>
    /// Provides logging extension for the <see cref="IEventsFacility"/>.
    /// </summary>
    public class EventLoggingExtension : FacilityExtension<IEventsFacility>
    {
        /// <inheritdoc />
        protected override void RegisterComponents(IComponentRegistry registry, IEventsFacility facility)
        {
            registry.Register(typeof(IEventPipelineBehavior<>))
                    .Add(typeof(EventLoggingBehavior<>))
                    .PerContainer()
                    .IfNotRegistered();
        }
    }
}
