using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097616507544540)]
    public class AddProductAttributeValueProductAttributeMappingFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ProductAttributeValue))
                .ForeignColumn(nameof(ProductAttributeValue.ProductAttributeMappingId))
                .ToTable(NopMappingDefaults.ProductProductAttributeTable)
                .PrimaryColumn(nameof(ProductAttributeMapping.Id));

            Create.Index().OnTable(nameof(ProductAttributeValue)).OnColumn(nameof(ProductAttributeValue.ProductAttributeMappingId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}