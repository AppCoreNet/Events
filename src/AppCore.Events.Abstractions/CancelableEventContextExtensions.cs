// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using AppCore.Diagnostics;
using AppCore.Events.Pipeline;

namespace AppCore.Events
{
    /// <summary>
    /// Provides extension methods for the <see cref="IEventContext"/> type.
    /// </summary>
    public static class CancelableEventContextExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the event is cancelable.
        /// </summary>
        /// <param name="context">The <see cref="IEventContext"/>.</param>
        /// <returns><c>true</c> if the event can be canceled; <c>false</c> otherwise.</returns>
        public static bool IsCancelable(this IEventContext context)
        {
            Ensure.Arg.NotNull(context, nameof(context));
            return context.HasFeature<ICancelableEventFeature>();
        }

        /// <summary>
        /// Cancels the event.
        /// </summary>
        /// <param name="context">The <see cref="IEventContext"/>.</param>
        public static void Cancel(this IEventContext context)
        {
            Ensure.Arg.NotNull(context, nameof(context));
            var feature = context.GetFeature<ICancelableEventFeature>();
            feature.Cancel();
        }
    }
}