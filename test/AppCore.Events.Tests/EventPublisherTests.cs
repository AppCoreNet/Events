// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.DependencyInjection;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;
using FluentAssertions;
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
        public async Task InvokesHandlers()
        {
            var handler1 = Substitute.For<IEventHandler<TestEvent>>();
            var handler2 = Substitute.For<IEventHandler<TestEvent>>();

            var container = Substitute.For<IContainer>();

            container.Resolve(typeof(IEnumerable<IEventPipelineBehavior<TestEvent>>))
                     .Returns(Enumerable.Empty<IEventPipelineBehavior<TestEvent>>());
            
            container.Resolve(typeof(IEnumerable<IEventHandler<TestEvent>>))
                     .Returns(
                         new[]
                         {
                             handler1,
                             handler2
                         });

            var pipeline = new EventPublisher(container, _contextFactory);

            var @event = new TestEvent();
            var token = new CancellationToken();

            await pipeline.PublishAsync(@event, token);

            await handler1.Received(1)
                          .HandleAsync(Arg.Is<IEventContext<TestEvent>>(c => c.Event.Equals(@event)), token);

            await handler2.Received(1)
                          .HandleAsync(Arg.Is<IEventContext<TestEvent>>(c => c.Event.Equals(@event)), token);
        }

        [Fact]
        public async Task InvokesBehaviors()
        {
            var invokedBehaviors = new List<IEventPipelineBehavior<TestEvent>>();

            var behavior1 = Substitute.For<IEventPipelineBehavior<TestEvent>>();
            behavior1.When(
                         b => b.HandleAsync(
                             Arg.Any<IEventContext<TestEvent>>(),
                             Arg.Any<EventPipelineDelegate<TestEvent>>(),
                             Arg.Any<CancellationToken>()))
                     .Do(
                         async ci =>
                         {
                             invokedBehaviors.Add(behavior1);
                             await ci.ArgAt<EventPipelineDelegate<TestEvent>>(1)(
                                 ci.ArgAt<IEventContext<TestEvent>>(0),
                                 ci.ArgAt<CancellationToken>(2));
                         });

            var behavior2 = Substitute.For<IEventPipelineBehavior<TestEvent>>();
            behavior2.When(
                         b => b.HandleAsync(
                             Arg.Any<IEventContext<TestEvent>>(),
                             Arg.Any<EventPipelineDelegate<TestEvent>>(),
                             Arg.Any<CancellationToken>()))
                     .Do(
                         async ci =>
                         {
                             invokedBehaviors.Add(behavior2);
                             await ci.ArgAt<EventPipelineDelegate<TestEvent>>(1)(
                                 ci.ArgAt<IEventContext<TestEvent>>(0),
                                 ci.ArgAt<CancellationToken>(2));
                         });

            var container = Substitute.For<IContainer>();
            
            container.Resolve(typeof(IEnumerable<IEventPipelineBehavior<TestEvent>>))
                     .Returns(
                         new[]
                         {
                             behavior1,
                             behavior2
                         });

            container.Resolve(typeof(IEnumerable<IEventHandler<TestEvent>>))
                     .Returns(Enumerable.Empty<IEventHandler<TestEvent>>());

            var pipeline = new EventPublisher(container, _contextFactory);
            var @event = new TestEvent();
            var token = new CancellationToken();
            await pipeline.PublishAsync(@event, token);

            await behavior1.Received(1)
                           .HandleAsync(
                               Arg.Is<IEventContext<TestEvent>>(e => e.Event.Equals(@event)),
                               Arg.Any<EventPipelineDelegate<TestEvent>>(),
                               token);

            invokedBehaviors[0]
                .Should()
                .Be(behavior1);

            await behavior2.Received(1)
                           .HandleAsync(
                               Arg.Is<IEventContext<TestEvent>>(e => e.Event.Equals(@event)),
                               Arg.Any<EventPipelineDelegate<TestEvent>>(),
                               token);

            invokedBehaviors[1]
                .Should()
                .Be(behavior2);
        }

        [Fact]
        public async Task AssignsEventContext()
        {
            var container = Substitute.For<IContainer>();

            container.Resolve(typeof(IEnumerable<IEventHandler<TestEvent>>))
                     .Returns(Enumerable.Empty<IEventHandler<TestEvent>>());

            container.Resolve(typeof(IEnumerable<IEventPipelineBehavior<TestEvent>>))
                     .Returns(Enumerable.Empty<IEventPipelineBehavior<TestEvent>>());

            var pipeline = new EventPublisher(container, _contextFactory, _accessor);
            var @event = new TestEvent();
            var token = new CancellationToken();
            await pipeline.PublishAsync(@event, token);

            _accessor.Received(1)
                     .EventContext = Arg.Is<EventContext<TestEvent>>(e => e.Event == @event);
        }
    }
}
