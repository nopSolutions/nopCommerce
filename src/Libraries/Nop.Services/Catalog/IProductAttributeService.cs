using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteProductAttributeAsync(ProductAttribute productAttribute);

        /// <summary>
        /// Deletes product attributes
        /// </summary>
        /// <param name="productAttributes">Product attributes</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteProductAttributesAsync(IList<ProductAttribute> productAttributes);

        /// <summary>
        /// Gets all product attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attributes
        /// </returns>
        Task<IPagedList<ProductAttribute>> GetAllProductAttributesAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a product attribute 
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute 
        /// </returns>
        Task<ProductAttribute> GetProductAttributeByIdAsync(int productAttributeId);

        /// <summary>
        /// Gets product attributes 
        /// </summary>
        /// <param name="productAttributeIds">Product attribute identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attributes
        /// </returns>
        Task<IList<ProductAttribute>> GetProductAttributeByIdsAsync(int[] productAttributeIds);

        /// <summary>
        /// Inserts a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertProductAttributeAsync(ProductAttribute productAttribute);

        /// <summary>
        /// Updates the product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateProductAttributeAsync(ProductAttribute productAttribute);

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of IDs not existing attributes
        /// </returns>
        Task<int[]> GetNotExistingAttributesAsync(int[] attributeId);

        #endregion

        #region Product attributes mappings

        /// <summary>
        /// Deletes a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping);

        /// <summary>
        /// Gets product attribute mappings by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute mapping collection
        /// </returns>
        Task<IList<ProductAttributeMapping>> GetProductAttributeMappingsByProductIdAsync(int productId);

        /// <summary>
        /// Gets a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute mapping
        /// </returns>
        Task<ProductAttributeMapping> GetProductAttributeMappingByIdAsync(int productAttributeMappingId);

        /// <summary>
        /// Inserts a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">The product attribute mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping);

        /// <summary>
        /// Updates the product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">The product attribute mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping);

        #endregion

        #region Product attribute values

        /// <summary>
        /// Deletes a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">Product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteProductAttributeValueAsync(ProductAttributeValue productAttributeValue);

        /// <summary>
        /// Gets product attribute values by product attribute mapping identifier
        /// </summary>
        /// <param name="productAttributeMappingId">The product attribute mapping identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute values
        /// </returns>
        Task<IList<ProductAttributeValue>> GetProductAttributeValuesAsync(int productAttributeMappingId);

        /// <summary>
        /// Gets a product attribute value
        /// </summary>
        /// <param name="productAttributeValueId">Product attribute value identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute value
        /// </returns>
        Task<ProductAttributeValue> GetProductAttributeValueByIdAsync(int productAttributeValueId);

        /// <summary>
        /// Inserts a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertProductAttributeValueAsync(ProductAttributeValue productAttributeValue);

        /// <summary>
        /// Updates the product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateProductAttributeValueAsync(ProductAttributeValue productAttributeValue);

        #endregion

        #region Predefined product attribute values

        /// <summary>
        /// Deletes a predefined product attribute value
        /// </summary>
        /// <param name="ppav">Predefined product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeletePredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav);

        /// <summary>
        /// Gets predefined product attribute values by product attribute identifier
        /// </summary>
        /// <param name="productAttributeId">The product attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute mapping collection
        /// </returns>
        Task<IList<PredefinedProductAttributeValue>> GetPredefinedProductAttributeValuesAsync(int productAttributeId);

        /// <summary>
        /// Gets a predefined product attribute value
        /// </summary>
        /// <param name="id">Predefined product attribute value identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the predefined product attribute value
        /// </returns>
        Task<PredefinedProductAttributeValue> GetPredefinedProductAttributeValueByIdAsync(int id);

        /// <summary>
        /// Inserts a predefined product attribute value
        /// </summary>
        /// <param name="ppav">The predefined product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertPredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav);

        /// <summary>
        /// Updates the predefined product attribute value
        /// </summary>
        /// <param name="ppav">The predefined product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdatePredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav);

        #endregion

        #region Product attribute combinations

        /// <summary>
        /// Deletes a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteProductAttributeCombinationAsync(ProductAttributeCombination combination);

        /// <summary>
        /// Gets all product attribute combinations
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute combinations
        /// </returns>
        Task<IList<ProductAttributeCombination>> GetAllProductAttributeCombinationsAsync(int productId);

        /// <summary>
        /// Gets a product attribute combination
        /// </summary>
        /// <param name="productAttributeCombinationId">Product attribute combination identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute combination
        /// </returns>
        Task<ProductAttributeCombination> GetProductAttributeCombinationByIdAsync(int productAttributeCombinationId);

        /// <summary>
        /// Gets a product attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute combination
        /// </returns>
        Task<ProductAttributeCombination> GetProductAttributeCombinationBySkuAsync(string sku);

        /// <summary>
        /// Inserts a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertProductAttributeCombinationAsync(ProductAttributeCombination combination);

        /// <summary>
        /// Updates a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateProductAttributeCombinationAsync(ProductAttributeCombination combination);

        #endregion
    }
}
