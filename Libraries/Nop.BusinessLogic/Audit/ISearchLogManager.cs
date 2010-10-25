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
    /// Search log manager interface
    /// </summary>
    public partial interface ISearchLogManager
    {
        /// <summary>
        /// Get search term stats
        /// </summary>
        /// <param name="startTime">Start time; null to load all</param>
        /// <param name="endTime">End time; null to load all</param>
        /// <param name="count">Item count. 0 if you want to get all items</param>
        /// <returns>Result</returns>
        List<SearchTermReportLine> SearchTermReport(DateTime? startTime,
            DateTime? endTime, int count);

        /// <summary>
        /// Gets all search log items
        /// </summary>
        /// <returns>Search log collection</returns>
        List<SearchLog> GetAllSearchLogs();

        /// <summary>
        /// Gets a search log item
        /// </summary>
        /// <param name="searchLogId">The search log item identifier</param>
        /// <returns>Search log item</returns>
        SearchLog GetSearchLogById(int searchLogId);

        /// <summary>
        /// Inserts a search log item
        /// </summary>
        /// <param name="searchLog">Search log item</param>
        void InsertSearchLog(SearchLog searchLog);

        /// <summary>
        /// Clear search log
        /// </summary>
        void ClearSearchLog();
    }
}
