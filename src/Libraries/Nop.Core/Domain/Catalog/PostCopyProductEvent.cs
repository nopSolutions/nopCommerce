namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Post copy product event
/// </summary>
public partial class PostCopyProductEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="originalProduct">Original product</param>
    /// <param name="copyProduct">Copy product</param>
    public PostCopyProductEvent(Product originalProduct, Product copyProduct)
    {
        OriginalProduct = originalProduct;
        CopyProduct = copyProduct;
    }

    /// <summary>
    /// Original product
    /// </summary>
    public Product OriginalProduct { get; }

    /// <summary>
    /// Copy product
    /// </summary>
    public Product CopyProduct { get; }
}