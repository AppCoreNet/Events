using System.Collections.Generic;
using AppCore.Data.EntityFrameworkCore;
using AppCore.EventModel.Formatters;

namespace AppCore.EventModel.EntityFrameworkCore
{
    public class InMemoryDbContextEventQueue : DbContextEventQueue<InMemoryDbContext>
    {
        public InMemoryDbContextEventQueue(
            IDbContextDataProvider<InMemoryDbContext> dataProvider,
            IEnumerable<IEventContextFormatter> formatters
        )
            : base(dataProvider, formatters)
        {
        }
    }
}