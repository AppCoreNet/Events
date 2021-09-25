// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.Diagnostics;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register logging with the <see cref="EventModelFacility"/>.
    /// </summary>
    public static class EventModelFacilityExtensions
    {
        /// <summary>
        /// Adds logging of events to the pipeline.
        /// </summary>
        /// <exception cref="ArgumentNullException">Argument <paramref name="facility"/> is <c>null</c>.</exception>
        public static EventModelFacility UseLogging(this EventModelFacility facility)
        {
            Ensure.Arg.NotNull(facility, nameof(facility));
            facility.AddExtension<LoggingEventModelFacilityExtension>();
            return facility;
        }
    }
}