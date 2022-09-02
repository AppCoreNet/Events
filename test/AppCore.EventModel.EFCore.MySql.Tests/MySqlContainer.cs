using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace AppCore.EventModel.EntityFrameworkCore.MySql;

public class MySqlContainer : IAsyncLifetime
{
    private MySqlTestcontainer? _container;

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
        if (_container != null)
        {
            await _container.DisposeAsync();
        }
    }
}