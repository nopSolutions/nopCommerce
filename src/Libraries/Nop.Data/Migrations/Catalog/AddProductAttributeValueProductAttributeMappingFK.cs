using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097616507544540)]
    public class AddProductAttributeValueProductAttributeMappingFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductAttributeValue)
                , nameof(ProductAttributeValue.ProductAttributeMappingId)
                , NopMappingDefaults.ProductProductAttributeTable
                , nameof(ProductAttributeMapping.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}