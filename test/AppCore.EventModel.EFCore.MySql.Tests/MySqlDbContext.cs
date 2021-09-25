using AppCore.EventModel.EntityFrameworkCore.Model;
using AppCore.EventModel.EntityFrameworkCore.MySql.Configuration;
using Microsoft.EntityFrameworkCore;

namespace AppCore.EventModel.EntityFrameworkCore.MySql
{
    public class MySqlDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Event> Events => Set<Event>();

        public DbSet<EventHistory> EventHistory => Set<EventHistory>();

        public MySqlDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            #if !NET5_0_OR_GREATER
            optionsBuilder.UseMySql(_connectionString);
            #else
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
            #endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EventTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EventHistoryTypeConfiguration());
        }
    }
}