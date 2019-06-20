// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Events.Metadata;
using AppCore.Events.Pipeline;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace AppCore.Events.Store
{
    public class InMemoryEventStoreTests
    {
        private IEventContextFactory CreateEventContextFactory()
        {
            var factory = Substitute.For<IEventContextFactory>();
            factory.CreateContext(Arg.Any<EventDescriptor>(), Arg.Any<TestEvent>())
                   .Returns(
                       ci =>
                       {
                           var c = Substitute.For<IEventContext<TestEvent>>();
                           ((IEventContext) c).Event.Returns(ci.ArgAt<TestEvent>(1));
                           c.Event.Returns(ci.ArgAt<TestEvent>(1));
                           c.EventDescriptor.Returns(ci.ArgAt<EventDescriptor>(0));
                           return (IEventContext) c;
                       });

            return factory;
        }

        private IEventContext<TestEvent> CreateEventContext(TestEvent @event)
        {
            var eventContext = Substitute.For<IEventContext<TestEvent>>();
            ((IEventContext) eventContext).Event.Returns(@event);
            eventContext.Event.Returns(@event);
            eventContext.EventDescriptor.Returns(
                new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>()));

            return eventContext;
        }

        [Fact]
        public async Task ReadAsyncReturnsEmptyAfterTimeoutElapsed()
        {
            var contextFactory = Substitute.For<IEventContextFactory>();
            var store = new InMemoryEventStore(contextFactory);

            IEnumerable<IEventContext> result = await store.ReadAsync(
                string.Empty,
                0,
                1,
                TimeSpan.FromMilliseconds(100),
                CancellationToken.None);

            result.Should()
                  .HaveCount(0);
        }

        [Fact]
        public async Task ReadAsyncReturnsAvailableEvents()
        {
            var contextFactory = Substitute.For<IEventContextFactory>();
            var store = new InMemoryEventStore(contextFactory);

            var @event = new TestEvent();
            IEventContext<TestEvent> eventContext = CreateEventContext(@event);

            await store.WriteAsync(new[] {eventContext}, CancellationToken.None);

            IEnumerable<IEventContext> result = await store.ReadAsync(
                string.Empty,
                0,
                2,
                TimeSpan.FromMilliseconds(100),
                CancellationToken.None);

            result.Should()
                  .HaveCount(1);
        }

        [Fact]
        public async Task ReadAsyncWaitsForEvents()
        {
            var contextFactory = Substitute.For<IEventContextFactory>();
            var store = new InMemoryEventStore(contextFactory);

            var @event = new TestEvent();
            IEventContext<TestEvent> eventContext = CreateEventContext(@event);

            var semaphore = new SemaphoreSlim(0, 1);

            Task<IEnumerable<IEventContext>> readerTask = Task.Run(
                async () =>
                {
                    semaphore.Release();
                    return await store.ReadAsync(
                        string.Empty,
                        0,
                        2,
                        TimeSpan.FromSeconds(1),
                        CancellationToken.None);
                });

            await semaphore.WaitAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(250));
            await store.WriteAsync(new[] {eventContext}, CancellationToken.None);

            IEnumerable<IEventContext> result = await readerTask;
            result.Should()
                  .HaveCount(1);
        }

        [Fact]
        public async Task ReadAsyncWaitsForEventsWithSequence()
        {
            var store = new InMemoryEventStore(CreateEventContextFactory());

            var semaphore = new SemaphoreSlim(0, 1);

            Task<IEnumerable<IEventContext>> readerTask = Task.Run(
                async () =>
                {
                    semaphore.Release();
                    return await store.ReadAsync(
                        string.Empty,
                        1,
                        2,
                        TimeSpan.FromSeconds(60),
                        CancellationToken.None);
                });

            await semaphore.WaitAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(250));
            var event1 = new TestEvent();
            IEventContext<TestEvent> eventContext1 = CreateEventContext(event1);
            await store.WriteAsync(new[] {eventContext1}, CancellationToken.None);
            
            await Task.Delay(TimeSpan.FromMilliseconds(250));
            var event2 = new TestEvent();
            IEventContext<TestEvent> eventContext2 = CreateEventContext(event2);
            
            await store.WriteAsync(new[] {eventContext2}, CancellationToken.None);

            IEnumerable<IEventContext> result = await readerTask;
            result.Should()
                  .HaveCount(1);

            result.First()
                  .Event
                  .Should()
                  .Be(event2);
        }
    }
}
