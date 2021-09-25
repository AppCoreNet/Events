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
        /// Registers MySql event queue.
        /// </summary>
        /// <param name="extension">The <see cref="EventQueueFacilityExtension"/>.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventQueueFacilityExtension WithMySqlQueue<TTag, TDbContext>(
            this EventQueueFacilityExtension extension)
            where TDbContext : DbContext
        {
            Ensure.Arg.NotNull(extension, nameof(extension));
            extension.Facility.AddExtension<MySqlEventQueueFacilityExtension<TTag, TDbContext>>();
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