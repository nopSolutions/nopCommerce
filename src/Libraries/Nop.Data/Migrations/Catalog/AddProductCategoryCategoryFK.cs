using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 12:04:22:5689396")]
    public class AddProductCategoryCategoryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductCategoryTable,
                nameof(ProductCategory.CategoryId),
                nameof(Category),
                nameof(Category.Id),
                Rule.Cascade);
        }
        
        #endregion
    }
}