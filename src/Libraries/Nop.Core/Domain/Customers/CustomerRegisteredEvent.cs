namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Customer registered event
    /// </summary>
    public class CustomerRegisteredEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customer">customer</param>
        public CustomerRegisteredEvent(Customer customer)
        {
            this.Customer = customer;
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