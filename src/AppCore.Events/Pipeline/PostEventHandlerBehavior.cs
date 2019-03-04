// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;

namespace AppCore.Events.Pipeline
{
    /// <summary>
    /// Pipeline behavior which invokes <see cref="IPostEventHandler{TEvent}"/>s.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event that is handled.</typeparam>
    public class PostEventHandlerBehavior<TEvent> : IEventPipelineBehavior<TEvent>
        where TEvent : IEvent
    {
        private readonly IEnumerable<IPostEventHandler<TEvent>> _handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostEventHandlerBehavior{TEvent}"/> class.
        /// </summary>
        /// <param name="handlers">An <see cref="IEnumerable{T}"/> of <see cref="IPostEventHandler{TEvent}"/>s.</param>
        /// <exception cref="ArgumentNullException">Argument <paramref name="handlers"/> is <c>null</c>.</exception>
        public PostEventHandlerBehavior(IEnumerable<IPostEventHandler<TEvent>> handlers)
        {
            Ensure.Arg.NotNull(handlers, nameof(handlers));
            _handlers = handlers;
        }

        /// <inheritdoc />
        public async Task HandleAsync(
            IEventContext<TEvent> context,
            EventPipelineDelegate<TEvent> next,
            CancellationToken cancellationToken)
        {
            await next(context, cancellationToken)
                .ConfigureAwait(false);

            foreach (IPostEventHandler<TEvent> handler in _handlers)
            {
                await handler.OnHandledAsync(context, cancellationToken)
                             .ConfigureAwait(false);
            }
        }
    }
}
