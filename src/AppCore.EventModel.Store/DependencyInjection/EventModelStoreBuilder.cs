// Licensed under the MIT License.
// Copyright (c) 2018-2022 the AppCore .NET project.

using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace AppCore.Extensions.DependencyInjection;

internal sealed class EventModelStoreBuilder : IEventModelStoreBuilder
{
    public IServiceCollection Services { get; }

    public EventModelStoreBuilder(IServiceCollection services)
    {
        Services = services;
    }
}