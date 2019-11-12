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

            builder.Property(value => value.Name).HasLength(400);
            builder.HasColumn(value => value.Name).IsColumnRequired();

            builder.Property(value => value.IsPreSelected);
            builder.Property(value => value.DisplayOrder);
            builder.Property(value => value.VendorAttributeId);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(value => value.VendorAttribute)
            //    .WithMany(attribute => attribute.VendorAttributeValues)
            //    .HasForeignKey(value => value.VendorAttributeId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}