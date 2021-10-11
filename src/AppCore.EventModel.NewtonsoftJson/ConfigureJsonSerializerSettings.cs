// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AppCore.EventModel.Formatters
{
    internal class ConfigureJsonSerializerSettings : ConfigureNamedOptions<JsonSerializerSettings>
    {
        public ConfigureJsonSerializerSettings(string name)
            : base(name, ConfigureOptions)
        {
        }

        private static void ConfigureOptions(JsonSerializerSettings o)
        {
            o.TypeNameHandling = TypeNameHandling.Auto;
            o.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}