// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System.Collections.Generic;
using System.IO;
using AppCore.Diagnostics;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AppCore.EventModel.Formatters;

/// <summary>
/// Provides a Newtonsoft.Json based event context formatter.
/// </summary>
public class NewtonsoftJsonFormatter : IEventContextFormatter
{
    private readonly IEventContextFactory _contextFactory;
    private readonly JsonSerializer _serializer;

    /// <inheritdoc />
    public string ContentType => "application/json";

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonsoftJsonFormatter"/> class.
    /// </summary>
    /// <param name="contextFactory">The <see cref="IEventContextFactory"/>.</param>
    /// <param name="options">The <see cref="NewtonsoftJsonFormatterOptions"/>.</param>
    public NewtonsoftJsonFormatter(IEventContextFactory contextFactory, IOptionsMonitor<NewtonsoftJsonFormatterOptions> options)
    {
        Ensure.Arg.NotNull(contextFactory, nameof(contextFactory));

        _contextFactory = contextFactory;
        _serializer = JsonSerializer.Create(options.CurrentValue.SerializerSettings);
    }

    /// <inheritdoc />
    public void Write(Stream stream, IEventContext context)
    {
        Ensure.Arg.NotNull(stream, nameof(stream));
        Ensure.Arg.NotNull(context, nameof(context));

        IDictionary<object,object> items = context.Items;
        IReadOnlyDictionary<string,object> metadata = context.EventDescriptor.Metadata;

        var serializedEvent = new JsonSerializedEvent(context.Event)
        {
            Items = items.Count > 0
                ? items
                : null,
            Metadata = metadata.Count > 0
                ? metadata
                : null
        };

        using (var writer = new StreamWriter(stream))
            _serializer.Serialize(writer, serializedEvent);
    }

    /// <inheritdoc />
    public IEventContext Read(Stream stream)
    {
        Ensure.Arg.NotNull(stream, nameof(stream));

        JsonSerializedEvent @event;
        using (var reader = new StreamReader(stream))
        {
            @event = (JsonSerializedEvent) _serializer.Deserialize(reader, typeof(JsonSerializedEvent))!;
        }

        var descriptor = new EventDescriptor(
            @event.Event.GetType(),
            @event.Metadata ?? new Dictionary<string, object>());

        IEventContext context = _contextFactory.CreateContext(descriptor, @event.Event);

        if (@event.Items != null)
        {
            foreach (KeyValuePair<object, object> data in @event.Items)
                context.Items.Add(data);
        }

        return context;
    }
}