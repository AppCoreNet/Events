// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using AppCore.DependencyInjection.Builder;
using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events;
using AppCore.Events.Pipeline;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to configure the <see cref="IEventsFacility"/>.
    /// </summary>
    public static class EventsRegistrationExtensions
    {
        /// <summary>
        /// Configure the default lifetime of event components.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityBuilder{TFacility}"/>.</param>
        /// <param name="lifetime">The default component lifetime.</param>
        /// <returns>The <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="builder"/> is <c>null</c>.</exception>
        public static IFacilityBuilder<IEventsFacility> WithLifetime(
            this IFacilityBuilder<IEventsFacility> builder,
            ComponentLifetime lifetime)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));

            builder.Configure(f => f.Lifetime = lifetime);
            return builder;
        }

        /// <summary>
        /// Adds event handlers to the container.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityBuilder{TFacility}"/>.</param>
        /// <param name="registrationBuilder">A delegate to configure the <see cref="IRegistrationBuilder"/>.</param>
        /// <returns>The <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="builder"/> is <c>null</c>.</exception>
        public static IFacilityBuilder<IEventsFacility> AddHandlers(
            this IFacilityBuilder<IEventsFacility> builder,
            Action<IRegistrationBuilder> registrationBuilder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));

            return builder.Add(
                new RegistrationFacilityExtension<IEventsFacility>(
                    typeof(IEventHandler<>),
                    (r,f) =>
                    {
                        r.WithDefaultLifetime(f.Lifetime);
                        registrationBuilder(r);
                    }));
        }

        /// <summary>
        /// Adds event pre-handlers to the container.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityBuilder{TFacility}"/>.</param>
        /// <param name="registrationBuilder">A delegate to configure the <see cref="IRegistrationBuilder"/>.</param>
        /// <returns>The <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="builder"/> is <c>null</c>.</exception>
        public static IFacilityBuilder<IEventsFacility> AddPreHandlers(
            this IFacilityBuilder<IEventsFacility> builder,
            Action<IRegistrationBuilder> registrationBuilder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));

            return builder.Add(
                new RegistrationFacilityExtension<IEventsFacility>(
                    typeof(IPreEventHandler<>),
                    (r,f) =>
                    {
                        r.WithDefaultLifetime(f.Lifetime);
                        registrationBuilder(r);
                    }));
        }

        /// <summary>
        /// Adds event post-handlers to the container.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityBuilder{TFacility}"/>.</param>
        /// <param name="registrationBuilder">A delegate to configure the <see cref="IRegistrationBuilder"/>.</param>
        /// <returns>The <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="builder"/> is <c>null</c>.</exception>
        public static IFacilityBuilder<IEventsFacility> AddPostHandlers(
            this IFacilityBuilder<IEventsFacility> builder,
            Action<IRegistrationBuilder> registrationBuilder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));

            return builder.Add(
                new RegistrationFacilityExtension<IEventsFacility>(
                    typeof(IPostEventHandler<>),
                    (r,f) =>
                    {
                        r.WithDefaultLifetime(f.Lifetime);
                        registrationBuilder(r);
                    }));
        }

        /// <summary>
        /// Adds event pipeline behaviors to the container.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityBuilder{TFacility}"/>.</param>
        /// <param name="registrationBuilder">A delegate to configure the <see cref="IRegistrationBuilder"/>.</param>
        /// <returns>The <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="builder"/> is <c>null</c>.</exception>
        public static IFacilityBuilder<IEventsFacility> AddBehaviors(
            this IFacilityBuilder<IEventsFacility> builder,
            Action<IRegistrationBuilder> registrationBuilder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));

            return builder.Add(
                new RegistrationFacilityExtension<IEventsFacility>(
                    typeof(IEventPipelineBehavior<>),
                    (r,f) =>
                    {
                        r.WithDefaultLifetime(f.Lifetime);
                        registrationBuilder(r);
                    }));
        }
    }
}
