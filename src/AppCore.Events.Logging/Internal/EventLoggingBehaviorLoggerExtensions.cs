// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using AppCore.Logging;

namespace AppCore.Events.Logging
{
    internal static class EventLoggingBehaviorLoggerExtensions
    {
        private static readonly LoggerEventDelegate<object> _eventHandling =
            LoggerEvent.Define<object>(
                LogLevel.Trace,
                EventLoggingBehaviorLogEventIds.EventHandling,
                "Publishing event {event} ...");

        private static readonly LoggerEventDelegate<object> _eventHandled =
            LoggerEvent.Define<object>(
                LogLevel.Debug,
                EventLoggingBehaviorLogEventIds.EventHandled,
                "Successfully published event {event}");

        private static readonly LoggerEventDelegate<object> _eventFailed =
            LoggerEvent.Define<object>(
                LogLevel.Error,
                EventLoggingBehaviorLogEventIds.EventFailed,
                "Error publishing event {event}");

        public static void EventHandling(this ILogger logger, IEventContext context)
        {
            _eventHandling(logger, context.Event);
        }

        public static void EventHandled(this ILogger logger, IEventContext context)
        {
            _eventHandled(logger, context.Event);
        }

        public static void EventFailed(this ILogger logger, IEventContext context, Exception exception)
        {
            _eventFailed(logger, context.Event, exception: exception);
        }
    }
}