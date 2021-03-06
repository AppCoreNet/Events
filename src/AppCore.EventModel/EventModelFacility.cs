// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.DependencyInjection;
using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;

namespace AppCore.EventModel
{
    /// <summary>
    /// Provides the events facility.
    /// </summary>
    public class EventModelFacility : Facility
    {
        /// <summary>
        /// Gets the lifetime of the event pipeline components.
        /// </summary>
        public ComponentLifetime Lifetime { get; private set; } = ComponentLifetime.Scoped;

        /// <summary>
        /// Registers the <see cref="IEventContextAccessor"/> with the DI container.
        /// </summary>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        public EventModelFacility WithEventContext()
        {
            ConfigureRegistry(
                r => r.TryAdd(
                    ComponentRegistration.Singleton<IEventContextAccessor, EventContextAccessor>()
                )
            );

            return this;
        }

        /// <summary>
        /// Configures the lifetime of event pipeline components.
        /// </summary>
        /// <param name="lifetime">The <see cref="ComponentLifetime"/>.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        public EventModelFacility WithLifetime(ComponentLifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }

        /// <summary>
        /// Adds event handler to the container.
        /// </summary>
        /// <param name="handlerType">The type of the event handler.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="handlerType"/> is <c>null</c>.</exception>
        public EventModelFacility WithHandler(Type handlerType)
        {
            Ensure.Arg.NotNull(handlerType, nameof(handlerType));
            ConfigureRegistry(
                r => r.TryAddEnumerable(
                    ComponentRegistration.Create(typeof(IEventHandler<>), handlerType, Lifetime)
                )
            );

            return this;
        }

        /// <summary>
        /// Adds event handlers to the container.
        /// </summary>
        /// <param name="configure">The delegate used to configure the registration sources.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="configure"/> is <c>null</c>.</exception>
        public EventModelFacility WithHandlersFrom(Action<IComponentRegistrationSources> configure)
        {
            Ensure.Arg.NotNull(configure, nameof(configure));
            ConfigureRegistry(r =>
            {
                var registrationSources = new ComponentRegistrationSources(typeof(IEventHandler<>), Lifetime);
                configure(registrationSources);
                r.TryAddEnumerable(registrationSources.GetRegistrations());
            });

            return this;
        }

        /// <summary>
        /// Adds event pre-handler to the container.
        /// </summary>
        /// <param name="handlerType">The type of the event handler.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="handlerType"/> is <c>null</c>.</exception>
        public EventModelFacility WithPreHandler(Type handlerType)
        {
            Ensure.Arg.NotNull(handlerType, nameof(handlerType));
            ConfigureRegistry(
                r => r.TryAddEnumerable(
                    ComponentRegistration.Create(typeof(IPreEventHandler<>), handlerType, Lifetime)
                )
            );

            return this;
        }

        /// <summary>
        /// Adds event pre-handlers to the container.
        /// </summary>
        /// <param name="configure">The delegate used to configure the registration sources.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="configure"/> is <c>null</c>.</exception>
        public EventModelFacility WithPreHandlersFrom(Action<IComponentRegistrationSources> configure)
        {
            Ensure.Arg.NotNull(configure, nameof(configure));
            ConfigureRegistry(r =>
            {
                var registrationSources = new ComponentRegistrationSources(typeof(IPreEventHandler<>), Lifetime);
                configure(registrationSources);
                r.TryAddEnumerable(registrationSources.GetRegistrations());
            });

            return this;
        }

        /// <summary>
        /// Adds event post-handler to the container.
        /// </summary>
        /// <param name="handlerType">The type of the event handler.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="handlerType"/> is <c>null</c>.</exception>
        public EventModelFacility WithPostHandler(Type handlerType)
        {
            Ensure.Arg.NotNull(handlerType, nameof(handlerType));
            ConfigureRegistry(
                r => r.TryAddEnumerable(
                    ComponentRegistration.Create(typeof(IPostEventHandler<>), handlerType, Lifetime)
                )
            );

            return this;
        }

        /// <summary>
        /// Adds event post-handlers to the container.
        /// </summary>
        /// <param name="configure">The delegate used to configure the registration sources.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="configure"/> is <c>null</c>.</exception>
        public EventModelFacility WithPostHandlersFrom(Action<IComponentRegistrationSources> configure)
        {
            Ensure.Arg.NotNull(configure, nameof(configure));
            ConfigureRegistry(r =>
            {
                var registrationSources = new ComponentRegistrationSources(typeof(IPostEventHandler<>), Lifetime);
                configure(registrationSources);
                r.TryAddEnumerable(registrationSources.GetRegistrations());
            });

            return this;
        }

        /// <summary>
        /// Adds event pipeline behavior to the container.
        /// </summary>
        /// <param name="handlerType">The type of the event handler.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="handlerType"/> is <c>null</c>.</exception>
        public EventModelFacility WithBehavior(Type handlerType)
        {
            Ensure.Arg.NotNull(handlerType, nameof(handlerType));
            ConfigureRegistry(
                r => r.TryAddEnumerable(
                    ComponentRegistration.Create(typeof(IEventPipelineBehavior<>), handlerType, Lifetime)
                )
            );

            return this;
        }

        /// <summary>
        /// Adds event pipeline behaviors to the container.
        /// </summary>
        /// <param name="configure">The delegate used to configure the registration sources.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="configure"/> is <c>null</c>.</exception>
        public EventModelFacility WithBehaviorsFrom(Action<IComponentRegistrationSources> configure)
        {
            Ensure.Arg.NotNull(configure, nameof(configure));
            ConfigureRegistry(r =>
            {
                var registrationSources = new ComponentRegistrationSources(typeof(IEventPipelineBehavior<>), Lifetime);
                configure(registrationSources);
                r.TryAddEnumerable(registrationSources.GetRegistrations());
            });

            return this;
        }

        /// <inheritdoc />
        protected override void Build(IComponentRegistry registry)
        {
            base.Build(registry);

            registry.AddLogging();

            registry.TryAdd(
                new[]
                {
                    ComponentRegistration.Create<IEventPipelineResolver, EventPipelineResolver>(Lifetime),
                    ComponentRegistration.Create(typeof(IEventPipeline<>), typeof(EventPipeline<>), Lifetime),
                    ComponentRegistration.Singleton<IEventContextFactory, EventContextFactory>(),
                    ComponentRegistration.Singleton<IEventDescriptorFactory, EventDescriptorFactory>(),
                    ComponentRegistration.Create<IEventPublisher, EventPublisher>(Lifetime)
                });

            registry.TryAddEnumerable(
                new[]
                {
                    ComponentRegistration.Singleton<IEventMetadataProvider, CancelableEventMetadataProvider>(),
                    ComponentRegistration.Singleton<IEventMetadataProvider, TopicMetadataProvider>(),
                    ComponentRegistration.Singleton(typeof(IEventPipelineBehavior<>), typeof(CancelableEventBehavior<>)),
                    ComponentRegistration.Create(typeof(IEventPipelineBehavior<>), typeof(PreEventHandlerBehavior<>), Lifetime),
                    ComponentRegistration.Create(typeof(IEventPipelineBehavior<>), typeof(PostEventHandlerBehavior<>), Lifetime)
                });
        }
    }
}
