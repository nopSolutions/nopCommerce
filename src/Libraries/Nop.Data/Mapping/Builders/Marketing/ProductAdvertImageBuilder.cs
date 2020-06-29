using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class ProductAdvertImageBuilder : NopEntityBuilder<ProductAdvertImage>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductAdvertImage.Title)).AsString(64).NotNullable()
                .WithColumn(nameof(ProductAdvertImage.Description)).AsString(255).Nullable()
                .WithColumn(nameof(ProductAdvertImage.ImageUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(ProductAdvertImage.ImageUrlOriginal)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(ProductAdvertImage.TagIdList)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(ProductAdvertImage.ProductId)).AsInt32().ForeignKey<Product>()
                ;
        }

        #endregion
    }
}
