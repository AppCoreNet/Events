// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.DependencyInjection;
using AppCore.Events.Pipeline;

namespace AppCore.Events.Store
{
    public class EventStorePublisher : IEventStorePublisher
    {
        private readonly IEventStore _store;
        private readonly IContainer _container;

        public EventStorePublisher(IEventStore store, IContainer container)
        {
            _store = store;
            _container = container;
        }

        public async Task PublishPendingAsync(CancellationToken cancellationToken)
        {
            string streamName = string.Empty;

            IEnumerable<IEventContext> events = await _store.ReadAsync(
                streamName,
                -2,
                64,
                Timeout.InfiniteTimeSpan,
                cancellationToken);

            var pipelines = new Dictionary<Type, IEventPipeline>();

            long lastOffset = -1;
            foreach (IEventContext eventContext in events)
            {
                Type eventType = eventContext.EventDescriptor.EventType;
                if (!pipelines.TryGetValue(eventType, out IEventPipeline pipeline))
                {
                    pipeline = (IEventPipeline) _container.Resolve(typeof(IEventPipeline<>).MakeGenericType(eventType));
                    pipelines.Add(eventType, pipeline);
                }

                await pipeline.PublishAsync(eventContext, cancellationToken);
                lastOffset = eventContext.GetEventStoreOffset();
            }

            if (lastOffset != -1 && _store is ICommittableEventStore committableEventStore)
            {
                await committableEventStore.CommitAsync(streamName, lastOffset, cancellationToken);
            }
        }
    }
}
