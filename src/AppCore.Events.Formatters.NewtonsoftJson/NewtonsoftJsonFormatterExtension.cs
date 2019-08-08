// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.Events;
using AppCore.Events.Formatters;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Represents Newtonsoft.Json event formatter extension.
    /// </summary>
    public class NewtonsoftJsonFormatterExtension : FacilityExtension<IEventsFacility>
    {
        /// <inheritdoc />
        protected override void RegisterComponents(IComponentRegistry registry, IEventsFacility facility)
        {
            registry.Register<IEventContextTextFormatter>()
                    .Add<NewtonsoftJsonFormatter>()
                    .IfNotRegistered()
                    .PerContainer();
        }
    }
}