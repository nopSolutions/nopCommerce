using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097611590754490)]
    public class AddPredefinedProductAttributeValueProductAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(PredefinedProductAttributeValue))
                .ForeignColumn(nameof(PredefinedProductAttributeValue.ProductAttributeId))
                .ToTable(nameof(ProductAttribute))
                .PrimaryColumn(nameof(ProductAttribute.Id));

            Create.Index().OnTable(nameof(PredefinedProductAttributeValue)).OnColumn(nameof(PredefinedProductAttributeValue.ProductAttributeId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}