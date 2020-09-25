// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

namespace AppCore.Events.EntityFrameworkCore.Data
{
    /// <summary>
    /// Represents the database entity for an event.
    /// </summary>
    public class Event
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