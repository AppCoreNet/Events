// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Data.EntityFrameworkCore;
using AppCore.EventModel.EntityFrameworkCore.Model;
using AppCore.EventModel.Formatters;
using AppCore.EventModel.Queue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppCore.EventModel.EntityFrameworkCore.PostgreSql
{
    /// <summary>
    /// Represents an <see cref="IEventQueue"/> targeting PostgreSQL.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
    public class PostgreSqlEventQueue<TDbContext> : DbContextEventQueue<TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlEventQueue{TDbContext}"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <param name="formatters">An enumerable of event formatters.</param>
        public PostgreSqlEventQueue(
            IDbContextDataProvider<TDbContext> dataProvider,
            IEnumerable<IEventContextFormatter> formatters)
            : base(dataProvider, formatters)
        {
        }

        /// <inheritdoc />
        protected override async Task<IReadOnlyCollection<Event>> ReadCoreAsync(
            int maxEventsToRead,
            CancellationToken cancellationToken)
        {
            FormattableString query = $@"
                select
                  Q.Offset,
                  Q.Topic,
                  Q.ContentType,
                  Q.Data as Q
                from EventQueue as Q
                  join (
                    select Topic as Topic
                    from EventQueue
                    order by Offset
                    for update skip locked
                    fetch next 1 rows only
                  ) as T
                    on Q.Topic = T.Topic
                order by Q.Offset
                for update skip locked
                fetch next {maxEventsToRead} rows only";

            return await Events.FromSqlInterpolated(query)
                               .AsNoTracking()
                               .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        protected override async Task CommitReadCoreAsync(string topic, long offset, CancellationToken cancellationToken)
        {
            DatabaseFacade database = Provider.GetContext().Database;

            // copy events to history
            FormattableString copyStmt = $@"
                insert into EventHistory (Offset,Topic,ContentType,Data)
                select Offset,Topic,ContentType,Data
                from EventQueue
                where Offset <= {offset} and Topic={topic}";

            await database.ExecuteSqlInterpolatedAsync(copyStmt, cancellationToken);

            // delete events
            FormattableString deleteStmt = $"delete from EventQueue where Offset <= {offset} and Topic={topic}";
            await database.ExecuteSqlInterpolatedAsync(deleteStmt, cancellationToken);
        }
    }
}
