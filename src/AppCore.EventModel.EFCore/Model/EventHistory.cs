// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

namespace AppCore.EventModel.EntityFrameworkCore.Model
{
    /// <summary>
    /// Represents the database entity for a processed event.
    /// </summary>
    public class EventHistory
    {
        /// <summary>
        /// Gets or sets the event offset (sequence number).
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// Gets or sets the topic of the event.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the content type of the event data.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the event data.
        /// </summary>
        public byte[] Data { get; set; }
    }
}