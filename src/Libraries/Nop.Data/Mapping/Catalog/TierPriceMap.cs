using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a tier price mapping configuration
    /// </summary>
    public partial class TierPriceMap : NopEntityTypeConfiguration<TierPrice>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<TierPrice> builder)
        {
            builder.HasTableName(nameof(TierPrice));

            builder.Property(price => price.Price).HasDecimal();
            builder.Property(price => price.ProductId);
            builder.Property(price => price.StoreId);
            builder.Property(price => price.CustomerRoleId);
            builder.Property(price => price.Quantity);
            builder.Property(price => price.StartDateTimeUtc);
            builder.Property(price => price.EndDateTimeUtc);
        }

        #endregion
    }
}