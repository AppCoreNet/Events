// Licensed under the MIT License.
// Copyright (c) 2018-2022 the AppCore .NET project.

using System;
using AppCore.Diagnostics;
using AppCore.EventModel;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to configure the <see cref="IEventModelBuilder"/>.
    /// </summary>
    public static class EventModelAppCoreBuilderExtensions
    {
        /// <summary>
        /// Adds the event model to the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="IAppCoreBuilder"/>.</param>
        /// <param name="configure">The configure delegate.</param>
        /// <returns></returns>
        public static IAppCoreBuilder AddEventModel(
            this IAppCoreBuilder builder,
            Action<IEventModelBuilder>? configure = null)
        {
            Ensure.Arg.NotNull(builder);

            IServiceCollection services = builder.Services;

            services.TryAdd(
                new[]
                {
                    ServiceDescriptor.Transient<IEventPipelineResolver, EventPipelineResolver>(),
                    ServiceDescriptor.Transient(typeof(IEventPipeline<>), typeof(EventPipeline<>)),
                    ServiceDescriptor.Singleton<IEventContextFactory, EventContextFactory>(),
                    ServiceDescriptor.Singleton<IEventDescriptorFactory, EventDescriptorFactory>(),
                    ServiceDescriptor.Transient<IEventPublisher, EventPublisher>()
                });

            services.TryAddEnumerable(
                new[]
                {
                    ServiceDescriptor.Singleton<IEventMetadataProvider, CancelableEventMetadataProvider>(),
                    ServiceDescriptor.Singleton<IEventMetadataProvider, TopicMetadataProvider>(),
                    ServiceDescriptor.Singleton(typeof(IEventPipelineBehavior<>), typeof(CancelableEventBehavior<>)),
                    ServiceDescriptor.Transient(
                        typeof(IEventPipelineBehavior<>),
                        typeof(PreEventHandlerBehavior<>)),
                    ServiceDescriptor.Transient(
                        typeof(IEventPipelineBehavior<>),
                        typeof(PostEventHandlerBehavior<>))
                });

            configure?.Invoke(new EventModelBuilder(services));

            return builder;
        }
    }
}
