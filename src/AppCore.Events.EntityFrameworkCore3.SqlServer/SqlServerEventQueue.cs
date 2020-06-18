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
using AppCore.Events.EntityFrameworkCore.SqlServer.Data;
using AppCore.Events.Formatters;
using AppCore.Events.Queue;
using Microsoft.EntityFrameworkCore;

namespace AppCore.Events.EntityFrameworkCore.SqlServer
{
    public class SqlServerEventQueue<TDbContext> : IEventQueue
        where TDbContext : DbContext
    {
        private const int PollInterval = 250;

        private readonly IDbContextDataProvider<TDbContext> _dataProvider;
        private readonly TDbContext _dbContext;
        private readonly DbSet<Event> _events;
        private readonly IEventContextFormatter _formatter;
        private ITransaction _transaction;

        public SqlServerEventQueue(
            IDbContextDataProvider<TDbContext> dataProvider,
            IEnumerable<IEventContextFormatter> formatters)
        {
            Ensure.Arg.NotNull(dataProvider, nameof(dataProvider));
            Ensure.Arg.NotNull(formatters, nameof(formatters));


            _dataProvider = dataProvider;
            _dbContext = dataProvider.GetContext();
            _events = _dbContext.Set<Event>();
            _formatter = formatters.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task WriteAsync(IEnumerable<IEventContext> events, CancellationToken cancellationToken)
        {
            Ensure.Arg.NotNull(events, nameof(events));

            using (_dataProvider.BeginChangeScope())
            {
                await _events.AddRangeAsync(
                    events.Select(
                        e =>
                        {
                            /*
                            string topic = e.EventDescriptor.GetMetadata(
                                EventStoreMetadataKeys.QueueTopicMetadataKey,
                                String.Empty);
                            */

                            string topic = null;
                            using (var stream = new MemoryStream())
                            {
                                _formatter.Write(stream, e);
                                return new Event
                                {
                                    Topic = topic,
                                    ContentType = _formatter.ContentType,
                                    Data = stream.ToArray()
                                };
                            }
                        }),
                    cancellationToken);

                await _dataProvider.SaveChangesAsync(cancellationToken);
            }
        }

        /*
        private const string _readEventsSql = "SET NOCOUNT ON;"
                                              + "WITH cte AS (" 
                                              + "  SELECT TOP(@maxEventsToRead) [Offset],[Topic],[ContentType],[Data] FROM [EventQueue] WITH (ROWLOCK) ORDER BY [Offset]"
                                              + ")"
                                              + "DELETE FROM cte"
                                              + "  OUTPUT deleted.[Offset], deleted.[Topic], deleted.[ContentType], deleted.[Data]";
        */

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
                await _dataProvider.TransactionManager.BeginTransactionAsync(IsolationLevel.Unspecified,
                    cancellationToken);

            try
            {
                Event[] events;
                do
                {
                    events = await _events
                        .FromSqlInterpolated(
                            $"SELECT TOP({maxEventsToRead}) [Offset],[Topic],[ContentType],[Data] FROM [EventQueue] WITH (UPDLOCK) ORDER BY [Offset]")
                        .AsNoTracking()
                        .ToArrayAsync(cancellationToken);

                    if (events.Length == 0)
                        await Task.Delay(PollInterval, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                } while (events.Length == 0);

                return events.Select(
                        e =>
                        {
                            //TODO: check content type
                            using (var stream = new MemoryStream(e.Data))
                            {
                                IEventContext context = _formatter.Read(stream);
                                context.Items.Add("Offset", e.Offset);
                                //context.AddFeature<IEventStoreFeature>(new EventStoreFeature(this, e.Offset));
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

        /// <inheritdoc />
        public async Task CommitReadAsync(IEventContext @event, CancellationToken cancellationToken)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Read transaction is not active.");
            }

            long offset = (long) @event.Items["Offset"];

            await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM [EventQueue] WHERE [Offset] <= {offset}", cancellationToken);

            await _transaction.CommitAsync(cancellationToken);
            _transaction = null;
        }
    }
}
