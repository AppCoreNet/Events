// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.DependencyInjection;

namespace AppCore.Events.Pipeline
{
    internal class EventPipeline<TEvent> : IEventPipeline
        where TEvent : IEvent
    {
        private readonly IEnumerable<IEventPipelineBehavior<TEvent>> _behaviors;
        private readonly IEnumerable<IEventHandler<TEvent>> _handlers;

        public EventPipeline(IContainer container)
        {
            _behaviors = container.ResolveAll<IEventPipelineBehavior<TEvent>>();
            _handlers = container.ResolveAll<IEventHandler<TEvent>>();
        }

        public Task InvokeAsync(IEventContext eventContext, CancellationToken cancellationToken)
        {
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
                       (IEventContext<TEvent>)eventContext,
                       cancellationToken);
        }
    }
}