// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using AppCore.DependencyInjection;
using AppCore.DependencyInjection.Facilities;
using AppCore.Events.Pipeline;

namespace AppCore.Events.Extensions
{
    /// <summary>
    /// Represents extension for the <see cref="IEventsFacility"/> which registers the <see cref="IEventContextAccessor"/>.
    /// </summary>
    public class EventContextExtension : FacilityExtension<IEventsFacility>
    {
        /// <inheritdoc />
        protected override void RegisterComponents(IComponentRegistry registry, IEventsFacility facility)
        {
            registry.Register<IEventContextAccessor>()
                    .Add<EventContextAccessor>()
                    .PerContainer()
                    .IfNoneRegistered();
        }
    }
}