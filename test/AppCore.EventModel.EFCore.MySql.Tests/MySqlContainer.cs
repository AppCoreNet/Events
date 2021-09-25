using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Xunit;

namespace AppCore.EventModel.EntityFrameworkCore.MySql
{
    public class MySqlContainer : IAsyncLifetime
    {
        private MySqlTestcontainer _container;

        public string ConnectionString => _container.ConnectionString;

        public async Task InitializeAsync()
        {
            _container = new TestcontainersBuilder<MySqlTestcontainer>()
                         .WithDatabase(
                             new MySqlTestcontainerConfiguration
                             {
                                 Database = "test",
                                 Username = "user",
                                 Password = "password"
                             })
                         .WithPortBinding(13306, 3306)
                         .Build();

            await _container.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
}
