// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
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
        private readonly IEventContextFactory _contextFactory;
        private readonly IEventContextAccessor _accessor;

        public EventPublisherTests()
        {
            _contextFactory = Substitute.For<IEventContextFactory>();
            _contextFactory.CreateContext(Arg.Any<TestEvent>())
                              .Returns(
                                  ci => new EventContext<TestEvent>(
                                      new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>()),
                                      ci.ArgAt<TestEvent>(0)));

            _accessor = Substitute.For<IEventContextAccessor>();
        }

        [Fact]
        public async Task AssignsEventContext()
        {
            var pipeline = Substitute.For<IEventPipeline<TestEvent>>();

            var container = Substitute.For<IContainer>();

            container.Resolve(typeof(IEventPipeline<TestEvent>))
                     .Returns(pipeline);

            var publisher = new EventPublisher(container, _contextFactory, _accessor);
            var @event = new TestEvent();
            var token = new CancellationToken();
            await publisher.PublishAsync(@event, token);

            _accessor.Received(1)
                     .EventContext = Arg.Is<EventContext<TestEvent>>(e => e.Event == @event);
        }

        [Fact]
        public async Task PublishesEventOnPipeline()
        {
            var pipeline = Substitute.For<IEventPipeline<TestEvent>>();

            var container = Substitute.For<IContainer>();

            container.Resolve(typeof(IEventPipeline<TestEvent>))
                     .Returns(pipeline);

            var publisher = new EventPublisher(container, _contextFactory, _accessor);
            var @event = new TestEvent();
            var token = new CancellationToken();
            await publisher.PublishAsync(@event, token);

            await pipeline.Received(1)
                          .ProcessAsync(
                              Arg.Is<IEventContext>(c => c.Event == @event),
                              Arg.Is(token));
        }
    }
}
