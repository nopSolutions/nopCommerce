using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Core.Caching;
using Nop.Services.Catalog;
using Nop.Services.Vendors;

namespace Nop.Web.Areas.Admin.Helpers
{
    /// <summary>
    /// Select list helper
    /// </summary>
    public static class SelectListHelper
    {
        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="categoryService">Category service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category list</returns>
        public static List<SelectListItem> GetCategoryList(ICategoryService categoryService, ICacheManager cacheManager, bool showHidden = false)
        {
            if (categoryService == null)
                throw new ArgumentNullException(nameof(categoryService));

            if (cacheManager == null)
                throw new ArgumentNullException(nameof(cacheManager));

            #region Extensions by QuanNH
            
            var storeId = 0;
            var _workContext = Nop.Core.Infrastructure.EngineContext.Current.Resolve<Core.IWorkContext>();
            var _storeMappingService = Nop.Core.Infrastructure.EngineContext.Current.Resolve<Nop.Services.Stores.IStoreMappingService>();
            var currentStoreId = _storeMappingService.GetStoreIdByEntityId(_workContext.CurrentCustomer.Id, "Stores").FirstOrDefault();
            if (currentStoreId > 0)
            {
                storeId = currentStoreId;
            }

            var categorys = categoryService.GetAllCategories(storeId: storeId, showHidden: true);

            var result = new List<SelectListItem>();
            foreach (var item in categorys)
            {
                result.Add(new SelectListItem
                {
                    Text = categoryService.GetFormattedBreadCrumb(item, categorys),
                    Value = item.Id.ToString()
                });
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Get manufacturer list
        /// </summary>
        /// <param name="manufacturerService">Manufacturer service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer list</returns>
        public static List<SelectListItem> GetManufacturerList(IManufacturerService manufacturerService, ICacheManager cacheManager, bool showHidden = false)
        {
            if (manufacturerService == null)
                throw new ArgumentNullException(nameof(manufacturerService));

            if (cacheManager == null)
                throw new ArgumentNullException(nameof(cacheManager));

            #region Extensions by QuanNH
            var result = new List<SelectListItem>();
            var manufacturers = manufacturerService.GetAllManufacturers(showHidden: showHidden);
            foreach (var item in manufacturers)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Get vendor list
        /// </summary>
        /// <param name="vendorService">Vendor service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendor list</returns>
        public static List<SelectListItem> GetVendorList(IVendorService vendorService, ICacheManager cacheManager, bool showHidden = false)
        {
            if (vendorService == null)
                throw new ArgumentNullException(nameof(vendorService));

            if (cacheManager == null)
                throw new ArgumentNullException(nameof(cacheManager));

            var cacheKey = string.Format(NopModelCacheDefaults.VendorsListKey, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {
                var vendors = vendorService.GetAllVendors(showHidden: showHidden);
                return vendors.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }
    }
}