using AppCore.EventModel.EntityFrameworkCore.Model;
using AppCore.EventModel.EntityFrameworkCore.SqlServer.Configuration;
using Microsoft.EntityFrameworkCore;

namespace AppCore.EventModel.EntityFrameworkCore.SqlServer;

public class SqlServerDbContext : DbContext
{
    private readonly string _connectionString;

    public DbSet<Event> Events => Set<Event>();

    public DbSet<EventHistory> EventHistory => Set<EventHistory>();

    public SqlServerDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new EventTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EventHistoryTypeConfiguration());
    }
}