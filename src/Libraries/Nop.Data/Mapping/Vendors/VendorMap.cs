using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    public partial class VendorMap : EntityTypeConfiguration<Vendor>
    {
        public VendorMap()
        {
            this.ToTable("Vendor");
            this.HasKey(v => v.Id);

            this.Property(v => v.Name).IsRequired().HasMaxLength(400);
            this.Property(v => v.Email).HasMaxLength(400);
        }
    }
}