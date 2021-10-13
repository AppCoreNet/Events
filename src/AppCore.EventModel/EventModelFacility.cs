// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.EventModel;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides the events facility.
    /// </summary>
    public class EventModelFacility : Facility
    {
        /// <summary>
        /// Gets the lifetime of the event pipeline components.
        /// </summary>
        public ServiceLifetime Lifetime { get; private set; } = ServiceLifetime.Scoped;

        /// <summary>
        /// Registers the <see cref="IEventContextAccessor"/> with the DI container.
        /// </summary>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        public EventModelFacility WithEventContext()
        {
            ConfigureServices(services => services.TryAddSingleton<IEventContextAccessor, EventContextAccessor>());
            return this;
        }

        /// <summary>
        /// Configures the lifetime of event pipeline components.
        /// </summary>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/>.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        public EventModelFacility WithLifetime(ServiceLifetime lifetime)
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
            ConfigureServices(
                services => services.TryAddEnumerable(
                    ServiceDescriptor.Describe(typeof(IEventHandler<>), handlerType, Lifetime)));

            return this;
        }

        /// <summary>
        /// Adds event handlers to the container.
        /// </summary>
        /// <param name="configure">The delegate used to configure the registration sources.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="configure"/> is <c>null</c>.</exception>
        public EventModelFacility WithHandlersFrom(Action<IServiceDescriptorReflectionBuilder> configure)
        {
            Ensure.Arg.NotNull(configure, nameof(configure));
            ConfigureServices(services =>
            {
                services.TryAddEnumerableFrom(
                    typeof(IEventHandler<>),
                    builder =>
                    {
                        builder.WithDefaultLifetime(Lifetime);
                        configure(builder);
                    });
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
            ConfigureServices(
                services => services.TryAddEnumerable(
                    ServiceDescriptor.Describe(typeof(IPreEventHandler<>), handlerType, Lifetime)));

            return this;
        }

        /// <summary>
        /// Adds event pre-handlers to the container.
        /// </summary>
        /// <param name="configure">The delegate used to configure the registration sources.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="configure"/> is <c>null</c>.</exception>
        public EventModelFacility WithPreHandlersFrom(Action<IServiceDescriptorReflectionBuilder> configure)
        {
            Ensure.Arg.NotNull(configure, nameof(configure));
            ConfigureServices(services =>
            {
                services.TryAddEnumerableFrom(
                    typeof(IPreEventHandler<>),
                    builder =>
                    {
                        builder.WithDefaultLifetime(Lifetime);
                        configure(builder);
                    });
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
            ConfigureServices(
                    services => services.TryAddEnumerable(
                        ServiceDescriptor.Describe(typeof(IPostEventHandler<>), handlerType, Lifetime)));

            return this;
        }

        /// <summary>
        /// Adds event post-handlers to the container.
        /// </summary>
        /// <param name="configure">The delegate used to configure the registration sources.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="configure"/> is <c>null</c>.</exception>
        public EventModelFacility WithPostHandlersFrom(Action<IServiceDescriptorReflectionBuilder> configure)
        {
            Ensure.Arg.NotNull(configure, nameof(configure));
            ConfigureServices(services =>
            {
                services.TryAddEnumerableFrom(
                    typeof(IPostEventHandler<>),
                    builder =>
                    {
                        builder.WithDefaultLifetime(Lifetime);
                        configure(builder);
                    });
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
            ConfigureServices(
                services => services.TryAddEnumerable(
                    ServiceDescriptor.Describe(typeof(IEventPipelineBehavior<>), handlerType, Lifetime)));

            return this;
        }

        /// <summary>
        /// Adds event pipeline behaviors to the container.
        /// </summary>
        /// <param name="configure">The delegate used to configure the registration sources.</param>
        /// <returns>The <see cref="EventModelFacility"/>.</returns>
        /// <exception cref="ArgumentNullException">Argument <paramref name="configure"/> is <c>null</c>.</exception>
        public EventModelFacility WithBehaviorsFrom(Action<IServiceDescriptorReflectionBuilder> configure)
        {
            Ensure.Arg.NotNull(configure, nameof(configure));
            ConfigureServices(services =>
            {
                services.TryAddEnumerableFrom(
                    typeof(IEventPipelineBehavior<>),
                    builder =>
                    {
                        builder.WithDefaultLifetime(Lifetime);
                        configure(builder);
                    });
            });

            return this;
        }

        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.TryAdd(
                new[]
                {
                    ServiceDescriptor.Describe(typeof(IEventPipelineResolver), typeof(EventPipelineResolver), Lifetime),
                    ServiceDescriptor.Describe(typeof(IEventPipeline<>), typeof(EventPipeline<>), Lifetime),
                    ServiceDescriptor.Singleton<IEventContextFactory, EventContextFactory>(),
                    ServiceDescriptor.Singleton<IEventDescriptorFactory, EventDescriptorFactory>(),
                    ServiceDescriptor.Describe(typeof(IEventPublisher), typeof(EventPublisher), Lifetime)
                });

            services.TryAddEnumerable(
                new[]
                {
                    ServiceDescriptor.Singleton<IEventMetadataProvider, CancelableEventMetadataProvider>(),
                    ServiceDescriptor.Singleton<IEventMetadataProvider, TopicMetadataProvider>(),
                    ServiceDescriptor.Singleton(typeof(IEventPipelineBehavior<>), typeof(CancelableEventBehavior<>)),
                    ServiceDescriptor.Describe(
                        typeof(IEventPipelineBehavior<>),
                        typeof(PreEventHandlerBehavior<>),
                        Lifetime),
                    ServiceDescriptor.Describe(
                        typeof(IEventPipelineBehavior<>),
                        typeof(PostEventHandlerBehavior<>),
                        Lifetime)
                });
        }
    }
}
