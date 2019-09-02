// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Threading;
using System.Threading.Tasks;
using AppCore.DependencyInjection;
using AppCore.Diagnostics;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;

namespace AppCore.Events
{
    /// <summary>
    /// Provides the default event publisher implementation.
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly IEventDescriptorFactory _descriptorFactory;
        private readonly IEventContextFactory _contextFactory;
        private readonly EventPipelineResolver _pipelineResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="descriptorFactory">The factory for <see cref="EventDescriptor"/>.</param>
        /// <param name="contextFactory">The factory for <see cref="IEventContext"/>'s.</param>
        /// <param name="container">The <see cref="IContainer"/> used to resolve handlers and behaviors.</param>
        /// <exception cref="ArgumentNullException">Argument <paramref name="container"/> is <c>null</c>.</exception>
        public EventPublisher(
            IEventDescriptorFactory descriptorFactory,
            IEventContextFactory contextFactory,
            IContainer container)
        {
            Ensure.Arg.NotNull(descriptorFactory, nameof(descriptorFactory));
            Ensure.Arg.NotNull(contextFactory, nameof(contextFactory));
            Ensure.Arg.NotNull(container, nameof(container));

            _descriptorFactory = descriptorFactory;
            _contextFactory = contextFactory;
            _pipelineResolver = new EventPipelineResolver(container);
        }

        /// <inheritdoc />
        public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
        {
            Ensure.Arg.NotNull(@event, nameof(@event));

            Type eventType = @event.GetType();

            EventDescriptor eventDescriptor = _descriptorFactory.CreateDescriptor(eventType);
            IEventContext eventContext = _contextFactory.CreateContext(eventDescriptor, @event);
            
            IEventPipeline pipeline = _pipelineResolver.Resolve(eventType);
            await pipeline.ProcessAsync(eventContext, cancellationToken)
                          .ConfigureAwait(false);
        }
    }
}