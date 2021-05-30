// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using AppCore.Logging;

namespace AppCore.EventModel.Pipeline
{
    /// <summary>
    /// Pipeline behavior which invokes <see cref="IPreEventHandler{TEvent}"/>s.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event that is handled.</typeparam>
    public class PreEventHandlerBehavior<TEvent> : IEventPipelineBehavior<TEvent>
        where TEvent : IEvent
    {
        private readonly List<IPreEventHandler<TEvent>> _handlers;
        private readonly ILogger<PreEventHandlerBehavior<TEvent>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreEventHandlerBehavior{TEvent}"/> class.
        /// </summary>
        /// <param name="handlers">An <see cref="IEnumerable{T}"/> of <see cref="IPreEventHandler{TEvent}"/>s.</param>
        /// <param name="logger">The <see cref="ILogger{TCategory}"/>.</param>
        /// <exception cref="ArgumentNullException">Argument <paramref name="handlers"/> is <c>null</c>.</exception>
        public PreEventHandlerBehavior(
            IEnumerable<IPreEventHandler<TEvent>> handlers,
            ILogger<PreEventHandlerBehavior<TEvent>> logger)
        {
            Ensure.Arg.NotNull(handlers, nameof(handlers));
            Ensure.Arg.NotNull(logger, nameof(logger));

            _handlers = handlers.ToList();
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task HandleAsync(
            IEventContext<TEvent> context,
            EventPipelineDelegate<TEvent> next,
            CancellationToken cancellationToken)
        {
            foreach (IPreEventHandler<TEvent> handler in _handlers)
            {
                _logger.InvokingPreEventHandler(typeof(TEvent), handler.GetType());

                await handler.OnHandlingAsync(context, cancellationToken)
                             .ConfigureAwait(false);
            }

            await next(context, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
