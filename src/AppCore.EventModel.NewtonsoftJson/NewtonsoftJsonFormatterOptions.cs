// Licensed under the MIT License.
// Copyright (c) 2018-2022 the AppCore .NET project.

using Newtonsoft.Json;

namespace AppCore.EventModel.Formatters;

/// <summary>
/// Provides options for the <see cref="NewtonsoftJsonFormatter"/> event formatter.
/// </summary>
public class NewtonsoftJsonFormatterOptions
{
    /// <summary>
    /// Gets the Newtonsoft.Json serializer settings.
    /// </summary>
    public JsonSerializerSettings SerializerSettings { get; } = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        NullValueHandling = NullValueHandling.Ignore
    };
}