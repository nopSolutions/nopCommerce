using System;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// A generic attribute (metafield) stored against an entity
/// </summary>
public record MetafieldDto
{
    /// <summary>
    /// Gets or sets the attribute identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the entity the attribute is stored against
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets the entity type, for example "product" or "customer"
    /// </summary>
    public string? KeyGroup { get; set; }

    /// <summary>
    /// Gets or sets the attribute key
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the attribute value
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the store the attribute is limited to, 0 for every store
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the date and time the attribute was last written (UTC)
    /// </summary>
    public DateTime? CreatedOrUpdatedDateUTC { get; set; }
}
