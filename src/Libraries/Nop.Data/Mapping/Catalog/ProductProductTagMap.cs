using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product-product tag mapping configuration
    /// </summary>
    public partial class ProductProductTagMap : NopEntityTypeConfiguration<ProductProductTagMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductProductTagMapping> builder)
        {
            builder.HasTableName(NopMappingDefaults.ProductProductTagTable);
            builder.HasPrimaryKey(mapping => new
            {
                mapping.ProductId,
                mapping.ProductTagId
            });

            builder.Property(mapping => mapping.ProductId).HasColumnName("Product_Id");
            builder.Property(mapping => mapping.ProductTagId).HasColumnName("ProductTag_Id");

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}