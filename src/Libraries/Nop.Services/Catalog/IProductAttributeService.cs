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
        /// <returns>Product attributes</returns>
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

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>List of IDs not existing attributes</returns>
        int[] GetNotExistingAttributes(int[] attributeId);

        #endregion

        #region Product attributes mappings

        /// <summary>
        /// Deletes a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        void DeleteProductAttributeMapping(ProductAttributeMapping productAttributeMapping);

        /// <summary>
        /// Gets product attribute mappings by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product attribute mapping collection</returns>
        IList<ProductAttributeMapping> GetProductAttributeMappingsByProductId(int productId);

        /// <summary>
        /// Gets a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
        /// <returns>Product attribute mapping</returns>
        ProductAttributeMapping GetProductAttributeMappingById(int productAttributeMappingId);

        /// <summary>
        /// Inserts a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">The product attribute mapping</param>
        void InsertProductAttributeMapping(ProductAttributeMapping productAttributeMapping);

        /// <summary>
        /// Updates the product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">The product attribute mapping</param>
        void UpdateProductAttributeMapping(ProductAttributeMapping productAttributeMapping);

        #endregion

        #region Product attribute values

        /// <summary>
        /// Deletes a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">Product attribute value</param>
        void DeleteProductAttributeValue(ProductAttributeValue productAttributeValue);

        /// <summary>
        /// Gets product attribute values by product attribute mapping identifier
        /// </summary>
        /// <param name="productAttributeMappingId">The product attribute mapping identifier</param>
        /// <returns>Product attribute values</returns>
        IList<ProductAttributeValue> GetProductAttributeValues(int productAttributeMappingId);

        /// <summary>
        /// Gets a product attribute value
        /// </summary>
        /// <param name="productAttributeValueId">Product attribute value identifier</param>
        /// <returns>Product attribute value</returns>
        ProductAttributeValue GetProductAttributeValueById(int productAttributeValueId);

        /// <summary>
        /// Inserts a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        void InsertProductAttributeValue(ProductAttributeValue productAttributeValue);

        /// <summary>
        /// Updates the product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        void UpdateProductAttributeValue(ProductAttributeValue productAttributeValue);

        #endregion

        #region Predefined product attribute values

        /// <summary>
        /// Deletes a predefined product attribute value
        /// </summary>
        /// <param name="ppav">Predefined product attribute value</param>
        void DeletePredefinedProductAttributeValue(PredefinedProductAttributeValue ppav);

        /// <summary>
        /// Gets predefined product attribute values by product attribute identifier
        /// </summary>
        /// <param name="productAttributeId">The product attribute identifier</param>
        /// <returns>Product attribute mapping collection</returns>
        IList<PredefinedProductAttributeValue> GetPredefinedProductAttributeValues(int productAttributeId);

        /// <summary>
        /// Gets a predefined product attribute value
        /// </summary>
        /// <param name="id">Predefined product attribute value identifier</param>
        /// <returns>Predefined product attribute value</returns>
        PredefinedProductAttributeValue GetPredefinedProductAttributeValueById(int id);

        /// <summary>
        /// Inserts a predefined product attribute value
        /// </summary>
        /// <param name="ppav">The predefined product attribute value</param>
        void InsertPredefinedProductAttributeValue(PredefinedProductAttributeValue ppav);

        /// <summary>
        /// Updates the predefined product attribute value
        /// </summary>
        /// <param name="ppav">The predefined product attribute value</param>
        void UpdatePredefinedProductAttributeValue(PredefinedProductAttributeValue ppav);

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
        /// <returns>Product attribute combinations</returns>
        IList<ProductAttributeCombination> GetAllProductAttributeCombinations(int productId);

        /// <summary>
        /// Gets a product attribute combination
        /// </summary>
        /// <param name="productAttributeCombinationId">Product attribute combination identifier</param>
        /// <returns>Product attribute combination</returns>
        ProductAttributeCombination GetProductAttributeCombinationById(int productAttributeCombinationId);

        /// <summary>
        /// Gets a product attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product attribute combination</returns>
        ProductAttributeCombination GetProductAttributeCombinationBySku(string sku);

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
