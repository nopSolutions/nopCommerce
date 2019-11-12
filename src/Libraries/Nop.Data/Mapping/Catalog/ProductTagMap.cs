using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product tag mapping configuration
    /// </summary>
    public partial class ProductTagMap : NopEntityTypeConfiguration<ProductTag>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductTag> builder)
        {
            builder.HasTableName(nameof(ProductTag));

            builder.Property(productTag => productTag.Name).HasLength(400);
            builder.HasColumn(productTag => productTag.Name).IsColumnRequired();
        }

        #endregion
    }
}