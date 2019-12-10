using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097774149883529)]
    public class AddDiscountManufacturerManufacturerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.DiscountAppliedToManufacturersTable)
                .ForeignColumn("Manufacturer_Id")
                .ToTable(nameof(Manufacturer))
                .PrimaryColumn(nameof(Manufacturer.Id));

            Create.Index().OnTable(NopMappingDefaults.DiscountAppliedToManufacturersTable).OnColumn("Manufacturer_Id").Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}