using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<ReviewType> builder)
        {
            builder.HasTableName(nameof(ReviewType));

            builder.Property(reviewType => reviewType.Name).HasLength(400);
            builder.Property(reviewType => reviewType.Description).HasLength(400);
            builder.HasColumn(reviewType => reviewType.Name).IsColumnRequired();
            builder.HasColumn(reviewType => reviewType.Description).IsColumnRequired();
            builder.Property(reviewtype => reviewtype.DisplayOrder);
            builder.Property(reviewtype => reviewtype.VisibleToAllCustomers);
            builder.Property(reviewtype => reviewtype.IsRequired);
        }

        #endregion
    }
}
