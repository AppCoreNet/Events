// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events;
using AppCore.Events.Logging;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register logging with the <see cref="IEventsFacility"/>.
    /// </summary>
    public static class EventLoggingRegistrationExtensions
    {
        /// <summary>
        /// Registers logging event pipeline behavior.
        /// </summary>
        /// <exception cref="ArgumentNullException">Argument <paramref name="builder"/> is <c>null</c>.</exception>
        public static IFacilityBuilder<IEventsFacility> AddLogging(
            this IFacilityBuilder<IEventsFacility> builder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));
            return builder.Add<EventLoggingExtension>();
        }
    }
}