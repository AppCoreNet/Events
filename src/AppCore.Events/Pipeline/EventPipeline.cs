// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;

namespace AppCore.Events.Pipeline
{
    /// <summary>
    /// Provides the default implementation of <see cref="IEventPipeline{TEvent}"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    public class EventPipeline<TEvent> : IEventPipeline<TEvent>
        where TEvent : IEvent
    {
        private readonly IEnumerable<IEventPipelineBehavior<TEvent>> _behaviors;
        private readonly IEnumerable<IEventHandler<TEvent>> _handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPipeline{TEvent}"/> class.
        /// </summary>
        /// <param name="behaviors">The pipeline behaviors.</param>
        /// <param name="handlers">The event handlers.</param>
        public EventPipeline(IEnumerable<IEventPipelineBehavior<TEvent>> behaviors, IEnumerable<IEventHandler<TEvent>> handlers)
        {
            Ensure.Arg.NotNull(behaviors, nameof(behaviors));
            Ensure.Arg.NotNull(handlers, nameof(handlers));

            _behaviors = behaviors;
            _handlers = handlers;
        }

        /// <inheritdoc />
        public Task PublishAsync(IEventContext<TEvent> eventContext, CancellationToken cancellationToken)
        {
            Ensure.Arg.NotNull(eventContext, nameof(eventContext));

            async Task Handler(IEventContext<TEvent> c, CancellationToken ct)
            {
                foreach (IEventHandler<TEvent> handler in _handlers)
                {
                    await handler.HandleAsync(c, ct)
                                 .ConfigureAwait(false);

                    ct.ThrowIfCancellationRequested();
                }
            }

            return _behaviors
                   .Reverse()
                   .Aggregate(
                       (EventPipelineDelegate<TEvent>) Handler,
                       (next, behavior) => (e, ct) => behavior.HandleAsync(e, next, ct))(
                       eventContext,
                       cancellationToken);
        }

        Task IEventPipeline.PublishAsync(IEventContext context, CancellationToken cancellationToken)
        {
            return PublishAsync((IEventContext<TEvent>) context, cancellationToken);
        }
    }
}