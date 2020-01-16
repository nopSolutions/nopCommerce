using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097774149883528)]
    public class AddDiscountManufacturerDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.DiscountAppliedToManufacturersTable,
                "Discount_Id",
                nameof(Discount),
                nameof(Discount.Id),
                Rule.Cascade);
        }

        #endregion
    }
}