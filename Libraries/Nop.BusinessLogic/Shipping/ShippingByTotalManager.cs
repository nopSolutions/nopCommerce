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

namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// "Shipping by total" manager
    /// </summary>
    public partial class ShippingByTotalManager
    {
        #region Methods
        /// <summary>
        /// Get a ShippingByTotal
        /// </summary>
        /// <param name="shippingByTotalId">ShippingByTotal identifier</param>
        /// <returns>ShippingByTotal</returns>
        public static ShippingByTotal GetById(int shippingByTotalId)
        {
            if (shippingByTotalId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from st in context.ShippingByTotal
                        where st.ShippingByTotalId == shippingByTotalId
                        select st;
            var shippingByTotal = query.SingleOrDefault();
            return shippingByTotal;
        }

        /// <summary>
        /// Deletes a ShippingByTotal
        /// </summary>
        /// <param name="shippingByTotalId">ShippingByTotal identifier</param>
        public static void DeleteShippingByTotal(int shippingByTotalId)
        {
            var shippingByTotal = GetById(shippingByTotalId);
            if (shippingByTotal == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(shippingByTotal))
                context.ShippingByTotal.Attach(shippingByTotal);
            context.DeleteObject(shippingByTotal);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets all ShippingByTotals
        /// </summary>
        /// <returns>ShippingByTotal collection</returns>
        public static List<ShippingByTotal> GetAll()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from st in context.ShippingByTotal
                        orderby st.ShippingMethodId, st.From
                        select st;
            var collection = query.ToList();
            return collection;
        }

        /// <summary>
        /// Inserts a ShippingByTotal
        /// </summary>
        /// <param name="shippingByTotal">ShippingByTotal</param>
        public static void InsertShippingByTotal(ShippingByTotal shippingByTotal)
        {
            if (shippingByTotal == null)
                throw new ArgumentNullException("shippingByTotal");

            var context = ObjectContextHelper.CurrentObjectContext;

            context.ShippingByTotal.AddObject(shippingByTotal);
            context.SaveChanges();
        }

        /// <summary>
        /// Updates the ShippingByTotal
        /// </summary>
        /// <param name="shippingByTotal">ShippingByTotal</param>
        public static void UpdateShippingByTotal(ShippingByTotal shippingByTotal)
        {
            if (shippingByTotal == null)
                throw new ArgumentNullException("shippingByTotal");

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(shippingByTotal))
                context.ShippingByTotal.Attach(shippingByTotal);

            context.SaveChanges();
        }

        /// <summary>
        /// Gets all ShippingByTotals by shipping method identifier
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <returns>ShippingByTotal collection</returns>
        public static List<ShippingByTotal> GetAllByShippingMethodId(int shippingMethodId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from st in context.ShippingByTotal
                        where st.ShippingMethodId == shippingMethodId
                        orderby st.From
                        select st;
            var collection = query.ToList();
            return collection;
        }
        #endregion
    }
}
