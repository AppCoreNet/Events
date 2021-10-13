using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Xunit;

namespace AppCore.EventModel.EntityFrameworkCore.PostgreSql
{
    public class PostgreSqlContainer : IAsyncLifetime
    {
        private PostgreSqlTestcontainer _container;

        public string ConnectionString => _container.ConnectionString;

        public async Task InitializeAsync()
        {
            _container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                         .WithDatabase(
                             new PostgreSqlTestcontainerConfiguration
                             {
                                 Database = "test",
                                 Username = "user",
                                 Password = "password"
                             })
                         .WithPortBinding(15432, 5432)
                         .Build();

            await _container.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
}
