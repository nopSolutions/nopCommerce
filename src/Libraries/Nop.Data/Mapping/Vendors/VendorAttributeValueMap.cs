using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class VendorAttributeValueMap : NopEntityTypeConfiguration<VendorAttributeValue>
    {
        public VendorAttributeValueMap()
        {
            this.ToTable("VendorAttributeValue");
            this.HasKey(vendorAttributeValue => vendorAttributeValue.Id);
            this.Property(vendorAttributeValue => vendorAttributeValue.Name).IsRequired().HasMaxLength(400);

            this.HasRequired(vendorAttributeValue => vendorAttributeValue.VendorAttribute)
                .WithMany(vendorAttribute => vendorAttribute.VendorAttributeValues)
                .HasForeignKey(vendorAttributeValue => vendorAttributeValue.VendorAttributeId);
        }
    }
}