// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Data.EntityFrameworkCore;
using AppCore.EventModel.EntityFrameworkCore.Model;
using AppCore.EventModel.Formatters;
using AppCore.EventModel.Queue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AppCore.EventModel.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Represents an <see cref="IEventQueue"/> targeting PostgreSQL.
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
public class PostgreSqlEventQueue<TDbContext> : DbContextEventQueue<TDbContext>
    where TDbContext : DbContext
{
    private readonly string _readStatement;
    private readonly string _copyStatement;
    private readonly string _deleteStatement;

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
        IModel model = Provider.GetContext().Model;

        _readStatement = BuildReadStmt(model);
        _copyStatement = BuildCopyStmt(model);
        _deleteStatement = BuildDeleteStmt(model);
    }

    private static string BuildReadStmt(IModel model)
    {
        var stmt = new SqlStmtBuilder(model);
        stmt.Append("select ")
            .AppendColumnNames<Event>(
                new[]
                {
                    nameof(Event.Offset),
                    nameof(Event.Topic),
                    nameof(Event.ContentType),
                    nameof(Event.Data)
                },
                "q")
            .Append(" from ")
            .AppendTableName<Event>("q")
            .Append(" join (")
            .Append(" select ")
            .AppendColumnName<Event>(nameof(Event.Topic))
            .Append(" from ")
            .AppendTableName<Event>()
            .Append(" order by ")
            .AppendColumnName<Event>(nameof(Event.Offset))
            .Append(" for update skip locked")
            .Append(" fetch next 1 rows only")
            .Append(" ) t on ")
            .AppendColumnName<Event>(nameof(Event.Topic), "q")
            .Append('=')
            .AppendColumnName<Event>(nameof(Event.Topic), "t")
            .Append(" order by ")
            .AppendColumnName<Event>(nameof(Event.Offset), "q")
            .Append(" for update skip locked")
            .Append(" fetch next {0} rows only");

        return stmt.ToString();
    }

    private static string BuildCopyStmt(IModel model)
    {
        var stmt = new SqlStmtBuilder(model);
        stmt.Append("insert into ")
            .AppendTableName<EventHistory>()
            .Append(" (")
            .AppendColumnNames<EventHistory>(
                new[]
                {
                    nameof(Model.EventHistory.Offset),
                    nameof(Model.EventHistory.Topic),
                    nameof(Model.EventHistory.ContentType),
                    nameof(Model.EventHistory.Data)
                })
            .Append(") ")
            .Append(" select ")
            .AppendColumnNames<Event>(
                new []
                {
                    nameof(Event.Offset),
                    nameof(Event.Topic),
                    nameof(Event.ContentType),
                    nameof(Event.Data)
                })
            .Append(" from ")
            .AppendTableName<Event>()
            .Append(" where ")
            .AppendColumnName<Event>(nameof(Event.Offset))
            .Append("<={0} and ")
            .AppendColumnName<Event>(nameof(Event.Topic))
            .Append("={1}");

        return stmt.ToString();
    }

    private static string BuildDeleteStmt(IModel model)
    {
        var stmt = new SqlStmtBuilder(model);
        stmt.Append("delete from ")
            .AppendTableName<Event>()
            .Append(" where ")
            .AppendColumnName<Event>(nameof(Event.Offset))
            .Append("<={0} and ")
            .AppendColumnName<Event>(nameof(Event.Topic))
            .Append("={1}");

        return stmt.ToString();
    }

    /// <inheritdoc />
    protected override IAsyncEnumerable<Event> ReadCoreAsync(int maxEventsToRead, CancellationToken cancellationToken)
    {
        return Events.FromSqlRaw(_readStatement, maxEventsToRead)
                     .AsNoTracking()
                     .AsAsyncEnumerable();
    }

    /// <inheritdoc />
    protected override async Task CommitReadCoreAsync(string topic, long offset, CancellationToken cancellationToken)
    {
        DatabaseFacade database = Provider.GetContext().Database;
        await database.ExecuteSqlRawAsync(
            _copyStatement,
            new object[]
            {
                offset,
                topic
            },
            cancellationToken);

        await database.ExecuteSqlRawAsync(
            _deleteStatement,
            new object[]
            {
                offset,
                topic
            },
            cancellationToken);
    }
}