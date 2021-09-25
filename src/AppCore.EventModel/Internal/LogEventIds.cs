// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace AppCore.EventModel
{
    internal class LogEventIds
    {
        // EventPipeline
        public static readonly EventId PipelineProcessing = new EventId(0, nameof(PipelineProcessing));

        public static readonly EventId PipelineProcessed = new EventId(1, nameof(PipelineProcessed));

        public static readonly EventId PipelineFailed = new EventId(2, nameof(PipelineFailed));

        public static readonly EventId PipelineShortCircuited = new EventId(3, nameof(PipelineShortCircuited));

        public static readonly EventId InvokingBehavior = new EventId(4, nameof(InvokingBehavior));

        public static readonly EventId InvokingPreEventHandler = new EventId(5, nameof(InvokingPreEventHandler));

        public static readonly EventId InvokingPostEventHandler = new EventId(6, nameof(InvokingPostEventHandler));

        // EventQueuePublisherService
        public static readonly EventId PublishingQueuedEventsFailed =
            new EventId(8, nameof(PublishingQueuedEventsFailed));

        // EventQueuePublisher
        public static readonly EventId DequeuingEvents = new EventId(9, nameof(DequeuingEvents));

        public static readonly EventId PublishingEvents = new EventId(10, nameof(PublishingEvents));

        public static readonly EventId PublishedEvents = new EventId(11, nameof(PublishedEvents));
    }
}
