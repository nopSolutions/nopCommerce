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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common;


namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// Shipping manager
    /// </summary>
    public partial class ShippingManager
    {
        #region Utilities

        /// <summary>
        /// Gets a value indicating whether shipping is free
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>A value indicating whether shipping is free</returns>
        protected static bool IsFreeShipping(ShoppingCart cart, Customer customer)
        {
            if (customer != null)
            {
                //check whether customer is in a customer role with free shipping applied
                var customerRoles = customer.CustomerRoles;
                foreach (var customerRole in customerRoles)
                    if (customerRole.FreeShipping)
                        return true;
            }

            bool shoppingCartRequiresShipping = ShoppingCartRequiresShipping(cart);
            if (!shoppingCartRequiresShipping)
                return true;

            //check whether we have subtotal enough to have free shipping
            decimal subTotalBase = decimal.Zero;
            decimal orderSubTotalDiscountAmount = decimal.Zero;
            Discount orderSubTotalAppliedDiscount = null;
            decimal subTotalWithoutDiscountBase = decimal.Zero; 
            decimal subTotalWithDiscountBase = decimal.Zero;
            string SubTotalError = ShoppingCartManager.GetShoppingCartSubTotal(cart,
                customer, out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscount,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            subTotalBase = subTotalWithDiscountBase;
            if (SettingManager.GetSettingValueBoolean("Shipping.FreeShippingOverX.Enabled"))
            {
                decimal freeShippingOverX = SettingManager.GetSettingValueDecimalNative("Shipping.FreeShippingOverX.Value");
                if (subTotalBase > freeShippingOverX)
                    return true;
            }

            //check whether all shopping cart items are marked as free shipping
            bool allItemsAreFreeShipping = true;
            foreach (var sc in cart)
                if (sc.IsShipEnabled && !sc.IsFreeShipping)
                {
                    allItemsAreFreeShipping = false;
                    break;
                }
            if (allItemsAreFreeShipping)
                return true;

            //otherwise, return false
            return false;
        }

        /// <summary>
        /// Create shipment package from shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <returns>Shipment package</returns>
        protected static ShipmentPackage CreateShipmentPackage(ShoppingCart cart, 
            Customer customer, Address shippingAddress)
        {
            var shipmentPackage = new ShipmentPackage();
            shipmentPackage.Customer = customer;
            shipmentPackage.Items = new ShoppingCart();
            foreach (var sc in cart)
                if (sc.IsShipEnabled)
                    shipmentPackage.Items.Add(sc);
            shipmentPackage.ShippingAddress = shippingAddress;
            //TODO set values from warehouses or shipping origin
            shipmentPackage.CountryFrom = null;
            shipmentPackage.StateProvinceFrom = null;
            shipmentPackage.ZipPostalCodeFrom = string.Empty;
            return shipmentPackage;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>Shopping cart weight</returns>
        public static decimal GetShoppingCartTotalWeight(ShoppingCart cart, Customer customer)
        {
            decimal totalWeight = decimal.Zero;
            //shopping cart items
            foreach (var shoppingCartItem in cart)
                totalWeight += shoppingCartItem.TotalWeight;

            //checkout attributes
            if (customer != null)
            {
                var caValues = CheckoutAttributeHelper.ParseCheckoutAttributeValues(customer.CheckoutAttributes);
                foreach (var caValue in caValues)
                    totalWeight += caValue.WeightAdjustment;
            }
            return totalWeight;
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Shipping total</returns>
        public static decimal? GetShoppingCartShippingTotal(ShoppingCart cart)
        {
            string error = string.Empty;
            return GetShoppingCartShippingTotal(cart, ref error);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping total</returns>
        public static decimal? GetShoppingCartShippingTotal(ShoppingCart cart, ref string error)
        {
            Customer customer = NopContext.Current.User;
            return GetShoppingCartShippingTotal(cart, customer, ref error);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>Shipping total</returns>
        public static decimal? GetShoppingCartShippingTotal(ShoppingCart cart, Customer customer)
        {
            string error = string.Empty;
            return GetShoppingCartShippingTotal(cart, customer, ref error);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping total</returns>
        public static decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
            Customer customer, ref string error)
        {
            bool includingTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetShoppingCartShippingTotal(cart, customer, includingTax, ref error);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <returns>Shipping total</returns>
        public static decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
            Customer customer, bool includingTax)
        {
            string error = string.Empty;
            return GetShoppingCartShippingTotal(cart, customer, includingTax, ref error);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping total</returns>
        public static decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
            Customer customer, bool includingTax, ref string error)
        {
            decimal taxRate = decimal.Zero;
            return GetShoppingCartShippingTotal(cart, customer,
                includingTax, out taxRate, ref error);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping total</returns>
        public static decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
            Customer customer, bool includingTax, out decimal taxRate, ref string error)
        {
            Discount appliedDiscount = null;
            return GetShoppingCartShippingTotal(cart, customer, 
                includingTax, out taxRate, out appliedDiscount, ref error);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping total</returns>
        public static decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
            Customer customer, bool includingTax, out decimal taxRate, 
            out Discount appliedDiscount, ref string error)
        {
            decimal? shippingTotalWithoutDiscount = null;
            decimal? shippingTotalWithDiscount = null;
            decimal? shippingTotalWithDiscountTaxed = null;
            appliedDiscount = null;
            taxRate = decimal.Zero;

            bool isFreeShipping = IsFreeShipping(cart, customer);
            if (isFreeShipping)
                return decimal.Zero;

            ShippingOption lastShippingOption = null;
            if (customer != null)
            {
                lastShippingOption = customer.LastShippingOption;
            }

            if (lastShippingOption != null)
            {
                //use last shipping option (get from cache)
                //we have already discounted cache value
                shippingTotalWithoutDiscount = lastShippingOption.Rate;

                //discount
                decimal discountAmount = GetShippingDiscount(customer, 
                    shippingTotalWithoutDiscount.Value, out appliedDiscount);
                shippingTotalWithDiscount = shippingTotalWithoutDiscount - discountAmount;
                if (shippingTotalWithDiscount < decimal.Zero)
                    shippingTotalWithDiscount = decimal.Zero;
                shippingTotalWithDiscount = Math.Round(shippingTotalWithDiscount.Value, 2);
            }
            else
            {
                //use fixed rate (if possible)
                Address shippingAddress = null;
                if (customer != null)
                {
                    shippingAddress = customer.ShippingAddress;
                }
                var ShipmentPackage = CreateShipmentPackage(cart, customer, shippingAddress);
                var shippingRateComputationMethods = ShippingRateComputationMethodManager.GetAllShippingRateComputationMethods(false);
                if (shippingRateComputationMethods.Count == 0)
                    throw new NopException("Shipping rate computation method could not be loaded");

                if (shippingRateComputationMethods.Count == 1)
                {
                    var shippingRateComputationMethod = shippingRateComputationMethods[0];
                    var iShippingRateComputationMethod = Activator.CreateInstance(Type.GetType(shippingRateComputationMethod.ClassName)) as IShippingRateComputationMethod;

                    decimal? fixedRate = iShippingRateComputationMethod.GetFixedRate(ShipmentPackage);
                    if (fixedRate.HasValue)
                    {
                        decimal additionalShippingCharge = GetShoppingCartAdditionalShippingCharge(cart, customer);
                        shippingTotalWithoutDiscount = fixedRate.Value + additionalShippingCharge;
                        shippingTotalWithoutDiscount = Math.Round(shippingTotalWithoutDiscount.Value, 2);
                        decimal shippingTotalDiscount = GetShippingDiscount(customer, shippingTotalWithoutDiscount.Value, out appliedDiscount);
                        shippingTotalWithDiscount = shippingTotalWithoutDiscount.Value - shippingTotalDiscount;
                        if (shippingTotalWithDiscount.Value < decimal.Zero)
                            shippingTotalWithDiscount = decimal.Zero;
                    }
                }
            }

            if (!shippingTotalWithDiscount.HasValue)
            {
                error = "Shipping total could not be calculated";
            }
            else
            {
                shippingTotalWithDiscountTaxed = TaxManager.GetShippingPrice(shippingTotalWithDiscount.Value,
                    includingTax,
                    customer,
                    out taxRate,
                    ref error);

                shippingTotalWithDiscountTaxed = Math.Round(shippingTotalWithDiscountTaxed.Value, 2);
            }

            return shippingTotalWithDiscountTaxed;
        }

        /// <summary>
        /// Gets a shipping discount
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shippingTotal">Shipping total</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Shipping discount</returns>
        public static decimal GetShippingDiscount(Customer customer, 
            decimal shippingTotal, out Discount appliedDiscount)
        {
            decimal shippingDiscountAmount = decimal.Zero;

            string customerCouponCode = string.Empty;
            if (customer != null)
                customerCouponCode = customer.LastAppliedCouponCode;

            var allDiscounts = DiscountManager.GetAllDiscounts(DiscountTypeEnum.AssignedToShipping);
            var allowedDiscounts = new List<Discount>();
            foreach (var _discount in allDiscounts)
            {
                if (_discount.IsActive(customerCouponCode) &&
                    _discount.DiscountType == DiscountTypeEnum.AssignedToShipping &&
                    !allowedDiscounts.ContainsDiscount(_discount.Name))
                {
                    //discount requirements
                    if (_discount.CheckDiscountRequirements(customer)
                        && _discount.CheckDiscountLimitations(customer))
                    {
                        allowedDiscounts.Add(_discount);
                    }
                }
            }

            appliedDiscount = DiscountManager.GetPreferredDiscount(allowedDiscounts, shippingTotal);
            if (appliedDiscount != null)
            {
                shippingDiscountAmount = appliedDiscount.GetDiscountAmount(shippingTotal);
            }

            if (shippingDiscountAmount < decimal.Zero)
                shippingDiscountAmount = decimal.Zero;

            shippingDiscountAmount = Math.Round(shippingDiscountAmount, 2);

            return shippingDiscountAmount;
        }
        
        /// <summary>
        /// Indicates whether the shopping cart requires shipping
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>True if the shopping cart requires shipping; otherwise, false.</returns>
        public static bool ShoppingCartRequiresShipping(ShoppingCart cart)
        {
            foreach (var shoppingCartItem in cart)
                if (shoppingCartItem.IsShipEnabled)
                    return true;
            return false;
        }

        /// <summary>
        /// Gets shopping cart additional shipping charge
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>Additional shipping charge</returns>
        public static decimal GetShoppingCartAdditionalShippingCharge(ShoppingCart cart, Customer customer)
        {
            decimal additionalShippingCharge = decimal.Zero;

            bool isFreeShipping = IsFreeShipping(cart, customer);
            if (isFreeShipping)
                return decimal.Zero;

            foreach (var shoppingCartItem in cart)
                additionalShippingCharge += shoppingCartItem.AdditionalShippingCharge;

            return additionalShippingCharge;
        }

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping options</returns>
        public static List<ShippingOption> GetShippingOptions(ShoppingCart cart, 
            Customer customer, Address shippingAddress, ref string error)
        {
            return GetShippingOptions(cart, customer, shippingAddress, null, ref error);
        }

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="allowedShippingRateComputationMethodId">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping options</returns>
        public static List<ShippingOption> GetShippingOptions(ShoppingCart cart,
            Customer customer, Address shippingAddress, 
            int? allowedShippingRateComputationMethodId, ref string error)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            var shippingOptions = new List<ShippingOption>();

            bool isFreeShipping = IsFreeShipping(cart, customer);

            //create a package
            var shipmentPackage = CreateShipmentPackage(cart, customer, shippingAddress);
            var shippingRateComputationMethods = ShippingRateComputationMethodManager.GetAllShippingRateComputationMethods(false);
            if (shippingRateComputationMethods.Count == 0)
                throw new NopException("Shipping rate computation method could not be loaded");

            //get shipping options
            foreach (var srcm in shippingRateComputationMethods)
            {
                if (allowedShippingRateComputationMethodId.HasValue &&
                    allowedShippingRateComputationMethodId.Value > 0 &&
                    allowedShippingRateComputationMethodId.Value != srcm.ShippingRateComputationMethodId)
                    continue;

                var iShippingRateComputationMethod = Activator.CreateInstance(Type.GetType(srcm.ClassName)) as IShippingRateComputationMethod;

                var shippingOptions2 = iShippingRateComputationMethod.GetShippingOptions(shipmentPackage, ref error);
                if (shippingOptions2 != null)
                {
                    foreach (var so2 in shippingOptions2)
                    {
                        so2.ShippingRateComputationMethodId = srcm.ShippingRateComputationMethodId;
                        shippingOptions.Add(so2);
                    }
                }
            }

            //no shipping options loaded
            if (shippingOptions.Count == 0 && String.IsNullOrEmpty(error))
            {
                error = "Shipping options could not be loaded";
            }

            //additional shipping charges
            decimal additionalShippingCharge = GetShoppingCartAdditionalShippingCharge(cart, customer);
            shippingOptions.ForEach(so => so.Rate += additionalShippingCharge);
            
            //free shipping
            if (isFreeShipping)
            {
                shippingOptions.ForEach(so => so.Rate = decimal.Zero);
            }

            return shippingOptions;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets a default shipping origin address
        /// </summary>
        public static Address ShippingOrigin
        {
            get
            {
                int countryId = SettingManager.GetSettingValueInteger("Shipping.ShippingOrigin.CountryId");
                int stateProvinceId = SettingManager.GetSettingValueInteger("Shipping.ShippingOrigin.StateProvinceId");
                string zipPostalCode = SettingManager.GetSettingValue("Shipping.ShippingOrigin.ZipPostalCode");
                var address = new Address();
                address.CountryId = countryId;
                address.StateProvinceId = stateProvinceId;
                address.ZipPostalCode = zipPostalCode;
                return address;
            }
            set
            {
                int countryId = 0;
                int stateProvinceId = 0;
                string zipPostalCode = string.Empty;

                if (value != null)
                {
                    countryId = value.CountryId;
                    stateProvinceId = value.StateProvinceId;
                    zipPostalCode = value.ZipPostalCode;
                }

                SettingManager.SetParam("Shipping.ShippingOrigin.CountryId", countryId.ToString());
                SettingManager.SetParam("Shipping.ShippingOrigin.StateProvinceId", stateProvinceId.ToString());
                SettingManager.SetParam("Shipping.ShippingOrigin.ZipPostalCode", zipPostalCode);
            }
        }
        #endregion
    }
}
