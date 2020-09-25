// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Collections.Generic;
using AppCore.DependencyInjection.Facilities;
using AppCore.Events;

// ReSharper disable once CheckNamespace
namespace AppCore.DependencyInjection
{
    /// <summary>
    /// Provides event queuing extension for the <see cref="IEventsFacility"/>.
    /// </summary>
    public interface IEventQueueExtension : IFacilityExtension<IEventsFacility>
    {
        /// <summary>
        /// Gets a list of additional registration callbacks.
        /// </summary>
        IList<Action<IComponentRegistry, IEventsFacility>> RegistrationCallbacks { get; }
    }
}