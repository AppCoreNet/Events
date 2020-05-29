// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;

namespace AppCore.Events.Queue
{
    /// <summary>
    /// Provides options for reading the event queue.
    /// </summary>
    public class EventQueueReadOptions
    {
        /// <summary>
        /// Gets or sets the timeout. Default is <see cref="System.Threading.Timeout.InfiniteTimeSpan"/>.
        /// </summary>
        public TimeSpan Timeout { get; set; } = System.Threading.Timeout.InfiniteTimeSpan;

        /// <summary>
        /// Gets or sets the maximum number of events to read.
        /// </summary>
        public int MaxEventsToRead { get; set; } = 64;
    }
}