// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using AppCore.Logging;

// ReSharper disable once CheckNamespace
namespace AppCore.EventModel.Logging
{
    internal static class EventLoggingExtensions
    {
        private static readonly LoggerEventDelegate<object> _eventHandled =
            LoggerEvent.Define<object>(
                LogLevel.Info,
                EventLoggingBehaviorLogEventIds.EventHandled,
                "Successfully published event {event}");

        private static readonly LoggerEventDelegate<object> _eventFailed =
            LoggerEvent.Define<object>(
                LogLevel.Error,
                EventLoggingBehaviorLogEventIds.EventFailed,
                "Error publishing event {event}");

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