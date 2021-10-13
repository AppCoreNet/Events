// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.EventModel.EntityFrameworkCore.MySql;
using AppCore.EventModel.Queue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides a MySql extension for the <see cref="EventQueueFacilityExtension"/>.
    /// </summary>
    /// <typeparam name="TTag">The data provider tag.</typeparam>
    /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
    public class MySqlEventQueueFacilityExtension<TTag, TDbContext> : FacilityExtension
        where TDbContext : DbContext
    {
        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddAppCore().AddDataProvider(d => d.UseEntityFrameworkCore<TTag, TDbContext>());
            services.TryAddScoped<IEventQueue, MySqlEventQueue<TDbContext>>();
        }
    }
}