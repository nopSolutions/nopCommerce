using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097645462261985)]
    public class AddProductSpecificationAttributeSpecificationAttributeOptionFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductSpecificationAttributeTable)
                .ForeignColumn(nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId))
                .ToTable(nameof(SpecificationAttributeOption))
                .PrimaryColumn(nameof(SpecificationAttributeOption.Id));

            Create.Index().OnTable(NopMappingDefaults.ProductSpecificationAttributeTable).OnColumn(nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}