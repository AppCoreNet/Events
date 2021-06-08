using AppCore.EventModel.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.EventModel.EntityFrameworkCore.Configuration
{
    /// <summary>
    /// Provides a base class for configuring the <see cref="EventHistory"/> entity.
    /// </summary>
    public abstract class EventHistoryTypeConfigurationBase : IEntityTypeConfiguration<EventHistory>
    {
        public virtual void Configure(EntityTypeBuilder<EventHistory> builder)
        {
            builder.HasKey(e => e.Offset);

            builder.HasIndex(e => e.Topic);

            builder.Property(e => e.Topic)
                   .HasMaxLength(64);

            builder.Property(e => e.ContentType)
                   .HasMaxLength(32)
                   .IsRequired();

            builder.Property(e => e.Data)
                   .IsRequired();
        }
    }
}