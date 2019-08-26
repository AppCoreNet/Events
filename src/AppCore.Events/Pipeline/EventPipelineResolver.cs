// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Concurrent;
using AppCore.DependencyInjection;

namespace AppCore.Events.Pipeline
{
    public sealed class EventPipelineResolver
    {
        private readonly IContainer _container;

        private readonly ConcurrentDictionary<Type, IEventPipeline> _pipelines =
            new ConcurrentDictionary<Type, IEventPipeline>(1, 32);

        public EventPipelineResolver(IContainer container)
        {
            _container = container;
        }

        public IEventPipeline Resolve(Type eventType)
        {
            return _pipelines.GetOrAdd(
                eventType,
                key => (IEventPipeline) _container.Resolve(typeof(IEventPipeline<>).MakeGenericType(key)));
        }
    }
}
