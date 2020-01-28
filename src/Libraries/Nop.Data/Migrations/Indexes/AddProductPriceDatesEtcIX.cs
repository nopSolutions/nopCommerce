using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037678")]
    public class AddProductPriceDatesEtcIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Product_PriceDatesEtc", nameof(Product), i => i.Ascending(),
                nameof(Product.Price), nameof(Product.AvailableStartDateTimeUtc),
                nameof(Product.AvailableEndDateTimeUtc), nameof(Product.Published),
                nameof(Product.Deleted));
        }

        #endregion
    }
}