// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Events.Metadata;
using FluentAssertions;
using Xunit;

namespace AppCore.Events.Queue
{
    public class InMemoryEventQueueTests
    {
        [Fact]
        public async Task WriteAsyncSucceeds()
        {
            var @event = new TestEvent();
            var descriptor = new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>());
            var context = new EventContext<TestEvent>(descriptor, @event);

            var ct = new CancellationToken();
            var queue = new InMemoryEventQueue();
            await queue.WriteAsync(new[] {context}, ct);
        }

        [Fact]
        public async Task ReadAsyncReturnsAvailable()
        {
            var ct = new CancellationToken();
            var queue = new InMemoryEventQueue();

            var @event = new TestEvent();
            var descriptor = new EventDescriptor(typeof(TestEvent), new Dictionary<string, object>());
            var context = new EventContext<TestEvent>(descriptor, @event);

            await queue.WriteAsync(new[] {context}, ct);
            await queue.WriteAsync(new[] {context}, ct);

            IReadOnlyCollection<IEventContext> events = await queue.ReadAsync(64, ct);

            events.Should()
                  .HaveCount(2);
        }

        [Fact]
        public async Task ReadAsyncWaitsForAvailable()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            var queue = new InMemoryEventQueue();

            Func<Task> readFunc = () => queue.ReadAsync(1, cts.Token);

            await readFunc.Should()
                          .ThrowAsync<OperationCanceledException>();

            cts.IsCancellationRequested.Should()
               .BeTrue();
        }
    }
}
