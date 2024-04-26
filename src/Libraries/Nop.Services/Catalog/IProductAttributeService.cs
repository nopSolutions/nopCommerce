using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog;

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

    #region Product attribute value pictures

    /// <summary>
    /// Deletes a product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteProductAttributeValuePictureAsync(ProductAttributeValuePicture valuePicture);

    /// <summary>
    /// Inserts a product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertProductAttributeValuePictureAsync(ProductAttributeValuePicture valuePicture);

    /// <summary>
    /// Updates a product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateProductAttributeValuePictureAsync(ProductAttributeValuePicture valuePicture);

    /// <summary>
    /// Get product attribute value pictures
    /// </summary>
    /// <param name="valueId">Value id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute value pictures
    /// </returns>
    Task<IList<ProductAttributeValuePicture>> GetProductAttributeValuePicturesAsync(int valueId);

    /// <summary>
    /// Returns a ProductAttributeValuePicture that has the specified values
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="valueId">Product attribute value identifier</param>
    /// <param name="pictureId">Picture identifier</param>
    /// <returns>A ProductAttributeValuePicture that has the specified values; otherwise null</returns>
    ProductAttributeValuePicture FindProductAttributeValuePicture(IList<ProductAttributeValuePicture> source, int valueId, int pictureId);

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

    #region Product attribute combination pictures

    /// <summary>
    /// Deletes a product attribute combination picture
    /// </summary>
    /// <param name="combination">Product attribute combination picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteProductAttributeCombinationPictureAsync(ProductAttributeCombinationPicture combinationPicture);

    /// <summary>
    /// Inserts a product attribute combination picture
    /// </summary>
    /// <param name="combination">Product attribute combination picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertProductAttributeCombinationPictureAsync(ProductAttributeCombinationPicture combinationPicture);

    /// <summary>
    /// Updates a product attribute combination picture
    /// </summary>
    /// <param name="combination">Product attribute combination picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateProductAttributeCombinationPictureAsync(ProductAttributeCombinationPicture combinationPicture);

    /// <summary>
    /// Get product attribute combination pictures
    /// </summary>
    /// <param name="combinationId">Combination id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute combination pictures
    /// </returns>
    Task<IList<ProductAttributeCombinationPicture>> GetProductAttributeCombinationPicturesAsync(int combinationId);

    /// <summary>
    /// Returns a ProductAttributeCombinationPicture that has the specified values
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="combinationId">Product attribute combination identifier</param>
    /// <param name="pictureId">Picture identifier</param>
    /// <returns>A ProductAttributeCombinationPicture that has the specified values; otherwise null</returns>
    ProductAttributeCombinationPicture FindProductAttributeCombinationPicture(IList<ProductAttributeCombinationPicture> source, int combinationId, int pictureId);

    #endregion
}