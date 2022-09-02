// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Generic;

namespace AppCore.EventModel.Metadata;

/// <summary>
/// Represents a type which provides metadata for events.
/// </summary>
public interface IEventMetadataProvider
{
    /// <summary>
    /// Gets the metadata for the specified <paramref name="eventType"/>.
    /// </summary>
    /// <param name="eventType">The type of the event.</param>
    /// <param name="metadata">The dictionary which should be populated with metadata.</param>
    void GetMetadata(Type eventType, IDictionary<string, object> metadata);
}