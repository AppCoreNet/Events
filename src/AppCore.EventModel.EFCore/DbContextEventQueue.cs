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

namespace AppCore.EventModel.EntityFrameworkCore
{
    /// <summary>
    /// Provides a base class for Entity Framework Core based implementations of <see cref="IEventQueue"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
    public abstract class DbContextEventQueue<TDbContext> : IEventQueue
        where TDbContext : DbContext
    {
        private const int PollInterval = 250;
        private static string OffsetItemKey = "AppCore.EventModel.EventQueueOffset";

        private readonly Dictionary<string, IEventContextFormatter> _formatters;
        private ITransaction _transaction;

        /// <summary>
        /// Gets the data provider.
        /// </summary>
        public IDbContextDataProvider<TDbContext> Provider { get; }

        /// <summary>
        /// Gets the <see cref="DbSet{TEntity}"/> of events.
        /// </summary>
        protected DbSet<Event> Events { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextEventQueue{TDbContext}"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <param name="formatters">The event formatters.</param>
        protected DbContextEventQueue(
            IDbContextDataProvider<TDbContext> dataProvider,
            IEnumerable<IEventContextFormatter> formatters)
        {
            Ensure.Arg.NotNull(dataProvider, nameof(dataProvider));
            Ensure.Arg.NotNull(formatters, nameof(formatters));

            _formatters = formatters.ToDictionary(
                f => f.ContentType,
                f => f,
                StringComparer.OrdinalIgnoreCase);

            if (_formatters.Count == 0)
                throw new NotSupportedException("The must be at least one event formatter registered.");

            Provider = dataProvider;
            Events = dataProvider.GetContext().Set<Event>();
        }

        private static string GetEventTopic(IEventContext @event)
        {
            return @event.EventDescriptor.GetMetadata(EventMetadataKeys.TopicMetadataKey, String.Empty);
        }

        private IEventContextFormatter GetFormatter(string contentType)
        {
            if (!_formatters.TryGetValue(contentType, out IEventContextFormatter formatter))
                throw new NotSupportedException($"No event formatter registered for content type '{contentType}'.");

            return formatter;
        }

        protected virtual async Task WriteCoreAsync(
            IEnumerable<Event> events,
            CancellationToken cancellationToken)
        {
            await Events.AddRangeAsync(events, cancellationToken);
        }

        /// <inheritdoc />
        public async Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken)
        {
            Ensure.Arg.NotNull(events, nameof(events));

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
                            return new Event
                            {
                                Topic = topic,
                                ContentType = formatter.ContentType,
                                Data = stream.ToArray()
                            };
                        }).ToArray(),
                    cancellationToken);

                await Provider.SaveChangesAsync(cancellationToken);
            }
        }

        protected virtual async Task<IReadOnlyCollection<Event>> ReadCoreAsync(
            int maxEventsToRead,
            CancellationToken cancellationToken)
        {
            return await Events.Where(
                                   e => e.Topic == Events.OrderBy(e => e.Offset)
                                                         .GroupBy(e => e.Topic)
                                                         .Select(g => g.Key)
                                                         .FirstOrDefault())
                               .OrderBy(e => e.Offset)
                               .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<IEventContext>> ReadAsync(
            int maxEventsToRead,
            CancellationToken cancellationToken)
        {
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
                IReadOnlyCollection<Event> events;
                do
                {
                    events = await ReadCoreAsync(maxEventsToRead, cancellationToken);
                    if (events.Count == 0)
                        await Task.Delay(PollInterval, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                } while (events.Count == 0);

                return events.Select(
                        e =>
                        {
                            using var stream = new MemoryStream(e.Data);
                            IEventContext context = GetFormatter(e.ContentType).Read(stream);
                            context.Items.Add(OffsetItemKey, e.Offset);
                            return context;
                        })
                    .ToList()
                    .AsReadOnly();
            }
            catch
            {
                await _transaction.RollbackAsync(cancellationToken);
                _transaction = null;

                throw;
            }
        }

        protected virtual async Task CommitReadCoreAsync(string topic, long offset, CancellationToken cancellationToken)
        {
            Event[] events = await Events.Where(e => e.Topic == topic && e.Offset <= offset)
                                         .ToArrayAsync(cancellationToken);

            Events.RemoveRange(events);
            await Provider.GetContext()
                          .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task CommitReadAsync(IEventContext @event, CancellationToken cancellationToken)
        {
            Ensure.Arg.NotNull(@event, nameof(@event));

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
    }
}
