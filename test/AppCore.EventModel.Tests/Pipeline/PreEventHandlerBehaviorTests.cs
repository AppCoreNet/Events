// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.EventModel.Metadata;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AppCore.EventModel.Pipeline;

public class PreEventHandlerBehaviorTests
{
    [Fact]
    public async Task InvokesHandlersBeforeNext()
    {
        var invokeOrder = new List<object>();

        var handlers = new[]
        {
            Substitute.For<IPreEventHandler<TestEvent>>(),
            Substitute.For<IPreEventHandler<TestEvent>>()
        };

        handlers[0]
            .When(h => h.OnHandlingAsync(Arg.Any<IEventContext<TestEvent>>(), Arg.Any<CancellationToken>()))
            .Do(_ => invokeOrder.Add(handlers[0]));

        handlers[1]
            .When(h => h.OnHandlingAsync(Arg.Any<IEventContext<TestEvent>>(), Arg.Any<CancellationToken>()))
            .Do(_ => invokeOrder.Add(handlers[1]));

        var next = Substitute.For<EventPipelineDelegate<TestEvent>>();
        next.When(n => n.Invoke(Arg.Any<IEventContext<TestEvent>>(), Arg.Any<CancellationToken>()))
            .Do(_ => { invokeOrder.Add(next); });

        var @event = new TestEvent();
        var context = new EventContext<TestEvent>(
            new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>()),
            @event);
        var ct = new CancellationToken();

        var behavior = new PreEventHandlerBehavior<TestEvent>(handlers, Substitute.For<ILogger<PreEventHandlerBehavior<TestEvent>>>());
        await behavior.HandleAsync(context, next, ct);

        await handlers[0]
              .Received(1)
              .OnHandlingAsync(Arg.Is<IEventContext<TestEvent>>(c => c.Event.Equals(@event)), ct);

        invokeOrder[0]
            .Should()
            .Be(handlers[0]);

        await handlers[1]
              .Received(1)
              .OnHandlingAsync(Arg.Is<IEventContext<TestEvent>>(c => c.Event.Equals(@event)), ct);

        invokeOrder[1]
            .Should()
            .Be(handlers[1]);

        await next.Received(1)
                  .Invoke(Arg.Is<IEventContext<TestEvent>>(e => e.Event.Equals(@event)), ct);

        invokeOrder[2]
            .Should()
            .Be(next);
    }
}