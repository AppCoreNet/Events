using System;
using AppCore.EventModel.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AppCore.EventModel.EntityFrameworkCore;

public class InMemoryDbContext : DbContext
{
    public DbSet<Event> Events => Set<Event>();

    public DbSet<EventHistory> EventHistory => Set<EventHistory>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                      .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new InMemoryEventTypeConfiguration());
        modelBuilder.ApplyConfiguration(new InMemoryEventHistoryTypeConfiguration());
    }
}