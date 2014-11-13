using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product attribute service interface
    /// </summary>
    public partial interface IProductAttributeService
    {
        #region Product attributes

        /// <summary>
        /// Deletes a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        void DeleteProductAttribute(ProductAttribute productAttribute);

        /// <summary>
        /// Gets all product attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Product attribute collection</returns>
        IPagedList<ProductAttribute> GetAllProductAttributes(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a product attribute 
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <returns>Product attribute </returns>
        ProductAttribute GetProductAttributeById(int productAttributeId);

        /// <summary>
        /// Inserts a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        void InsertProductAttribute(ProductAttribute productAttribute);

        /// <summary>
        /// Updates the product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        void UpdateProductAttribute(ProductAttribute productAttribute);

        #endregion

        #region Product variant attributes mappings (ProductVariantAttribute)

        /// <summary>
        /// Deletes a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">Product variant attribute mapping</param>
        void DeleteProductVariantAttribute(ProductVariantAttribute productVariantAttribute);

        /// <summary>
        /// Gets product variant attribute mappings by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product variant attribute mapping collection</returns>
        IList<ProductVariantAttribute> GetProductVariantAttributesByProductId(int productId);

        /// <summary>
        /// Gets a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttributeId">Product variant attribute mapping identifier</param>
        /// <returns>Product variant attribute mapping</returns>
        ProductVariantAttribute GetProductVariantAttributeById(int productVariantAttributeId);

        /// <summary>
        /// Inserts a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">The product variant attribute mapping</param>
        void InsertProductVariantAttribute(ProductVariantAttribute productVariantAttribute);

        /// <summary>
        /// Updates the product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">The product variant attribute mapping</param>
        void UpdateProductVariantAttribute(ProductVariantAttribute productVariantAttribute);

        #endregion

        #region Product variant attribute values (ProductVariantAttributeValue)

        /// <summary>
        /// Deletes a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">Product variant attribute value</param>
        void DeleteProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue);

        /// <summary>
        /// Gets product variant attribute values by product identifier
        /// </summary>
        /// <param name="productVariantAttributeId">The product variant attribute mapping identifier</param>
        /// <returns>Product variant attribute mapping collection</returns>
        IList<ProductVariantAttributeValue> GetProductVariantAttributeValues(int productVariantAttributeId);

        /// <summary>
        /// Gets a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValueId">Product variant attribute value identifier</param>
        /// <returns>Product variant attribute value</returns>
        ProductVariantAttributeValue GetProductVariantAttributeValueById(int productVariantAttributeValueId);

        /// <summary>
        /// Inserts a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">The product variant attribute value</param>
        void InsertProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue);

        /// <summary>
        /// Updates the product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">The product variant attribute value</param>
        void UpdateProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue);

        #endregion

        #region Product attribute combinations

        /// <summary>
        /// Deletes a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        void DeleteProductAttributeCombination(ProductAttributeCombination combination);

        /// <summary>
        /// Gets all product attribute combinations
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product attribute combination collection</returns>
        IList<ProductAttributeCombination> GetAllProductAttributeCombinations(int productId);

        /// <summary>
        /// Gets a product attribute combination
        /// </summary>
        /// <param name="productAttributeCombinationId">Product attribute combination identifier</param>
        /// <returns>Product attribute combination</returns>
        ProductAttributeCombination GetProductAttributeCombinationById(int productAttributeCombinationId);

        /// <summary>
        /// Inserts a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        void InsertProductAttributeCombination(ProductAttributeCombination combination);

        /// <summary>
        /// Updates a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        void UpdateProductAttributeCombination(ProductAttributeCombination combination);

        #endregion
    }
}
