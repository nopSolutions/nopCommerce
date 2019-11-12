using LinqToDB.Mapping;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    /// <summary>
    /// Represents an vendor attribute mapping configuration
    /// </summary>
    public partial class VendorAttributeMap : NopEntityTypeConfiguration<VendorAttribute>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<VendorAttribute> builder)
        {
            builder.HasTableName(nameof(VendorAttribute));

            builder.HasColumn(attribute => attribute.Name).IsColumnRequired();
            builder.Property(attribute => attribute.Name).HasLength(400);

            builder.Property(attribute => attribute.IsRequired);
            builder.Property(attribute => attribute.DisplayOrder);
            builder.Property(attribute => attribute.AttributeControlTypeId);

            builder.Ignore(attribute => attribute.AttributeControlType);
        }

        #endregion
    }
}