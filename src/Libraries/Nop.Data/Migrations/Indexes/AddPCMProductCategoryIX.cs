using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:08:9037707")]
    public class AddPCMProductCategoryIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_PCM_Product_and_Category").OnTable(NameCompatibilityManager.GetTableName(typeof(ProductCategory)))
                .OnColumn(nameof(ProductCategory.CategoryId)).Ascending()
                .OnColumn(nameof(ProductCategory.ProductId)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}