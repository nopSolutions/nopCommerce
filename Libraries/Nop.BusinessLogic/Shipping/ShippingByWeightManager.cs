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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;

namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// "ShippingByWeight" manager
    /// </summary>
    public partial class ShippingByWeightManager
    {
        #region Methods
        /// <summary>
        /// Gets a ShippingByWeight
        /// </summary>
        /// <param name="shippingByWeightId">ShippingByWeight identifier</param>
        /// <returns>ShippingByWeight</returns>
        public static ShippingByWeight GetById(int shippingByWeightId)
        {
            if (shippingByWeightId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sw in context.ShippingByWeight
                        where sw.ShippingByWeightId == shippingByWeightId
                        select sw;
            var shippingByWeight = query.SingleOrDefault();
            return shippingByWeight;
        }

        /// <summary>
        /// Deletes a ShippingByWeight
        /// </summary>
        /// <param name="shippingByWeightId">ShippingByWeight identifier</param>
        public static void DeleteShippingByWeight(int shippingByWeightId)
        {
            var shippingByWeight = GetById(shippingByWeightId);
            if (shippingByWeight == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(shippingByWeight))
                context.ShippingByWeight.Attach(shippingByWeight);
            context.DeleteObject(shippingByWeight);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets all ShippingByWeights
        /// </summary>
        /// <returns>ShippingByWeight collection</returns>
        public static List<ShippingByWeight> GetAll()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sw in context.ShippingByWeight
                        orderby sw.ShippingMethodId, sw.From
                        select sw;
            var collection = query.ToList();
            return collection;
        }

        /// <summary>
        /// Inserts a ShippingByWeight
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="from">The "from" value</param>
        /// <param name="to">The "to" value</param>
        /// <param name="usePercentage">A value indicating whether to use percentage</param>
        /// <param name="shippingChargePercentage">The shipping charge percentage</param>
        /// <param name="shippingChargeAmount">The shipping charge amount</param>
        /// <returns>ShippingByWeight</returns>
        public static ShippingByWeight InsertShippingByWeight(int shippingMethodId,
            decimal from, decimal to, bool usePercentage,
            decimal shippingChargePercentage, decimal shippingChargeAmount)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var shippingByWeight = context.ShippingByWeight.CreateObject();
            shippingByWeight.ShippingMethodId = shippingMethodId;
            shippingByWeight.From = from;
            shippingByWeight.To = to;
            shippingByWeight.UsePercentage = usePercentage;
            shippingByWeight.ShippingChargePercentage = shippingChargePercentage;
            shippingByWeight.ShippingChargeAmount = shippingChargeAmount;

            context.ShippingByWeight.AddObject(shippingByWeight);
            context.SaveChanges();

            return shippingByWeight;
        }

        /// <summary>
        /// Updates the ShippingByWeight
        /// </summary>
        /// <param name="shippingByWeightId">The ShippingByWeight identifier</param>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="from">The "from" value</param>
        /// <param name="to">The "to" value</param>
        /// <param name="usePercentage">A value indicating whether to use percentage</param>
        /// <param name="shippingChargePercentage">The shipping charge percentage</param>
        /// <param name="shippingChargeAmount">The shipping charge amount</param>
        /// <returns>ShippingByWeight</returns>
        public static ShippingByWeight UpdateShippingByWeight(int shippingByWeightId,
            int shippingMethodId, decimal from, decimal to, bool usePercentage,
            decimal shippingChargePercentage, decimal shippingChargeAmount)
        {
            var shippingByWeight = GetById(shippingByWeightId);
            if (shippingByWeight == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(shippingByWeight))
                context.ShippingByWeight.Attach(shippingByWeight);

            shippingByWeight.ShippingMethodId = shippingMethodId;
            shippingByWeight.From = from;
            shippingByWeight.To = to;
            shippingByWeight.UsePercentage = usePercentage;
            shippingByWeight.ShippingChargePercentage = shippingChargePercentage;
            shippingByWeight.ShippingChargeAmount = shippingChargeAmount;
            context.SaveChanges();

            return shippingByWeight;
        }

        /// <summary>
        /// Gets all ShippingByWeights by shipping method identifier
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <returns>ShippingByWeight collection</returns>
        public static List<ShippingByWeight> GetAllByShippingMethodId(int shippingMethodId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sw in context.ShippingByWeight
                        where sw.ShippingMethodId == shippingMethodId
                        orderby sw.From
                        select sw;
            var collection = query.ToList();

            return collection;
        }
        #endregion

        #region Properties

         /// <summary>
        /// Gets or sets a value indicating whether to calculate per weight unit (e.g. per lb)
        /// </summary>
        public static bool CalculatePerWeightUnit
        {
            get
            {
                bool val1 = SettingManager.GetSettingValueBoolean("ShippingByWeight.CalculatePerWeightUnit");
                return val1;
            }
            set
            {
                SettingManager.SetParam("ShippingByWeight.CalculatePerWeightUnit", value.ToString());
            }
        }

        #endregion
    }
}
