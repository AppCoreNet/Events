// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppCore.EventModel.Metadata;
using AppCore.EventModel.Pipeline;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace AppCore.EventModel.Store
{
    public class InMemoryEventStoreTests
    {
        private IEventContext<TestEvent> CreateEventContext(TestEvent @event, EventDescriptor descriptor = null)
        {
            var features = new Dictionary<Type, object>();

            var eventContext = Substitute.For<IEventContext<TestEvent>>();
            ((IEventContext) eventContext).Event.Returns(@event);
            eventContext.Features.Returns(features);
            eventContext.Event.Returns(@event);
            eventContext.EventDescriptor.Returns(
                descriptor ?? new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>()));

            return eventContext;
        }

        private IEventContextFactory CreateEventContextFactory()
        {
            var factory = Substitute.For<IEventContextFactory>();
            factory.CreateContext(Arg.Any<EventDescriptor>(), Arg.Any<TestEvent>())
                   .Returns(
                       ci => (IEventContext) CreateEventContext(
                           ci.ArgAt<TestEvent>(1),
                           ci.ArgAt<EventDescriptor>(0)));

            return factory;
        }

        [Fact]
        public async Task ReadAsyncReturnsEmptyAfterTimeoutElapsed()
        {
            var store = new InMemoryEventStore(CreateEventContextFactory());

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
            var store = new InMemoryEventStore(CreateEventContextFactory());

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
        public async Task ReadAsyncReturnsAvailableEventsFromStart()
        {
            var store = new InMemoryEventStore(CreateEventContextFactory());

            var @event = new TestEvent();
            IEventContext<TestEvent> eventContext = CreateEventContext(@event);

            await store.WriteAsync(new[] {eventContext}, CancellationToken.None);

            IEnumerable<IEventContext> result = await store.ReadAsync(
                string.Empty,
                -1,
                2,
                TimeSpan.FromMilliseconds(100),
                CancellationToken.None);

            result.Should()
                  .HaveCount(1);
        }

        [Fact]
        public async Task ReadAsyncWaitsForEvents()
        {
            var store = new InMemoryEventStore(CreateEventContextFactory());

            var @event = new TestEvent();
            IEventContext<TestEvent> eventContext = CreateEventContext(@event);

            var semaphore = new SemaphoreSlim(0, 1);

            Task<IReadOnlyCollection<IEventContext>> readerTask = Task.Run(
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
        public async Task ReadAsyncWaitsForEventsByOffset()
        {
            var store = new InMemoryEventStore(CreateEventContextFactory());

            var semaphore = new SemaphoreSlim(0, 1);

            Task<IReadOnlyCollection<IEventContext>> readerTask = Task.Run(
                async () =>
                {
                    semaphore.Release();
                    return await store.ReadAsync(
                        string.Empty,
                        1,
                        2,
                        TimeSpan.FromSeconds(1),
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

        [Fact]
        public async Task ReadAsyncAddsEventStoreFeature()
        {
            var store = new InMemoryEventStore(CreateEventContextFactory());

            var @event = new TestEvent();
            IEventContext<TestEvent> eventContext = CreateEventContext(@event);

            await store.WriteAsync(new[] {eventContext}, CancellationToken.None);

            IEnumerable<IEventContext> result = await store.ReadAsync(
                string.Empty,
                0,
                2,
                TimeSpan.FromMilliseconds(100),
                CancellationToken.None);

            IEventContext context = result.First();

            context.HasFeature<IEventStoreFeature>()
                   .Should()
                   .BeTrue();

            context.GetEventStore()
                   .Should()
                   .Be(store);

            context.GetEventStoreOffset()
                   .Should()
                   .Be(0);
        }
    }
}