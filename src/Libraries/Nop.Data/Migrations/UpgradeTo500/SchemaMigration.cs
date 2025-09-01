using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-02-02 00:00:04", "SchemaMigration for 5.00")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#7400
        var manufacturerTableName = nameof(Manufacturer);

        var physicalAddressColumnName = nameof(Manufacturer.PhysicalAddress);
        if (!Schema.Table(manufacturerTableName).Column(physicalAddressColumnName).Exists())
        {
            Alter.Table(manufacturerTableName)
                .AddColumn(physicalAddressColumnName)
                .AsString()
                .SetExistingRowsTo(null);
        }

        var electronicAddressColumnName = nameof(Manufacturer.ElectronicAddress);
        if (!Schema.Table(manufacturerTableName).Column(electronicAddressColumnName).Exists())
        {
            Alter.Table(manufacturerTableName)
                .AddColumn(electronicAddressColumnName)
                .AsString()
                .SetExistingRowsTo(null);
        }

        var responsiblePersonColumnName = nameof(Manufacturer.ResponsiblePerson);
        if (!Schema.Table(manufacturerTableName).Column(responsiblePersonColumnName).Exists())
        {
            Alter.Table(manufacturerTableName)
                .AddColumn(responsiblePersonColumnName)
                .AsString()
                .SetExistingRowsTo(null);
        }

        var responsiblePersonPhysicalAddressColumnName = nameof(Manufacturer.ResponsiblePersonPhysicalAddress);
        if (!Schema.Table(manufacturerTableName).Column(responsiblePersonPhysicalAddressColumnName).Exists())
        {
            Alter.Table(manufacturerTableName)
                .AddColumn(responsiblePersonPhysicalAddressColumnName)
                .AsString()
                .SetExistingRowsTo(null);
        }

        var responsiblePersonElectronicAddressColumnName = nameof(Manufacturer.ResponsiblePersonElectronicAddress);
        if (!Schema.Table(manufacturerTableName).Column(responsiblePersonElectronicAddressColumnName).Exists())
        {
            Alter.Table(manufacturerTableName)
                .AddColumn(responsiblePersonElectronicAddressColumnName)
                .AsString()
                .SetExistingRowsTo(null);
        }

        //#7743
        this.AddOrAlterColumnFor<Customer>(t => t.LastShoppingCartUpdateDateUtc)
            .AsDateTime2()
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.LastAbandonedCartFollowUpNumber)
            .AsInt32()
            .Nullable();
        this.AddOrAlterColumnFor<Customer>(t => t.LastAbandonedCartFollowUpDateUtc)
            .AsDateTime2()
            .Nullable();

        this.AddOrAlterColumnFor<Customer>(t => t.LastRegistrationFollowUpNumber)
            .AsInt32()
            .Nullable();
        this.AddOrAlterColumnFor<Customer>(t => t.LastRegistrationFollowUpDateUtc)
            .AsDateTime2()
            .Nullable();

        this.AddOrAlterColumnFor<Order>(t => t.LastPendingOrderFollowUpNumber)
            .AsInt32()
            .Nullable();
        this.AddOrAlterColumnFor<Order>(t => t.LastPendingOrderFollowUpDateUtc)
            .AsDateTime2()
            .Nullable();
    }
}
