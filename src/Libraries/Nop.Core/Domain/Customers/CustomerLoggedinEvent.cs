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
    /// <param name="guestCustomer">Guest customer</param>
    public CustomerLoggedinEvent(Customer customer, Customer guestCustomer = null)
    {
        Customer = customer;
        GuestCustomer = guestCustomer;
    }

    /// <summary>
    /// Customer
    /// </summary>
    public Customer Customer
    {
        get;
    }

    /// <summary>
    /// Guest customer
    /// </summary>
    public Customer GuestCustomer
    {
        get;
    }
}