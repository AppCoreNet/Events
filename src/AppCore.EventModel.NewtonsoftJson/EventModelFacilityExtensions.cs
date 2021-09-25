// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using System;
using AppCore.Diagnostics;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to register 
    /// </summary>
    public static class EventModelFacilityExtensions
    {
        /// <summary>
        /// Registers Newtonsoft.Json event formatter.
        /// </summary>
        /// <param name="facility">The <see cref="EventModelFacility"/>.</param>
        /// <param name="configure">The settings configuration delegate.</param>
        /// <returns>The passed facility to allow chaining.</returns>
        public static EventModelFacility UseNewtonsoftJson(this EventModelFacility facility, Action<NewtonsoftJsonEventModelFacilityExtension> configure = null)
        {
            Ensure.Arg.NotNull(facility, nameof(facility));
            facility.AddExtension(configure);
            return facility;
        }
    }
}