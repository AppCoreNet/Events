// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Concurrent;
using AppCore.DependencyInjection;
using AppCore.Diagnostics;

namespace AppCore.EventModel.Pipeline
{
    /// <summary>
    /// Represents a resolver for <see cref="IEventPipeline{TEvent}"/> instances.
    /// </summary>
    public sealed class EventPipelineResolver : IEventPipelineResolver
    {
        private readonly IContainer _container;

        private readonly ConcurrentDictionary<Type, IEventPipeline> _pipelines =
            new ConcurrentDictionary<Type, IEventPipeline>(1, 32);

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPipelineResolver"/> class.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> used to resolve a <see cref="IEventPipeline{TEvent}"/>.</param>
        public EventPipelineResolver(IContainer container)
        {
            Ensure.Arg.NotNull(container, nameof(container));
            _container = container;
        }

        /// <summary>
        /// Resolves an <see cref="IEventPipeline{TEvent}"/> for the specified <paramref name="eventType"/>.
        /// </summary>
        /// <param name="eventType">The type of the event.</param>
        /// <returns>The <see cref="IEventPipeline{TEvent}"/> instance.</returns>
        public IEventPipeline Resolve(Type eventType)
        {
            Ensure.Arg.NotNull(eventType, nameof(eventType));

            return _pipelines.GetOrAdd(
                eventType,
                key => (IEventPipeline) _container.Resolve(typeof(IEventPipeline<>).MakeGenericType(key)));
        }
    }
}
