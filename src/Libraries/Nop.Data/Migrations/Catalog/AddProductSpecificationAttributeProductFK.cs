using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 12:49:06:2261986")]
    public class AddProductSpecificationAttributeProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductSpecificationAttributeTable, 
                nameof(ProductSpecificationAttribute.ProductId), 
                nameof(Product), 
                nameof(Product.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}