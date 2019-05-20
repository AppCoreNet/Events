// Licensed under the MIT License.
// Copyright (c) 2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Events.Pipeline
{
    /// <summary>
    /// Represents an event pipeline.
    /// </summary>
    public interface IEventPipeline
    {
        /// <summary>
        /// Publishes an event.
        /// </summary>
        /// <param name="context">The event context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous event operation.</returns>
        Task PublishAsync(IEventContext context, CancellationToken cancellationToken);
    }

    /// <inheritdoc />
    public interface IEventPipeline<in TEvent> : IEventPipeline
        where TEvent : IEvent
    {
        /// <summary>
        /// Publishes an event.
        /// </summary>
        /// <param name="context">The event context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous event operation.</returns>
        Task PublishAsync(IEventContext<TEvent> context, CancellationToken cancellationToken);
    }
}
