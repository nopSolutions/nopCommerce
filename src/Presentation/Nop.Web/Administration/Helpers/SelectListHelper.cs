using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Infrastructure.Cache;
using Nop.Core.Caching;
using Nop.Services.Catalog;
using Nop.Services.Vendors;

namespace Nop.Admin.Helpers
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
                throw new ArgumentNullException("categoryService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORIES_LIST_KEY, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {
                var categories = categoryService.GetAllCategories(showHidden: showHidden);
                return categories.Select(c => new SelectListItem
                {
                    Text = c.GetFormattedBreadCrumb(categories),
                    Value = c.Id.ToString()
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
                throw new ArgumentNullException("manufacturerService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURERS_LIST_KEY, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {
                var manufacturers = manufacturerService.GetAllManufacturers(showHidden: showHidden);
                return manufacturers.Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
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
                throw new ArgumentNullException("vendorService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.VENDORS_LIST_KEY, showHidden);
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