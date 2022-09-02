// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

namespace AppCore.EventModel.Store;

/// <summary>
/// Provides event store related metadata keys.
/// </summary>
public static class EventStoreMetadataKeys
{
    /// <summary>
    /// Metadata key identifying the value which indicated whether the event should be persisted.
    /// </summary>
    public const string PersistentMetadataKey = "AppCore.Events.Storage.Persistent";

    /// <summary>
    /// Metadata key identifying the name of the stream of a event.
    /// </summary>
    public const string StreamNameMetadataKey = "AppCore.Events.Storage.StreamName";
}