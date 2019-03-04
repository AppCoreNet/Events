// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Concurrent;
using AppCore.DependencyInjection;

namespace AppCore.Events.Pipeline
{
    internal static class EventPipelineFactory
    {
        private static readonly Type _eventPipelineType = typeof(EventPipeline<>);

        private static readonly ConcurrentDictionary<Type, Type> _eventPipelineTypes =
            new ConcurrentDictionary<Type, Type>();

        private static Type GetEventPipelineType(Type eventType)
        {
            return _eventPipelineTypes.GetOrAdd(eventType, t => _eventPipelineType.MakeGenericType(t));
        }

        public static IEventPipeline CreateEventPipeline(Type eventType, IContainer container)
        {
            Type eventContextType = GetEventPipelineType(eventType);
            return (IEventPipeline) Activator.CreateInstance(eventContextType, container);
        }
    }
}