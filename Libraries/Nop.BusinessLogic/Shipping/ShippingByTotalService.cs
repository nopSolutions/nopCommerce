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
    /// "Shipping by total" service
    /// </summary>
    public partial class ShippingByTotalService : IShippingByTotalService
    {
        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public ShippingByTotalService(NopObjectContext context)
        {
            _context = context;
            _cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Get a ShippingByTotal
        /// </summary>
        /// <param name="shippingByTotalId">ShippingByTotal identifier</param>
        /// <returns>ShippingByTotal</returns>
        public ShippingByTotal GetById(int shippingByTotalId)
        {
            if (shippingByTotalId == 0)
                return null;

            
            var query = from st in _context.ShippingByTotal
                        where st.ShippingByTotalId == shippingByTotalId
                        select st;
            var shippingByTotal = query.SingleOrDefault();
            return shippingByTotal;
        }

        /// <summary>
        /// Deletes a ShippingByTotal
        /// </summary>
        /// <param name="shippingByTotalId">ShippingByTotal identifier</param>
        public void DeleteShippingByTotal(int shippingByTotalId)
        {
            var shippingByTotal = GetById(shippingByTotalId);
            if (shippingByTotal == null)
                return;

            
            if (!_context.IsAttached(shippingByTotal))
                _context.ShippingByTotal.Attach(shippingByTotal);
            _context.DeleteObject(shippingByTotal);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all ShippingByTotals
        /// </summary>
        /// <returns>ShippingByTotal collection</returns>
        public List<ShippingByTotal> GetAll()
        {
            
            var query = from st in _context.ShippingByTotal
                        orderby st.ShippingMethodId, st.From
                        select st;
            var collection = query.ToList();
            return collection;
        }

        /// <summary>
        /// Inserts a ShippingByTotal
        /// </summary>
        /// <param name="shippingByTotal">ShippingByTotal</param>
        public void InsertShippingByTotal(ShippingByTotal shippingByTotal)
        {
            if (shippingByTotal == null)
                throw new ArgumentNullException("shippingByTotal");

            

            _context.ShippingByTotal.AddObject(shippingByTotal);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the ShippingByTotal
        /// </summary>
        /// <param name="shippingByTotal">ShippingByTotal</param>
        public void UpdateShippingByTotal(ShippingByTotal shippingByTotal)
        {
            if (shippingByTotal == null)
                throw new ArgumentNullException("shippingByTotal");

            
            if (!_context.IsAttached(shippingByTotal))
                _context.ShippingByTotal.Attach(shippingByTotal);

            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all ShippingByTotals by shipping method identifier
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <returns>ShippingByTotal collection</returns>
        public List<ShippingByTotal> GetAllByShippingMethodId(int shippingMethodId)
        {
            
            var query = from st in _context.ShippingByTotal
                        where st.ShippingMethodId == shippingMethodId
                        orderby st.From
                        select st;
            var collection = query.ToList();
            return collection;
        }
        #endregion
    }
}
