using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097611590754490)]
    public class AddPredefinedProductAttributeValueProductAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(PredefinedProductAttributeValue)
                , nameof(PredefinedProductAttributeValue.ProductAttributeId)
                , nameof(ProductAttribute)
                , nameof(ProductAttribute.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}