// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.Data;
using AppCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extensions for the <see cref="EventQueueFacilityExtension"/>.
    /// </summary>
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
            extension.Facility.AddExtension<PostgreSqlEventQueueFacilityExtension<TTag, TDbContext>>();
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