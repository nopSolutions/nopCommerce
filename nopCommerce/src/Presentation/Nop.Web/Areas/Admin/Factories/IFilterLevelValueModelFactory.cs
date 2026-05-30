using Nop.Core.Domain.FilterLevels;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the filter level value model factory implementation
/// </summary>
public partial interface IFilterLevelValueModelFactory
{
    /// <summary>
    /// Prepare filter level value search model
    /// </summary>
    /// <param name="searchModel">Filter level value search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value search model
    /// </returns>
    Task<FilterLevelValueSearchModel> PrepareFilterLevelValueSearchModelAsync(FilterLevelValueSearchModel searchModel);

    /// <summary>
    /// Prepare paged filter level value list model
    /// </summary>
    /// <param name="searchModel">Filter level value search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value list model
    /// </returns>
    Task<FilterLevelValueListModel> PrepareFilterLevelValueListModelAsync(FilterLevelValueSearchModel searchModel);

    /// <summary>
    /// Prepare filter level value model
    /// </summary>
    /// <param name="model">Filter level value model</param>
    /// <param name="filterLevelValue">Filter level value</param>
    /// <returns>
    /// The result contains the filter level value model
    /// </returns>
    FilterLevelValueModel PrepareFilterLevelValueModel(FilterLevelValueModel model, FilterLevelValue filterLevelValue);

    /// <summary>
    /// Prepare paged filter level value product list model
    /// </summary>
    /// <param name="searchModel">Filter level value product search model</param>
    /// <param name="filterLevelValue">Filter level value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level value product list model
    /// </returns>
    Task<FilterLevelValueProductListModel> PrepareFilterLevelValueProductListModelAsync(FilterLevelValueProductSearchModel searchModel, FilterLevelValue filterLevelValue);

    /// <summary>
    /// Prepare product search model to add to the filter level value
    /// </summary>
    /// <param name="searchModel">Product search model to add to the filter level value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the filter level value
    /// </returns>
    Task<AddProductToFilterLevelValueSearchModel> PrepareAddProductToFilterLevelValueSearchModelAsync(AddProductToFilterLevelValueSearchModel searchModel);

    /// <summary>
    /// Prepare paged product list model to add to the filter level value
    /// </summary>
    /// <param name="searchModel">Product search model to add to the filter level value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product list model to add to the filter level value
    /// </returns>
    Task<AddProductToFilterLevelValueListModel> PrepareAddProductToFilterLevelValueListModelAsync(AddProductToFilterLevelValueSearchModel searchModel);
}
