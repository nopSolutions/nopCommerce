using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Common
{
    public partial class AddressAttributeMap : NopEntityTypeConfiguration<AddressAttribute>
    {
        public override void Configure(EntityTypeBuilder<AddressAttribute> builder)
        {
            base.Configure(builder);
            builder.ToTable("AddressAttribute");
            builder.HasKey(aa => aa.Id);
            builder.Property(aa => aa.Name).IsRequired().HasMaxLength(400);

            builder.Ignore(aa => aa.AttributeControlType);
        }
        public AddressAttributeMap()
        {
            
        }
    }
}