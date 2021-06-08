// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.Data;
using AppCore.Diagnostics;
using AppCore.EventModel;
using AppCore.EventModel.EntityFrameworkCore.PostgreSql;
using AppCore.EventModel.Queue;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    public static class EventModelFacilityExtensions
    {
        /// <summary>
        /// Registers PostgreSQL event queue.
        /// </summary>
        /// <param name="extension">The <see cref="EventQueueFacilityExtension"/>.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventQueueFacilityExtension WithPostgreSqlQueue<TTag, TDbContext>(
            this EventQueueFacilityExtension extension)
            where TDbContext : DbContext
        {
            Ensure.Arg.NotNull(extension, nameof(extension));

            extension.ConfigureRegistry(
                r =>
                {
                    r.AddDataProvider(d => d.UseEntityFrameworkCore<TTag, TDbContext>());
                    r.TryAdd(ComponentRegistration.Scoped<IEventQueue, PostgreSqlEventQueue<TDbContext>>());
                });

            return extension;
        }

        /// <summary>
        /// Registers PostgreSQL event queue.
        /// </summary>
        /// <param name="extension">The <see cref="EventQueueFacilityExtension"/>.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventQueueFacilityExtension WithPostgreSqlQueue<TDbContext>(this EventQueueFacilityExtension extension)
            where TDbContext : DbContext
        {
            return extension.WithPostgreSqlQueue<DefaultDataProvider, TDbContext>();
        }
    }
}