// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events.EntityFrameworkCore.MySql;
using AppCore.Events.Queue;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    public static class MySqlEventQueueRegistrationExtensions
    {
        /// <summary>
        /// Registers MySql event queue.
        /// </summary>
        /// <param name="extension">The <see cref="EventQueueExtension"/>.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventQueueExtension WithMySqlEventQueue<TDbContext>(this EventQueueExtension extension)
            where TDbContext : DbContext
        {
            Ensure.Arg.NotNull(extension, nameof(extension));

            extension.ConfigureRegistry(
                r =>
                {
                    r.TryAdd(ComponentRegistration.Scoped<IEventQueue, MySqlEventQueue<TDbContext>>());
                });

            return extension;
        }
    }
}