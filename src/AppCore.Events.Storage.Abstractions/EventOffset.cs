// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using AppCore.Diagnostics;

namespace AppCore.Events.Storage
{
    /// <summary>
    /// Represents the offset of an event in a store.
    /// </summary>
    public readonly struct EventOffset : IEquatable<EventOffset>
    {
        /// <summary>
        /// Gets the offset value.
        /// </summary>
        public long Value { get; }

        /// <summary>
        /// Specifies the first event.
        /// </summary>
        public static readonly EventOffset Start = new EventOffset(-1);

        /// <summary>
        /// Specifies the first uncommitted event.
        /// </summary>
        public static readonly EventOffset Next = new EventOffset(-2);

        public EventOffset(long value)
        {
            Ensure.Arg.InRange(value, -2, long.MaxValue, nameof(value));
            Value = value;
        }

        public static implicit operator EventOffset(long value)
        {
            return new EventOffset(value);
        }

        /// <inheritdoc />
        public bool Equals(EventOffset other)
        {
            return Value == other.Value;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is EventOffset other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(EventOffset left, EventOffset right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EventOffset left, EventOffset right)
        {
            return !left.Equals(right);
        }
    }
}