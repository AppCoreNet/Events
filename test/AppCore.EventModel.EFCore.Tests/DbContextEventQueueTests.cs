using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Data;
using AppCore.Data.EntityFrameworkCore;
using AppCore.EventModel.EntityFrameworkCore.Model;
using AppCore.EventModel.Formatters;
using AppCore.EventModel.Metadata;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AppCore.EventModel.EntityFrameworkCore
{
    public abstract class DbContextEventQueueTests<TDbContext> : IAsyncLifetime
        where TDbContext : DbContext
    {
        protected DbContextDataProvider<DefaultDataProvider, TDbContext> Provider { get; }

        protected DbContextEventQueueTests(TDbContext context)
        {
            Provider = new DbContextDataProvider<DefaultDataProvider, TDbContext>(
                context,
                Substitute.For<ILogger<DbContextDataProvider>>());
        }

        protected abstract DbContextEventQueue<TDbContext> CreateEventQueue(
            IDbContextDataProvider<TDbContext> provider,
            IEnumerable<IEventContextFormatter> formatters);

        protected static IEventContext<TestEvent> CreateEventContext(TestEvent @event, EventDescriptor descriptor = null)
        {
            var features = new Dictionary<Type, object>();

            var eventContext = Substitute.For<IEventContext<TestEvent>>();
            ((IEventContext)eventContext).Event.Returns(@event);
            eventContext.Items.Returns(new Dictionary<object, object>());
            eventContext.Features.Returns(features);
            eventContext.Event.Returns(@event);
            eventContext.EventDescriptor.Returns(
                descriptor ?? new EventDescriptor(
                    typeof(TestEvent),
                    new Dictionary<string, object>
                    {
                        {EventMetadataKeys.TopicMetadataKey, "test-topic"}
                    }));

            return eventContext;
        }

        private static IEventContextFormatter CreateEventContextFormatter(string contentType)
        {
            var formatter = Substitute.For<IEventContextFormatter>();
            formatter.ContentType.Returns(contentType);
            formatter.When(f => f.Write(Arg.Any<Stream>(), Arg.Any<IEventContext>()))
                     .Do(
                         ci =>
                         {
                             var stream = ci.ArgAt<Stream>(0);
                             var eventContext = ci.ArgAt<IEventContext>(1);
                             var @event = (TestEvent) eventContext.Event;
                             byte[] buffer = Encoding.UTF8.GetBytes(@event.Value);
                             stream.Write(buffer, 0, buffer.Length);
                         });

            formatter.Read(Arg.Any<Stream>())
                     .Returns(
                         ci =>
                         {
                             var stream = ci.ArgAt<Stream>(0);
                             return CreateEventContext(new TestEvent());
                         });

            return formatter;
        }

        [Fact]
        public async Task WriteAsyncWritesAllEvents()
        {
            string contentType = "text/plain";
            IEventContextFormatter formatter = CreateEventContextFormatter(contentType);

            var event1 = new TestEvent {Value = "1"};
            var event2 = new TestEvent {Value = "2"};

            DbContextEventQueue<TDbContext> queue = CreateEventQueue(Provider, new[] {formatter});
            await queue.WriteAsync(
                new[]
                {
                    CreateEventContext(event1),
                    CreateEventContext(event2)
                },
                CancellationToken.None);

            queue.Provider.GetContext()
                 .Set<Event>()
                 .Should()
                 .BeEquivalentTo(
                     new Event
                     {
                         ContentType = contentType,
                         Data = new[] {(byte) '1'},
                         Offset = 1,
                         Topic = "test-topic"
                     },
                     new Event
                     {
                         ContentType = contentType,
                         Offset = 2,
                         Data = new[] {(byte) '2'},
                         Topic = "test-topic"
                     }
                 );
        }

        [Fact]
        public async Task ReadAsyncReadsOnlyOneTopic()
        {
            string contentType = "text/plain";
            IEventContextFormatter formatter = CreateEventContextFormatter(contentType);
            DbContextEventQueue<TDbContext> queue = CreateEventQueue(Provider, new[] { formatter });
            TDbContext dbContext = queue.Provider.GetContext();
            dbContext
                 .Set<Event>()
                 .AddRange(
                     new Event
                     {
                         ContentType = contentType,
                         Data = new[] {(byte) '1'},
                         Topic = "topic1"
                     },
                     new Event
                     {
                         ContentType = contentType,
                         Data = new[] {(byte) '2'},
                         Topic = "topic2"
                     });

            await dbContext.SaveChangesAsync();

            IReadOnlyCollection<IEventContext> events = await queue.ReadAsync(10, CancellationToken.None);
            events.Should()
                  .HaveCount(1);

            await queue.CommitReadAsync(events.Last(), CancellationToken.None);
        }

        [Fact]
        public async Task CommitReadAsyncMovesEventsFromQueueToHistory()
        {
            string contentType = "text/plain";
            IEventContextFormatter formatter = CreateEventContextFormatter(contentType);
            DbContextEventQueue<TDbContext> queue = CreateEventQueue(Provider, new[] { formatter });
            TDbContext dbContext = queue.Provider.GetContext();
            dbContext
                .Set<Event>()
                .AddRange(
                    new Event
                    {
                        ContentType = contentType,
                        Data = new[] { (byte)'1' },
                        Topic = "test-topic"
                    },
                    new Event
                    {
                        ContentType = contentType,
                        Data = new[] { (byte)'2' },
                        Topic = "test-topic"
                    });

            await dbContext.SaveChangesAsync();

            IReadOnlyCollection<IEventContext> events = await queue.ReadAsync(10, CancellationToken.None);
            events.Should()
                  .HaveCount(2);

            await queue.CommitReadAsync(events.Last(), CancellationToken.None);

            int eventCount = await dbContext
                              .Set<Event>()
                              .CountAsync();

            eventCount.Should()
                      .Be(0);

            queue.Provider.GetContext()
                 .Set<EventHistory>()
                 .Should()
                 .BeEquivalentTo(
                     new Event
                     {
                         ContentType = contentType,
                         Data = new[] { (byte)'1' },
                         Offset = 1,
                         Topic = "test-topic"
                     },
                     new Event
                     {
                         ContentType = contentType,
                         Offset = 2,
                         Data = new[] { (byte)'2' },
                         Topic = "test-topic"
                     }
                 );

        }

        public virtual async Task InitializeAsync()
        {
            await Provider.GetContext().Database.EnsureCreatedAsync();
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
