using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a product attribute value picture entity builder
    /// </summary>
    public partial class ProductAttributeValuePictureBuilder : NopEntityBuilder<ProductAttributeValuePicture>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductAttributeValuePicture.ProductAttributeValueId)).AsInt32().ForeignKey<ProductAttributeValue>()
                .WithColumn(nameof(ProductAttributeValuePicture.PictureId)).AsInt32();
        }

        #endregion
    }
}