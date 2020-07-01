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
using AppCore.Events.EntityFrameworkCore.Data;
using AppCore.Events.Formatters;
using AppCore.Events.Metadata;
using AppCore.Events.Queue;
using Microsoft.EntityFrameworkCore;

namespace AppCore.Events.EntityFrameworkCore
{
    /// <summary>
    /// Provides a base class for Entity Framework Core based implementations of <see cref="IEventQueue"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
    public abstract class DbContextEventQueue<TDbContext> : IEventQueue
        where TDbContext : DbContext
    {
        private const int PollInterval = 250;
        private static string OffsetItemKey = "AppCore.Events.EventQueueOffset";

        private readonly Dictionary<string, IEventContextFormatter> _formatters;
        private ITransaction _transaction;

        /// <summary>
        /// Gets the data provider.
        /// </summary>
        protected IDbContextDataProvider<TDbContext> Provider { get; }

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
                await WriteCoreAsync(
                    events.Select(
                        e =>
                        {
                            string topic = GetEventTopic(e);
                            using (var stream = new MemoryStream())
                            {
                                formatter.Write(stream, e);
                                return new Event
                                {
                                    Topic = topic,
                                    ContentType = formatter.ContentType,
                                    Data = stream.ToArray()
                                };
                            }
                        }),
                    cancellationToken);

                await Provider.SaveChangesAsync(cancellationToken);
            }
        }

        protected abstract Task<IReadOnlyCollection<Event>> ReadCoreAsync(
            int maxEventsToRead,
            CancellationToken cancellationToken);

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
                            using (var stream = new MemoryStream(e.Data))
                            {
                                IEventContext context = GetFormatter(e.ContentType).Read(stream);
                                context.Items.Add(OffsetItemKey, e.Offset);
                                return context;
                            }
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

        protected abstract Task CommitReadCoreAsync(string topic, long offset, CancellationToken cancellationToken);

        /// <inheritdoc />
        public async Task CommitReadAsync(IEventContext @event, CancellationToken cancellationToken)
        {
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
