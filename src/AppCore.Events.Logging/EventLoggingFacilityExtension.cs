// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.Events.Logging;
using AppCore.Events.Pipeline;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides logging extension for the <see cref="IEventsFacility"/>.
    /// </summary>
    public class EventLoggingFacilityExtension : FacilityExtension<IEventsFacility>
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
