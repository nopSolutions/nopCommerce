using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097618625689396)]
    public class AddProductCategoryCategoryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductCategoryTable)
                .ForeignColumn(nameof(ProductCategory.CategoryId))
                .ToTable(nameof(Category))
                .PrimaryColumn(nameof(Category.Id));
        }
        
        #endregion
    }
}