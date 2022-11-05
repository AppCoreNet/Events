// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace AppCore.EventModel.Logging;

internal static class EventLoggingExtensions
{
    private static readonly Action<ILogger, object, Exception?> _eventHandled =
        LoggerMessage.Define<object>(
            LogLevel.Information,
            EventLoggingBehaviorLogEventIds.EventHandled,
            "Successfully published event {event}");

    private static readonly Action<ILogger, object, Exception?> _eventFailed =
        LoggerMessage.Define<object>(
            LogLevel.Error,
            EventLoggingBehaviorLogEventIds.EventFailed,
            "Error publishing event {event}");

    public static void EventHandled(this ILogger logger, IEventContext context)
    {
        _eventHandled(logger, context.Event, null);
    }

    public static void EventFailed(this ILogger logger, IEventContext context, Exception exception)
    {
        _eventFailed(logger, context.Event, exception);
    }
}