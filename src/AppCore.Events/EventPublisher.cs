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
        private readonly IEventDescriptorFactory _eventDescriptorFactory;
        private readonly IEventContextAccessor _eventContextAccessor;
        private readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> used to resolve handlers and behaviors.</param>
        /// <param name="eventDescriptorFactory">The factory for <see cref="EventDescriptor"/>'s.</param>
        /// <param name="eventContextAccessor">The accessor for the current <see cref="IEventContext"/>.</param>
        /// <exception cref="ArgumentNullException">Argument <paramref name="container"/> is <c>null</c>.</exception>
        public EventPublisher(
            IContainer container,
            IEventDescriptorFactory eventDescriptorFactory,
            IEventContextAccessor eventContextAccessor = null)
        {
            Ensure.Arg.NotNull(eventDescriptorFactory, nameof(eventDescriptorFactory));
            Ensure.Arg.NotNull(container, nameof(container));

            _eventDescriptorFactory = eventDescriptorFactory;
            _eventContextAccessor = eventContextAccessor;
            _container = container;
        }

        /// <inheritdoc />
        public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
        {
            Ensure.Arg.NotNull(@event, nameof(@event));

            Type eventType = @event.GetType();
            EventDescriptor eventDescriptor = _eventDescriptorFactory.CreateDescriptor(eventType);
            IEventContext eventContext = EventContextFactory.CreateEventContext(eventDescriptor, @event);
            
            if (_eventContextAccessor != null)
                _eventContextAccessor.EventContext = eventContext;

            try
            {
                IEventPipeline pipeline = EventPipelineFactory.CreateEventPipeline(eventType, _container);
                await pipeline.InvokeAsync(eventContext, cancellationToken)
                              .ConfigureAwait(false);
            }
            finally
            {
                if (_eventContextAccessor != null)
                    _eventContextAccessor.EventContext = null;
            }
        }
    }
}