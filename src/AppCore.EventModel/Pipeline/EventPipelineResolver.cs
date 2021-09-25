// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Concurrent;
using AppCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.EventModel.Pipeline
{
    /// <summary>
    /// Represents a resolver for <see cref="IEventPipeline{TEvent}"/> instances.
    /// </summary>
    public sealed class EventPipelineResolver : IEventPipelineResolver
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentDictionary<Type, IEventPipeline> _pipelines =
            new ConcurrentDictionary<Type, IEventPipeline>(1, 32);

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPipelineResolver"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to resolve a <see cref="IEventPipeline{TEvent}"/>.</param>
        public EventPipelineResolver(IServiceProvider serviceProvider)
        {
            Ensure.Arg.NotNull(serviceProvider, nameof(serviceProvider));
            _serviceProvider = serviceProvider;
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
                key => (IEventPipeline) _serviceProvider.GetRequiredService(typeof(IEventPipeline<>).MakeGenericType(key)));
        }
    }
}
