
using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;


namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductRatingMap : EntityTypeConfiguration<ProductRating>
    {
        public ProductRatingMap()
        {
            this.ToTable("ProductRating");
            this.HasKey(pr => pr.Id);


            this.HasRequired(pr => pr.Product)
                .WithMany(p => p.ProductRatings)
                .HasForeignKey(pr => pr.ProductId);
        }
    }
}