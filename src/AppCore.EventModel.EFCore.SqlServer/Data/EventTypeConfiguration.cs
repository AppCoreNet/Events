// Licensed under the MIT License.
// Copyright (c) 2020 the AppCore .NET project.

using AppCore.Events.EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Events.EntityFrameworkCore.SqlServer.Data
{
    /// <summary>
    /// Event entity type configuration for SQL Server.
    /// </summary>
    public class EventTypeConfiguration : EventTypeConfigurationBase
    {
        /// <summary>
        /// The name of the event queue table.
        /// </summary>
        public const string TableName = "EventQueue";

        /// <summary>
        /// The name of the event queue sequence.
        /// </summary>
        public const string SequenceName = "EventQueueSequence";

        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.Offset)
                   .IsClustered();

            builder.Property(e => e.Offset)
                   .UseHiLo(SequenceName);
        }
    }
}