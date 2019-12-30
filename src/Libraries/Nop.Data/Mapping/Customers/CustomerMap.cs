using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<Customer> builder)
        {
            builder.HasTableName(nameof(Customer));

            builder.Property(customer => customer.Username).HasLength(1000);
            builder.Property(customer => customer.Email).HasLength(1000);
            builder.Property(customer => customer.EmailToRevalidate).HasLength(1000);
            builder.Property(customer => customer.SystemName).HasLength(400);

            builder.Property(customer => customer.BillingAddressId).HasColumnName("BillingAddress_Id");
            builder.Property(customer => customer.ShippingAddressId).HasColumnName("ShippingAddress_Id");

            builder.Property(customer => customer.CustomerGuid);
            builder.Property(customer => customer.AdminComment);
            builder.Property(customer => customer.IsTaxExempt);
            builder.Property(customer => customer.AffiliateId);
            builder.Property(customer => customer.VendorId);
            builder.Property(customer => customer.HasShoppingCartItems);
            builder.Property(customer => customer.RequireReLogin);
            builder.Property(customer => customer.FailedLoginAttempts);
            builder.Property(customer => customer.CannotLoginUntilDateUtc);
            builder.Property(customer => customer.Active);
            builder.Property(customer => customer.Deleted);
            builder.Property(customer => customer.IsSystemAccount);
            builder.Property(customer => customer.LastIpAddress);
            builder.Property(customer => customer.CreatedOnUtc);
            builder.Property(customer => customer.LastLoginDateUtc);
            builder.Property(customer => customer.LastActivityDateUtc);
            builder.Property(customer => customer.RegisteredInStoreId);
        }

        #endregion
    }
}