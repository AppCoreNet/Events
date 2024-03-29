﻿// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Collections.Generic;

namespace AppCore.EventModel.Formatters;

internal sealed class JsonSerializedEvent
{
    public IReadOnlyDictionary<string, object>? Metadata { get; set; }

    public IDictionary<object, object>? Items { get; set; }

    public IEvent Event { get; set; }

    public JsonSerializedEvent(IEvent @event)
    {
        Event = @event;
    }
}