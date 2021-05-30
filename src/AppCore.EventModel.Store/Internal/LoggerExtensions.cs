// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using AppCore.Logging;

// ReSharper disable once CheckNamespace
namespace AppCore.EventModel.Store
{
    internal static class LoggerExtensions
    {
        private static readonly LoggerEventDelegate<string, long> _readingEvents =
            LoggerEvent.Define<string, long>(
                LogLevel.Trace,
                LogEventIds.ReadingEvents,
                "Reading events from stream '{streamName}' at offset #{streamOffset} ...");

        private static readonly LoggerEventDelegate<int, string, long, long> _publishingEvents =
            LoggerEvent.Define<int, string, long, long>(
                LogLevel.Trace,
                LogEventIds.PublishingEvents,
                "Publishing {eventCount} events from stream '{streamName}' offset #{firstOffset}-#{lastOffset} ...");

        private static readonly LoggerEventDelegate<int, string> _publishedEvents =
            LoggerEvent.Define<int, string>(
                LogLevel.Debug,
                LogEventIds.PublishedEvents,
                "Successfully published {eventCount} events from stream '{streamName}'.");

        private static readonly LoggerEventDelegate<string> _storingEvent =
            LoggerEvent.Define<string>(
                LogLevel.Trace,
                LogEventIds.StoringEvent,
                "Writing event {eventType} to store and skipping remaining pipeline ...");

        private static readonly LoggerEventDelegate _publishingStoredEvents =
            LoggerEvent.Define(
                LogLevel.Trace,
                LogEventIds.PublishingStoredEvents,
                "Publishing events from event store ...");

        private static readonly LoggerEventDelegate _publishingStoredEventsFailed =
            LoggerEvent.Define(
                LogLevel.Error,
                LogEventIds.PublishingStoredEventsFailed,
                "Publishing events from event store failed.");

        public static void ReadingEvents(this ILogger logger, string streamName, long streamOffset)
        {
            _readingEvents(logger, streamName, streamOffset);
        }

        public static void PublishingEvents(this ILogger logger, int eventCount, string streamName, long firstOffset, long lastOffset)
        {
            _publishingEvents(logger, eventCount, streamName, firstOffset, lastOffset);
        }

        public static void PublishedEvents(this ILogger logger, int eventCount, string streamName)
        {
            _publishedEvents(logger, eventCount, streamName);
        }

        public static void StoringEvent(this ILogger logger, Type eventType)
        {
            _storingEvent(logger, eventType.GetDisplayName());
        }

        public static void PublishingStoredEvents(this ILogger logger)
        {
            _publishingStoredEvents(logger);
        }

        public static void PublishingStoredEventsFailed(this ILogger logger, Exception error)
        {
            _publishingStoredEventsFailed(logger, exception:error);
        }
    }
}