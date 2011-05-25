namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer attribute
    /// </summary>
    public partial class CustomerAttribute : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public virtual int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public virtual string Value { get; set; }
        
        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }
    }

}
