// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;
using AppCore.EventModel.Metadata;

namespace AppCore.EventModel.Pipeline;

internal static class CancelableEventBehavior
{
    internal const string IsCancelableMetadataKey = "IsCancelable";
}

/// <summary>
/// Implements cancellation support for events. The event must be decorated with the
/// <see cref="CancelableAttribute"/>.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public class CancelableEventBehavior<TEvent> : IEventPipelineBehavior<TEvent>
    where TEvent : IEvent
{
    /// <inheritdoc />
    public async Task HandleAsync(
        IEventContext<TEvent> context,
        EventPipelineDelegate<TEvent> next,
        CancellationToken cancellationToken)
    {
        bool isCancelable = context.EventDescriptor.GetMetadata(
            CancelableEventBehavior.IsCancelableMetadataKey,
            false);

        if (isCancelable)
        {
            var cts = new CancellationTokenSource();
            cancellationToken.Register(() => cts.Cancel());

            context.AddFeature<ICancelableEventFeature>(new CancelableEventFeature(cts));
            cancellationToken = cts.Token;
        }

        await next(context, cancellationToken)
            .ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();
    }
}