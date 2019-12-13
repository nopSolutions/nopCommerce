using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097618625689396)]
    public class AddProductCategoryCategoryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductCategoryTable
                , nameof(ProductCategory.CategoryId)
                , nameof(Category)
                , nameof(Category.Id)
                , Rule.Cascade);
        }
        
        #endregion
    }
}