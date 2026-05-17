using FluentMigrator;
using Nop.Data.Migrations;

namespace Nop.Plugin.Shipping.CarrierTracking.Data.Migrations;

[NopMigration("2026/05/17 12:30:00", "Nop.Plugin.Shipping.CarrierTracking add carrier columns to Shipment", MigrationProcessType.Installation)]
public class AddShipmentCarrierColumnsMigration : Migration
{
    public override void Up()
    {
        if (!Schema.Table("Shipment").Column("ExternalShipmentId").Exists())
            Alter.Table("Shipment")
                .AddColumn("ExternalShipmentId").AsString(128).Nullable();

        if (!Schema.Table("Shipment").Column("ExternalCarrierCode").Exists())
            Alter.Table("Shipment")
                .AddColumn("ExternalCarrierCode").AsString(32).Nullable();

        if (!Schema.Table("Shipment").Column("ExternalShippingStatus").Exists())
            Alter.Table("Shipment")
                .AddColumn("ExternalShippingStatus").AsString(64).Nullable();

        if (!Schema.Table("Shipment").Column("LastStatusOccurredAtUtc").Exists())
            Alter.Table("Shipment")
                .AddColumn("LastStatusOccurredAtUtc").AsDateTime().Nullable();
    }

    public override void Down()
    {
        if (Schema.Table("Shipment").Column("LastStatusOccurredAtUtc").Exists())
            Delete.Column("LastStatusOccurredAtUtc").FromTable("Shipment");

        if (Schema.Table("Shipment").Column("ExternalShippingStatus").Exists())
            Delete.Column("ExternalShippingStatus").FromTable("Shipment");

        if (Schema.Table("Shipment").Column("ExternalCarrierCode").Exists())
            Delete.Column("ExternalCarrierCode").FromTable("Shipment");

        if (Schema.Table("Shipment").Column("ExternalShipmentId").Exists())
            Delete.Column("ExternalShipmentId").FromTable("Shipment");
    }
}
