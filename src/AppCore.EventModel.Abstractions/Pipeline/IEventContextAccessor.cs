// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

namespace AppCore.EventModel.Pipeline;

/// <summary>
/// Provides access to the currently processed <see cref="IEventContext"/>.
/// </summary>
public interface IEventContextAccessor
{
    /// <summary>
    /// Gets or sets the current <see cref="IEventContext"/>.
    /// </summary>
    IEventContext? EventContext { get; set; }
}