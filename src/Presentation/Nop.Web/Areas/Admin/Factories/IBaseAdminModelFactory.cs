using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the base model factory that implements a most common admin model factories methods
    /// </summary>
    public partial interface IBaseAdminModelFactory
    {
        /// <summary>
        /// Prepare available activity log types
        /// </summary>
        /// <param name="items">Activity log type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareActivityLogTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available order statuses
        /// </summary>
        /// <param name="items">Order status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareOrderStatusesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available payment statuses
        /// </summary>
        /// <param name="items">Payment status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PreparePaymentStatusesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available shipping statuses
        /// </summary>
        /// <param name="items">Shipping status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareShippingStatusesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available countries
        /// </summary>
        /// <param name="items">Country items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareCountriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available states and provinces
        /// </summary>
        /// <param name="items">State and province items</param>
        /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareStatesAndProvincesAsync(IList<SelectListItem> items, int? countryId,
            bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available languages
        /// </summary>
        /// <param name="items">Language items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareLanguagesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available stores
        /// </summary>
        /// <param name="items">Store items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareStoresAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available customer roles
        /// </summary>
        /// <param name="items">Customer role items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareCustomerRolesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available email accounts
        /// </summary>
        /// <param name="items">Email account items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareEmailAccountsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available tax categories
        /// </summary>
        /// <param name="items">Tax category items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareTaxCategoriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available categories
        /// </summary>
        /// <param name="items">Category items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareCategoriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available manufacturers
        /// </summary>
        /// <param name="items">Manufacturer items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareManufacturersAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available vendors
        /// </summary>
        /// <param name="items">Vendor items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareVendorsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available product types
        /// </summary>
        /// <param name="items">Product type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareProductTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available category templates
        /// </summary>
        /// <param name="items">Category template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareCategoryTemplatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available time zones
        /// </summary>
        /// <param name="items">Time zone items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareTimeZonesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available shopping cart types
        /// </summary>
        /// <param name="items">Shopping cart type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareShoppingCartTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available tax display types
        /// </summary>
        /// <param name="items">Tax display type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareTaxDisplayTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available currencies
        /// </summary>
        /// <param name="items">Currency items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareCurrenciesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available discount types
        /// </summary>
        /// <param name="items">Discount type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareDiscountTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available log levels
        /// </summary>
        /// <param name="items">Log level items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareLogLevelsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available manufacturer templates
        /// </summary>
        /// <param name="items">Manufacturer template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareManufacturerTemplatesAsync(IList<SelectListItem> items,
            bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available load plugin modes
        /// </summary>
        /// <param name="items">Load plugin mode items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareLoadPluginModesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available plugin groups
        /// </summary>
        /// <param name="items">Plugin group items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PreparePluginGroupsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available return request statuses
        /// </summary>
        /// <param name="items">Return request status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareReturnRequestStatusesAsync(IList<SelectListItem> items,
            bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available product templates
        /// </summary>
        /// <param name="items">Product template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareProductTemplatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available topic templates
        /// </summary>
        /// <param name="items">Topic template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareTopicTemplatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available warehouses
        /// </summary>
        /// <param name="items">Warehouse items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareWarehousesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available delivery dates
        /// </summary>
        /// <param name="items">Delivery date items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareDeliveryDatesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available product availability ranges
        /// </summary>
        /// <param name="items">Product availability range items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareProductAvailabilityRangesAsync(IList<SelectListItem> items,
            bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available GDPR request types
        /// </summary>
        /// <param name="items">Request type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareGdprRequestTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

        /// <summary>
        /// Prepare available specification attribute groups
        /// </summary>
        /// <param name="items">Specification attributes</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareSpecificationAttributeGroupsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
    }
}