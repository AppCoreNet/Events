// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.Diagnostics;
using AppCore.EventModel;
using AppCore.EventModel.Formatters;
using AppCore.EventModel.Pipeline;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register 
    /// </summary>
    public static class EventModelFacilityExtensions
    {
        /// <summary>
        /// Registers Newtonsoft.Json event formatter.
        /// </summary>
        /// <param name="facility">The <see cref="EventModelFacility"/>.</param>
        /// <param name="configure">The settings configuration delegate.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventModelFacility UseNewtonsoftJson(this EventModelFacility facility, Action<JsonSerializerSettings> configure = null)
        {
            Ensure.Arg.NotNull(facility, nameof(facility));

            facility.ConfigureRegistry(
                r => r.TryAddEnumerable(
                    ComponentRegistration.Singleton<IEventContextFormatter>(
                        ComponentFactory.Create(
                            c =>
                            {
                                var settings = new JsonSerializerSettings();
                                configure?.Invoke(settings);
                                return new NewtonsoftJsonFormatter(c.Resolve<IEventContextFactory>(), settings);
                            })
                    )
                )
            );

            return facility;
        }
    }
}