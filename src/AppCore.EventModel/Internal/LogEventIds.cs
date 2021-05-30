// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using AppCore.Logging;

// ReSharper disable once CheckNamespace
namespace AppCore.EventModel
{
    internal class LogEventIds
    {
        // EventPipeline
        public static readonly LogEventId PipelineProcessing = new LogEventId(0, nameof(PipelineProcessing));

        public static readonly LogEventId PipelineProcessed = new LogEventId(1, nameof(PipelineProcessed));

        public static readonly LogEventId PipelineFailed = new LogEventId(2, nameof(PipelineFailed));

        public static readonly LogEventId PipelineShortCircuited = new LogEventId(3, nameof(PipelineShortCircuited));

        public static readonly LogEventId InvokingBehavior = new LogEventId(4, nameof(InvokingBehavior));

        public static readonly LogEventId InvokingPreEventHandler = new LogEventId(5, nameof(InvokingPreEventHandler));

        public static readonly LogEventId InvokingPostEventHandler = new LogEventId(6, nameof(InvokingPostEventHandler));

        // EventQueuePublisherService
        public static readonly LogEventId PublishingQueuedEventsFailed =
            new LogEventId(8, nameof(PublishingQueuedEventsFailed));

        // EventQueuePublisher
        public static readonly LogEventId DequeuingEvents = new LogEventId(9, nameof(DequeuingEvents));

        public static readonly LogEventId PublishingEvents = new LogEventId(10, nameof(PublishingEvents));

        public static readonly LogEventId PublishedEvents = new LogEventId(11, nameof(PublishedEvents));
    }
}
