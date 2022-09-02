using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace AppCore.EventModel.EntityFrameworkCore.SqlServer
{
    public class SqlServerContainer : IAsyncLifetime
    {
        private MsSqlTestcontainer? _container;

        public string ConnectionString
        {
            get
            {
                if (_container == null)
                    throw new InvalidOperationException("Container not initialized.");

                return _container.ConnectionString;
            }
        }

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
            if (_container != null)
            {
                await _container.DisposeAsync();
            }
        }
    }
}
