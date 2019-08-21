// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events;
using AppCore.Events.Extensions;
using AppCore.Events.Pipeline;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extensions methods to configure the <see cref="IEventsFacility"/>.
    /// </summary>
    public static class EventContextRegistrationExtensions
    {
        /// <summary>
        /// Registers the <see cref="IEventContextAccessor"/> with the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityBuilder{TFacility}"/>.</param>
        /// <returns>The passed <paramref name="builder"/>.</returns>
        public static IFacilityBuilder<IEventsFacility> AddContextAccessor(
            this IFacilityBuilder<IEventsFacility> builder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));
            return builder.Add<EventContextExtension>();
        }
    }
}