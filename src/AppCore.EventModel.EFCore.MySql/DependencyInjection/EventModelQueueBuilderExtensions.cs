// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.Data;
using AppCore.Diagnostics;
using AppCore.EventModel.EntityFrameworkCore.MySql;
using AppCore.EventModel.Queue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extensions methods to register MySql event queue.
    /// </summary>
    public static class EventModelQueueBuilderExtensions
    {
        /// <summary>
        /// Registers MySql event queue.
        /// </summary>
        /// <param name="builder">The <see cref="IEventModelQueueBuilder"/>.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IEventModelQueueBuilder AddMySql<TTag, TDbContext>(this IEventModelQueueBuilder builder)
            where TDbContext : DbContext
        {
            Ensure.Arg.NotNull(builder);

            IServiceCollection services = builder.Services;

            services.AddAppCore()
                    .AddDataProvider(d => d.AddDbContext<TTag, TDbContext>());

            services.TryAddScoped<IEventQueue, MySqlEventQueue<TDbContext>>();

            return builder;
        }

        /// <summary>
        /// Registers MySql event queue.
        /// </summary>
        /// <param name="builder">The <see cref="IEventModelQueueBuilder"/>.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IEventModelQueueBuilder AddMySql<TDbContext>(this IEventModelQueueBuilder builder)
            where TDbContext : DbContext
        {
            return builder.AddMySql<DefaultDataProvider, TDbContext>();
        }
    }
}