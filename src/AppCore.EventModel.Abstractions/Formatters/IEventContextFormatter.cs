// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.IO;

namespace AppCore.Events.Formatters
{
    /// <summary>
    /// Represents a formatter for <see cref="IEventContext"/>.
    /// </summary>
    public interface IEventContextFormatter
    {
        /// <summary>
        /// Gets the content type supported by the formatter.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Serializes the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> where the context is serialized to.</param>
        /// <param name="context">The <see cref="IEventContext"/> to serialize.</param>
        void Write(Stream stream, IEventContext context);

        /// <summary>
        /// Deserializes a <see cref="IEventContext"/> from the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> where the context is deserialized from.</param>
        /// <returns>The <see cref="IEventContext"/>.</returns>
        IEventContext Read(Stream stream);
    }
}