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
    /// Pipeline behavior which invokes <see cref="IPreEventHandler{TEvent}"/>s.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event that is handled.</typeparam>
    public class PreEventHandlerBehavior<TEvent> : IEventPipelineBehavior<TEvent>
        where TEvent : IEvent
    {
        private readonly IEnumerable<IPreEventHandler<TEvent>> _handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreEventHandlerBehavior{TEvent}"/> class.
        /// </summary>
        /// <param name="handlers">An <see cref="IEnumerable{T}"/> of <see cref="IPreEventHandler{TEvent}"/>s.</param>
        /// <exception cref="ArgumentNullException">Argument <paramref name="handlers"/> is <c>null</c>.</exception>
        public PreEventHandlerBehavior(IEnumerable<IPreEventHandler<TEvent>> handlers)
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
            foreach (IPreEventHandler<TEvent> handler in _handlers)
            {
                await handler.OnHandlingAsync(context, cancellationToken)
                             .ConfigureAwait(false);
            }

            await next(context, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
