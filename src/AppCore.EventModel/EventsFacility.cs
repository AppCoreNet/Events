// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.DependencyInjection;
using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;

namespace AppCore.Events
{
    /// <summary>
    /// Provides the events facility.
    /// </summary>
    public class EventsFacility : Facility
    {
        /// <summary>
        /// Gets the lifetime of the event pipeline components.
        /// </summary>
        public ComponentLifetime Lifetime { get; private set; }

        /// <summary>
        /// Registers the <see cref="IEventContextAccessor"/> with the DI container.
        /// </summary>
        /// <returns>The <see cref="EventsFacility"/>.</returns>
        public EventsFacility WithEventContext()
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
        /// <returns>The <see cref="EventsFacility"/>.</returns>
        public EventsFacility WithLifetime(ComponentLifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }

        /// <summary>
        /// Adds event handler to the container.
        /// </summary>
        /// <param name="handlerType">The type of the event handler.</param>
        /// <returns>The <see cref="EventsFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="handlerType"/> is <c>null</c>.</exception>
        public EventsFacility WithHandler(Type handlerType)
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
        /// Adds event pre-handler to the container.
        /// </summary>
        /// <param name="handlerType">The type of the event handler.</param>
        /// <returns>The <see cref="EventsFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="handlerType"/> is <c>null</c>.</exception>
        public EventsFacility WithPreHandler(Type handlerType)
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
        /// Adds event post-handler to the container.
        /// </summary>
        /// <param name="handlerType">The type of the event handler.</param>
        /// <returns>The <see cref="EventsFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="handlerType"/> is <c>null</c>.</exception>
        public EventsFacility WithPostHandler(Type handlerType)
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
        /// Adds event pipeline behavior to the container.
        /// </summary>
        /// <param name="handlerType">The type of the event handler.</param>
        /// <returns>The <see cref="EventsFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="handlerType"/> is <c>null</c>.</exception>
        public EventsFacility WithBehavior(Type handlerType)
        {
            Ensure.Arg.NotNull(handlerType, nameof(handlerType));
            ConfigureRegistry(
                r => r.TryAddEnumerable(
                    ComponentRegistration.Create(typeof(IPostEventHandler<>), handlerType, Lifetime)
                )
            );

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
                    ComponentRegistration.Singleton(
                        typeof(IEventPipelineBehavior<>),
                        typeof(CancelableEventBehavior<>)),
                    ComponentRegistration.Create(
                        typeof(IEventPipelineBehavior<>),
                        typeof(PreEventHandlerBehavior<>),
                        Lifetime),
                    ComponentRegistration.Create(
                        typeof(IEventPipelineBehavior<>),
                        typeof(PostEventHandlerBehavior<>),
                        Lifetime)
                });
        }
    }
}
