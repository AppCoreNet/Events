// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace AppCore.Events
{
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
                new[] {handler});

            var @event = new CancelableTestEvent();

            var eventContext = (IEventContext<CancelableTestEvent>) contextFactory.CreateContext(
                new EventDescriptor(typeof(CancelableTestEvent), metadata),
                @event);

            Func<Task> invoke = async ()=>
            {
                await pipeline.PublishAsync(eventContext, CancellationToken.None);
            };

            invoke.Should()
                  .Throw<OperationCanceledException>();
        }
    }
}
