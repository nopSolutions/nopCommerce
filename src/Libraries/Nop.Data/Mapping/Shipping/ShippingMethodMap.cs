using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a shipping method mapping configuration
    /// </summary>
    public partial class ShippingMethodMap : NopEntityTypeConfiguration<ShippingMethod>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ShippingMethod> builder)
        {
            builder.ToTable(nameof(ShippingMethod));
            builder.HasKey(method => method.Id);

            builder.Property(method => method.Name).HasMaxLength(400).IsRequired();

#if EF6
            builder.HasMany(method => method.RestrictedCountries)
                .WithMany(country => country.RestrictedShippingMethods)
                .Map(mapping => mapping.ToTable("ShippingMethodRestrictions"));
#else
            builder.Ignore(method => method.RestrictedCountries);
#endif

            //add custom configuration
            this.PostConfigure(builder);
        }

        #endregion
    }
}