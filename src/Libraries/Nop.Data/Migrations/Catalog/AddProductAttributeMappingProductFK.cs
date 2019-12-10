using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097615386806324)]
    public class AddProductAttributeMappingProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductProductAttributeTable)
                .ForeignColumn(nameof(ProductAttributeMapping.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id))
                .OnDelete(Rule.Cascade);

            Create.Index().OnTable(NopMappingDefaults.ProductProductAttributeTable).OnColumn(nameof(ProductAttributeMapping.ProductId)).Ascending().WithOptions().NonClustered();
        }
        
        #endregion
    }
}