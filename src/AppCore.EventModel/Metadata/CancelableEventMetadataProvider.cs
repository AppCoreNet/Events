// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Reflection;
using AppCore.EventModel.Pipeline;

namespace AppCore.EventModel.Metadata;

/// <summary>
/// Provides metadata for cancelable events.
/// </summary>
public class CancelableEventMetadataProvider : IEventMetadataProvider
{
    /// <inheritdoc />
    public void GetMetadata(Type eventType, IDictionary<string, object> metadata)
    {
        bool isCancelable = eventType.GetTypeInfo()
                                     .GetCustomAttribute<CancelableAttribute>() != null;

        if (isCancelable)
            metadata.Add(CancelableEventBehavior.IsCancelableMetadataKey, true);
    }
}