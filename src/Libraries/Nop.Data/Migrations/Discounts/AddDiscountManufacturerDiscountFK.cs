using FluentMigrator;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097774149883528)]
    public class AddDiscountManufacturerDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.DiscountAppliedToManufacturersTable)
                .ForeignColumn("Discount_Id")
                .ToTable(nameof(Discount))
                .PrimaryColumn(nameof(Discount.Id));
        }

        #endregion
    }
}