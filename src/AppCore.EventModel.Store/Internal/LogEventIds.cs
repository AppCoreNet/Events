// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using AppCore.Logging;

namespace AppCore.Events.Storage
{
    internal class LogEventIds
    {
        public static readonly LogEventId ReadingEvents = new LogEventId(0, nameof(ReadingEvents));

        public static readonly LogEventId PublishingEvents = new LogEventId(1, nameof(PublishingEvents));

        public static readonly LogEventId PublishedEvents = new LogEventId(2, nameof(PublishedEvents));

        public static readonly LogEventId StoringEvent = new LogEventId(3, nameof(StoringEvent));

        public static readonly LogEventId PublishingStoredEvents = new LogEventId(4, nameof(PublishingStoredEvents));

        public static readonly LogEventId PublishingStoredEventsFailed =
            new LogEventId(4, nameof(PublishingStoredEventsFailed));
    }
}
