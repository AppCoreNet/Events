// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

namespace AppCore.Events.Storage
{
    [Persistent(StreamName = "test")]
    public class TestEvent : IEvent
    {
    }
}