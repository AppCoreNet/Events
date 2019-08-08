// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.IO;

namespace AppCore.Events.Formatters
{
    /// <summary>
    /// Represents a formatter for <see cref="IEventContext"/>.
    /// </summary>
    public interface IEventContextTextFormatter
    {
        /// <summary>
        /// Gets the content type supported by the formatter.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Serializes the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> where the context is serialized to.</param>
        /// <param name="context">The <see cref="IEventContext"/> to serialize.</param>
        void Write(TextWriter writer, IEventContext context);

        /// <summary>
        /// Deserializes a <see cref="IEventContext"/> from the specified <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> where the context is deserialized from.</param>
        /// <returns>The <see cref="IEventContext"/>.</returns>
        IEventContext Read(TextReader reader);
    }
}