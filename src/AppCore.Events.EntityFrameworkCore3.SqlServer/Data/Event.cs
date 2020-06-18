// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

namespace AppCore.Events.EntityFrameworkCore.SqlServer.Data
{
    public class Event
    {
        public long Offset { get; set; }

        public string Topic { get; set; }

        public string ContentType { get; set; }

        public byte[] Data { get; set; }
    }
}