using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037702")]
    public class AddProductDeletedPublishedIX : AutoReversingMigration
    {
        #region Methods         

        public override void Up()
        {
            this.AddIndex("IX_Product_Deleted_and_Published", nameof(Product), i => i.Ascending(),
                nameof(Product.Published), nameof(Product.Deleted));
        }

        #endregion
    }
}