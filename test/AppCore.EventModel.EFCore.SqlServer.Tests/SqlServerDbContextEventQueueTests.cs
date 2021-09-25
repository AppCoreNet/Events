using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.Data.EntityFrameworkCore;
using AppCore.EventModel.EntityFrameworkCore.SqlServer.Configuration;
using AppCore.EventModel.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace AppCore.EventModel.EntityFrameworkCore.SqlServer
{
    [CollectionDefinition("sqlserver")]
    public class SqlServerTestCollection : ICollectionFixture<SqlServerContainer>
    {
    }

    [Collection("sqlserver")]
    public class SqlServerDbContextEventQueueTests : DbContextEventQueueTests<SqlServerDbContext>
    {
        public SqlServerDbContextEventQueueTests(SqlServerContainer container)
            : base(new SqlServerDbContext(container.ConnectionString))
        {
        }

        protected override DbContextEventQueue<SqlServerDbContext> CreateEventQueue(
            IDbContextDataProvider<SqlServerDbContext> provider,
            IEnumerable<IEventContextFormatter> formatters)
        {
            return new SqlServerEventQueue<SqlServerDbContext>(provider, formatters);
        }

        public override async Task DisposeAsync()
        {
            DatabaseFacade database = Provider.GetContext().Database;
            await database.ExecuteSqlRawAsync($"DROP TABLE [{EventHistoryTypeConfiguration.TableName}]");
            await database.ExecuteSqlRawAsync($"DROP TABLE [{EventTypeConfiguration.TableName}]");
        }
    }
}