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

namespace AppCore.EventModel.EntityFrameworkCore.SqlServer
{
    /// <summary>
    /// Represents an <see cref="IEventQueue"/> targeting SQL Server.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
    public class SqlServerEventQueue<TDbContext> : DbContextEventQueue<TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerEventQueue{TDbContext}"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <param name="formatters">An enumerable of event formatters.</param>
        public SqlServerEventQueue(
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
                        SELECT TOP({maxEventsToRead}) Q.Offset,Q.Topic,Q.ContentType,Q.Data
                        FROM EventQueue Q WITH (UPDLOCK,READPAST)
                        INNER JOIN (SELECT TOP(1) Topic FROM EventQueue WITH (UPDLOCK,READPAST) ORDER BY Offset) T
                        ON Q.Topic = T.Topic
                        ORDER BY Q.Offset";

            return await Events.FromSqlInterpolated(query)
                               .AsNoTracking()
                               .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        protected override async Task CommitReadCoreAsync(string topic, long offset, CancellationToken cancellationToken)
        {
            await Provider.GetContext().Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM [EventQueue] WHERE [Offset] <= {offset} AND [Topic]={topic}", cancellationToken);
        }
    }
}
