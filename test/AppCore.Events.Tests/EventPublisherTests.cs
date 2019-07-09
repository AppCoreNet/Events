// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.DependencyInjection;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;
using NSubstitute;
using Xunit;

namespace AppCore.Events
{
    public class EventPublisherTests
    {
        private readonly IEventDescriptorFactory _descriptorFactory;
        private readonly IEventContextFactory _contextFactory;

        public EventPublisherTests()
        {
            _descriptorFactory = Substitute.For<IEventDescriptorFactory>();
            _descriptorFactory.CreateDescriptor(typeof(TestEvent))
                              .Returns(new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>()));

            _contextFactory = Substitute.For<IEventContextFactory>();
            _contextFactory.CreateContext(Arg.Any<EventDescriptor>(),Arg.Any<TestEvent>())
                              .Returns(
                                  ci => new EventContext<TestEvent>(
                                      ci.ArgAt<EventDescriptor>(0),
                                      ci.ArgAt<TestEvent>(1)));
        }

        [Fact]
        public async Task PublishesEventOnPipeline()
        {
            var pipeline = Substitute.For<IEventPipeline<TestEvent>>();

            var container = Substitute.For<IContainer>();

            container.Resolve(typeof(IEventPipeline<TestEvent>))
                     .Returns(pipeline);

            var publisher = new EventPublisher(_descriptorFactory, _contextFactory, container);
            var @event = new TestEvent();
            var token = new CancellationToken();
            await publisher.PublishAsync(@event, token);

            await pipeline.Received(1)
                          .PublishAsync(
                              Arg.Is<IEventContext>(c => c.Event == @event),
                              Arg.Is(token));
        }
    }
}
