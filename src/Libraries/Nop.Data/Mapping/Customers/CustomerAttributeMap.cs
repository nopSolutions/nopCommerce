using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerAttributeMap : EntityTypeConfiguration<CustomerAttribute>
    {
        public CustomerAttributeMap()
        {
            this.ToTable("CustomerAttribute");

            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Key).IsRequired().HasMaxLength(200);
            this.Property(ca => ca.Value).IsRequired().HasMaxLength(4000);

            this.HasRequired(ca => ca.Customer)
                .WithMany(c => c.CustomerAttributes)
                .HasForeignKey(ca => ca.CustomerId);

        }
    }
}