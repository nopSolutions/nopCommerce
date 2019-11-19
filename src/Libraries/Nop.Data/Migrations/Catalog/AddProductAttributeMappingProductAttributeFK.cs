using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097615386806325)]
    public class AddProductAttributeMappingProductAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ProductAttributeMapping))
                .ForeignColumn(nameof(ProductAttributeMapping.ProductAttributeId))
                .ToTable(nameof(ProductAttribute))
                .PrimaryColumn(nameof(ProductAttribute.Id));
        }
        
        #endregion
    }
}