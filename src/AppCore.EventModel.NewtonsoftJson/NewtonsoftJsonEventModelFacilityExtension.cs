// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using System.Collections.Generic;
using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.EventModel.Formatters;
using AppCore.EventModel.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides a Newtonsoft JSON extension for the <see cref="EventModelFacility"/>.
    /// </summary>
    public class NewtonsoftJsonEventModelFacilityExtension : FacilityExtension
    {
        private const string OptionsName = "EventModelNewtonsoftJson";

        public NewtonsoftJsonEventModelFacilityExtension Configure(Action<JsonSerializerSettings> configure)
        {
            Ensure.Arg.NotNull(configure, nameof(configure));
            ConfigureServices(s => s.Configure(OptionsName, configure));
            return this;
        }

        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.TryAddEnumerable(
                new[]
                {
                    ServiceDescriptor.Singleton<IConfigureOptions<JsonSerializerSettings>, ConfigureJsonSerializerSettings>(),
                    ServiceDescriptor.Singleton<IEventContextFormatter, NewtonsoftJsonFormatter>(
                        sp =>
                        {
                            var settings = sp.GetRequiredService<IOptionsSnapshot<JsonSerializerSettings>>();
                            return new NewtonsoftJsonFormatter(
                                sp.GetRequiredService<IEventContextFactory>(),
                                settings.Get(OptionsName));
                        })
                }
            );
        }
    }
}