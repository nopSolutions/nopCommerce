namespace Nop.Core.Domain.Customers;

/// <summary>
/// Represents the context and state information for a one-time password (OTP) operation
/// </summary>
public partial class OtpContext
{
    /// <summary>
    /// Gets or sets the code
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the date when the code was generated
    /// </summary>
    public DateTime? CodeGeneratedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the number of messages that have been sent
    /// </summary>
    public int SentCount { get; set; }

    /// <summary>
    /// Gets or sets the date when the most recent attempt was made
    /// </summary>
    public DateTime? LastAttemptAtUtc { get; set; }
}
