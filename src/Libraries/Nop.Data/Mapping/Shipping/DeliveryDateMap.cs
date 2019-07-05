using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a delivery date mapping configuration
    /// </summary>
    public partial class DeliveryDateMap : NopEntityTypeConfiguration<DeliveryDate>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<DeliveryDate> builder)
        {
            builder.ToTable(nameof(DeliveryDate));
            builder.HasKey(date => date.Id);

            builder.Property(date => date.Name).HasMaxLength(400).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}