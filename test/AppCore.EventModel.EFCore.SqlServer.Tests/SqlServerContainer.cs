using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using DotNet.Testcontainers.Containers.WaitStrategies;
using Xunit;

namespace AppCore.EventModel.EntityFrameworkCore.SqlServer
{
    public class SqlServerContainer : IAsyncLifetime
    {
        private MsSqlTestcontainer _container;

        public string ConnectionString => _container.ConnectionString;

        public async Task InitializeAsync()
        {
            _container = new TestcontainersBuilder<MsSqlTestcontainer>()
                         .WithEnvironment("ACCEPT_EULA", "y")
                         .WithDatabase(
                             new MsSqlTestcontainerConfiguration
                             {
                                 Password = "<My!Strong!Password123>"
                             })
                         .WithPortBinding(11434, 1434)
                         .Build();

            await _container.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
}
