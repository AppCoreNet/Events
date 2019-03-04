// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.Events.Pipeline;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Represents extension for the <see cref="IEventsFacility"/> which registers the <see cref="IEventContextAccessor"/>.
    /// </summary>
    public class EventContextEventsFacilityExtension : FacilityExtension<IEventsFacility>
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