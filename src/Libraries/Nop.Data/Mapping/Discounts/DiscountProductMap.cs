using LinqToDB.Mapping;
using Nop.Core.Domain.Discounts;
using Nop.Data.Data;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount-product mapping configuration
    /// </summary>
    public partial class DiscountProductMap : NopEntityTypeConfiguration<DiscountProductMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<DiscountProductMapping> builder)
        {
            builder.HasTableName(NopMappingDefaults.DiscountAppliedToProductsTable);
            builder.HasPrimaryKey(mapping => new
            {
                mapping.DiscountId,
                mapping.ProductId
            });

            builder.Property(mapping => mapping.DiscountId).HasColumnName("Discount_Id");
            builder.Property(mapping => mapping.ProductId).HasColumnName("Product_Id");

            builder.Ignore(mapping => mapping.Id);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(mapping => mapping.Discount)
            //    .WithMany(discount => discount.DiscountProductMappings)
            //    .HasForeignKey(mapping => mapping.DiscountId)
            //    .IsColumnRequired();

            //builder.HasOne(mapping => mapping.Product)
            //    .WithMany(product => product.DiscountProductMappings)
            //    .HasForeignKey(mapping => mapping.ProductId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}