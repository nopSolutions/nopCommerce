using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.CustomProductReviews.Domains;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.CustomProductReviews.Mapping.Builders
{
    public class CustomProductReviewsBuilder : NopEntityBuilder<CustomProductReviewMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            //TODO:mappingler sıçtı çöz
            table.WithColumn(nameof(CustomProductReviewMapping.DisplayOrder)).AsInt32().Nullable()
                .WithColumn(nameof(CustomProductReviewMapping.ProductReviewId)).AsInt32().PrimaryKey().ForeignKey<ProductReview>()
                .WithColumn(nameof(CustomProductReviewMapping.PictureId)).AsInt32().ForeignKey<Picture>().Nullable()
                .WithColumn(nameof(CustomProductReviewMapping.VideoId)).AsInt32().ForeignKey<Video>().Nullable();
        }

        #endregion
    }
}