// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.EventModel;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to configure the <see cref="EventModelFacility"/>.
    /// </summary>
    public static class EventModelRegistrationExtensions
    {
        /// <summary>
        /// Adds the <see cref="EventModelFacility"/> to the DI container.
        /// </summary>
        /// <param name="registry">The <see cref="IComponentRegistry"/>.</param>
        /// <param name="configure">The configure delegate.</param>
        /// <returns></returns>
        public static IComponentRegistry AddEventModel(
            this IComponentRegistry registry,
            Action<EventModelFacility> configure = null)
        {
            return registry.AddFacility(configure);
        }
    }
}
