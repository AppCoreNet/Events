// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Logging;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace AppCore.EventModel.Pipeline
{
    public class EventPipelineTests
    {
        [Fact]
        public async Task InvokesHandlers()
        {
            var handler1 = Substitute.For<IEventHandler<TestEvent>>();
            var handler2 = Substitute.For<IEventHandler<TestEvent>>();

            var pipeline = new EventPipeline<TestEvent>(
                Enumerable.Empty<IEventPipelineBehavior<TestEvent>>(),
                new[]
                {
                    handler1,
                    handler2
                },
                Substitute.For<ILogger<EventPipeline<TestEvent>>>());

            var @event = new TestEvent();

            var eventContext = Substitute.For<IEventContext<TestEvent>>();
            eventContext.Event.Returns(@event);

            var token = new CancellationToken();

            await pipeline.ProcessAsync(eventContext, token);

            await handler1.Received(1)
                          .HandleAsync(eventContext, token);

            await handler2.Received(1)
                          .HandleAsync(eventContext, token);
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

            var pipeline = new EventPipeline<TestEvent>(
                new[]
                {
                    behavior1,
                    behavior2
                },
                Enumerable.Empty<IEventHandler<TestEvent>>(),
                Substitute.For<ILogger<EventPipeline<TestEvent>>>());

            var @event = new TestEvent();

            var eventContext = Substitute.For<IEventContext<TestEvent>>();
            eventContext.Event.Returns(@event);

            var token = new CancellationToken();
            await pipeline.ProcessAsync(eventContext, token);

            await behavior1.Received(1)
                           .HandleAsync(
                               eventContext,
                               Arg.Any<EventPipelineDelegate<TestEvent>>(),
                               token);

            invokedBehaviors[0]
                .Should()
                .Be(behavior1);

            await behavior2.Received(1)
                           .HandleAsync(
                               eventContext,
                               Arg.Any<EventPipelineDelegate<TestEvent>>(),
                               token);

            invokedBehaviors[1]
                .Should()
                .Be(behavior2);
        }

        [Fact]
        public async Task AssignsEventContext()
        {
            var handler = Substitute.For<IEventHandler<TestEvent>>();
            var accessor = Substitute.For<IEventContextAccessor>();
            var @event = new TestEvent();
            var context = Substitute.For<IEventContext<TestEvent>>();
            context.Event.Returns(@event);
            
            var pipeline = new EventPipeline<TestEvent>(
                Enumerable.Empty<IEventPipelineBehavior<TestEvent>>(),
                new[] { handler }, Substitute.For<ILogger<EventPipeline<TestEvent>>>(), accessor);

            await pipeline.ProcessAsync(context, CancellationToken.None);

            accessor.Received(1)
                    .EventContext = context;

            accessor.Received(1)
                    .EventContext = null;
        }
    }
}