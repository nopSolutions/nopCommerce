namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer logged-in event
/// </summary>
public partial class CustomerLoggedinEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    public CustomerLoggedinEvent(Customer customer)
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