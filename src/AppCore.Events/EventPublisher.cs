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
        private readonly IContainer _container;
        private readonly IEventDescriptorFactory _eventDescriptorFactory;
        private readonly IEventContextFactory _eventContextFactory;
        private readonly IEventContextAccessor _eventContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> used to resolve handlers and behaviors.</param>
        /// <param name="eventDescriptorFactory">The factory for <see cref="EventDescriptor"/>.</param>
        /// <param name="eventContextFactory">The factory for <see cref="IEventContext"/>'s.</param>
        /// <param name="eventContextAccessor">The accessor for the current <see cref="IEventContext"/>.</param>
        /// <exception cref="ArgumentNullException">Argument <paramref name="container"/> is <c>null</c>.</exception>
        public EventPublisher(
            IContainer container,
            IEventDescriptorFactory eventDescriptorFactory,
            IEventContextFactory eventContextFactory,
            IEventContextAccessor eventContextAccessor = null)
        {
            Ensure.Arg.NotNull(eventContextFactory, nameof(eventContextFactory));
            Ensure.Arg.NotNull(container, nameof(container));

            _container = container;
            _eventDescriptorFactory = eventDescriptorFactory;
            _eventContextFactory = eventContextFactory;
            _eventContextAccessor = eventContextAccessor;
        }

        /// <inheritdoc />
        public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
        {
            Ensure.Arg.NotNull(@event, nameof(@event));

            Type eventType = @event.GetType();

            EventDescriptor eventDescriptor = _eventDescriptorFactory.CreateDescriptor(eventType);
            IEventContext eventContext = _eventContextFactory.CreateContext(eventDescriptor, @event);
            
            if (_eventContextAccessor != null)
                _eventContextAccessor.EventContext = eventContext;

            try
            {
                var pipeline = (IEventPipeline) _container.Resolve(typeof(IEventPipeline<>).MakeGenericType(eventType));
                await pipeline.PublishAsync(eventContext, cancellationToken)
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