using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CustomerPasswordMap : NopEntityTypeConfiguration<CustomerPassword>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CustomerPasswordMap()
        {
            this.ToTable("CustomerPassword");
            this.HasKey(password => password.Id);

            this.HasRequired(password => password.Customer)
                .WithMany()
                .HasForeignKey(password => password.CustomerId);

            this.Ignore(password => password.PasswordFormat);
        }
    }
}