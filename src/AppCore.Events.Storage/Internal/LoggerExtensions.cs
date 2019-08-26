// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using AppCore.Logging;

namespace AppCore.Events.Storage
{
    internal static class LoggerExtensions
    {
        private static readonly LoggerEventDelegate<string, long> _readingEvents =
            LoggerEvent.Define<string, long>(
                LogLevel.Trace,
                LogEventIds.ReadingEvents,
                "Reading events from stream '{streamName}' at offset #{streamOffset}.");

        private static readonly LoggerEventDelegate<int, string, long, long> _publishingEvents =
            LoggerEvent.Define<int, string, long, long>(
                LogLevel.Trace,
                LogEventIds.PublishingEvents,
                "Publishing {eventCount} events from stream '{streamName}' offset #{firstOffset}-#{lastOffset}.");

        private static readonly LoggerEventDelegate<int, string> _publishedEvents =
            LoggerEvent.Define<int, string>(
                LogLevel.Debug,
                LogEventIds.PublishedEvents,
                "Published {eventCount} events from stream '{streamName}'.");

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
    }
}