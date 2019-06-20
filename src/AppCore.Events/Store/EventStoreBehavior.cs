// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;

namespace AppCore.Events.Store
{
    public class EventStoreBehavior<TEvent> : IEventPipelineBehavior<TEvent>
        where TEvent : IEvent
    {
        private readonly IEventStore _store;

        public EventStoreBehavior(IEventStore store)
        {
            Ensure.Arg.NotNull(store, nameof(store));
            _store = store;
        }

        protected virtual bool ShouldStoreEvent(IEventContext<TEvent> context)
        {
            return context.EventDescriptor.GetMetadata(EventStoreMetadataKeys.PersistentMetadataKey, false);
        }

        public async Task HandleAsync(
            IEventContext<TEvent> context,
            EventPipelineDelegate<TEvent> next,
            CancellationToken cancellationToken)
        {
            if ((!context.TryGetFeature(out IEventStoreFeature feature) || !feature.IsPersisted) && ShouldStoreEvent(context))
            {
                await _store.WriteAsync(new[] {context}, cancellationToken)
                            .ConfigureAwait(false);
            }
            else
            {
                await next(context, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}