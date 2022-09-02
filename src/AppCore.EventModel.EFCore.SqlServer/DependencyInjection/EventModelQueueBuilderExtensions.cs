// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.Data;
using AppCore.Diagnostics;
using AppCore.EventModel.EntityFrameworkCore.SqlServer;
using AppCore.EventModel.Queue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extensions methods to register SQL Server event queue.
    /// </summary>
    public static class EventModelQueueBuilderExtensions
    {
        /// <summary>
        /// Registers SQL Server event queue.
        /// </summary>
        /// <param name="builder">The <see cref="IEventModelQueueBuilder"/>.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IEventModelQueueBuilder WithSqlServerQueue<TTag, TDbContext>(this IEventModelQueueBuilder builder)
            where TDbContext : DbContext
        {
            Ensure.Arg.NotNull(builder);

            IServiceCollection services = builder.Services;

            services.AddAppCore()
                    .AddDataProvider(d => d.AddDbContext<TTag, TDbContext>());

            services.TryAddScoped<IEventQueue, SqlServerEventQueue<TDbContext>>();

            return builder;
        }

        /// <summary>
        /// Registers SQL Server event queue.
        /// </summary>
        /// <param name="builder">The <see cref="IEventModelQueueBuilder"/>.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IEventModelQueueBuilder WithSqlServerQueue<TDbContext>(this IEventModelQueueBuilder builder)
            where TDbContext : DbContext
        {
            return builder.WithSqlServerQueue<DefaultDataProvider, TDbContext>();
        }
    }
}