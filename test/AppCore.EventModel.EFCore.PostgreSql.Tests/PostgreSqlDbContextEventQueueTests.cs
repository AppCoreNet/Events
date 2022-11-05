using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.Data.EntityFrameworkCore;
using AppCore.EventModel.EntityFrameworkCore.PostgreSql.Configuration;
using AppCore.EventModel.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace AppCore.EventModel.EntityFrameworkCore.PostgreSql;

[CollectionDefinition("postgres")]
public class PostgreSqlTestCollection : ICollectionFixture<PostgreSqlContainer>
{
}

[Collection("postgres")]
[Trait("Category", "Integration")]
public class PostgreSqlDbContextEventQueueTests : DbContextEventQueueTests<PostgreSqlDbContext>
{
    public PostgreSqlDbContextEventQueueTests(PostgreSqlContainer container)
        : base(new PostgreSqlDbContext(container.ConnectionString))
    {
    }

    protected override DbContextEventQueue<PostgreSqlDbContext> CreateEventQueue(
        IDbContextDataProvider<PostgreSqlDbContext> provider,
        IEnumerable<IEventContextFormatter> formatters)
    {
        return new PostgreSqlEventQueue<PostgreSqlDbContext>(provider, formatters);
    }

    public override async Task DisposeAsync()
    {
        DatabaseFacade database = Provider.GetContext().Database;
        await database.ExecuteSqlRawAsync($"DROP TABLE \"{EventHistoryTypeConfiguration.TableName}\"");
        await database.ExecuteSqlRawAsync($"DROP TABLE \"{EventTypeConfiguration.TableName}\"");
    }
}