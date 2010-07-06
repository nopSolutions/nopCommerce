//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Audit
{
    /// <summary>
    /// Search log manager
    /// </summary>
    public partial class SearchLogManager
    {
        #region Methods

        /// <summary>
        /// Get search term stats
        /// </summary>
        /// <param name="startTime">Start time; null to load all</param>
        /// <param name="endTime">End time; null to load all</param>
        /// <param name="count">Item count. 0 if you want to get all items</param>
        /// <returns>Result</returns>
        public static List<SearchTermReportLine> SearchTermReport(DateTime? startTime, 
            DateTime? endTime, int count)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var report = context.Sp_SearchTermReport(startTime,
                endTime, count);
            return report;
        }

        /// <summary>
        /// Gets all search log items
        /// </summary>
        /// <returns>Search log collection</returns>
        public static List<SearchLog> GetAllSearchLogs()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from s in context.SearchLog
                        orderby s.CreatedOn descending
                        select s;
            var searchLog = query.ToList();

            return searchLog;
        }

        /// <summary>
        /// Gets a search log item
        /// </summary>
        /// <param name="searchLogId">The search log item identifier</param>
        /// <returns>Search log item</returns>
        public static SearchLog GetSearchLogById(int searchLogId)
        {
            if (searchLogId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from s in context.SearchLog
                        where s.SearchLogId == searchLogId
                        select s;
            var searchLog = query.SingleOrDefault();
            return searchLog;
        }

        /// <summary>
        /// Inserts a search log item
        /// </summary>
        /// <param name="searchTerm">The search term</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>Search log item</returns>
        public static SearchLog InsertSearchLog(string searchTerm,
            int customerId, DateTime createdOn)
        {
            searchTerm = CommonHelper.EnsureMaximumLength(searchTerm, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var searchLog = context.SearchLog.CreateObject();
            searchLog.SearchTerm = searchTerm;
            searchLog.CustomerId = customerId;
            searchLog.CreatedOn = createdOn;

            context.SearchLog.AddObject(searchLog);
            context.SaveChanges();
            return searchLog;
        }

        /// <summary>
        /// Clear search log
        /// </summary>
        public static void ClearSearchLog()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            context.Sp_SearchLogClear();
        }
        #endregion
    }
}
