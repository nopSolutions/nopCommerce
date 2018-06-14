using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<ShippingMethodCountryMapping> builder)
        {
            builder.ToTable(NopMappingDefaults.ShippingMethodRestrictionsTable);
            builder.HasKey(mapping => new { mapping.ShippingMethodId, mapping.CountryId});

            builder.Property(mapping => mapping.ShippingMethodId).HasColumnName("ShippingMethod_Id");
            builder.Property(mapping => mapping.CountryId).HasColumnName("Country_Id");

            builder.HasOne(mapping => mapping.Country)
                .WithMany(country => country.ShippingMethodCountryMappings)
                .HasForeignKey(mapping => mapping.CountryId)
                .IsRequired();

            builder.HasOne(mapping => mapping.ShippingMethod)
                .WithMany(method => method.ShippingMethodCountryMappings)
                .HasForeignKey(mapping => mapping.ShippingMethodId)
                .IsRequired();

            builder.Ignore(mapping => mapping.Id);

            base.Configure(builder);
        }

        #endregion
    }
}