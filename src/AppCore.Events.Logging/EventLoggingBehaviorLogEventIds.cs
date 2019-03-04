// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using AppCore.Logging;

namespace AppCore.Events.Logging
{
    /// <summary>
    /// Provides <see cref="LogEventId"/>'s for logged events.
    /// </summary>
    public static class EventLoggingBehaviorLogEventIds
    {
        /// <summary>
        /// Identifies the log event for a event which is about to be handled.
        /// </summary>
        public static readonly LogEventId EventHandling = new LogEventId(0, nameof(EventHandling));

        /// <summary>
        /// Identifies the log event for a event which has been successfully handled.
        /// </summary>
        public static readonly LogEventId EventHandled = new LogEventId(0, nameof(EventHandled));

        /// <summary>
        /// Identifies the log event for a event which could not be handled.
        /// </summary>
        public static readonly LogEventId EventFailed = new LogEventId(0, nameof(EventFailed));
    }
}