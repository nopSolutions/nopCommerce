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
    /// Search log service
    /// </summary>
    public partial class SearchLogService : ISearchLogService
    {
        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        private readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public SearchLogService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get search term stats
        /// </summary>
        /// <param name="startTime">Start time; null to load all</param>
        /// <param name="endTime">End time; null to load all</param>
        /// <param name="count">Item count. 0 if you want to get all items</param>
        /// <returns>Result</returns>
        public List<SearchTermReportLine> SearchTermReport(DateTime? startTime,
            DateTime? endTime, int count)
        {
            var query = from st in _context.SearchLog
                        where (!startTime.HasValue || startTime.Value <= st.CreatedOn) &&
                        (!endTime.HasValue || endTime.Value >= st.CreatedOn)
                        group st by st.SearchTerm into grp
                        orderby grp.Count() descending
                        select new
                        {
                            SearchTerm = grp.Key,
                            SearchCount = grp.Count()
                        };
            if (count > 0)
                query = query.Take(count);
            var tmp1 = query.ToList();
            List<SearchTermReportLine> report = new List<SearchTermReportLine>();
            foreach (var r1 in tmp1)
            {
                report.Add(new SearchTermReportLine()
                    {
                        SearchTerm = r1.SearchTerm,
                        SearchCount = r1.SearchCount,
                    });
            }
            return report;
        }

        /// <summary>
        /// Gets all search log items
        /// </summary>
        /// <returns>Search log collection</returns>
        public List<SearchLog> GetAllSearchLogs()
        {
            var query = from s in _context.SearchLog
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
        public SearchLog GetSearchLogById(int searchLogId)
        {
            if (searchLogId == 0)
                return null;
                        
            var query = from s in _context.SearchLog
                        where s.SearchLogId == searchLogId
                        select s;
            var searchLog = query.SingleOrDefault();
            return searchLog;
        }

        /// <summary>
        /// Inserts a search log item
        /// </summary>
        /// <param name="searchLog">Search log item</param>
        public void InsertSearchLog(SearchLog searchLog)
        {
            if (searchLog == null)
                throw new ArgumentNullException("searchLog");

            searchLog.SearchTerm = CommonHelper.EnsureNotNull(searchLog.SearchTerm);
            searchLog.SearchTerm = CommonHelper.EnsureMaximumLength(searchLog.SearchTerm, 100);

            
            
            _context.SearchLog.AddObject(searchLog);
            _context.SaveChanges();
        }

        /// <summary>
        /// Clear search log
        /// </summary>
        public void ClearSearchLog()
        {
            
            var log = _context.SearchLog.ToList();
            foreach (var logItem in log)
                _context.DeleteObject(logItem);
            _context.SaveChanges();
        }
        #endregion
    }
}
