using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037693)]
    public class AddProductAttributeValueProductAttributeMappingIdDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_ProductAttributeValue_ProductAttributeMappingId_DisplayOrder",
                nameof(ProductAttributeValue), i => i.Ascending(),
                nameof(ProductAttributeValue.ProductAttributeMappingId), nameof(ProductAttributeValue.DisplayOrder));
        }

        #endregion
    }
}