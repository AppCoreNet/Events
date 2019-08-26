// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Reflection;
using AppCore.Events.Metadata;

namespace AppCore.Events.Storage
{
    /// <summary>
    /// Provides metadata for persistent events.
    /// </summary>
    public class EventStoreMetadataProvider : IEventMetadataProvider
    {
        /// <inheritdoc />
        public void GetMetadata(Type eventType, IDictionary<string, object> metadata)
        {
            TypeInfo eventTypeInfo = eventType.GetTypeInfo();

            var persistentAttribute = eventTypeInfo.GetCustomAttribute<PersistentAttribute>();
            if (persistentAttribute != null)
            {
                metadata.Add(EventStoreMetadataKeys.PersistentMetadataKey, true);

                if (!String.IsNullOrEmpty(persistentAttribute.StreamName))
                {
                    metadata.Add(EventStoreMetadataKeys.StreamNameMetadataKey, persistentAttribute.StreamName);
                }
            }
        }
    }
}