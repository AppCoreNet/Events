// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to configure the <see cref="EventModelFacility"/>.
    /// </summary>
    public static class EventModelAppCoreBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="EventModelFacility"/> to the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IAppCoreBuilder"/>.</param>
        /// <param name="configure">The configure delegate.</param>
        /// <returns></returns>
        public static IAppCoreBuilder AddEventModel(
            this IAppCoreBuilder builder,
            Action<EventModelFacility> configure = null)
        {
            builder.Services.AddFacility(configure);
            return builder;
        }
    }
}
