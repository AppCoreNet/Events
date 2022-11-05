using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.Data.EntityFrameworkCore;
using AppCore.EventModel.EntityFrameworkCore.MySql.Configuration;
using AppCore.EventModel.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace AppCore.EventModel.EntityFrameworkCore.MySql;

[CollectionDefinition("mysql")]
public class MySqlTestCollection : ICollectionFixture<MySqlContainer>
{
}

[Collection("mysql")]
[Trait("Category", "Integration")]
public class MySqlDbContextEventQueueTests : DbContextEventQueueTests<MySqlDbContext>
{
    public MySqlDbContextEventQueueTests(MySqlContainer container)
        : base(new MySqlDbContext(container.ConnectionString))
    {
    }

    protected override DbContextEventQueue<MySqlDbContext> CreateEventQueue(
        IDbContextDataProvider<MySqlDbContext> provider,
        IEnumerable<IEventContextFormatter> formatters)
    {
        return new MySqlEventQueue<MySqlDbContext>(provider, formatters);
    }

    public override async Task DisposeAsync()
    {
        DatabaseFacade database = Provider.GetContext().Database;
        await database.ExecuteSqlRawAsync($"DROP TABLE `{EventHistoryTypeConfiguration.TableName}`");
        await database.ExecuteSqlRawAsync($"DROP TABLE `{EventTypeConfiguration.TableName}`");
    }
}