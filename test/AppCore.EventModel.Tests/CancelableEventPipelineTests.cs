// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AppCore.EventModel;

public class CancelableEventPublisherTests
{
    [Fact]
    public void CancelThrowsOperationCanceledException()
    {
        var handler = Substitute.For<IEventHandler<CancelableTestEvent>>();
        handler.When(
                   h => h.HandleAsync(Arg.Any<IEventContext<CancelableTestEvent>>(), Arg.Any<CancellationToken>()))
               .Do(
                   ci => ci.ArgAt<IEventContext<CancelableTestEvent>>(0)
                           .Cancel());

        var metadata = new Dictionary<string, object>();
        new CancelableEventMetadataProvider().GetMetadata(typeof(CancelableTestEvent), metadata);

        var contextFactory = Substitute.For<IEventContextFactory>();
        contextFactory.CreateContext(Arg.Any<EventDescriptor>(), Arg.Any<CancelableTestEvent>())
                      .Returns(
                          ci => new EventContext<CancelableTestEvent>(
                              ci.ArgAt<EventDescriptor>(0),
                              ci.ArgAt<CancelableTestEvent>(1)));

        var pipeline = new EventPipeline<CancelableTestEvent>(
            new[]
            {
                new CancelableEventBehavior<CancelableTestEvent>()
            },
            new[] {handler},
            Substitute.For<ILogger<EventPipeline<CancelableTestEvent>>>());

        var @event = new CancelableTestEvent();

        var eventContext = (IEventContext<CancelableTestEvent>) contextFactory.CreateContext(
            new EventDescriptor(typeof(CancelableTestEvent), metadata),
            @event);

        Func<Task> invoke = async ()=>
        {
            await pipeline.ProcessAsync(eventContext, CancellationToken.None);
        };

        invoke.Should()
              .Throw<OperationCanceledException>();
    }
}