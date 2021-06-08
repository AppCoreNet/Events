// Licensed under the MIT License.
// Copyright (c) 2018-2021 the AppCore .NET project.

using AppCore.EventModel.EntityFrameworkCore.Configuration;
using AppCore.EventModel.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.EventModel.EntityFrameworkCore.SqlServer.Configuration
{
    /// <summary>
    /// EventHistory entity type configuration for SQL Server.
    /// </summary>
    public class EventHistoryTypeConfiguration : EventHistoryTypeConfigurationBase
    {
        /// <summary>
        /// The name of the event history table.
        /// </summary>
        public const string TableName = "EventHistory";

        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<EventHistory> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.Offset)
                   .IsClustered();
        }
    }
}