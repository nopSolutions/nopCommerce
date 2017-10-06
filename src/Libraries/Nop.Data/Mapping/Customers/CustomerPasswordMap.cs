using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerPasswordMap : NopEntityTypeConfiguration<CustomerPassword>
    {
        public override void Configure(EntityTypeBuilder<CustomerPassword> builder)
        {
            base.Configure(builder);
            builder.ToTable("CustomerPassword");
            builder.HasKey(password => password.Id);

            builder.HasOne(password => password.Customer)
                .WithMany()
                .HasForeignKey(password => password.CustomerId)
                .IsRequired(true);

            builder.Ignore(password => password.PasswordFormat);
        }
    }
}