using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductReviewMap : NopEntityTypeConfiguration<ProductReview>
    {
        public ProductReviewMap()
        {
            this.ToTable("ProductReview");
            this.HasKey(pr => pr.Id);

            this.HasRequired(pr => pr.Product)
                .WithMany(p => p.ProductReviews)
                .HasForeignKey(pr => pr.ProductId);

            this.HasRequired(pr => pr.Customer)
                .WithMany()
                .HasForeignKey(pr => pr.CustomerId);

            this.HasRequired(pr => pr.Store)
                .WithMany()
                .HasForeignKey(pr => pr.StoreId);
        }
    }
}