using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount-manufacturer mapping configuration
    /// </summary>
    public partial class DiscountManufacturerMap : NopEntityTypeConfiguration<DiscountManufacturerMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<DiscountManufacturerMapping> builder)
        {
            builder.ToTable(NopMappingDefaults.DiscountAppliedToManufacturersTable);
            builder.HasKey(mapping => new { mapping.DiscountId, mapping.ManufacturerId });

            builder.Property(mapping => mapping.DiscountId).HasColumnName("Discount_Id");
            builder.Property(mapping => mapping.ManufacturerId).HasColumnName("Manufacturer_Id");

            builder.Ignore(mapping => mapping.Id);

            base.Configure(builder);
        }

        #endregion
    }
}