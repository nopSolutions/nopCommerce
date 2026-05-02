namespace Nop.Services.ExportImport;

/// <summary>
/// Represents the type of the exported additional product info
/// </summary>
public enum ExportedAdditionalProductInfoType
{
    /// <summary>
    /// Not specified
    /// </summary>
    NotSpecified = 1,

    /// <summary>
    /// Product attribute
    /// </summary>
    ProductAttribute = 10,

    /// <summary>
    /// Specification attribute
    /// </summary>
    SpecificationAttribute = 20,

    /// <summary>
    /// Tier prices
    /// </summary>
    TierPrices = 30
}