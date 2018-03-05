using System.Collections.Generic;
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
        void PrepareActivityLogTypes(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available order statuses
        /// </summary>
        /// <param name="items">Order status items</param>
        void PrepareOrderStatuses(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available payment statuses
        /// </summary>
        /// <param name="items">Payment status items</param>
        void PreparePaymentStatuses(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available shipping statuses
        /// </summary>
        /// <param name="items">Shipping status items</param>
        void PrepareShippingStatuses(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available countries
        /// </summary>
        /// <param name="items">Country items</param>
        void PrepareCountries(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available states and provinces
        /// </summary>
        /// <param name="items">State and province items</param>
        /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
        void PrepareStatesAndProvinces(IList<SelectListItem> items, int? countryId);

        /// <summary>
        /// Prepare available languages
        /// </summary>
        /// <param name="items">Language items</param>
        void PrepareLanguages(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available stores
        /// </summary>
        /// <param name="items">Store items</param>
        void PrepareStores(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available customer roles
        /// </summary>
        /// <param name="items">Customer role items</param>
        void PrepareCustomerRoles(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available email accounts
        /// </summary>
        /// <param name="items">Email account items</param>
        void PrepareEmailAccounts(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available tax categories
        /// </summary>
        /// <param name="items">Tax category items</param>
        void PrepareTaxCategories(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available categories
        /// </summary>
        /// <param name="items">Category items</param>
        void PrepareCategories(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available manufacturers
        /// </summary>
        /// <param name="items">Manufacturer items</param>
        void PrepareManufacturers(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available vendors
        /// </summary>
        /// <param name="items">Vendor items</param>
        void PrepareVendors(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available product types
        /// </summary>
        /// <param name="items">Product type items</param>
        void PrepareProductTypes(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available parent categories
        /// </summary>
        /// <param name="items">Category items</param>
        void PrepareParentCategories(IList<SelectListItem> items);

        /// <summary>
        /// Prepare available category templates
        /// </summary>
        /// <param name="items">Category template items</param>
        void PrepareCategoryTemplates(IList<SelectListItem> items);
    }
}