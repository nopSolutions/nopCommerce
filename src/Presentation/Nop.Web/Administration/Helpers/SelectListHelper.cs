using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Infrastructure.Cache;
using Nop.Core.Caching;
using Nop.Services.Catalog;

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
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORIES_LIST_KEY, showHidden);
            var categoryListItems = cacheManager.Get(cacheKey, () =>
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
            foreach (var item in categoryListItems)
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