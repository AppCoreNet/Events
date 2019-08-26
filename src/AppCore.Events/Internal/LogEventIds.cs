// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using AppCore.Logging;

namespace AppCore.Events
{
    internal class LogEventIds
    {
        public static readonly LogEventId PipelineProcessing = new LogEventId(0, nameof(PipelineProcessing));

        public static readonly LogEventId PipelineProcessed = new LogEventId(1, nameof(PipelineProcessed));

        public static readonly LogEventId PipelineFailed = new LogEventId(2, nameof(PipelineFailed));

        public static readonly LogEventId PipelineShortCircuited = new LogEventId(3, nameof(PipelineShortCircuited));

        public static readonly LogEventId InvokingPreEventHandlers = new LogEventId(4, nameof(InvokingPreEventHandlers));

        public static readonly LogEventId InvokingPostEventHandlers = new LogEventId(4, nameof(InvokingPostEventHandlers));
    }
}
