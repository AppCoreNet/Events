// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Collections.Generic;
using AppCore.Events.Metadata;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace AppCore.Events.Pipeline
{
    public class EventContextFactoryTests
    {
        [Fact]
        public void CreatesEventContext()
        {
            var descriptorFactory = Substitute.For<IEventDescriptorFactory>();
            var eventDescriptor = new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>());
            descriptorFactory.CreateDescriptor(Arg.Is(typeof(TestEvent)))
                             .Returns(eventDescriptor);

            var factory = new EventContextFactory(descriptorFactory);

            var @event = new TestEvent();
            IEventContext context = factory.CreateContext(@event);

            context.Should()
                   .BeOfType<EventContext<TestEvent>>();

            context.Event.Should()
                   .Be(@event);

            context.EventDescriptor.Should()
                   .Be(eventDescriptor);
        }
    }
}
