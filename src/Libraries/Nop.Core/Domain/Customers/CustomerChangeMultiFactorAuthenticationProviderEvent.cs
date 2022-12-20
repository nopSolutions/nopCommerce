namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// "Customer is change multi-factor authentication provider" event
    /// </summary>
    public partial class CustomerChangeMultiFactorAuthenticationProviderEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customer">Customer</param>
        public CustomerChangeMultiFactorAuthenticationProviderEvent(Customer customer)
        {
            Customer = customer;
        }

        // <summary>
        /// Get or set the customer
        /// </summary>
        public Customer Customer { get; }
    }
}
