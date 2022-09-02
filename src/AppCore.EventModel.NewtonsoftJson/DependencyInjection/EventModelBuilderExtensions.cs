// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.Diagnostics;
using AppCore.EventModel.Formatters;
using AppCore.EventModel.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace AppCore.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register
    /// </summary>
    public static class EventModelBuilderExtensions
    {
        /// <summary>
        /// Registers Newtonsoft.Json event formatter.
        /// </summary>
        /// <param name="builder">The <see cref="IEventModelBuilder"/>.</param>
        /// <param name="configure">The settings configuration delegate.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IEventModelBuilder UseNewtonsoftJson(this IEventModelBuilder builder, Action<NewtonsoftJsonFormatterOptions>? configure = null)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));

            IServiceCollection services = builder.Services;

            if (configure != null)
            {
                services.Configure(configure);
            }

            services.TryAddEnumerable(ServiceDescriptor.Transient<IEventContextFormatter, NewtonsoftJsonFormatter>());

            return builder;
        }
    }
}