namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer change working language event
/// </summary>
public partial class CustomerChangeWorkingLanguageEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    public CustomerChangeWorkingLanguageEvent(Customer customer)
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
