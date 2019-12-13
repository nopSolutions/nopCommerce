using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097778951975257)]
    public class AddDiscountProductProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.DiscountAppliedToProductsTable
                , "Product_Id"
                , nameof(Product)
                , nameof(Product.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}