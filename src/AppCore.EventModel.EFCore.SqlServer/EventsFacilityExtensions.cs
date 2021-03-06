// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.Data;
using AppCore.Diagnostics;
using AppCore.EventModel;
using AppCore.EventModel.EntityFrameworkCore.SqlServer;
using AppCore.EventModel.Queue;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    public static class EventsFacilityExtensions
    {
        /// <summary>
        /// Registers SQL Server event queue.
        /// </summary>
        /// <param name="extension">The <see cref="EventQueueFacilityExtension"/>.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventQueueFacilityExtension WithSqlServerQueue<TTag, TDbContext>(
            this EventQueueFacilityExtension extension)
            where TDbContext : DbContext
        {
            Ensure.Arg.NotNull(extension, nameof(extension));

            extension.ConfigureRegistry(
                r =>
                {
                    r.AddDataProvider(d => d.UseEntityFrameworkCore<TTag, TDbContext>());
                    r.TryAdd(ComponentRegistration.Scoped<IEventQueue, SqlServerEventQueue<TDbContext>>());
                });

            return extension;
        }

        /// <summary>
        /// Registers SQL Server event queue.
        /// </summary>
        /// <param name="extension">The <see cref="EventQueueFacilityExtension"/>.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventQueueFacilityExtension WithSqlServerQueue<TDbContext>(
            this EventQueueFacilityExtension extension)
            where TDbContext : DbContext
        {
            return extension.WithSqlServerQueue<DefaultDataProvider, TDbContext>();
        }
    }
}