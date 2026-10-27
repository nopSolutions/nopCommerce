
using Nop.Core.Domain.PriceLists;
using Nop.Web.Areas.Admin.Models.PriceLists;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the price list model factory
/// </summary>
public partial interface IPriceListModelFactory
{
    /// <summary>
    /// Prepare price list search model
    /// </summary>
    /// <param name="searchModel">Price list search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list search model
    /// </returns>
    Task<PriceListSearchModel> PreparePriceListSearchModelAsync(PriceListSearchModel searchModel);

    /// <summary>
    /// Prepare paged price list list model
    /// </summary>
    /// <param name="searchModel">Price list search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list list model
    /// </returns>
    Task<PriceListListModel> PreparePriceListListModelAsync(PriceListSearchModel searchModel);

    /// <summary>
    /// Prepare price list model
    /// </summary>
    /// <param name="model">Price list model</param>
    /// <param name="priceList">Price list</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list model
    /// </returns>
    Task<PriceListModel> PreparePriceListModelAsync(PriceListModel model, PriceList priceList, bool excludeProperties = false);

    #region Product mapping

    /// <summary>
    /// Prepare paged price list item list model
    /// </summary>
    /// <param name="searchModel">Price list item search model</param>
    /// <param name="priceList">Price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list item list model
    /// </returns>
    Task<PriceListItemListModel> PreparePriceListItemListModelAsync(PriceListItemSearchModel searchModel, PriceList priceList);

    /// <summary>
    /// Prepare product search model to add to the price list
    /// </summary>
    /// <param name="searchModel">Product search model to add to the price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the price list
    /// </returns>
    Task<AddProductToPriceListSearchModel> PrepareAddProductToPriceListSearchModelAsync(AddProductToPriceListSearchModel searchModel);

    /// <summary>
    /// Prepare paged product list model to add to the price list
    /// </summary>
    /// <param name="searchModel">Product search model to add to the price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product list model to add to the price list
    /// </returns>
    Task<AddProductToPriceListListModel> PrepareAddProductToPriceListListModelAsync(AddProductToPriceListSearchModel searchModel);

    #endregion

    #region Customer mapping

    /// <summary>
    /// Prepare paged price list customer list model
    /// </summary>
    /// <param name="searchModel">Price list customer search model</param>
    /// <param name="priceList">Price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list customer list model
    /// </returns>
    Task<PriceListCustomerListModel> PreparePriceListCustomerListModelAsync(PriceListCustomerSearchModel searchModel, PriceList priceList);

    /// <summary>
    /// Prepare customer search model to add to the price list
    /// </summary>
    /// <param name="searchModel">Customer search model to add to the price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer search model to add to the price list
    /// </returns>
    Task<AddCustomerToPriceListSearchModel> PrepareAddCustomerToPriceListSearchModelAsync(AddCustomerToPriceListSearchModel searchModel);

    /// <summary>
    /// Prepare paged customer list model to add to the price list
    /// </summary>
    /// <param name="searchModel">Customer search model to add to the price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer list model to add to the price list
    /// </returns>
    Task<AddCustomerToPriceListListModel> PrepareAddCustomerToPriceListListModelAsync(AddCustomerToPriceListSearchModel searchModel);

    #endregion
}
