using System;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// A manufacturer
/// </summary>
public record ManufacturerDto
{
    /// <summary>
    /// Gets or sets the manufacturer identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the manufacturer is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the picture identifier
    /// </summary>
    public int PictureId { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation (UTC)
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance update (UTC)
    /// </summary>
    public DateTime UpdatedOnUtc { get; set; }
}
