// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events.Logging;
using AppCore.Events.Pipeline;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register logging with the <see cref="EventsFacility"/>.
    /// </summary>
    public static class LoggingEventsFacilityExtensions
    {
        /// <summary>
        /// Adds logging of events to the pipeline.
        /// </summary>
        /// <exception cref="ArgumentNullException">Argument <paramref name="facility"/> is <c>null</c>.</exception>
        public static EventsFacility UseLogging(this EventsFacility facility)
        {
            Ensure.Arg.NotNull(facility, nameof(facility));

            facility.ConfigureRegistry(
                r => r.TryAddEnumerable(
                    ComponentRegistration.Singleton(typeof(IEventPipelineBehavior<>), typeof(EventLoggingBehavior<>))));

            return facility;
        }
    }
}