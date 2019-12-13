using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097645462261986)]
    public class AddProductSpecificationAttributeProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductSpecificationAttributeTable
                , nameof(ProductSpecificationAttribute.ProductId)
                , nameof(Product)
                , nameof(Product.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}