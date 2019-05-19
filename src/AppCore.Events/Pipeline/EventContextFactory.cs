﻿// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Concurrent;
using AppCore.Diagnostics;
using AppCore.Events.Metadata;

namespace AppCore.Events.Pipeline
{
    using EventContextFactoryDelegate = Func<EventDescriptor, IEvent, IEventContext>;

    /// <inheritdoc />
    public class EventContextFactory : IEventContextFactory
    {
        private static readonly Type _eventContextType = typeof(EventContext<>);

        private static readonly ConcurrentDictionary<Type, EventContextFactoryDelegate> _eventContextFactories =
            new ConcurrentDictionary<Type, EventContextFactoryDelegate>();

        private readonly IEventDescriptorFactory _descriptorFactory;

        private static EventContextFactoryDelegate GetEventContextFactory(Type eventType)
        {
            return _eventContextFactories.GetOrAdd(
                eventType,
                t =>
                {
                    Type eventContextType = _eventContextType.MakeGenericType(t);

                    return TypeActivator.GetFactoryDelegate<EventContextFactoryDelegate>(
                        eventContextType,
                        typeof(EventDescriptor),
                        eventType);
                });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventContextFactory"/> class.
        /// </summary>
        /// <param name="descriptorFactory">The <see cref="IEventDescriptorFactory"/>.</param>
        public EventContextFactory(IEventDescriptorFactory descriptorFactory)
        {
            Ensure.Arg.NotNull(descriptorFactory, nameof(descriptorFactory));
            _descriptorFactory = descriptorFactory;
        }

        /// <inheritdoc />
        public IEventContext CreateContext(IEvent @event)
        {
            Ensure.Arg.NotNull(@event, nameof(@event));

            Type eventType = @event.GetType();
            EventDescriptor descriptor = _descriptorFactory.CreateDescriptor(eventType);
            EventContextFactoryDelegate factory = GetEventContextFactory(eventType);
            return factory(descriptor, @event);
        }
    }
}
