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
                .ToTable(nameof(ProductAttributeMapping))
                .PrimaryColumn(nameof(ProductAttributeMapping.Id));
        }

        #endregion
    }
}