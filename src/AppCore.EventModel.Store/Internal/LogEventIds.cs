// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace AppCore.EventModel.Store;

internal class LogEventIds
{
    public static readonly EventId ReadingEvents = new EventId(0, nameof(ReadingEvents));

    public static readonly EventId PublishingEvents = new EventId(1, nameof(PublishingEvents));

    public static readonly EventId PublishedEvents = new EventId(2, nameof(PublishedEvents));

    public static readonly EventId StoringEvent = new EventId(3, nameof(StoringEvent));

    public static readonly EventId PublishingStoredEvents = new EventId(4, nameof(PublishingStoredEvents));

    public static readonly EventId PublishingStoredEventsFailed =
        new EventId(4, nameof(PublishingStoredEventsFailed));
}