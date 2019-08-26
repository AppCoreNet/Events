// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.Events;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides the default <see cref="IEventsFacility"/> implementation.
    /// </summary>
    public class EventsFacility : Facility, IEventsFacility
    {
        /// <summary>
        /// Gets or sets the lifetime when registering components.
        /// </summary>
        public ComponentLifetime Lifetime { get; set; } = ComponentLifetime.Scoped;

        protected override void RegisterComponentsCore(IComponentRegistry registry)
        {
            RegisterExtensionComponents(registry);
            RegisterComponents(registry);
        }

        /// <inheritdoc />
        protected override void RegisterComponents(IComponentRegistry registry)
        {
            registry.Register(typeof(IEventPipeline<>))
                    .Add(typeof(EventPipeline<>))
                    .WithLifetime(Lifetime)
                    .IfNoneRegistered();

            registry.Register(typeof(IEventPipelineBehavior<>))
                    .Add(typeof(CancelableEventBehavior<>))
                    .PerContainer()
                    .IfNotRegistered();

            registry.Register(typeof(IEventPipelineBehavior<>))
                    .Add(typeof(PreEventHandlerBehavior<>))
                    .WithLifetime(Lifetime)
                    .IfNotRegistered();

            registry.Register(typeof(IEventPipelineBehavior<>))
                    .Add(typeof(PostEventHandlerBehavior<>))
                    .WithLifetime(Lifetime)
                    .IfNotRegistered();

            registry.Register<IEventMetadataProvider>()
                    .Add<CancelableEventMetadataProvider>()
                    .PerContainer()
                    .IfNotRegistered();

            registry.Register<IEventDescriptorFactory>()
                    .Add<EventDescriptorFactory>()
                    .PerContainer()
                    .IfNoneRegistered();

            registry.Register<IEventContextFactory>()
                    .Add<EventContextFactory>()
                    .PerContainer()
                    .IfNoneRegistered();

            registry.Register<IEventPublisher>()
                    .Add<EventPublisher>()
                    .WithLifetime(Lifetime)
                    .IfNoneRegistered();
        }
    }
}
