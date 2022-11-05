using AppCore.EventModel.EntityFrameworkCore.Configuration;
using AppCore.EventModel.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.EventModel.EntityFrameworkCore.PostgreSql.Configuration;

/// <summary>
/// EventHistory entity type configuration for PostgreSql.
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
        base.Configure(builder);

        builder.ToTable(TableName);
    }
}