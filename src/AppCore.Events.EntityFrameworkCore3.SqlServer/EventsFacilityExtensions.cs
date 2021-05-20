// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.Diagnostics;
using AppCore.Events;
using AppCore.Events.EntityFrameworkCore.SqlServer;
using AppCore.Events.Queue;
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
        public static EventQueueFacilityExtension WithSqlServerEventQueue<TDbContext>(this EventQueueFacilityExtension extension)
            where TDbContext : DbContext
        {
            Ensure.Arg.NotNull(extension, nameof(extension));

            extension.ConfigureRegistry(
                r =>
                {
                    r.AddData(d => d.UseEntityFrameworkCore<TDbContext>());
                    r.TryAdd(ComponentRegistration.Scoped<IEventQueue, SqlServerEventQueue<TDbContext>>());
                });

            return extension;
        }
    }
}