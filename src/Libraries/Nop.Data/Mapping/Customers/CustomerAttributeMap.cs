using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerAttributeMap : NopEntityTypeConfiguration<CustomerAttribute>
    {
        public override void Configure(EntityTypeBuilder<CustomerAttribute> builder)
        {
            base.Configure(builder);
            builder.ToTable("CustomerAttribute");
            builder.HasKey(ca => ca.Id);
            builder.Property(ca => ca.Name).IsRequired().HasMaxLength(400);

            builder.Ignore(ca => ca.AttributeControlType);
        }
    }
}