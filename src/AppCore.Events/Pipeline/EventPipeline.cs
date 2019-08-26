// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using AppCore.Logging;

namespace AppCore.Events.Pipeline
{
    /// <summary>
    /// Provides the default implementation of <see cref="IEventPipeline{TEvent}"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    public class EventPipeline<TEvent> : IEventPipeline<TEvent>
        where TEvent : IEvent
    {
        private readonly List<IEventPipelineBehavior<TEvent>> _behaviors;
        private readonly List<IEventHandler<TEvent>> _handlers;
        private readonly ILogger<EventPipeline<TEvent>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPipeline{TEvent}"/> class.
        /// </summary>
        /// <param name="behaviors">The pipeline behaviors.</param>
        /// <param name="handlers">The event handlers.</param>
        /// <param name="logger"></param>
        public EventPipeline(
            IEnumerable<IEventPipelineBehavior<TEvent>> behaviors,
            IEnumerable<IEventHandler<TEvent>> handlers,
            ILogger<EventPipeline<TEvent>> logger)
        {
            Ensure.Arg.NotNull(behaviors, nameof(behaviors));
            Ensure.Arg.NotNull(handlers, nameof(handlers));
            Ensure.Arg.NotNull(logger, nameof(logger));

            _behaviors = behaviors.ToList();
            _handlers = handlers.ToList();
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task ProcessAsync(IEventContext<TEvent> eventContext, CancellationToken cancellationToken)
        {
            Ensure.Arg.NotNull(eventContext, nameof(eventContext));

            var handlerInvoked = false;
            async Task Handler(IEventContext<TEvent> c, CancellationToken ct)
            {
                handlerInvoked = true;
                foreach (IEventHandler<TEvent> handler in _handlers)
                {
                    await handler.HandleAsync(c, ct);
                    ct.ThrowIfCancellationRequested();
                }
            }

            _logger.PipelineProcessing(typeof(TEvent));

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                IEventPipelineBehavior<TEvent> current = null;
                await ((IEnumerable<IEventPipelineBehavior<TEvent>>)_behaviors)
                      .Reverse()
                      .Aggregate(
                          (EventPipelineDelegate<TEvent>) Handler,
                          (next, behavior) => async (e, ct) =>
                          {
                              current = behavior;
                              await behavior.HandleAsync(e, next, ct);
                          }
                      )(eventContext, cancellationToken)
                      .ConfigureAwait(false);

                if (handlerInvoked)
                {
                    _logger.PipelineProcessed(typeof(TEvent), stopwatch.Elapsed);
                }
                else
                {
                    _logger.PipelineShortCircuited(typeof(TEvent), current.GetType(), stopwatch.Elapsed);
                }
            }
            catch (Exception error)
            {
                _logger.PipelineFailed(typeof(TEvent), stopwatch.Elapsed, error);
            }
        }

        Task IEventPipeline.ProcessAsync(IEventContext context, CancellationToken cancellationToken)
        {
            return ProcessAsync((IEventContext<TEvent>) context, cancellationToken);
        }
    }
}