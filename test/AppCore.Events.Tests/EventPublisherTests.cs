// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;
using AppCore.Events.Queue;
using NSubstitute;
using Xunit;

namespace AppCore.Events
{
    public class EventPublisherTests
    {
        private readonly IEventDescriptorFactory _descriptorFactory;
        private readonly IEventContextFactory _contextFactory;
        private readonly IEventPipelineResolver _pipelineResolver;
        private readonly IEventPipeline<TestEvent> _pipeline;

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

            _pipeline = Substitute.For<IEventPipeline<TestEvent>>();

            _pipelineResolver = Substitute.For<IEventPipelineResolver>();
            _pipelineResolver.Resolve(typeof(TestEvent))
                             .Returns(ci => _pipeline);
        }

        [Fact]
        public async Task PublishesEventUsingPipeline()
        {
            var publisher = new EventPublisher(_descriptorFactory, _contextFactory, _pipelineResolver);
            var @event = new TestEvent();
            var token = new CancellationToken();
            await publisher.PublishAsync(@event, token);

            await _pipeline.Received(1)
                          .ProcessAsync(
                              Arg.Is<IEventContext>(c => c.Event == @event),
                              Arg.Is(token));
        }

        [Fact]
        public async Task PublishesEventUsingQueue()
        {
            var queue = Substitute.For<IEventQueue>();

            var publisher = new EventPublisher(_descriptorFactory, _contextFactory, _pipelineResolver, queue);
            var @event = new TestEvent();
            var token = new CancellationToken();
            await publisher.PublishAsync(@event, token);

            await queue.Received(1)
                       .WriteAsync(
                           Arg.Is<IEnumerable<IEventContext>>(
                               c => c.Count() == 1 && c.First().Event == @event),
                           Arg.Is(token));

            await _pipeline.DidNotReceiveWithAnyArgs()
                           .ProcessAsync(Arg.Any<IEventContext>(),
                                         Arg.Any<CancellationToken>());
        }
    }
}
