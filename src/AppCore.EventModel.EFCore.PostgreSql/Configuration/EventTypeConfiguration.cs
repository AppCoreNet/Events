// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.EventModel.EntityFrameworkCore.Configuration;
using AppCore.EventModel.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.EventModel.EntityFrameworkCore.PostgreSql.Configuration
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

        /// <summary>
        /// The name of the event queue sequence.
        /// </summary>
        public const string SequenceName = "EventQueueSequence";

        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.Offset);

            builder.Property(e => e.Offset)
                   .UseHiLo(SequenceName);
        }
    }
}