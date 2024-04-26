namespace Nop.Core.Domain.Customers;

/// <summary>
/// Customer password changed event
/// </summary>
public partial class CustomerPasswordChangedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="password">Password</param>
    public CustomerPasswordChangedEvent(CustomerPassword password)
    {
        Password = password;
    }

    /// <summary>
    /// Customer password
    /// </summary>
    public CustomerPassword Password { get; }
}