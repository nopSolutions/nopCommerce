using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductReviewReviewTypeMappingMap : NopEntityTypeConfiguration<ProductReviewReviewTypeMapping>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ProductReviewReviewTypeMappingMap()
        {
            this.ToTable("ProductReview_ReviewType_Mapping");
            this.HasKey(pam => pam.Id);

            this.HasRequired(pam => pam.ProductReview)
                .WithMany(r => r.ProductReviewReviewTypeMappingEntries)
                .HasForeignKey(pam => pam.ProductReviewId)
                .WillCascadeOnDelete(true);

            this.HasRequired(pam => pam.ReviewType)
                .WithMany()
                .HasForeignKey(pam => pam.ReviewTypeId)
                .WillCascadeOnDelete(true);
        }
    }
}
