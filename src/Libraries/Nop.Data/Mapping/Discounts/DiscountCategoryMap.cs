using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;

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
        public override void Configure(EntityTypeBuilder<DiscountCategoryMapping> builder)
        {
            builder.ToTable(NopMappingDefaults.DiscountAppliedToCategoriesTable);
            builder.HasKey(mapping => new { mapping.DiscountId, mapping.CategoryId});

            builder.Property(mapping => mapping.DiscountId).HasColumnName("Discount_Id");
            builder.Property(mapping => mapping.CategoryId).HasColumnName("Category_Id");

            builder.HasOne<Discount>().WithMany().HasForeignKey(mapping => mapping.DiscountId).IsRequired();

            builder.HasOne<Category>().WithMany().HasForeignKey(mapping => mapping.CategoryId).IsRequired();

            builder.Ignore(mapping => mapping.Id);

            base.Configure(builder);
        }

        #endregion
    }
}