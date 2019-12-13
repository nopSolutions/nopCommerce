using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097615386806325)]
    public class AddProductAttributeMappingProductAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductProductAttributeTable
                , nameof(ProductAttributeMapping.ProductAttributeId)
                , nameof(ProductAttribute)
                , nameof(ProductAttribute.Id)
                , Rule.Cascade);
        }
        
        #endregion
    }
}