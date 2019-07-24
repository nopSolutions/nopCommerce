using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a product availability range mapping configuration
    /// </summary>
    public partial class ProductAvailabilityRangeMap : NopEntityTypeConfiguration<ProductAvailabilityRange>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ProductAvailabilityRange> builder)
        {
            builder.ToTable(nameof(ProductAvailabilityRange));
            builder.HasKey(range => range.Id);

            builder.Property(range => range.Name).HasMaxLength(400).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}