namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents a 3D object associated with a product
/// </summary>
public partial class Product3dObject : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the uploaded 3D object file
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the picture identifier associated with the 3D object
    /// </summary>
    public int? PreviewPictureId { get; set; }

    /// <summary>
    /// Gets or sets the alternative text for the 3D object
    /// </summary>
    public string AltAttribute { get; set; }
}
