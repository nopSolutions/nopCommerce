using System;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// A log record
/// </summary>
public record LogDto
{
    /// <summary>
    /// Gets or sets the log record identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the log level identifier, 10 Debug to 50 Fatal
    /// </summary>
    public int LogLevelId { get; set; }

    /// <summary>
    /// Gets or sets the short message
    /// </summary>
    public string? ShortMessage { get; set; }

    /// <summary>
    /// Gets or sets the full message, which carries the exception detail on failures
    /// </summary>
    public string? FullMessage { get; set; }

    /// <summary>
    /// Gets or sets the IP address the request came from
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the customer the record belongs to, null for anonymous requests
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the page that was requested
    /// </summary>
    public string? PageUrl { get; set; }

    /// <summary>
    /// Gets or sets the referrer URL
    /// </summary>
    public string? ReferrerUrl { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation (UTC)
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}
