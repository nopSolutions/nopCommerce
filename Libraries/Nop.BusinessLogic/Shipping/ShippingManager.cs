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
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;


namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// Shipping manager
    /// </summary>
    public partial class ShippingManager : IShippingManager
    {
        #region Constants

        private const string SHIPPINGMETHODS_BY_ID_KEY = "Nop.shippingMethod.id-{0}";
        private const string SHIPPINGMETHODS_PATTERN_KEY = "Nop.shippingMethod.";
        
        private const string SHIPPINGTATUSES_ALL_KEY = "Nop.shippingstatus.all";
        private const string SHIPPINGTATUSES_BY_ID_KEY = "Nop.shippingstatus.id-{0}";
        private const string SHIPPINGTATUSES_PATTERN_KEY = "Nop.shippingstatus.";
        
        private const string SHIPPINGRATECOMPUTATIONMETHODS_ALL_KEY = "Nop.shippingratecomputationmethod.all-{0}";
        private const string SHIPPINGRATECOMPUTATIONMETHODS_BY_ID_KEY = "Nop.shippingratecomputationmethod.id-{0}";
        private const string SHIPPINGRATECOMPUTATIONMETHODS_PATTERN_KEY = "Nop.shippingratecomputationmethod.";

        #endregion

        #region Fields

        /// <summary>
        /// object context
        /// </summary>
        protected NopObjectContext _context;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public ShippingManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a value indicating whether shipping is free
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>A value indicating whether shipping is free</returns>
        protected bool IsFreeShipping(ShoppingCart cart, Customer customer)
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
            string SubTotalError = IoCFactory.Resolve<IShoppingCartManager>().GetShoppingCartSubTotal(cart,
                customer, out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscount,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            subTotalBase = subTotalWithDiscountBase;
            if (IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Shipping.FreeShippingOverX.Enabled"))
            {
                decimal freeShippingOverX = IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("Shipping.FreeShippingOverX.Value");
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
        protected ShipmentPackage CreateShipmentPackage(ShoppingCart cart, 
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

        #region Shipping rate computation methods
        
        /// <summary>
        /// Deletes a shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethodId">Shipping rate computation method identifier</param>
        public void DeleteShippingRateComputationMethod(int shippingRateComputationMethodId)
        {
            var shippingRateComputationMethod = GetShippingRateComputationMethodById(shippingRateComputationMethodId);
            if (shippingRateComputationMethod == null)
                return;

            
            if (!_context.IsAttached(shippingRateComputationMethod))
                _context.ShippingRateComputationMethods.Attach(shippingRateComputationMethod);
            _context.DeleteObject(shippingRateComputationMethod);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGRATECOMPUTATIONMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethodId">Shipping rate computation method identifier</param>
        /// <returns>Shipping rate computation method</returns>
        public ShippingRateComputationMethod GetShippingRateComputationMethodById(int shippingRateComputationMethodId)
        {
            if (shippingRateComputationMethodId == 0)
                return null;

            string key = string.Format(SHIPPINGRATECOMPUTATIONMETHODS_BY_ID_KEY, shippingRateComputationMethodId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ShippingRateComputationMethod)obj2;
            }

            
            var query = from s in _context.ShippingRateComputationMethods
                        where s.ShippingRateComputationMethodId == shippingRateComputationMethodId
                        select s;
            var shippingRateComputationMethod = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, shippingRateComputationMethod);
            }
            return shippingRateComputationMethod;
        }

        /// <summary>
        /// Gets all shipping rate computation methods
        /// </summary>
        /// <returns>Shipping rate computation method collection</returns>
        public List<ShippingRateComputationMethod> GetAllShippingRateComputationMethods()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllShippingRateComputationMethods(showHidden);
        }

        /// <summary>
        /// Gets all shipping rate computation methods
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Shipping rate computation method collection</returns>
        public List<ShippingRateComputationMethod> GetAllShippingRateComputationMethods(bool showHidden)
        {
            string key = string.Format(SHIPPINGRATECOMPUTATIONMETHODS_ALL_KEY, showHidden);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<ShippingRateComputationMethod>)obj2;
            }

            
            var query = from s in _context.ShippingRateComputationMethods
                        orderby s.DisplayOrder
                        where showHidden || s.IsActive
                        select s;
            var shippingRateComputationMethods = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, shippingRateComputationMethods);
            }
            return shippingRateComputationMethods;
        }

        /// <summary>
        /// Inserts a shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethod">Shipping rate computation method</param>
        public void InsertShippingRateComputationMethod(ShippingRateComputationMethod shippingRateComputationMethod)
        {
            if (shippingRateComputationMethod == null)
                throw new ArgumentNullException("shippingRateComputationMethod");
                        
            shippingRateComputationMethod.Name = CommonHelper.EnsureNotNull(shippingRateComputationMethod.Name);
            shippingRateComputationMethod.Name = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.Name, 100);
            shippingRateComputationMethod.Description = CommonHelper.EnsureNotNull(shippingRateComputationMethod.Description);
            shippingRateComputationMethod.Description = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.Description, 4000);
            shippingRateComputationMethod.ConfigureTemplatePath = CommonHelper.EnsureNotNull(shippingRateComputationMethod.ConfigureTemplatePath);
            shippingRateComputationMethod.ConfigureTemplatePath = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.ConfigureTemplatePath, 500);
            shippingRateComputationMethod.ClassName = CommonHelper.EnsureNotNull(shippingRateComputationMethod.ClassName);
            shippingRateComputationMethod.ClassName = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.ClassName, 500);

            

            _context.ShippingRateComputationMethods.AddObject(shippingRateComputationMethod);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGRATECOMPUTATIONMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethod">Shipping rate computation method</param>
        public void UpdateShippingRateComputationMethod(ShippingRateComputationMethod shippingRateComputationMethod)
        {
            if (shippingRateComputationMethod == null)
                throw new ArgumentNullException("shippingRateComputationMethod");

            shippingRateComputationMethod.Name = CommonHelper.EnsureNotNull(shippingRateComputationMethod.Name);
            shippingRateComputationMethod.Name = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.Name, 100);
            shippingRateComputationMethod.Description = CommonHelper.EnsureNotNull(shippingRateComputationMethod.Description);
            shippingRateComputationMethod.Description = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.Description, 4000);
            shippingRateComputationMethod.ConfigureTemplatePath = CommonHelper.EnsureNotNull(shippingRateComputationMethod.ConfigureTemplatePath);
            shippingRateComputationMethod.ConfigureTemplatePath = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.ConfigureTemplatePath, 500);
            shippingRateComputationMethod.ClassName = CommonHelper.EnsureNotNull(shippingRateComputationMethod.ClassName);
            shippingRateComputationMethod.ClassName = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.ClassName, 500);

            
            if (!_context.IsAttached(shippingRateComputationMethod))
                _context.ShippingRateComputationMethods.Attach(shippingRateComputationMethod);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGRATECOMPUTATIONMETHODS_PATTERN_KEY);
            }
        }
        
        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        /// <param name="shippingRateComputationMethodId">The shipping rate computation method identifier</param>
        /// <returns>A shipping rate computation method type</returns>
        public ShippingRateComputationMethodTypeEnum GetShippingRateComputationMethodTypeEnum(int shippingRateComputationMethodId)
        {
            var method = GetShippingRateComputationMethodById(shippingRateComputationMethodId);
            if (method == null)
                return ShippingRateComputationMethodTypeEnum.Unknown;
            var iMethod = Activator.CreateInstance(Type.GetType(method.ClassName)) as IShippingRateComputationMethod;
            return iMethod.ShippingRateComputationMethodType;
        }
        
        #endregion

        #region Shipping statuses
        
        /// <summary>
        /// Gets a shipping status full name
        /// </summary>
        /// <param name="shippingStatusId">Shipping status identifier</param>
        /// <returns>Shipping status name</returns>
        public string GetShippingStatusName(int shippingStatusId)
        {
            var shippingStatus = GetShippingStatusById(shippingStatusId);
            if (shippingStatus != null)
            {
                string name = string.Empty;
                if (NopContext.Current != null)
                {
                    name = LocalizationManager.GetLocaleResourceString(string.Format("ShippingStatus.{0}", (ShippingStatusEnum)shippingStatus.ShippingStatusId), NopContext.Current.WorkingLanguage.LanguageId, true, shippingStatus.Name);
                }
                else
                {
                    name = shippingStatus.Name;
                }
                return name;
            }
            else
            {
                return ((ShippingStatusEnum)shippingStatusId).ToString();
            }
        }

        /// <summary>
        /// Gets a shipping status by identifier
        /// </summary>
        /// <param name="shippingStatusId">Shipping status identifier</param>
        /// <returns>Shipping status</returns>
        public ShippingStatus GetShippingStatusById(int shippingStatusId)
        {
            if (shippingStatusId == 0)
                return null;

            string key = string.Format(SHIPPINGTATUSES_BY_ID_KEY, shippingStatusId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ShippingStatus)obj2;
            }

            
            var query = from ss in _context.ShippingStatuses
                        where ss.ShippingStatusId == shippingStatusId
                        select ss;
            var shippingStatus = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, shippingStatus);
            }
            return shippingStatus;
        }

        /// <summary>
        /// Gets all shipping statuses
        /// </summary>
        /// <returns>Shipping status collection</returns>
        public List<ShippingStatus> GetAllShippingStatuses()
        {
            string key = string.Format(SHIPPINGTATUSES_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<ShippingStatus>)obj2;
            }

            
            var query = from ss in _context.ShippingStatuses
                        orderby ss.ShippingStatusId
                        select ss;
            var shippingStatuses = query.ToList();
            
            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, shippingStatuses);
            }
            return shippingStatuses;
        }

        #endregion

        #region Shipping methods

        /// <summary>
        /// Deletes a shipping method
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        public void DeleteShippingMethod(int shippingMethodId)
        {
            var shippingMethod = GetShippingMethodById(shippingMethodId);
            if (shippingMethod == null)
                return;


            if (!_context.IsAttached(shippingMethod))
                _context.ShippingMethods.Attach(shippingMethod);
            _context.DeleteObject(shippingMethod);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a shipping method
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <returns>Shipping method</returns>
        public ShippingMethod GetShippingMethodById(int shippingMethodId)
        {
            if (shippingMethodId == 0)
                return null;

            string key = string.Format(SHIPPINGMETHODS_BY_ID_KEY, shippingMethodId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ShippingMethod)obj2;
            }


            var query = from sm in _context.ShippingMethods
                        where sm.ShippingMethodId == shippingMethodId
                        select sm;
            var shippingMethod = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, shippingMethod);
            }
            return shippingMethod;
        }

        /// <summary>
        /// Gets all shipping methods
        /// </summary>
        /// <returns>Shipping method collection</returns>
        public List<ShippingMethod> GetAllShippingMethods()
        {
            return GetAllShippingMethods(null);
        }

        /// <summary>
        /// Gets all shipping methods
        /// </summary>
        /// <param name="filterByCountryId">The country indentifier</param>
        /// <returns>Shipping method collection</returns>
        public List<ShippingMethod> GetAllShippingMethods(int? filterByCountryId)
        {

            var shippingMethods = _context.Sp_ShippingMethodLoadAll(filterByCountryId).ToList();
            return shippingMethods;
        }

        /// <summary>
        /// Inserts a shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        public void InsertShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            shippingMethod.Name = CommonHelper.EnsureNotNull(shippingMethod.Name);
            shippingMethod.Name = CommonHelper.EnsureMaximumLength(shippingMethod.Name, 100);
            shippingMethod.Description = CommonHelper.EnsureNotNull(shippingMethod.Description);
            shippingMethod.Description = CommonHelper.EnsureMaximumLength(shippingMethod.Description, 2000);



            _context.ShippingMethods.AddObject(shippingMethod);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        public void UpdateShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            shippingMethod.Name = CommonHelper.EnsureNotNull(shippingMethod.Name);
            shippingMethod.Name = CommonHelper.EnsureMaximumLength(shippingMethod.Name, 100);
            shippingMethod.Description = CommonHelper.EnsureNotNull(shippingMethod.Description);
            shippingMethod.Description = CommonHelper.EnsureMaximumLength(shippingMethod.Description, 2000);


            if (!_context.IsAttached(shippingMethod))
                _context.ShippingMethods.Attach(shippingMethod);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Creates the shipping method country mapping
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">The country identifier</param>
        public void CreateShippingMethodCountryMapping(int shippingMethodId, int countryId)
        {
            var shippingMethod = GetShippingMethodById(shippingMethodId);
            if (shippingMethod == null)
                return;

            var country = IoCFactory.Resolve<ICountryManager>().GetCountryById(countryId);
            if (country == null)
                return;


            if (!_context.IsAttached(shippingMethod))
                _context.ShippingMethods.Attach(shippingMethod);
            if (!_context.IsAttached(country))
                _context.Countries.Attach(country);

            //ensure that navigation property is loaded
            if (country.NpRestrictedShippingMethods == null)
                _context.LoadProperty(country, c => c.NpRestrictedShippingMethods);

            country.NpRestrictedShippingMethods.Add(shippingMethod);
            _context.SaveChanges();
        }

        /// <summary>
        /// Checking whether the shipping method country mapping exists
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <returns>True if mapping exist, otherwise false</returns>
        public bool DoesShippingMethodCountryMappingExist(int shippingMethodId, int countryId)
        {


            var shippingMethod = GetShippingMethodById(shippingMethodId);
            if (shippingMethod == null)
                return false;

            //ensure that navigation property is loaded
            if (shippingMethod.NpRestrictedCountries == null)
                _context.LoadProperty(shippingMethod, sm => sm.NpRestrictedCountries);

            bool result = shippingMethod.NpRestrictedCountries.ToList().Find(c => c.CountryId == countryId) != null;
            return result;

            //var query = from sm in _context.ShippingMethods
            //            from c in sm.NpRestrictedCountries
            //            where sm.ShippingMethodId == shippingMethodId &&
            //            c.CountryId == countryId
            //            select sm;

            //bool result = query.Count() > 0;
            //return result;
        }

        /// <summary>
        /// Deletes the shipping method country mapping
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">The country identifier</param>
        public void DeleteShippingMethodCountryMapping(int shippingMethodId, int countryId)
        {
            var shippingMethod = GetShippingMethodById(shippingMethodId);
            if (shippingMethod == null)
                return;

            var country = IoCFactory.Resolve<ICountryManager>().GetCountryById(countryId);
            if (country == null)
                return;


            if (!_context.IsAttached(shippingMethod))
                _context.ShippingMethods.Attach(shippingMethod);
            if (!_context.IsAttached(country))
                _context.Countries.Attach(country);

            //ensure that navigation property is loaded
            if (country.NpRestrictedShippingMethods == null)
                _context.LoadProperty(country, c => c.NpRestrictedShippingMethods);

            country.NpRestrictedShippingMethods.Remove(shippingMethod);
            _context.SaveChanges();
        }

        #endregion

        #region Workflow

        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>Shopping cart weight</returns>
        public decimal GetShoppingCartTotalWeight(ShoppingCart cart, Customer customer)
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
        public decimal? GetShoppingCartShippingTotal(ShoppingCart cart)
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
        public decimal? GetShoppingCartShippingTotal(ShoppingCart cart, ref string error)
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
        public decimal? GetShoppingCartShippingTotal(ShoppingCart cart, Customer customer)
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
        public decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
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
        public decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
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
        public decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
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
        public decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
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
        public decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
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
                var shippingRateComputationMethods = GetAllShippingRateComputationMethods(false);
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
                shippingTotalWithDiscountTaxed = IoCFactory.Resolve<ITaxManager>().GetShippingPrice(shippingTotalWithDiscount.Value,
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
        public decimal GetShippingDiscount(Customer customer, 
            decimal shippingTotal, out Discount appliedDiscount)
        {
            decimal shippingDiscountAmount = decimal.Zero;

            string customerCouponCode = string.Empty;
            if (customer != null)
                customerCouponCode = customer.LastAppliedCouponCode;

            var allDiscounts = IoCFactory.Resolve<IDiscountManager>().GetAllDiscounts(DiscountTypeEnum.AssignedToShipping);
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

            appliedDiscount = IoCFactory.Resolve<IDiscountManager>().GetPreferredDiscount(allowedDiscounts, shippingTotal);
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
        public bool ShoppingCartRequiresShipping(ShoppingCart cart)
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
        public decimal GetShoppingCartAdditionalShippingCharge(ShoppingCart cart, Customer customer)
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
        public List<ShippingOption> GetShippingOptions(ShoppingCart cart, 
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
        public List<ShippingOption> GetShippingOptions(ShoppingCart cart,
            Customer customer, Address shippingAddress, 
            int? allowedShippingRateComputationMethodId, ref string error)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            var shippingOptions = new List<ShippingOption>();

            bool isFreeShipping = IsFreeShipping(cart, customer);

            //create a package
            var shipmentPackage = CreateShipmentPackage(cart, customer, shippingAddress);
            var shippingRateComputationMethods = GetAllShippingRateComputationMethods(false);
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

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.ShippingManager.CacheEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a default shipping origin address
        /// </summary>
        public Address ShippingOrigin
        {
            get
            {
                int countryId = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Shipping.ShippingOrigin.CountryId");
                int stateProvinceId = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Shipping.ShippingOrigin.StateProvinceId");
                string zipPostalCode = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Shipping.ShippingOrigin.ZipPostalCode");
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

                IoCFactory.Resolve<ISettingManager>().SetParam("Shipping.ShippingOrigin.CountryId", countryId.ToString());
                IoCFactory.Resolve<ISettingManager>().SetParam("Shipping.ShippingOrigin.StateProvinceId", stateProvinceId.ToString());
                IoCFactory.Resolve<ISettingManager>().SetParam("Shipping.ShippingOrigin.ZipPostalCode", zipPostalCode);
            }
        }
        #endregion
    }
}
