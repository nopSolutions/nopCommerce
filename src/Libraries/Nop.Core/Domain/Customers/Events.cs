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
    /// "Customer is logged out" event
    /// </summary>
    public class CustomerLoggedOutEvent
    {
        public CustomerLoggedOutEvent(Customer customer)
        {
            this.Customer = customer;
        }

        /// <summary>
        /// Get or set the customer
        /// </summary>
        public Customer Customer { get; private set; }
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

    /// <summary>
    /// Customer password changed event
    /// </summary>
    public class CustomerPasswordChangedEvent
    {
        public CustomerPasswordChangedEvent(CustomerPassword password)
        {
            this.Password = password;
        }

        /// <summary>
        /// Customer password
        /// </summary>
        public CustomerPassword Password { get; private set; }
    }

}