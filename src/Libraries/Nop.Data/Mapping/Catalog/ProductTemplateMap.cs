using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product template mapping configuration
    /// </summary>
    public partial class ProductTemplateMap : NopEntityTypeConfiguration<ProductTemplate>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductTemplate> builder)
        {
            builder.HasTableName(nameof(ProductTemplate));

            builder.Property(template => template.Name).HasLength(400).IsNullable(false);
            builder.Property(template => template.ViewPath).HasLength(400).IsNullable(false);

            builder.Property(producttemplate => producttemplate.DisplayOrder);
            builder.Property(producttemplate => producttemplate.IgnoredProductTypes);
        }

        #endregion
    }
}