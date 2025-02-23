namespace Nop.Core.Domain.Gdpr;

/// <summary>
/// Customer permanently deleted (GDPR)
/// </summary>
public partial class CustomerPermanentlyDeleted
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="email">Email</param>
    public CustomerPermanentlyDeleted(int customerId, string email)
    {
        CustomerId = customerId;
        Email = email;
    }

    /// <summary>
    /// Customer identifier
    /// </summary>
    public int CustomerId { get; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; }
}