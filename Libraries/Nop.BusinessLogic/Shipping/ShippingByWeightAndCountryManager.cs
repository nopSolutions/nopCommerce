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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// "ShippingByWeightAndCountry" manager
    /// </summary>
    public partial class ShippingByWeightAndCountryManager : IShippingByWeightAndCountryManager
    {
        #region Methods

        /// <summary>
        /// Gets a ShippingByWeightAndCountry
        /// </summary>
        /// <param name="shippingByWeightAndCountryId">ShippingByWeightAndCountry identifier</param>
        /// <returns>ShippingByWeightAndCountry</returns>
        public ShippingByWeightAndCountry GetById(int shippingByWeightAndCountryId)
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
        public void DeleteShippingByWeightAndCountry(int shippingByWeightAndCountryId)
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
        public List<ShippingByWeightAndCountry> GetAll()
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
        /// <param name="shippingByWeightAndCountry">ShippingByWeightAndCountry</param>
        public void InsertShippingByWeightAndCountry(ShippingByWeightAndCountry shippingByWeightAndCountry)
        {
            if (shippingByWeightAndCountry == null)
                throw new ArgumentNullException("shippingByWeightAndCountry");

            var context = ObjectContextHelper.CurrentObjectContext;
            
            context.ShippingByWeightAndCountry.AddObject(shippingByWeightAndCountry);
            context.SaveChanges();
        }

        /// <summary>
        /// Updates the ShippingByWeightAndCountry
        /// </summary>
        /// <param name="shippingByWeightAndCountry">ShippingByWeightAndCountry</param>
        public void UpdateShippingByWeightAndCountry(ShippingByWeightAndCountry shippingByWeightAndCountry)
        {
            if (shippingByWeightAndCountry == null)
                throw new ArgumentNullException("shippingByWeightAndCountry");

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(shippingByWeightAndCountry))
                context.ShippingByWeightAndCountry.Attach(shippingByWeightAndCountry);

            context.SaveChanges();
        }

        /// <summary>
        /// Gets all ShippingByWeightAndCountrys by shipping method identifier
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <returns>ShippingByWeightAndCountry collection</returns>
        public List<ShippingByWeightAndCountry> GetAllByShippingMethodIdAndCountryId(int shippingMethodId, 
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
        public bool CalculatePerWeightUnit
        {
            get
            {
                bool val1 = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("ShippingByWeightAndCountry.CalculatePerWeightUnit");
                return val1;
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("ShippingByWeightAndCountry.CalculatePerWeightUnit", value.ToString());
            }
        }

        #endregion
    }
}
