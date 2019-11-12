using LinqToDB.Mapping;
using Nop.Core.Domain.Shipping;
using Nop.Data.Data;

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
            builder.HasTableName(NopMappingDefaults.ShippingMethodRestrictionsTable);
            builder.HasPrimaryKey(mapping => new
            {
                mapping.ShippingMethodId,
                mapping.CountryId
            });

            builder.Property(mapping => mapping.ShippingMethodId).HasColumnName("ShippingMethod_Id");
            builder.Property(mapping => mapping.CountryId).HasColumnName("Country_Id");

            builder.Ignore(mapping => mapping.Id);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(mapping => mapping.Country)
            //    .WithMany(country => country.ShippingMethodCountryMappings)
            //    .HasForeignKey(mapping => mapping.CountryId)
            //    .IsColumnRequired();

            //builder.HasOne(mapping => mapping.ShippingMethod)
            //    .WithMany(method => method.ShippingMethodCountryMappings)
            //    .HasForeignKey(mapping => mapping.ShippingMethodId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}