// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AppCore.Diagnostics;

namespace AppCore.Events.Metadata
{
    /// <summary>
    /// Creates new <see cref="EventDescriptor"/> instances.
    /// </summary>
    public class EventDescriptorFactory : IEventDescriptorFactory
    {
        private readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, object>> _metadataCache =
            new ConcurrentDictionary<Type, IReadOnlyDictionary<string, object>>();

        private readonly IEnumerable<IEventMetadataProvider> _metadataProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDescriptorFactory"/> class.
        /// </summary>
        /// <param name="metadataProviders">The <see cref="IEnumerable{T}"/> of <see cref="IEventMetadataProvider"/>'s.</param>
        public EventDescriptorFactory(IEnumerable<IEventMetadataProvider> metadataProviders)
        {
            Ensure.Arg.NotNull(metadataProviders, nameof(metadataProviders));
            _metadataProviders = metadataProviders;
        }

        private IReadOnlyDictionary<string, object> GetMetadata(Type eventType)
        {
            return _metadataCache.GetOrAdd(
                eventType,
                t =>
                {
                    var metadata = new Dictionary<string, object>();
                    foreach (IEventMetadataProvider eventMetadataProvider in _metadataProviders)
                    {
                        eventMetadataProvider.GetMetadata(t, metadata);
                    }

                    return new ReadOnlyDictionary<string, object>(metadata);
                });
        }

        /// <inheritdoc />
        public EventDescriptor CreateDescriptor(Type eventType)
        {
            Ensure.Arg.NotNull(eventType, nameof(eventType));
            Ensure.Arg.OfType<IEvent>(eventType, nameof(eventType));

            return new EventDescriptor(eventType, GetMetadata(eventType));
        }
    }
}
