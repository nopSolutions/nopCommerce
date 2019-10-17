using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;

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
        public override void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.ToTable(nameof(ProductReview));
            builder.HasKey(productReview => productReview.Id);

            builder.HasOne<Product>().WithMany().HasForeignKey(productReview => productReview.ProductId).IsRequired();

            builder.HasOne<Customer>().WithMany().HasForeignKey(productReview => productReview.CustomerId).IsRequired();

            builder.HasOne<Store>().WithMany().HasForeignKey(productReview => productReview.StoreId).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}