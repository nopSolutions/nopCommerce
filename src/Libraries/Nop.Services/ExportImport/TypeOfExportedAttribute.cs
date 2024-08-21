namespace Nop.Services.ExportImport;

/// <summary>
/// Represents the type of the exported attribute
/// </summary>
public enum ExportedAttributeType
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
    SpecificationAttribute = 20
}