using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 11:58:58:6806325")]
    public class AddProductAttributeMappingProductAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductProductAttributeTable,
                nameof(ProductAttributeMapping.ProductAttributeId),
                nameof(ProductAttribute),
                nameof(ProductAttribute.Id),
                Rule.Cascade);
        }
        
        #endregion
    }
}