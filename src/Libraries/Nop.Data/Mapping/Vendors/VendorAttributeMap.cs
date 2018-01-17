using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class VendorAttributeMap : NopEntityTypeConfiguration<VendorAttribute>
    {
        public VendorAttributeMap()
        {
            this.ToTable("VendorAttribute");
            this.HasKey(vendorAttribute => vendorAttribute.Id);
            this.Property(vendorAttribute => vendorAttribute.Name).IsRequired().HasMaxLength(400);

            this.Ignore(vendorAttribute => vendorAttribute.AttributeControlType);
        }
    }
}