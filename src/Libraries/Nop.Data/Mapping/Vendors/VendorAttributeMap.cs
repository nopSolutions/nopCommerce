using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class VendorAttributeMap : NopEntityTypeConfiguration<VendorAttribute>
    {
        public override void Configure(EntityTypeBuilder<VendorAttribute> builder)
        {
            base.Configure(builder);
            builder.ToTable("VendorAttribute");
            builder.HasKey(vendorAttribute => vendorAttribute.Id);
            builder.Property(vendorAttribute => vendorAttribute.Name).IsRequired().HasMaxLength(400);
            builder.Ignore(vendorAttribute => vendorAttribute.AttributeControlType);
        }
    }
}