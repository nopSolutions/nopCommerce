namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Customer registered event
    /// </summary>
    public partial class CustomerRegisteredEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customer">customer</param>
        public CustomerRegisteredEvent(Customer customer)
        {
            Customer = customer;
        }

        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer
        {
            get;
        }
    }
}