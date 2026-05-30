namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents a product attribute combination picture
/// </summary>
public partial class ProductAttributeCombinationPicture : BaseEntity
{
    /// <summary>
    /// Gets or sets the product attribute combination id
    /// </summary>
    public int ProductAttributeCombinationId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of picture associated with this combination
    /// </summary>
    public int PictureId { get; set; }
}