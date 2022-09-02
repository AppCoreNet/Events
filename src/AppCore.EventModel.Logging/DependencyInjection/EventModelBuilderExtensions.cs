// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.Diagnostics;
using AppCore.EventModel;
using AppCore.EventModel.Logging;
using AppCore.EventModel.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register logging with the event model.
    /// </summary>
    public static class EventModelBuilderExtensions
    {
        /// <summary>
        /// Adds logging of events to the pipeline.
        /// </summary>
        /// <exception cref="ArgumentNullException">Argument <paramref name="builder"/> is <c>null</c>.</exception>
        public static IEventModelBuilder UseLogging(this IEventModelBuilder builder)
        {
            Ensure.Arg.NotNull(builder);

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton(
                    typeof(IEventPipelineBehavior<>),
                    typeof(EventLoggingBehavior<>)));

            return builder;
        }
    }
}