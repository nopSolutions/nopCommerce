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

#if EF6
            builder.HasMany(customer => customer.CustomerRoles)
                .WithMany()
                .Map(mapping => mapping.ToTable("Customer_CustomerRole_Mapping"));

            builder.HasMany(customer => customer.Addresses)
                .WithMany()
                .Map(mapping => mapping.ToTable("CustomerAddresses"));
#else
            builder.Ignore(customer => customer.CustomerRoles);
            builder.Ignore(customer => customer.Addresses);
#endif

            builder.HasOne(customer => customer.BillingAddress)
                .WithMany()
                .HasForeignKey(customer => customer.BillingAddressId);

            builder.HasOne(customer => customer.ShippingAddress)
                .WithMany()
                .HasForeignKey(customer => customer.ShippingAddressId);


            //add custom configuration
            this.PostConfigure(builder);
        }

        #endregion
    }
}