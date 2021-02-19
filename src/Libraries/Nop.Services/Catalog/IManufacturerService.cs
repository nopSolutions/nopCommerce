using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Manufacturer service
    /// </summary>
    public partial interface IManufacturerService
    {
        /// <summary>
        /// Clean up manufacturer references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        Task ClearDiscountManufacturerMappingAsync(Discount discount);

        /// <summary>
        /// Deletes a discount-manufacturer mapping record
        /// </summary>
        /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
        Task DeleteDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping);

        /// <summary>
        /// Deletes a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        Task DeleteManufacturerAsync(Manufacturer manufacturer);

        /// <summary>
        /// Delete manufacturers
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        Task DeleteManufacturersAsync(IList<Manufacturer> manufacturers);

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="manufacturerName">Manufacturer name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>Manufacturers</returns>
        Task<IPagedList<Manufacturer>> GetAllManufacturersAsync(string manufacturerName = "",
            int storeId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false,
            bool? overridePublished = null);

        /// <summary>
        /// Get manufacturer identifiers to which a discount is applied
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Manufacturer identifiers</returns>
        Task<IList<int>> GetAppliedManufacturerIdsAsync(Discount discount, Customer customer);

        /// <summary>
        /// Gets a manufacturer
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Manufacturer</returns>
        Task<Manufacturer> GetManufacturerByIdAsync(int manufacturerId);

        /// <summary>
        /// Gets the manufacturers by category identifier
        /// </summary>
        /// <param name="categoryId">Cateogry identifier</param>
        /// <returns>Manufacturers</returns>
        Task<IList<Manufacturer>> GetManufacturersByCategoryIdAsync(int categoryId);

        /// <summary>
        /// Gets manufacturers by identifier
        /// </summary>
        /// <param name="manufacturerIds">manufacturer identifiers</param>
        /// <returns>Manufacturers</returns>
        Task<IList<Manufacturer>> GetManufacturersByIdsAsync(int[] manufacturerIds);

        /// <summary>
        /// Get manufacturers for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted manufacturers</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of manufacturers</returns>
        Task<IPagedList<Manufacturer>> GetManufacturersWithAppliedDiscountAsync(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        Task InsertManufacturerAsync(Manufacturer manufacturer);

        /// <summary>
        /// Updates the manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        Task UpdateManufacturerAsync(Manufacturer manufacturer);

        /// <summary>
        /// Deletes a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        Task DeleteProductManufacturerAsync(ProductManufacturer productManufacturer);

        /// <summary>
        /// Gets product manufacturer collection
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product manufacturer collection</returns>
        Task<IPagedList<ProductManufacturer>> GetProductManufacturersByManufacturerIdAsync(int manufacturerId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a product manufacturer mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product manufacturer mapping collection</returns>
        Task<IList<ProductManufacturer>> GetProductManufacturersByProductIdAsync(int productId, bool showHidden = false);

        /// <summary>
        /// Gets a product manufacturer mapping 
        /// </summary>
        /// <param name="productManufacturerId">Product manufacturer mapping identifier</param>
        /// <returns>Product manufacturer mapping</returns>
        Task<ProductManufacturer> GetProductManufacturerByIdAsync(int productManufacturerId);

        /// <summary>
        /// Inserts a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        Task InsertProductManufacturerAsync(ProductManufacturer productManufacturer);

        /// <summary>
        /// Updates the product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        Task UpdateProductManufacturerAsync(ProductManufacturer productManufacturer);

        /// <summary>
        /// Get manufacturer IDs for products
        /// </summary>
        /// <param name="productIds">Products IDs</param>
        /// <returns>Manufacturer IDs for products</returns>
        Task<IDictionary<int, int[]>> GetProductManufacturerIdsAsync(int[] productIds);

        /// <summary>
        /// Returns a list of names of not existing manufacturers
        /// </summary>
        /// <param name="manufacturerIdsNames">The names and/or IDs of the manufacturers to check</param>
        /// <returns>List of names and/or IDs not existing manufacturers</returns>
        Task<string[]> GetNotExistingManufacturersAsync(string[] manufacturerIdsNames);

        //TODO: migrate to an extension method
        /// <summary>
        /// Returns a ProductManufacturer that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>A ProductManufacturer that has the specified values; otherwise null</returns>
        ProductManufacturer FindProductManufacturer(IList<ProductManufacturer> source, int productId, int manufacturerId);

        /// <summary>
        /// Get a discount-manufacturer mapping record
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Result</returns>
        Task<DiscountManufacturerMapping> GetDiscountAppliedToManufacturerAsync(int manufacturerId, int discountId);

        /// <summary>
        /// Inserts a discount-manufacturer mapping record
        /// </summary>
        /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
        Task InsertDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping);
    }
}