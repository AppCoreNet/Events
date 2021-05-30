// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;
using AppCore.Logging;
using NSubstitute;
using Xunit;

namespace AppCore.EventModel.Store
{
    public class EventStoreBehaviorTests
    {
        private readonly IEventStore _store;
        private readonly ILogger<EventStoreBehavior<TestEvent>> _logger;
        private readonly EventStoreBehavior<TestEvent> _behavior;
        private readonly EventPipelineDelegate<TestEvent> _next;

        public EventStoreBehaviorTests()
        {
            _store = Substitute.For<IEventStore>();
            _logger = Substitute.For<ILogger<EventStoreBehavior<TestEvent>>>();
            _behavior = new EventStoreBehavior<TestEvent>(_store, _logger);
            _next = Substitute.For<EventPipelineDelegate<TestEvent>>();
        }

        private static EventContext<TestEvent> CreateEventContext(bool isPersistent = true)
        {
            var testEvent = new TestEvent();
            var metadata = new Dictionary<string, object>();

            if (isPersistent)
                metadata.Add(EventStoreMetadataKeys.PersistentMetadataKey, true);

            var eventDescriptor = new EventDescriptor(typeof(TestEvent), metadata);
            var eventContext = new EventContext<TestEvent>(eventDescriptor, testEvent);
            return eventContext;
        }

        [Fact]
        public async Task WritesEventWithPersistentMetadataToStore()
        {
            EventContext<TestEvent> eventContext = CreateEventContext();

            await _behavior.HandleAsync(eventContext, _next, CancellationToken.None);

            await _store.Received(1)
                        .WriteAsync(
                            Arg.Is<IEnumerable<IEventContext>>(e => e.First() == eventContext),
                            Arg.Any<CancellationToken>());

            await _next.DidNotReceive()
                       .Invoke(Arg.Any<IEventContext<TestEvent>>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task InvokesNextBehaviorWhenEventReadFromStore()
        {
            EventContext<TestEvent> eventContext = CreateEventContext();
            eventContext.AddFeature<IEventStoreFeature>(new EventStoreFeature(_store, 0));

            await _behavior.HandleAsync(eventContext, _next, CancellationToken.None);

            await _store.DidNotReceive()
                        .WriteAsync(
                            Arg.Any<IEnumerable<IEventContext>>(),
                            Arg.Any<CancellationToken>());

            await _next.Received(1)
                       .Invoke(eventContext, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task InvokesNextBehaviorWhenEventIsNotPersistent()
        {
            EventContext<TestEvent> eventContext = CreateEventContext(false);

            await _behavior.HandleAsync(eventContext, _next, CancellationToken.None);

            await _store.DidNotReceive()
                        .WriteAsync(
                            Arg.Any<IEnumerable<IEventContext>>(),
                            Arg.Any<CancellationToken>());

            await _next.Received(1)
                       .Invoke(eventContext, Arg.Any<CancellationToken>());
        }
    }
}
