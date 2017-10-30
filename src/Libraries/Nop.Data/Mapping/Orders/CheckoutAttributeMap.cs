using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CheckoutAttributeMap : NopEntityTypeConfiguration<CheckoutAttribute>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CheckoutAttributeMap()
        {
            this.ToTable("CheckoutAttribute");
            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Name).IsRequired().HasMaxLength(400);

            this.Ignore(ca => ca.AttributeControlType);
        }
    }
}