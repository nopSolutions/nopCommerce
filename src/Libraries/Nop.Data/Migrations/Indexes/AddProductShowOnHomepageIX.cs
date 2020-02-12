using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037704")]
    public class AddProductShowOnHomepageIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Product_ShowOnHomepage", nameof(Product), i => i.Ascending(),
                nameof(Product.ShowOnHomepage));
        }

        #endregion
    }
}