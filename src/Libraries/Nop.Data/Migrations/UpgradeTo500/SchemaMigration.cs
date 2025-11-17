using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2025-11-17 15:07:00", "SchemaMigration for 5.00")]
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
    }
}
