using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Humanizer;
using Humanizer.Localisation;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Extensions
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// In-memory paging of entities (models)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="current">Entities (models)</param>
        /// <param name="command">Command (paging details)</param>
        /// <returns>Paged entities (models)</returns>
        public static IEnumerable<T> PagedForCommand<T>(this IEnumerable<T> current, DataSourceRequest command)
        {
            return current.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize);
        }

        /// <summary>
        /// In-memory paging of objects
        /// </summary>
        /// <typeparam name="T">Type of objects</typeparam>
        /// <param name="collection">Object collection</param>
        /// <param name="requestModel">Paging request model</param>
        /// <returns>Paged collection of objects</returns>
        public static IEnumerable<T> PaginationByRequestModel<T>(this IEnumerable<T> collection, IPagingRequestModel requestModel)
        {
            return collection.Skip((requestModel.Page - 1) * requestModel.PageSize).Take(requestModel.PageSize);
        }

        /// <summary>
        /// Returns a value indicating whether real selection is not possible
        /// </summary>
        /// <param name="items">Items</param>
        /// <param name="ignoreZeroValue">A value indicating whether we should ignore items with "0" value</param>
        /// <returns>A value indicating whether real selection is not possible</returns>
        public static bool SelectionIsNotPossible(this IList<SelectListItem> items, bool ignoreZeroValue = true)
        {
            if (items == null)
                throw  new ArgumentNullException(nameof(items));

            //we ignore items with "0" value? Usually it's something like "Select All", "etc
            return items.Count(x => !ignoreZeroValue || !x.Value.ToString().Equals("0")) < 2;
        }

        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <param name="languageCode">Language culture code</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source,
            string languageCode = "en-US")
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - source.Ticks);
            var delta = ts.TotalSeconds;

            CultureInfo culture = null;
            try
            {
                culture = new CultureInfo(languageCode);
            }
            catch (CultureNotFoundException)
            {
                culture = new CultureInfo("en-US");
            }
            return TimeSpan.FromSeconds(delta).Humanize(precision: 1, culture: culture, maxUnit: TimeUnit.Year);
        }
    }
}
