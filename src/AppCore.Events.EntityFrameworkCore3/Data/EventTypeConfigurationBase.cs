// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Events.EntityFrameworkCore.Data
{
    /// <summary>
    /// Provides a base class for configuring the <see cref="Event"/> entity.
    /// </summary>
    public abstract class EventTypeConfigurationBase : IEntityTypeConfiguration<Event>
    {
        /// <inheritdoc />
        public virtual void Configure(EntityTypeBuilder<Event> builder)
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