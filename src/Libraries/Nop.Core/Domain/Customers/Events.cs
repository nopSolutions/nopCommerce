namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Customer registered event
    /// </summary>
    public class CustomerRegisteredEvent
    {
        public CustomerRegisteredEvent(Customer customer)
        {
            this.Customer = customer;
        }

        public Customer Customer
        {
            get; private set;
        }
    }

}