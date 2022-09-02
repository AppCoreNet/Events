// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace AppCore.EventModel.Store;

internal static class LoggerExtensions
{
    private static readonly Action<ILogger, string, long, Exception?> _readingEvents =
        LoggerMessage.Define<string, long>(
            LogLevel.Trace,
            LogEventIds.ReadingEvents,
            "Reading events from stream '{streamName}' at offset #{streamOffset} ...");

    private static readonly Action<ILogger, int, string, long, long, Exception?> _publishingEvents =
        LoggerMessage.Define<int, string, long, long>(
            LogLevel.Trace,
            LogEventIds.PublishingEvents,
            "Publishing {eventCount} events from stream '{streamName}' offset #{firstOffset}-#{lastOffset} ...");

    private static readonly Action<ILogger, int, string, Exception?> _publishedEvents =
        LoggerMessage.Define<int, string>(
            LogLevel.Debug,
            LogEventIds.PublishedEvents,
            "Successfully published {eventCount} events from stream '{streamName}'.");

    private static readonly Action<ILogger, string, Exception?> _storingEvent =
        LoggerMessage.Define<string>(
            LogLevel.Trace,
            LogEventIds.StoringEvent,
            "Writing event {eventType} to store and skipping remaining pipeline ...");

    private static readonly Action<ILogger, Exception?> _publishingStoredEvents =
        LoggerMessage.Define(
            LogLevel.Trace,
            LogEventIds.PublishingStoredEvents,
            "Publishing events from event store ...");

    private static readonly Action<ILogger, Exception?> _publishingStoredEventsFailed =
        LoggerMessage.Define(
            LogLevel.Error,
            LogEventIds.PublishingStoredEventsFailed,
            "Publishing events from event store failed.");

    public static void ReadingEvents(this ILogger logger, string streamName, long streamOffset)
    {
        _readingEvents(logger, streamName, streamOffset, null);
    }

    public static void PublishingEvents(this ILogger logger, int eventCount, string streamName, long firstOffset, long lastOffset)
    {
        _publishingEvents(logger, eventCount, streamName, firstOffset, lastOffset, null);
    }

    public static void PublishedEvents(this ILogger logger, int eventCount, string streamName)
    {
        _publishedEvents(logger, eventCount, streamName, null);
    }

    public static void StoringEvent(this ILogger logger, Type eventType)
    {
        _storingEvent(logger, eventType.GetDisplayName(), null);
    }

    public static void PublishingStoredEvents(this ILogger logger)
    {
        _publishingStoredEvents(logger, null);
    }

    public static void PublishingStoredEventsFailed(this ILogger logger, Exception error)
    {
        _publishingStoredEventsFailed(logger, error);
    }
}