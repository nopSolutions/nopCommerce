using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a related product mapping configuration
    /// </summary>
    public partial class RelatedProductMap : NopEntityTypeConfiguration<RelatedProduct>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<RelatedProduct> builder)
        {
            builder.HasTableName(nameof(RelatedProduct));

            builder.Property(product => product.ProductId1);
            builder.Property(product => product.ProductId2);
            builder.Property(product => product.DisplayOrder);
        }

        #endregion
    }
}