using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a category mapping configuration
    /// </summary>
    public partial class CategoryMap : NopEntityTypeConfiguration<Category>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable(nameof(Category));
            builder.HasKey(category => category.Id);

            builder.Property(category => category.Name).HasMaxLength(400).IsRequired();
            builder.Property(category => category.MetaKeywords).HasMaxLength(400);
            builder.Property(category => category.MetaTitle).HasMaxLength(400);
            builder.Property(category => category.PriceRanges).HasMaxLength(400);
            builder.Property(category => category.PageSizeOptions).HasMaxLength(200);
            
            builder.Ignore(category => category.AppliedDiscounts);

            base.Configure(builder);
        }

        #endregion
    }
}