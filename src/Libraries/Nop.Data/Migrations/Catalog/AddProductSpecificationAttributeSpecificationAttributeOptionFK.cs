using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097645462261985)]
    public class AddProductSpecificationAttributeSpecificationAttributeOptionFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductSpecificationAttributeTable
                , nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId)
                , nameof(SpecificationAttributeOption)
                , nameof(SpecificationAttributeOption.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}