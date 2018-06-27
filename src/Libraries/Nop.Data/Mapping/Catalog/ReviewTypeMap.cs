using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represent a review type mapping class
    /// </summary>
    public partial class ReviewTypeMap : NopEntityTypeConfiguration<ReviewType>
    {
        #region Ctor

        public ReviewTypeMap()
        {
            this.ToTable("ReviewType");
            this.HasKey(reviewType => reviewType.Id);
            this.Property(reviewType => reviewType.Name).IsRequired().HasMaxLength(400);
            this.Property(reviewType => reviewType.Description).IsRequired().HasMaxLength(400);
        }

        #endregion
    }
}
