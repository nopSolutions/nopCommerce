using LinqToDB.Mapping;
using Nop.Core.Domain.Discounts;
using Nop.Data.Data;

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
        public override void Configure(EntityMappingBuilder<DiscountManufacturerMapping> builder)
        {
            builder.HasTableName(NopMappingDefaults.DiscountAppliedToManufacturersTable);
            builder.HasPrimaryKey(mapping => new { mapping.DiscountId, mapping.ManufacturerId });

            builder.Property(mapping => mapping.DiscountId).HasColumnName("Discount_Id");
            builder.Property(mapping => mapping.ManufacturerId).HasColumnName("Manufacturer_Id");

            builder.Ignore(mapping => mapping.Id);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(mapping => mapping.Discount)
            //    .WithMany(discount => discount.DiscountManufacturerMappings)
            //    .HasForeignKey(mapping => mapping.DiscountId)
            //    .IsColumnRequired();

            //builder.HasOne(mapping => mapping.Manufacturer)
            //    .WithMany(manufacturer => manufacturer.DiscountManufacturerMappings)
            //    .HasForeignKey(mapping => mapping.ManufacturerId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}