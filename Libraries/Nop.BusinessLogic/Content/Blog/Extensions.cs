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
using System.Linq;
using System.Text;


namespace NopSolutions.NopCommerce.BusinessLogic.Content.Blog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns all posts published between the two dates.
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="dateFrom">Date from</param>
        /// <param name="dateTo">Date to</param>
        /// <returns>Filtered posts</returns>
        public static List<BlogPost> GetPostsByDate(this List<BlogPost> source,
            DateTime dateFrom, DateTime dateTo)
        {
            var list = source.FindAll(delegate(BlogPost p)
            {
                return (dateFrom.Date <= p.CreatedOn && p.CreatedOn.Date <= dateTo);
            });

            list.TrimExcess();
            return list;
        }
    }
}
