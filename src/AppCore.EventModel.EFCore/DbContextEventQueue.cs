// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Data;
using AppCore.Data.EntityFrameworkCore;
using AppCore.Diagnostics;
using AppCore.EventModel.EntityFrameworkCore.Model;
using AppCore.EventModel.Formatters;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Queue;
using Microsoft.EntityFrameworkCore;

namespace AppCore.EventModel.EntityFrameworkCore;

/// <summary>
/// Provides a base class for Entity Framework Core based implementations of <see cref="IEventQueue"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
public abstract class DbContextEventQueue<TDbContext> : IEventQueue, IAsyncDisposable, IDisposable
    where TDbContext : DbContext
{
    private const int PollInterval = 250;
    private static string OffsetItemKey = "AppCore.EventModel.EventQueueOffset";

    private readonly Dictionary<string, IEventContextFormatter> _formatters;
    private ITransaction? _transaction;

    /// <summary>
    /// Gets the data provider.
    /// </summary>
    public IDbContextDataProvider<TDbContext> Provider { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> of events.
    /// </summary>
    protected DbSet<Event> Events { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> of events.
    /// </summary>
    protected DbSet<EventHistory> EventHistory { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbContextEventQueue{TDbContext}"/> class.
    /// </summary>
    /// <param name="dataProvider">The data provider.</param>
    /// <param name="formatters">The event formatters.</param>
    protected DbContextEventQueue(
        IDbContextDataProvider<TDbContext> dataProvider,
        IEnumerable<IEventContextFormatter> formatters)
    {
        Ensure.Arg.NotNull(dataProvider);
        Ensure.Arg.NotNull(formatters);

        _formatters = formatters.ToDictionary(
            f => f.ContentType,
            f => f,
            StringComparer.OrdinalIgnoreCase);

        if (_formatters.Count == 0)
            throw new NotSupportedException("The must be at least one event formatter registered.");

        Provider = dataProvider;
        TDbContext dbContext = dataProvider.GetContext();
        Events = dbContext.Set<Event>();
        EventHistory = dbContext.Set<EventHistory>();
    }

    /// <summary>
    /// Cleans up unmanaged resources.
    /// </summary>
    ~DbContextEventQueue()
    {
        Dispose(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Can be overridden to cleanup managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose also managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _transaction?.Dispose();
        }

        _transaction = null;
    }

    /// <summary>
    /// Can be overridden to cleanup managed resources.
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync()
                              .ConfigureAwait(false);
        }

        _transaction = null;
    }

    private static string GetEventTopic(IEventContext @event)
    {
        return @event.EventDescriptor.GetMetadata(EventMetadataKeys.TopicMetadataKey, string.Empty)!;
    }

    private IEventContextFormatter GetFormatter(string contentType)
    {
        if (!_formatters.TryGetValue(contentType, out IEventContextFormatter? formatter))
            throw new NotSupportedException($"No event formatter registered for content type '{contentType}'.");

        return formatter;
    }

    /// <summary>
    /// Can be overridden to write events to the queue.
    /// </summary>
    /// <param name="events">The event to enqueue.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    protected virtual async Task WriteCoreAsync(
        IEnumerable<Event> events,
        CancellationToken cancellationToken)
    {
        await Events.AddRangeAsync(events, cancellationToken);
    }

    /// <inheritdoc />
    public async Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken)
    {
        Ensure.Arg.NotNull(events);

        IEventContextFormatter formatter = _formatters.First().Value;
        using (Provider.BeginChangeScope())
        {
            using var stream = new MemoryStream();

            await WriteCoreAsync(
                events.Select(
                    e =>
                    {
                        string topic = GetEventTopic(e);
                        stream.SetLength(0);
                        stream.Seek(0, SeekOrigin.Begin);
                        formatter.Write(stream, e);
                        return new Event(formatter.ContentType, stream.ToArray())
                        {
                            Topic = topic
                        };
                    }).ToArray(),
                cancellationToken);

            await Provider.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Can be overridden to read pending events from the queue.
    /// </summary>
    /// <param name="maxEventsToRead">The maximum number of events to read.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    protected virtual IAsyncEnumerable<Event> ReadCoreAsync(int maxEventsToRead, CancellationToken cancellationToken)
    {
        return Events
               .Where(
                   e => e.Topic == Events.OrderBy(oe => oe.Offset)
                                         .GroupBy(ge => ge.Topic)
                                         .Select(g => g.Key)
                                         .FirstOrDefault())
               .OrderBy(e => e.Offset)
               .AsNoTracking()
               .AsAsyncEnumerable();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IEventContext>> ReadAsync(
        int maxEventsToRead,
        CancellationToken cancellationToken)
    {
        Ensure.Arg.InRange(maxEventsToRead, 1, int.MaxValue);

        if (_transaction != null)
        {
            throw new InvalidOperationException(
                "Read transaction is already pending, commit it before reading further events.");
        }

        _transaction =
            await Provider.TransactionManager.BeginTransactionAsync(
                IsolationLevel.Unspecified,
                cancellationToken);

        try
        {
            var result = new List<IEventContext>(maxEventsToRead);
            do
            {
                IAsyncEnumerable<Event> events = ReadCoreAsync(maxEventsToRead, cancellationToken);
                await foreach (Event @event in events.WithCancellation(cancellationToken))
                {
                    using var stream = new MemoryStream(@event.Data);
                    IEventContext eventContext = GetFormatter(@event.ContentType).Read(stream);
                    eventContext.Items.Add(OffsetItemKey, @event.Offset);
                    result.Add(eventContext);
                }

                if (result.Count == 0)
                    await Task.Delay(PollInterval, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

            } while (result.Count == 0);

            return result.AsReadOnly();
        }
        catch
        {
            await _transaction.RollbackAsync(cancellationToken);
            _transaction = null;

            throw;
        }
    }

    /// <summary>
    /// Can be overridden to commit the last processed read event.
    /// </summary>
    /// <param name="topic">The topic of the event.</param>
    /// <param name="offset">The offset to commit.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    protected virtual async Task CommitReadCoreAsync(string topic, long offset, CancellationToken cancellationToken)
    {
        Event[] events = await Events.Where(e => e.Topic == topic && e.Offset <= offset)
                                     .ToArrayAsync(cancellationToken);

        Events.RemoveRange(events);

        EventHistory.AddRange(
            events.Select(
                e => new EventHistory(e.ContentType, e.Data)
                {
                    Offset = e.Offset,
                    Topic = e.Topic
                }));

        await Provider.GetContext()
                      .SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task CommitReadAsync(IEventContext @event, CancellationToken cancellationToken)
    {
        Ensure.Arg.NotNull(@event);

        if (_transaction == null)
        {
            throw new InvalidOperationException("Read transaction is not active.");
        }

        long offset = (long) @event.Items[OffsetItemKey];
        string topic = GetEventTopic(@event);

        await CommitReadCoreAsync(topic, offset, cancellationToken);
        await _transaction.CommitAsync(cancellationToken);

        _transaction = null;
    }

    /// <summary>
    /// Can be overridden to reads the event history from the queue.
    /// </summary>
    /// <param name="offset">The starting offset.</param>
    /// <param name="maxEventsToRead">The maximum number of events to read.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    protected IAsyncEnumerable<EventHistory> ReadHistoryCoreAsync(long offset, int maxEventsToRead, CancellationToken cancellationToken)
    {
        return EventHistory.Where(e => e.Offset >= offset)
                           .OrderBy(e => e.Offset)
                           .Take(maxEventsToRead)
                           .AsNoTracking()
                           .AsAsyncEnumerable();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IEventContext>> ReadHistoryAsync(long offset, int maxEventsToRead, CancellationToken cancellationToken)
    {
        Ensure.Arg.InRange(offset, 0, long.MaxValue);
        Ensure.Arg.InRange(maxEventsToRead, 1, int.MaxValue);

        var result = new List<IEventContext>(maxEventsToRead);
        IAsyncEnumerable<EventHistory> eventHistory = ReadHistoryCoreAsync(offset, maxEventsToRead, cancellationToken);
        await foreach(EventHistory @event in eventHistory.WithCancellation(cancellationToken))
        {
            using var stream = new MemoryStream(@event.Data);
            IEventContext eventContext = GetFormatter(@event.ContentType).Read(stream);
            eventContext.Items.Add(OffsetItemKey, @event.Offset);
            result.Add(eventContext);
        }

        return result.AsReadOnly();
    }
}