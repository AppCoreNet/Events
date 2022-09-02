// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;

namespace AppCore.EventModel.Store
{
    /// <summary>
    /// Decorates an event to be persisted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PersistentAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the stream where the event is persisted.
        /// </summary>
        public string? StreamName { get; set; }
    }
}