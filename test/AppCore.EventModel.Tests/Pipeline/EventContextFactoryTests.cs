// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Collections.Generic;
using AppCore.EventModel.Metadata;
using FluentAssertions;
using Xunit;

namespace AppCore.EventModel.Pipeline
{
    public class EventContextFactoryTests
    {
        [Fact]
        public void CreatesEventContext()
        {
            var eventDescriptor = new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>());
            
            var factory = new EventContextFactory();

            var @event = new TestEvent();
            IEventContext context = factory.CreateContext(eventDescriptor, @event);

            context.Should()
                   .BeOfType<EventContext<TestEvent>>();

            context.Event.Should()
                   .Be(@event);

            context.EventDescriptor.Should()
                   .Be(eventDescriptor);
        }
    }
}
