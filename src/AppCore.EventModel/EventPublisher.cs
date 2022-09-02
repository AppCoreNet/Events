// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;
using AppCore.EventModel.Queue;

namespace AppCore.EventModel
{
    /// <summary>
    /// Provides the default event publisher implementation.
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly IEventDescriptorFactory _descriptorFactory;
        private readonly IEventContextFactory _contextFactory;
        private readonly IEventPipelineResolver _pipelineResolver;
        private readonly IEventQueue? _queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="descriptorFactory">The factory for <see cref="EventDescriptor"/>.</param>
        /// <param name="contextFactory">The factory for <see cref="IEventContext"/>'s.</param>
        /// <param name="pipelineResolver">The event pipeline resolver.</param>
        /// <param name="queue">The <see cref="IEventQueue"/>.</param>
        /// <exception cref="ArgumentNullException">Some argument is <c>null</c>.</exception>
        public EventPublisher(
            IEventDescriptorFactory descriptorFactory,
            IEventContextFactory contextFactory,
            IEventPipelineResolver pipelineResolver,
            IEventQueue? queue = null)
        {
            Ensure.Arg.NotNull(descriptorFactory, nameof(descriptorFactory));
            Ensure.Arg.NotNull(contextFactory, nameof(contextFactory));
            Ensure.Arg.NotNull(pipelineResolver, nameof(pipelineResolver));

            _descriptorFactory = descriptorFactory;
            _contextFactory = contextFactory;
            _pipelineResolver = pipelineResolver;
            _queue = queue;
        }

        /// <inheritdoc />
        public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
        {
            Ensure.Arg.NotNull(@event, nameof(@event));

            Type eventType = @event.GetType();

            EventDescriptor eventDescriptor = _descriptorFactory.CreateDescriptor(eventType);
            IEventContext eventContext = _contextFactory.CreateContext(eventDescriptor, @event);

            if (_queue != null)
            {
                await _queue.WriteAsync(new[] {eventContext}, cancellationToken)
                            .ConfigureAwait(false);
            }
            else
            {
                IEventPipeline pipeline = _pipelineResolver.Resolve(eventType);
                await pipeline.ProcessAsync(eventContext, cancellationToken)
                              .ConfigureAwait(false);
            }
        }
    }
}