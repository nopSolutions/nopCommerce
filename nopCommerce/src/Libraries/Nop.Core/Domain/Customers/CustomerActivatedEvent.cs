namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer activated event
/// </summary>
public partial class CustomerActivatedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">customer</param>
    public CustomerActivatedEvent(Customer customer)
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