using System;

namespace AppCore.EventModel.Queue;

/// <summary>
/// Provides options for the event queueing.
/// </summary>
public class EventQueueOptions
{
    /// <summary>
    /// Gets or sets the event processing batch size when publishing queued events.
    /// </summary>
    public int BatchSize { get; set; } = 64;

    /// <summary>
    /// Gets or sets the delay between failed publishing attempts.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
}