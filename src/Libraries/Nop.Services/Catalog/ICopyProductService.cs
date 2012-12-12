
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Copy product service
    /// </summary>
    public partial interface ICopyProductService
    {
        /// <summary>
        /// Create a copy of product variant with all depended data
        /// </summary>
        /// <param name="productVariant">The product variant to copy</param>
        /// <param name="productId">The product identifier</param>
        /// <param name="newName">The name of product variant duplicate</param>
        /// <param name="isPublished">A value indicating whether the product variant duplicate should be published</param>
        /// <param name="copyImage">A value indicating whether the product variant image should be copied</param>
        /// <returns>Product variant copy</returns>
        ProductVariant CopyProductVariant(ProductVariant productVariant, int productId,
            string newName, bool isPublished, bool copyImage);

        /// <summary>
        /// Create a copy of product with all depended data
        /// </summary>
        /// <param name="product">The product to copy</param>
        /// <param name="newName">The name of product duplicate</param>
        /// <param name="isPublished">A value indicating whether the product duplicate should be published</param>
        /// <param name="copyImages">A value indicating whether the product images should be copied</param>
        /// <returns>Product copy</returns>
        Product CopyProduct(Product product, string newName, bool isPublished, bool copyImages);
    }
}
