using System;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// A catalog category
/// </summary>
public record CategoryDto
{
    /// <summary>
    /// Gets or sets the category identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the category name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the category description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the parent category identifier, 0 for a root category
    /// </summary>
    public int ParentCategoryId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the category is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the category is shown on the home page
    /// </summary>
    public bool ShowOnHomepage { get; set; }

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
