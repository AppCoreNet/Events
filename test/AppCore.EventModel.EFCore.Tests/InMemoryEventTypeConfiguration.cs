using AppCore.EventModel.EntityFrameworkCore.Configuration;
using AppCore.EventModel.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.EventModel.EntityFrameworkCore
{
    public class InMemoryEventTypeConfiguration : EventTypeConfigurationBase
    {
        public override void Configure(EntityTypeBuilder<Event> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Offset)
                   .ValueGeneratedOnAdd();
        }
    }
}