using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductPictureBuilder : BaseEntityBuilder<ProductPicture>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductPicture.PictureId))
                    .AsInt32()
                    .ForeignKey<Picture>()
                .WithColumn(nameof(ProductPicture.ProductId))
                    .AsInt32()
                    .ForeignKey<Product>();
        }

        #endregion
    }
}