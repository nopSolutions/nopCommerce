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
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Shipping.Methods.ShippingByTotalCM
{
    /// <summary>
    /// Shipping by order total computation method
    /// </summary>
    public class ShippingByTotalComputationMethod : IShippingRateComputationMethod
    {
        #region Utilities

        /// <summary>
        /// Gets a shipping rate
        /// </summary>
        /// <param name="subTotal">Subtotal</param>
        /// <param name="ShippingMethodID">Shipping method identifier</param>
        /// <returns>Shipping rate</returns>
        protected decimal? GetRate(decimal subTotal, int ShippingMethodID)
        {
            decimal? shippingTotal = null;

            bool limitMethodsToCreated = SettingManager.GetSettingValueBoolean("ShippingByTotal.LimitMethodsToCreated");

            ShippingByTotal shippingByTotal = null;
            var shippingByTotalCollection = ShippingByTotalManager.GetAllByShippingMethodId(ShippingMethodID);
            foreach (ShippingByTotal shippingByTotal2 in shippingByTotalCollection)
            {
                if ((subTotal >= shippingByTotal2.From) && (subTotal <= shippingByTotal2.To))
                {
                    shippingByTotal = shippingByTotal2;
                    break;
                }
            }
            if (shippingByTotal == null)
            {
                if (limitMethodsToCreated)
                {
                    return null;
                }
                else
                {
                    return decimal.Zero;
                }
            }
            if (shippingByTotal.UsePercentage && shippingByTotal.ShippingChargePercentage <= decimal.Zero)
                return decimal.Zero;
            if (!shippingByTotal.UsePercentage && shippingByTotal.ShippingChargeAmount <= decimal.Zero)
                return decimal.Zero;
            if (shippingByTotal.UsePercentage)
                shippingTotal = Math.Round((decimal)((((float)subTotal) * ((float)shippingByTotal.ShippingChargePercentage)) / 100f), 2);
            else
                shippingTotal = shippingByTotal.ShippingChargeAmount;

            if (shippingTotal < decimal.Zero)
                shippingTotal = decimal.Zero;
            return shippingTotal;
        }
        #endregion

        #region Methods
        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="shipmentPackage">Shipment package</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping options</returns>
        public List<ShippingOption> GetShippingOptions(ShipmentPackage shipmentPackage, ref string error)
        {
            var shippingOptions = new List<ShippingOption>();

            if (shipmentPackage == null)
                throw new ArgumentNullException("shipmentPackage");
            if (shipmentPackage.Items == null)
                throw new NopException("No shipment items");
            if (shipmentPackage.ShippingAddress == null)
            {
                error = "Shipping address is not set";
                return shippingOptions;
            }
            if (shipmentPackage.ShippingAddress.Country == null)
            {
                error = "Shipping country is not set";
                return shippingOptions;
            }

            decimal subTotal = decimal.Zero;
            foreach (var shoppingCartItem in shipmentPackage.Items)
            {
                if (shoppingCartItem.IsFreeShipping)
                    continue;
                subTotal += PriceHelper.GetSubTotal(shoppingCartItem, shipmentPackage.Customer, true);
            }

            var shippingMethods = ShippingMethodManager.GetAllShippingMethods(shipmentPackage.ShippingAddress.CountryId);
            foreach (var shippingMethod in shippingMethods)
            {
                decimal? rate = GetRate(subTotal, shippingMethod.ShippingMethodId);
                if (rate.HasValue)
                {
                    var shippingOption = new ShippingOption();
                    shippingOption.Name = shippingMethod.Name;
                    shippingOption.Description = shippingMethod.Description;
                    shippingOption.Rate = rate.Value;
                    shippingOptions.Add(shippingOption);
                }
            }

            return shippingOptions;
        }
        
        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="shipmentPackage">Shipment package</param>
        /// <returns>Fixed shipping rate; or null if shipping rate could not be calculated before checkout</returns>
        public decimal? GetFixedRate(ShipmentPackage shipmentPackage)
        {
            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        /// <returns>A shipping rate computation method type</returns>
        public ShippingRateComputationMethodTypeEnum ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodTypeEnum.Offline;
            }
        }

        #endregion
    }
}
