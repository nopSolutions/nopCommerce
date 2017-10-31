using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductReviewHelpfulnessMap : NopEntityTypeConfiguration<ProductReviewHelpfulness>
    {
        public override void Configure(EntityTypeBuilder<ProductReviewHelpfulness> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductReviewHelpfulness");
            builder.HasKey(pr => pr.Id);

            builder.HasOne(prh => prh.ProductReview)
                .WithMany(pr => pr.ProductReviewHelpfulnessEntries)
                .HasForeignKey(prh => prh.ProductReviewId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
        }
    }
}