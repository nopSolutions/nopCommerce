using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a cross-sell product mapping configuration
    /// </summary>
    public partial class CrossSellProductMap : NopEntityTypeConfiguration<CrossSellProduct>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<CrossSellProduct> builder)
        {
            builder.HasTableName(nameof(CrossSellProduct));

            builder.Property(crosssellproduct => crosssellproduct.ProductId1);
            builder.Property(crosssellproduct => crosssellproduct.ProductId2);
        }

        #endregion
    }
}