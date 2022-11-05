// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AppCore.EventModel.Store;

public class EventStorePublisherTests
{
    private readonly IEventStore _store;
    private readonly IEventStoreConsumerOffset _storeOffset;
    private readonly IEventPipelineResolver _pipelineResolver;
    private readonly ILogger<EventStoreConsumer> _logger;
    private readonly EventStoreConsumer _consumer;
    private readonly IEventPipeline<TestEvent> _pipeline;

    public EventStorePublisherTests()
    {
        _store = Substitute.For<IEventStore>();
        _storeOffset = Substitute.For<IEventStoreConsumerOffset>();
        _pipelineResolver = Substitute.For<IEventPipelineResolver>();
        _logger = Substitute.For<ILogger<EventStoreConsumer>>();
        _pipeline = Substitute.For<IEventPipeline<TestEvent>>();
        _consumer = new EventStoreConsumer(_store, _storeOffset, _pipelineResolver, _logger);

        _pipelineResolver.Resolve(typeof(TestEvent))
                         .Returns(_pipeline);
    }

    [Fact]
    public async Task ProcessesEventsFromStore()
    {
        var event1 = new EventContext<TestEvent>(
            new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>()),
            new TestEvent());
        event1.AddFeature(Substitute.For<IEventStoreFeature>());

        var event2 = new EventContext<TestEvent>(
            new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>()),
            new TestEvent());
        event2.AddFeature(Substitute.For<IEventStoreFeature>());

        var events = new List<IEventContext>
        {
            event1,
            event2
        };

        _store.ReadAsync(
                  string.Empty,
                  Arg.Any<EventOffset>(),
                  Arg.Any<int>(),
                  Arg.Any<TimeSpan>(),
                  Arg.Any<CancellationToken>())
              .Returns(events);

        await _consumer.PublishPendingAsync(CancellationToken.None);

        await _pipeline.Received(1)
                       .ProcessAsync(events[0], Arg.Any<CancellationToken>());

        await _pipeline.Received(1)
                       .ProcessAsync(events[1], Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReadsEventsFromStoredOffset()
    {
        var offset = new EventOffset(12345);

        _storeOffset.GetNextOffset(Arg.Any<CancellationToken>())
                    .Returns(offset);
            
        await _consumer.PublishPendingAsync(CancellationToken.None);

        await _store.Received(1)
                    .ReadAsync(
                        string.Empty,
                        offset,
                        Arg.Any<int>(),
                        Arg.Any<TimeSpan>(),
                        Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CommitsOffsetOfProcessedEvent()
    {
        long offset = 12345;

        var event1 = new EventContext<TestEvent>(
            new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>()),
            new TestEvent());

        var eventStoreFeature = Substitute.For<IEventStoreFeature>();
        eventStoreFeature.Offset.Returns(offset);
        event1.AddFeature(eventStoreFeature);

        var events = new List<IEventContext>
        {
            event1
        };

        _store.ReadAsync(
                  string.Empty,
                  Arg.Any<EventOffset>(),
                  Arg.Any<int>(),
                  Arg.Any<TimeSpan>(),
                  Arg.Any<CancellationToken>())
              .Returns(events);

        await _consumer.PublishPendingAsync(CancellationToken.None);

        await _storeOffset.Received(1)
                          .CommitOffset(offset, Arg.Any<CancellationToken>());
    }
}