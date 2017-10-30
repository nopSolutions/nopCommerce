using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CustomerAttributeMap : NopEntityTypeConfiguration<CustomerAttribute>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CustomerAttributeMap()
        {
            this.ToTable("CustomerAttribute");
            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Name).IsRequired().HasMaxLength(400);

            this.Ignore(ca => ca.AttributeControlType);
        }
    }
}