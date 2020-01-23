using LinqToDB.Mapping;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a shipping method-country mapping configuration
    /// </summary>
    public partial class ShippingMethodCountryMap : NopEntityTypeConfiguration<ShippingMethodCountryMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ShippingMethodCountryMapping> builder)
        {
            builder.HasTableName(nameof(ShippingMethodCountryMapping));
            builder.HasPrimaryKey(mapping => new
            {
                mapping.ShippingMethodId,
                mapping.CountryId
            });

            builder.Property(mapping => mapping.ShippingMethodId);
            builder.Property(mapping => mapping.CountryId);

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}