using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 11:35:09:1647940")]
    public class AddProductDeleteIdIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.Index("IX_Product_Delete_Id").OnTable(nameof(Product))
                .OnColumn(nameof(Product.Deleted)).Ascending()
                .OnColumn(nameof(Product.Id)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}