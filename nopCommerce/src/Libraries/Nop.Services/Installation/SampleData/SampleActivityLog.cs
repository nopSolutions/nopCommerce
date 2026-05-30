namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample activity log record
/// </summary>
public partial class SampleActivityLog
{
    /// <summary>
    /// Gets or sets the activity comment
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Gets or sets the IP address
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the activity log type
    /// </summary>
    public string ActivityLogType { get; set; }
}
