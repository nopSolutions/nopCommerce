using FluentMigrator;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Migrations.Shipping
{
    [Migration(637097822410528357)]
    public class AddShippingMethodCountryShippingMethodFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ShippingMethodRestrictionsTable)
                .ForeignColumn("ShippingMethod_Id")
                .ToTable(nameof(ShippingMethod))
                .PrimaryColumn(nameof(ShippingMethod.Id));
        }

        #endregion
    }
}