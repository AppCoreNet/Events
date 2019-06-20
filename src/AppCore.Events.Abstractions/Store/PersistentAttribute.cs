// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;

namespace AppCore.Events.Store
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PersistentAttribute : Attribute
    {
        public string StreamName { get; set; }
    }
}