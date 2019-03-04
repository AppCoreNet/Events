// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Concurrent;
using AppCore.Events.Metadata;

namespace AppCore.Events
{
    internal static class EventContextFactory
    {
        private static readonly Type _eventContextType = typeof(EventContext<>);

        private static readonly ConcurrentDictionary<Type, Delegate> _eventContextFactories =
            new ConcurrentDictionary<Type, Delegate>();

        private static Delegate GetEventContextFactory(Type eventType)
        {
            return _eventContextFactories.GetOrAdd(
                eventType,
                t =>
                {
                    Type eventContextType = _eventContextType.MakeGenericType(t);
                    return TypeActivator.GetFactoryDelegate(eventContextType, typeof(EventDescriptor), eventType);
                });
        }

        public static IEventContext CreateEventContext(
            EventDescriptor eventDescriptor,
            IEvent @event)
        {
            Delegate factory = GetEventContextFactory(eventDescriptor.EventType);
            return (IEventContext) factory.DynamicInvoke(eventDescriptor, @event);
        }
    }
}