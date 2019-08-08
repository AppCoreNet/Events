// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Collections.Generic;
using System.IO;
using AppCore.Diagnostics;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;
using Newtonsoft.Json;

namespace AppCore.Events.Formatters
{
    /// <summary>
    /// Provides a Newtonsoft.Json based event context formatter.
    /// </summary>
    public class NewtonsoftJsonFormatter : IEventContextTextFormatter
    {
        private const string _contentType = "application/json";

        private readonly IEventContextFactory _contextFactory;
        private readonly JsonSerializer _serializer;

        /// <inheritdoc />
        public string ContentType => _contentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewtonsoftJsonFormatter"/> class.
        /// </summary>
        /// <param name="contextFactory">The <see cref="IEventContextFactory"/>.</param>
        public NewtonsoftJsonFormatter(IEventContextFactory contextFactory)
        {
            Ensure.Arg.NotNull(contextFactory, nameof(contextFactory));

            _contextFactory = contextFactory;
            _serializer = JsonSerializer.Create(
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <inheritdoc />
        public void Write(TextWriter writer, IEventContext context)
        {
            Ensure.Arg.NotNull(writer, nameof(writer));
            Ensure.Arg.NotNull(context, nameof(context));

            var serializedEvent = new JsonSerializedEvent
            {
                Event = context.Event,
                Items = context.Items.Count > 0 ? context.Items : null,
                Metadata = context.EventDescriptor.Metadata
            };

            _serializer.Serialize(writer, serializedEvent);
        }

        /// <inheritdoc />
        public IEventContext Read(TextReader reader)
        {
            Ensure.Arg.NotNull(reader, nameof(reader));

            var serializedEvent = (JsonSerializedEvent) _serializer.Deserialize(reader, typeof(JsonSerializedEvent));

            IEventContext result = _contextFactory.CreateContext(
                new EventDescriptor(serializedEvent.Event.GetType(), serializedEvent.Metadata),
                serializedEvent.Event);

            if (serializedEvent.Items != null)
            {
                foreach (KeyValuePair<object, object> data in serializedEvent.Items)
                    result.Items.Add(data);
            }

            return result;
        }
    }
}