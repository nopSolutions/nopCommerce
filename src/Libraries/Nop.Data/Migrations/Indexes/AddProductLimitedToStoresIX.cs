using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647933)]
    public class AddProductLimitedToStoresIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Product_LimitedToStores", nameof(Product), i => i.Ascending(),
                nameof(Product.LimitedToStores));
        }

        #endregion
    }
}