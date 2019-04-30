// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using AppCore.DependencyInjection;
using AppCore.DependencyInjection.Facilities;

namespace AppCore.Events
{
    /// <summary>
    /// Represents the events facility.
    /// </summary>
    /// <seealso cref="IFacility"/>
    public interface IEventsFacility : IFacility
    {
        /// <summary>
        /// Gets or sets the default lifetime of the components.
        /// </summary>
        ComponentLifetime Lifetime { get; set; }
    }
}
