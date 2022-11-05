// Licensed under the MIT License.
// Copyright (c) 2018-2022 the AppCore .NET project.

using AppCore.Diagnostics;
using AppCore.EventModel.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods to register event store services.
/// </summary>
public static class EventModelStoreBuilderExtensions
{
    /// <summary>
    /// Registers in-memory event store.
    /// </summary>
    /// <param name="builder">The <see cref="IEventModelStoreBuilder"/>.</param>
    /// <returns>The <see cref="IEventModelStoreBuilder"/> to allow chaining.</returns>
    public static IEventModelStoreBuilder AddInMemoryStore(this IEventModelStoreBuilder builder)
    {
        Ensure.Arg.NotNull(builder);

        IServiceCollection services = builder.Services;
        services.TryAddSingleton<IEventStore, InMemoryEventStore>();

        return builder;
    }

    /// <summary>
    /// Register background service which consumes events from the store and publishes them.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>The <see cref="IEventModelStoreBuilder"/> to allow chaining.</returns>
    public static IEventModelStoreBuilder AddBackgroundConsumer(this IEventModelStoreBuilder builder)
    {
        Ensure.Arg.NotNull(builder);

        IServiceCollection services = builder.Services;
        services.TryAddSingleton<IEventStoreConsumerOffset, EventStoreConsumerOffset>();
        services.TryAddScoped<IEventStoreConsumer, EventStoreConsumer>();
        services.AddHostedService<EventStoreConsumerService>();

        return builder;
    }
}