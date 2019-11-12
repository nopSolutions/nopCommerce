using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product review mapping configuration
    /// </summary>
    public partial class ProductReviewMap : NopEntityTypeConfiguration<ProductReview>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductReview> builder)
        {
            builder.HasTableName(nameof(ProductReview));

            builder.Property(productreview => productreview.CustomerId);
            builder.Property(productreview => productreview.ProductId);
            builder.Property(productreview => productreview.StoreId);
            builder.Property(productreview => productreview.IsApproved);
            builder.Property(productreview => productreview.Title);
            builder.Property(productreview => productreview.ReviewText);
            builder.Property(productreview => productreview.ReplyText);
            builder.Property(productreview => productreview.CustomerNotifiedOfReply);
            builder.Property(productreview => productreview.Rating);
            builder.Property(productreview => productreview.HelpfulYesTotal);
            builder.Property(productreview => productreview.HelpfulNoTotal);
            builder.Property(productreview => productreview.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(productReview => productReview.Product)
            //    .WithMany(product => product.ProductReviews)
            //    .HasForeignKey(productReview => productReview.ProductId)
            //    .IsColumnRequired();

            //builder.HasOne(productReview => productReview.Customer)
            //    .WithMany()
            //    .HasForeignKey(productReview => productReview.CustomerId)
            //    .IsColumnRequired();

            //builder.HasOne(productReview => productReview.Store)
            //    .WithMany()
            //    .HasForeignKey(productReview => productReview.StoreId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}