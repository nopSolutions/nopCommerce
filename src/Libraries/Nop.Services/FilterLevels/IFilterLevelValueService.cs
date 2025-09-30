using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.FilterLevels;

namespace Nop.Services.FilterLevels;

/// <summary>
/// Filter level value service interface
/// </summary>
public partial interface IFilterLevelValueService
{
    /// <summary>
    /// Check if filter levels are disabled
    /// </summary>
    /// <returns>
    /// Tuple of booleans indicating if filter levels 1, 2, and 3 are disabled
    /// </returns>
    (bool filterLevel1, bool filterLevel2, bool filterLevel3) IsFilterLevelDisabled();


    /// <summary>
    /// Gets filter level values
    /// </summary>
    /// <param name="filterLevel1Value">Filter level 1 value</param>
    /// <param name="filterLevel2Value">Filter level 2 value</param>
    /// <param name="filterLevel3Value">Filter level 3 value</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value list model
    /// </returns>
    Task<IPagedList<FilterLevelValue>> GetAllFilterLevelValuesAsync(string filterLevel1Value = null, string filterLevel2Value = null, string filterLevel3Value = null, int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Inserts Filter level value
    /// </summary>
    /// <param name="filterLevelValue">Filter level value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertFilterLevelValueAsync(FilterLevelValue filterLevelValue);

    /// <summary>
    /// Gets a filter level value
    /// </summary>
    /// <param name="filterLevelValueId">Filter level value identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value
    /// </returns>
    Task<FilterLevelValue> GetFilterLevelValueByIdAsync(int filterLevelValueId);

    /// <summary>
    /// Gets filter level values by product identifier
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level values
    /// </returns>
    Task<IList<FilterLevelValue>> GetFilterLevelValuesByProductIdAsync(int productId);
   
    /// <summary>
    /// Gets filter level values by identifier
    /// </summary>
    /// <param name="filterLevelValueIds">Filter level value identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level values
    /// </returns>
    Task<IList<FilterLevelValue>> GetFilterLevelValuesByIdsAsync(int[] filterLevelValueIds);

    /// <summary>
    /// Updates the filter level value
    /// </summary>
    /// <param name="filterLevelValue">Filter level value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateFilterLevelValueAsync(FilterLevelValue filterLevelValue);

    /// <summary>
    /// Delete filter level value
    /// </summary>
    /// <param name="filterLevelValue">Filter level value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteFilterLevelValueAsync(FilterLevelValue filterLevelValue);

    /// <summary>
    /// Delete filter level values
    /// </summary>
    /// <param name="filterLevelValues">Filter level values</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteFilterLevelValuesAsync(IList<FilterLevelValue> filterLevelValues);

    #region Mapping

    /// <summary>
    /// Gets products collection by filter level value identifier
    /// </summary>
    /// <param name="filterLevelValueId">Filter level value identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="storeId">Store identifier; 0 to load all records</param>
    /// <param name="orderBy">Order by</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products collection
    /// </returns>
    Task<IPagedList<Product>> GetProductsByFilterLevelValueIdAsync(int filterLevelValueId,
        int pageIndex = 0,
        int pageSize = int.MaxValue,
        int storeId = 0,
        ProductSortingEnum orderBy = ProductSortingEnum.Position);

    /// <summary>
    /// Gets filter level value product mapping collection
    /// </summary>
    /// <param name="filterLevelValueId">Filter level value identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value product mapping collection
    /// </returns>
    Task<IPagedList<FilterLevelValueProductMapping>> GetFilterLevelValueProductsByFilterLevelValueIdAsync(int filterLevelValueId,
        int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Gets a filter level value product mapping
    /// </summary>
    /// <param name="filterLevelValueProductId">Filter level value product mapping identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value product mapping
    /// </returns>
    Task<FilterLevelValueProductMapping> GetFilterLevelValueProductByIdAsync(int filterLevelValueProductId);

    /// <summary>
    /// Deletes a filter level value product mapping
    /// </summary>
    /// <param name="filterLevelValueProduct">Filter level value product mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteFilterLevelValueProductAsync(FilterLevelValueProductMapping filterLevelValueProduct);

    /// <summary>
    /// Inserts a filter level value product mapping
    /// </summary>
    /// <param name="filterLevelValueProduct">Filter level value product mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertProductFilterLevelValueAsync(FilterLevelValueProductMapping filterLevelValueProduct);

    #endregion
}
