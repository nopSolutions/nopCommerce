using LinqToDB.Mapping;
using Nop.Core.Domain.Discounts;
using Nop.Data.Data;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount-category mapping configuration
    /// </summary>
    public partial class DiscountCategoryMap : NopEntityTypeConfiguration<DiscountCategoryMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<DiscountCategoryMapping> builder)
        {
            builder.HasTableName(NopMappingDefaults.DiscountAppliedToCategoriesTable);
            builder.HasPrimaryKey(mapping => new { mapping.DiscountId, mapping.CategoryId});

            builder.Property(mapping => mapping.DiscountId).HasColumnName("Discount_Id");
            builder.Property(mapping => mapping.CategoryId).HasColumnName("Category_Id");

            builder.Ignore(mapping => mapping.Id);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(mapping => mapping.Discount)
            //    .WithMany(discount => discount.DiscountCategoryMappings)
            //    .HasForeignKey(mapping => mapping.DiscountId)
            //    .IsColumnRequired();

            //builder.HasOne(mapping => mapping.Category)
            //    .WithMany(category => category.DiscountCategoryMappings)
            //    .HasForeignKey(mapping => mapping.CategoryId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}