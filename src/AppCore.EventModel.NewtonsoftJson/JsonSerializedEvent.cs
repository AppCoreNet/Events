// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Collections.Generic;

namespace AppCore.Events.Formatters
{
    internal class JsonSerializedEvent
    {
        public IReadOnlyDictionary<string, object> Metadata { get; set; }

        public IDictionary<object, object> Items { get; set; }

        public IEvent Event { get; set; }
    }
}