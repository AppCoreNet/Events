// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Diagnostics;
using AppCore.EventModel.Pipeline;
using AppCore.Logging;

namespace AppCore.EventModel.Logging
{
    /// <summary>
    /// Provides a event pipeline behavior which logs events.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event that is handled.</typeparam>
    public class EventLoggingBehavior<TEvent> : IEventPipelineBehavior<TEvent>
        where TEvent : IEvent
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLoggingBehavior{TEvent}"/> class.
        /// </summary>
        /// <param name="logger">The logger instance used to log events.</param>
        public EventLoggingBehavior(ILogger<IEventPublisher> logger)
        {
            Ensure.Arg.NotNull(logger, nameof(logger));
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task HandleAsync(
            IEventContext<TEvent> context,
            EventPipelineDelegate<TEvent> next,
            CancellationToken cancellationToken)
        {
            try
            {
                await next(context, cancellationToken)
                    .ConfigureAwait(false);

                _logger.EventHandled(context);
            }
            catch (Exception exception)
            {
                _logger.EventFailed(context, exception);
                throw;
            }
        }
    }
}
