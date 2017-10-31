using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductReviewMap : NopEntityTypeConfiguration<ProductReview>
    {
        public override void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductReview");
            builder.HasKey(pr => pr.Id);

            builder.HasOne(pr => pr.Product)
                .WithMany(p => p.ProductReviews)
                .HasForeignKey(pr => pr.ProductId)
                .IsRequired(true);

            builder.HasOne(pr => pr.Customer)
                .WithMany()
                .HasForeignKey(pr => pr.CustomerId)
                .IsRequired(true);

            builder.HasOne(pr => pr.Store)
                .WithMany()
                .HasForeignKey(pr => pr.StoreId)
                .IsRequired(true);
        }
    }
}