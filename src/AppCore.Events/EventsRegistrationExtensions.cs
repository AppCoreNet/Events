// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.DependencyInjection.Facilities;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to configure the <see cref="EventsFacility"/>.
    /// </summary>
    public static class EventsRegistrationExtensions
    {
        /// <summary>
        /// Adds the <see cref="EventsFacility"/> to the DI container.
        /// </summary>
        /// <param name="registry">The <see cref="IComponentRegistry"/>.</param>
        /// <param name="configure">The configure delegate.</param>
        /// <returns></returns>
        public static IComponentRegistry AddEvents(
            this IComponentRegistry registry,
            Action<EventsFacility> configure = null)
        {
            return registry.AddFacility(configure);
        }
    }
}
