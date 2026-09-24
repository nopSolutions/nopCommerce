namespace Nop.Plugin.Misc.PunchOut.Domain;

/// <summary>
/// Represents a PunchOut session
/// </summary>
public class PunchOutSession
{
    /// <summary>
    /// Session token identifier for the PunchOut session
    /// </summary>
    public string SessionId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier used for PunchOut integration
    /// </summary>
    public string BuyerCookie { get; set; }

    /// <summary>
    /// Gets or sets the return url used for PunchOut PunchOutOrderMessage response
    /// </summary>
    public string ReturnUrl { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the session is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the session was created
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}
