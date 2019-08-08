// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using AppCore.DependencyInjection.Facilities;
using AppCore.Diagnostics;
using AppCore.Events;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register 
    /// </summary>
    public static class NewtonsoftJsonRegistrationExtensions
    {
        /// <summary>
        /// Registers Newtonsoft.Json event formatter.
        /// </summary>
        /// <param name="builder">The <see cref="IFacilityExtensionBuilder{TFacility,TExtension}"/>.</param>
        /// <returns>The passed builder to allow chaining.</returns>
        public static IFacilityBuilder<IEventsFacility> AddNewtonsoftJsonFormatter(
            this IFacilityBuilder<IEventsFacility> builder)
        {
            Ensure.Arg.NotNull(builder, nameof(builder));
            return builder.Add<NewtonsoftJsonFormatterExtension>();
        }
    }
}