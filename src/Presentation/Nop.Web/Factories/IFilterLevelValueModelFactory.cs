using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.FilterLevels;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories;

public partial interface IFilterLevelValueModelFactory
{
    /// <summary>
    /// Prepare the filter level value overview model
    /// </summary>
    /// <param name="filterLevelValues">Filter level values</param>
    /// <returns>
    /// The result contains the filter level value overview model
    /// </returns>
    FilterLevelValueOverviewModel PrepareFilterLevelValueOverviewModel(IList<FilterLevelValue> filterLevelValues);

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
    /// Prepare search filter level value model
    /// </summary>
    /// <param name="model">Search filter level value model</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search filter level value model
    /// </returns>
    Task<SearchFilterLevelValueModel> PrepareSearchFilterLevelValueModelAsync(SearchFilterLevelValueModel model, CatalogProductsCommand command);
}
