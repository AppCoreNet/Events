// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using AppCore.EventModel.EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.EventModel.EntityFrameworkCore.MySql.Data
{
    /// <summary>
    /// Event entity type configuration for MySql.
    /// </summary>
    public class EventTypeConfiguration : EventTypeConfigurationBase
    {
        /// <summary>
        /// The name of the event queue table.
        /// </summary>
        public const string TableName = "EventQueue";

        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.Offset);

            builder.Property(e => e.Offset)
                   .UseMySqlIdentityColumn();
        }
    }
}