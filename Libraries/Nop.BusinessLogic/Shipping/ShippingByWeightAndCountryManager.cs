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
    /// "ShippingByWeightAndCountry" manager
    /// </summary>
    public partial class ShippingByWeightAndCountryManager
    {
        #region Methods
        /// <summary>
        /// Gets a ShippingByWeightAndCountry
        /// </summary>
        /// <param name="shippingByWeightAndCountryId">ShippingByWeightAndCountry identifier</param>
        /// <returns>ShippingByWeightAndCountry</returns>
        public static ShippingByWeightAndCountry GetById(int shippingByWeightAndCountryId)
        {
            if (shippingByWeightAndCountryId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from swc in context.ShippingByWeightAndCountry
                        where swc.ShippingByWeightAndCountryId == shippingByWeightAndCountryId
                        select swc;
            var shippingByWeightAndCountry = query.SingleOrDefault();
            return shippingByWeightAndCountry;
        }

        /// <summary>
        /// Deletes a ShippingByWeightAndCountry
        /// </summary>
        /// <param name="shippingByWeightAndCountryId">ShippingByWeightAndCountry identifier</param>
        public static void DeleteShippingByWeightAndCountry(int shippingByWeightAndCountryId)
        {
            var shippingByWeightAndCountry = GetById(shippingByWeightAndCountryId);
            if (shippingByWeightAndCountry == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(shippingByWeightAndCountry))
                context.ShippingByWeightAndCountry.Attach(shippingByWeightAndCountry);
            context.DeleteObject(shippingByWeightAndCountry);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets all ShippingByWeightAndCountrys
        /// </summary>
        /// <returns>ShippingByWeightAndCountry collection</returns>
        public static List<ShippingByWeightAndCountry> GetAll()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from swc in context.ShippingByWeightAndCountry
                        orderby swc.CountryId, swc.ShippingMethodId, swc.From
                        select swc;
            var collection = query.ToList();
            return collection;
        }

        /// <summary>
        /// Inserts a ShippingByWeightAndCountry
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <param name="from">The "from" value</param>
        /// <param name="to">The "to" value</param>
        /// <param name="usePercentage">A value indicating whether to use percentage</param>
        /// <param name="shippingChargePercentage">The shipping charge percentage</param>
        /// <param name="shippingChargeAmount">The shipping charge amount</param>
        /// <returns>ShippingByWeightAndCountry</returns>
        public static ShippingByWeightAndCountry InsertShippingByWeightAndCountry(int shippingMethodId,
            int countryId, decimal from, decimal to, bool usePercentage,
            decimal shippingChargePercentage, decimal shippingChargeAmount)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var shippingByWeightAndCountry = context.ShippingByWeightAndCountry.CreateObject();
            shippingByWeightAndCountry.ShippingMethodId = shippingMethodId;
            shippingByWeightAndCountry.CountryId = countryId;
            shippingByWeightAndCountry.From = from;
            shippingByWeightAndCountry.To = to;
            shippingByWeightAndCountry.UsePercentage = usePercentage;
            shippingByWeightAndCountry.ShippingChargePercentage = shippingChargePercentage;
            shippingByWeightAndCountry.ShippingChargeAmount = shippingChargeAmount;

            context.ShippingByWeightAndCountry.AddObject(shippingByWeightAndCountry);
            context.SaveChanges();

            return shippingByWeightAndCountry;
        }

        /// <summary>
        /// Updates the ShippingByWeightAndCountry
        /// </summary>
        /// <param name="shippingByWeightAndCountryId">The ShippingByWeightAndCountry identifier</param>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <param name="from">The "from" value</param>
        /// <param name="to">The "to" value</param>
        /// <param name="usePercentage">A value indicating whether to use percentage</param>
        /// <param name="shippingChargePercentage">The shipping charge percentage</param>
        /// <param name="shippingChargeAmount">The shipping charge amount</param>
        /// <returns>ShippingByWeightAndCountry</returns>
        public static ShippingByWeightAndCountry UpdateShippingByWeightAndCountry(int shippingByWeightAndCountryId,
            int shippingMethodId, int countryId, decimal from, decimal to, bool usePercentage,
            decimal shippingChargePercentage, decimal shippingChargeAmount)
        {
            var shippingByWeightAndCountry = GetById(shippingByWeightAndCountryId);
            if (shippingByWeightAndCountry == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(shippingByWeightAndCountry))
                context.ShippingByWeightAndCountry.Attach(shippingByWeightAndCountry);

            shippingByWeightAndCountry.ShippingMethodId = shippingMethodId;
            shippingByWeightAndCountry.CountryId = countryId;
            shippingByWeightAndCountry.From = from;
            shippingByWeightAndCountry.To = to;
            shippingByWeightAndCountry.UsePercentage = usePercentage;
            shippingByWeightAndCountry.ShippingChargePercentage = shippingChargePercentage;
            shippingByWeightAndCountry.ShippingChargeAmount = shippingChargeAmount;
            context.SaveChanges();

            return shippingByWeightAndCountry;
        }

        /// <summary>
        /// Gets all ShippingByWeightAndCountrys by shipping method identifier
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <returns>ShippingByWeightAndCountry collection</returns>
        public static List<ShippingByWeightAndCountry> GetAllByShippingMethodIdAndCountryId(int shippingMethodId, 
            int countryId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from swc in context.ShippingByWeightAndCountry
                        where swc.ShippingMethodId == shippingMethodId && swc.CountryId == countryId
                        orderby swc.From
                        select swc;
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
                bool val1 = SettingManager.GetSettingValueBoolean("ShippingByWeightAndCountry.CalculatePerWeightUnit");
                return val1;
            }
            set
            {
                SettingManager.SetParam("ShippingByWeightAndCountry.CalculatePerWeightUnit", value.ToString());
            }
        }

        #endregion
    }
}
