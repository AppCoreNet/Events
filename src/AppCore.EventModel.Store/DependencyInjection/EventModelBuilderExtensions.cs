// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.Diagnostics;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;
using AppCore.EventModel.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.Extensions.DependencyInjection;

/// <summary>
/// Provides extensions to register the event model.
/// </summary>
public static class EventModelBuilderExtensions
{
    /// <summary>
    /// Registers event store services.
    /// </summary>
    /// <returns>The <see cref="IEventModelBuilder"/> to allow chaining.</returns>
    public static IEventModelStoreBuilder AddEventStore(this IEventModelBuilder builder)
    {
        Ensure.Arg.NotNull(builder);

        IServiceCollection services = builder.Services;

        services.TryAddEnumerable(
            new[]
            {
                ServiceDescriptor.Singleton<IEventMetadataProvider, EventStoreMetadataProvider>(),
                ServiceDescriptor.Transient(typeof(IEventPipelineBehavior<>), typeof(EventStoreBehavior<>))
            });

        return new EventModelStoreBuilder(services);
    }
}