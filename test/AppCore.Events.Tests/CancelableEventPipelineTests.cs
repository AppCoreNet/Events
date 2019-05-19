// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System;
using System.Collections.Generic;
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
    public class CancelableEventPublisherTests
    {
        [Fact]
        public void CancelThrowsOperationCanceledException()
        {
            var accessor = Substitute.For<IEventContextAccessor>();

            var handler = Substitute.For<IEventHandler<CancelableTestEvent>>();
            handler.When(
                       h => h.HandleAsync(Arg.Any<IEventContext<CancelableTestEvent>>(), Arg.Any<CancellationToken>()))
                   .Do(
                       ci => ci.ArgAt<IEventContext<CancelableTestEvent>>(0)
                               .Cancel());

            var container = Substitute.For<IContainer>();
            container.Resolve(typeof(IEnumerable<IEventPipelineBehavior<CancelableTestEvent>>))
                     .Returns(
                         new[]
                         {
                             new CancelableEventBehavior<CancelableTestEvent>()
                         });
            container.Resolve(typeof(IEnumerable<IEventHandler<CancelableTestEvent>>))
                     .Returns(new[] { handler });

            var metadata = new Dictionary<string, object>();
            new CancelableEventMetadataProvider().GetMetadata(typeof(CancelableTestEvent), metadata);

            var contextFactory = Substitute.For<IEventContextFactory>();
            contextFactory.CreateContext(Arg.Any<CancelableTestEvent>())
                          .Returns(
                              ci => new EventContext<CancelableTestEvent>(
                                  new EventDescriptor(typeof(CancelableTestEvent), metadata),
                                  ci.ArgAt<CancelableTestEvent>(0)));

            var pipeline = new EventPublisher(container, contextFactory, accessor);

            Func<Task> invoke = async ()=>
            {
                await pipeline.PublishAsync(new CancelableTestEvent(), CancellationToken.None);
            };

            invoke.Should()
                  .Throw<OperationCanceledException>();
        }
    }
}
