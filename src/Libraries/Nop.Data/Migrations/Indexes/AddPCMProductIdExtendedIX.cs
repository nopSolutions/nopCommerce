using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123537559280389)]
    public class AddPCMProductIdExtendedIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_PCM_ProductId_Extended", NopMappingDefaults.ProductCategoryTable, i => i.Ascending(),
                    nameof(ProductCategory.ProductId), nameof(ProductCategory.IsFeaturedProduct))
                .Include(nameof(ProductCategory.CategoryId));
        }

        #endregion
    }
}