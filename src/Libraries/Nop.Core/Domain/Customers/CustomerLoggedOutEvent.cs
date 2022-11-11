namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// "Customer is logged out" event
    /// </summary>
    public partial class CustomerLoggedOutEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customer">Customer</param>
        public CustomerLoggedOutEvent(Customer customer)
        {
            Customer = customer;
        }

        /// <summary>
        /// Get or set the customer
        /// </summary>
        public Customer Customer { get; }
    }
}