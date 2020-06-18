// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Events.EntityFrameworkCore.SqlServer.Data
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public const string TableName = "EventQueue";

        public const string SequenceName = "EventQueueSequence";

        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.Offset)
                   .IsClustered();

            builder.HasIndex(e => new { e.Topic, e.Offset });

            builder.Property(e => e.Offset)
                   .UseHiLo(SequenceName);

            builder.Property(e => e.Topic)
                   .HasMaxLength(32);

            builder.Property(e => e.ContentType)
                   .HasMaxLength(32)
                   .IsRequired();

            builder.Property(e => e.Data)
                   .IsRequired();
        }
    }
}