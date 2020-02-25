using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 12:00:50:7544540")]
    public class AddProductAttributeValueProductAttributeMappingFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductAttributeValue),
                nameof(ProductAttributeValue.ProductAttributeMappingId),
                NopMappingDefaults.ProductProductAttributeTable,
                nameof(ProductAttributeMapping.Id),
                Rule.Cascade);
        }

        #endregion
    }
}