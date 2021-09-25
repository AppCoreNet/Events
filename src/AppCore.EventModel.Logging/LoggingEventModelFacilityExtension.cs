// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.EventModel.Logging;
using AppCore.EventModel.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides a extension for <see cref="EventModelFacility"/> to add support for logging.
    /// </summary>
    public class LoggingEventModelFacilityExtension : FacilityExtension
    {
        /// <inheritdoc />
        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.TryAddEnumerable(
                ServiceDescriptor.Singleton(
                    typeof(IEventPipelineBehavior<>),
                    typeof(EventLoggingBehavior<>)));
        }
    }
}