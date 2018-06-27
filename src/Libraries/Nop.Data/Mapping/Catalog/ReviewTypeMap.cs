using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;


namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represent a review type mapping class
    /// </summary>
    public partial class ReviewTypeMap : NopEntityTypeConfiguration<ReviewType>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ReviewType> builder)
        {
            builder.ToTable(nameof(ReviewType));
            builder.HasKey(reviewType => reviewType.Id);

            builder.Property(reviewType => reviewType.Name).IsRequired().HasMaxLength(400);
            builder.Property(reviewType => reviewType.Description).IsRequired().HasMaxLength(400);            

            base.Configure(builder);
        }

        #endregion
    }
}
