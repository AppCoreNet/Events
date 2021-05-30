// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

namespace AppCore.EventModel.Store
{
    [Persistent(StreamName = "test")]
    public class TestEvent : IEvent
    {
    }
}