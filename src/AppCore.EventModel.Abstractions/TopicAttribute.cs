// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;

namespace AppCore.EventModel;

/// <summary>
/// Decorates an event with a topic.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TopicAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the event topic.
    /// </summary>
    public string? Name { get; set; }
}