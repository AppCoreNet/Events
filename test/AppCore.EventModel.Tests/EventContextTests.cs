// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Generic;
using AppCore.Events.Metadata;
using FluentAssertions;
using Xunit;

namespace AppCore.Events
{
    public class EventContextTests
    {
        [Fact]
        public void ConstructorThrowsForIncompatibleArguments()
        {
            Action action = () =>
            {
                var unused = new EventContext<TestEvent>(
                    new EventDescriptor(typeof(CancelableTestEvent), new Dictionary<string, object>()),
                    new TestEvent());
            };

            action.Should()
                  .Throw<ArgumentException>();
        }
    }
}
