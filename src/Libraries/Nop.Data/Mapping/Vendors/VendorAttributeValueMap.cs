using LinqToDB.Mapping;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    /// <summary>
    /// Represents a vendor attribute value mapping configuration
    /// </summary>
    public partial class VendorAttributeValueMap : NopEntityTypeConfiguration<VendorAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<VendorAttributeValue> builder)
        {
            builder.HasTableName(nameof(VendorAttributeValue));

            builder.Property(value => value.Name).HasLength(400).IsNullable(false);
            builder.Property(value => value.IsPreSelected);
            builder.Property(value => value.DisplayOrder);
            builder.Property(value => value.VendorAttributeId);
        }

        #endregion
    }
}