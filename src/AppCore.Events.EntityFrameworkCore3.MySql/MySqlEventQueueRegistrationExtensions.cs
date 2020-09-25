// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events;
using AppCore.Events.EntityFrameworkCore.MySql;
using AppCore.Events.Queue;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    public static class MySqlEventQueueRegistrationExtensions
    {
        /// <summary>
        /// Registers SQL Server event store.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityExtensionBuilder{TFacility,TExtension}"/>.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IFacilityExtensionBuilder<IEventsFacility, IEventQueueExtension>
            AddSqlServer<TDbContext>(
                this IFacilityExtensionBuilder<IEventsFacility, IEventQueueExtension> builder
                )
            where TDbContext : DbContext
        {
            Ensure.Arg.NotNull(builder, nameof(builder));

            builder.Configure(
                (f, e) => e.RegistrationCallbacks.Add(
                    (r, f2) => r.Register<IEventQueue>()
                                .Add<MySqlEventQueue<TDbContext>>()
                                .IfNoneRegistered()
                                .PerScope()));

            return builder;
        }
    }
}