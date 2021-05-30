// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Data.EntityFrameworkCore;
using AppCore.EventModel.EntityFrameworkCore.Data;
using AppCore.EventModel.Formatters;
using AppCore.EventModel.Queue;
using Microsoft.EntityFrameworkCore;

namespace AppCore.EventModel.EntityFrameworkCore.MySql
{
    /// <summary>
    /// Represents an <see cref="IEventQueue"/> targeting MySql.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
    public class MySqlEventQueue<TDbContext> : DbContextEventQueue<TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlEventQueue{TDbContext}"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <param name="formatters">An enumerable of event formatters.</param>
        public MySqlEventQueue(
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
                from EventQueue as Q with (UPDLOCK,READPAST)
                  join (
                    select Topic as Topic
                    from EventQueue with (UPDLOCK,READPAST)
                    order by Offset
                    limit 1
                  ) as T
                    on Q.Topic = T.Topic
                order by Q.Offset
                limit {maxEventsToRead}";

            return await Events.FromSqlInterpolated(query)
                               .AsNoTracking()
                               .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        protected override async Task CommitReadCoreAsync(string topic, long offset, CancellationToken cancellationToken)
        {
            await Provider.GetContext().Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM EventQueue WHERE Offset <= {offset} AND Topic={topic}", cancellationToken);
        }
    }
}
