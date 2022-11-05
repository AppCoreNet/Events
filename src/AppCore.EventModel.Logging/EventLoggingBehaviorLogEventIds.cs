// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using Microsoft.Extensions.Logging;

namespace AppCore.EventModel.Logging;

/// <summary>
/// Provides <see cref="EventId"/>'s for logged events.
/// </summary>
public static class EventLoggingBehaviorLogEventIds
{
    /// <summary>
    /// Identifies the log event for a event which has been successfully handled.
    /// </summary>
    public static readonly EventId EventHandled = new EventId(0, nameof(EventHandled));

    /// <summary>
    /// Identifies the log event for a event which could not be handled.
    /// </summary>
    public static readonly EventId EventFailed = new EventId(0, nameof(EventFailed));
}