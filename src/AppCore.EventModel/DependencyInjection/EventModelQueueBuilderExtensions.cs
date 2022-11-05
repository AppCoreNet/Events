// Licensed under the MIT License.
// Copyright (c) 2018-2022 the AppCore .NET project.

using AppCore.Diagnostics;
using AppCore.EventModel.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods to register in-memory event queue.
/// </summary>
public static class EventModelQueueBuilderExtensions
{
    /// <summary>
    /// Registers in-memory event queue.
    /// </summary>
    /// <returns>The <see cref="IEventModelQueueBuilder"/> to allow chaining.</returns>
    public static IEventModelQueueBuilder AddInMemoryQueue(this IEventModelQueueBuilder builder)
    {
        Ensure.Arg.NotNull(builder);

        IServiceCollection services = builder.Services;
        services.TryAddSingleton<IEventQueue, InMemoryEventQueue>();

        return builder;
    }
}