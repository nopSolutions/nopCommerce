using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 11:35:09:1647940")]
    public class AddProductDeleteIdIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Product_Delete_Id", nameof(Product), i => i.Ascending(), nameof(Product.Deleted),
                nameof(Product.Id));
        }

        #endregion
    }
}