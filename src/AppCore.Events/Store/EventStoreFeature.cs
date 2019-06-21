// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using AppCore.Diagnostics;

namespace AppCore.Events.Store
{
    /// <inheritdoc />
    public class EventStoreFeature : IEventStoreFeature
    {
        /// <inheritdoc />
        public IEventStore Store { get; }

        /// <inheritdoc />
        public long Offset { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreFeature"/> class.
        /// </summary>
        /// <param name="store">The <see cref="IEventStore"/> of the event.</param>
        /// <param name="offset">The offset of the event.</param>
        public EventStoreFeature(IEventStore store, long offset)
        {
            Ensure.Arg.NotNull(store, nameof(store));
            Ensure.Arg.InRange(offset, 0, long.MaxValue, nameof(offset));

            Store = store;
            Offset = offset;
        }
    }
}