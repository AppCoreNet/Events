using System.Collections.Generic;
using AppCore.Data.EntityFrameworkCore;
using AppCore.EventModel.Formatters;

namespace AppCore.EventModel.EntityFrameworkCore;

public class InMemoryDbContextEventQueueTests : DbContextEventQueueTests<InMemoryDbContext>
{
    public InMemoryDbContextEventQueueTests()
        : base(new InMemoryDbContext())
    {
    }

    protected override DbContextEventQueue<InMemoryDbContext> CreateEventQueue(
        IDbContextDataProvider<InMemoryDbContext> provider,
        IEnumerable<IEventContextFormatter> formatters)
    {
        return new InMemoryDbContextEventQueue(provider, formatters);
    }
}