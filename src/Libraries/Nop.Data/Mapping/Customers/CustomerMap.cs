using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Represents a customer mapping configuration
    /// </summary>
    public partial class CustomerMap : NopEntityTypeConfiguration<Customer>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable(nameof(Customer));
            builder.HasKey(customer => customer.Id);

            builder.Property(customer => customer.Username).HasMaxLength(1000);
            builder.Property(customer => customer.Email).HasMaxLength(1000);
            builder.Property(customer => customer.EmailToRevalidate).HasMaxLength(1000);
            builder.Property(customer => customer.SystemName).HasMaxLength(400);

            builder.Property(customer => customer.BillingAddressId).HasColumnName("BillingAddress_Id");
            builder.Property(customer => customer.ShippingAddressId).HasColumnName("ShippingAddress_Id");

            builder.HasOne(customer => customer.BillingAddress)
                .WithMany()
                .HasForeignKey(customer => customer.BillingAddressId);

            builder.HasOne(customer => customer.ShippingAddress)
                .WithMany()
                .HasForeignKey(customer => customer.ShippingAddressId);

            builder.Ignore(customer => customer.CustomerRoles);
            builder.Ignore(customer => customer.Addresses);

            base.Configure(builder);
        }

        #endregion
    }
}