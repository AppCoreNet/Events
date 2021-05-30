// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.Data;
using AppCore.Diagnostics;
using AppCore.EventModel.EntityFrameworkCore.MySql;
using AppCore.EventModel.Queue;
using AppCore.Events;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    public static class EventsFacilityExtensions
    {
        /// <summary>
        /// Registers MySql event queue.
        /// </summary>
        /// <param name="extension">The <see cref="EventQueueFacilityExtension"/>.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventQueueFacilityExtension WithMySqlQueue<TTag, TDbContext>(
            this EventQueueFacilityExtension extension)
            where TDbContext : DbContext
        {
            Ensure.Arg.NotNull(extension, nameof(extension));

            extension.ConfigureRegistry(
                r =>
                {
                    r.AddDataProvider(d => d.UseEntityFrameworkCore<TTag, TDbContext>());
                    r.TryAdd(ComponentRegistration.Scoped<IEventQueue, MySqlEventQueue<TDbContext>>());
                });

            return extension;
        }

        /// <summary>
        /// Registers MySql event queue.
        /// </summary>
        /// <param name="extension">The <see cref="EventQueueFacilityExtension"/>.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventQueueFacilityExtension WithMySqlQueue<TDbContext>(this EventQueueFacilityExtension extension)
            where TDbContext : DbContext
        {
            return extension.WithMySqlQueue<DefaultDataProvider, TDbContext>();
        }
    }
}