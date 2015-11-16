namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Customer logged-in event
    /// </summary>
    public class CustomerLoggedinEvent
    {
        public CustomerLoggedinEvent(Customer customer)
        {
            this.Customer = customer;
        }

        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer
        {
            get; private set;
        }
    }

    /// <summary>
    /// Customer registered event
    /// </summary>
    public class CustomerRegisteredEvent
    {
        public CustomerRegisteredEvent(Customer customer)
        {
            this.Customer = customer;
        }

        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer
        {
            get; private set;
        }
    }

}