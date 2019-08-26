﻿// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;

namespace AppCore.Events.Storage
{
    /// <summary>
    /// Event pipeline behavior which stores events.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    public class EventStoreBehavior<TEvent> : IEventPipelineBehavior<TEvent>
        where TEvent : IEvent
    {
        private readonly IEventStore _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreBehavior{TEvent}"/> class.
        /// </summary>
        /// <param name="store">The <see cref="IEventStore"/>.</param>
        public EventStoreBehavior(IEventStore store)
        {
            Ensure.Arg.NotNull(store, nameof(store));
            _store = store;
        }

        /// <summary>
        /// Can be overridden to get a value indicating whether the event should be stored.
        /// </summary>
        /// <param name="context">The context of the event.</param>
        /// <returns><c>true</c> if the event should be stored; <c>false</c> otherwise.</returns>
        protected virtual bool ShouldStoreEvent(IEventContext<TEvent> context)
        {
            return context.EventDescriptor.GetMetadata(EventStoreMetadataKeys.PersistentMetadataKey, false);
        }

        /// <inheritdoc />
        public async Task HandleAsync(
            IEventContext<TEvent> context,
            EventPipelineDelegate<TEvent> next,
            CancellationToken cancellationToken)
        {
            if (!context.IsFromEventStore() && ShouldStoreEvent(context))
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