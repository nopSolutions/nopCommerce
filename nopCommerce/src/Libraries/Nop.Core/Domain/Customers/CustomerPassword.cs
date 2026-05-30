namespace Nop.Core.Domain.Customers;

/// <summary>
/// Represents a customer password
/// </summary>
public partial class CustomerPassword : BaseEntity
{
    public CustomerPassword()
    {
        PasswordFormat = PasswordFormat.Clear;
    }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the password format identifier
    /// </summary>
    public int PasswordFormatId { get; set; }

    /// <summary>
    /// Gets or sets the password salt
    /// </summary>
    public string PasswordSalt { get; set; }

    /// <summary>
    /// Gets or sets the date and time of entity creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the password format
    /// </summary>
    public PasswordFormat PasswordFormat
    {
        get => (PasswordFormat)PasswordFormatId;
        set => PasswordFormatId = (int)value;
    }
}