// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

namespace AppCore.EventModel.EntityFrameworkCore
{
    public class TestEvent : IEvent
    {
        public string? Value { get; set; }
    }
}