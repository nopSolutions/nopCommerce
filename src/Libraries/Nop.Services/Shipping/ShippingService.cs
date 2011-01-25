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
using System.Reflection;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Data;
using Nop.Core.Caching;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipping service
    /// </summary>
    public partial class ShippingService : IShippingService
    {
        #region Constants

        private const string SHIPPINGMETHODS_BY_ID_KEY = "Nop.shippingMethod.id-{0}";
        private const string SHIPPINGMETHODS_PATTERN_KEY = "Nop.shippingMethod.";

        #endregion

        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IRepository<ShippingMethod> _shippingMethodRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;
        private readonly IDiscountService _discountService;
        private readonly ITaxService _taxService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ShippingSettings _shippingSettings;
        
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="workContext">Work context</param>
        /// <param name="shippingMethodRepository">Shipping method repository</param>
        /// <param name="logger">Logger</param>
        /// <param name="discountService">Discount service</param>
        /// <param name="taxService">Tax service</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="shippingSettings">Shipping settings</param>
        public ShippingService(ICacheManager cacheManager, 
            IWorkContext workContext,
            IRepository<ShippingMethod> shippingMethodRepository,
            ILogger logger,
            IDiscountService discountService,
            ITaxService taxService,
            IProductAttributeParser productAttributeParser,
            ICheckoutAttributeParser checkoutAttributeParser,
            ShippingSettings shippingSettings)
        {
            this._cacheManager = cacheManager;
            this._workContext = workContext;
            this._shippingMethodRepository = shippingMethodRepository;
            this._logger = logger;
            this._discountService = discountService;
            this._taxService = taxService;
            this._productAttributeParser = productAttributeParser;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._shippingSettings = shippingSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a value indicating whether shipping is free
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>A value indicating whether shipping is free</returns>
        protected bool IsFreeShipping(IList<ShoppingCartItem> cart, Customer customer)
        {
            if (customer != null)
            {
                //check whether customer is in a customer role with free shipping applied
                var customerRoles = customer.CustomerRoles;  //TODO filter active roles
                foreach (var customerRole in customerRoles)
                    if (customerRole.FreeShipping)
                        return true;
            }

            bool shoppingCartRequiresShipping = cart.RequiresShipping();
            if (!shoppingCartRequiresShipping)
                return true;

            //TODO uncomment below (free shipping over $X)
            //check whether we have subtotal enough to have free shipping
            //decimal subTotalBase = decimal.Zero;
            //decimal orderSubTotalDiscountAmount = decimal.Zero;
            //Discount orderSubTotalAppliedDiscount = null;
            //decimal subTotalWithoutDiscountBase = decimal.Zero; 
            //decimal subTotalWithDiscountBase = decimal.Zero;
            //string subTotalError = IoC.Resolve<IShoppingCartService>().GetShoppingCartSubTotal(cart,
            //    customer, out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscount,
            //    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            //subTotalBase = subTotalWithDiscountBase;
            //if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Shipping.FreeShippingOverX.Enabled"))
            //{
            //    decimal freeShippingOverX = IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("Shipping.FreeShippingOverX.Value");
            //    if (subTotalBase > freeShippingOverX)
            //        return true;
            //}

            //check whether all shopping cart items are marked as free shipping
            bool allItemsAreFreeShipping = true;
            foreach (var sc in cart)
            {
                if (sc.IsShipEnabled && !sc.IsFreeShipping)
                {
                    allItemsAreFreeShipping = false;
                    break;
                }
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
        protected GetShippingOptionRequest CreateShippingOptionRequest(IList<ShoppingCartItem> cart, 
            Customer customer, Address shippingAddress)
        {
            var request = new GetShippingOptionRequest();
            request.Customer = customer;
            request.Items = new List<ShoppingCartItem>();
            foreach (var sc in cart)
                if (sc.IsShipEnabled)
                    request.Items.Add(sc);
            request.ShippingAddress = shippingAddress;
            //TODO set values from warehouses or shipping origin
            request.CountryFrom = null;
            request.StateProvinceFrom = null;
            request.ZipPostalCodeFrom = string.Empty;
            return request;

        }

        #endregion

        #region Methods

        #region Shipping rate computation methods

        /// <summary>
        /// Load active shipping rate computation methods
        /// </summary>
        /// <returns>Shipping rate computation methods</returns>
        public IList<IShippingRateComputationMethod> LoadActiveShippingRateComputationMethods()
        {
            var systemNames = _shippingSettings.ActiveShippingRateComputationMethodSystemNames;
            var providers = new List<IShippingRateComputationMethod>();
            foreach (var systemName in systemNames)
            {
                var provider = LoadShippingRateComputationMethodBySystemName(systemName);
                providers.Add(provider);
            }
            return providers;
        }

        /// <summary>
        /// Load tax provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found tax provider</returns>
        public IShippingRateComputationMethod LoadShippingRateComputationMethodBySystemName(string systemName)
        {
            var providers = LoadAllShippingRateComputationMethods();
            var provider = providers.SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
            return provider;
        }

        /// <summary>
        /// Load all tax providers
        /// </summary>
        /// <returns>Tax providers</returns>
        public IList<IShippingRateComputationMethod> LoadAllShippingRateComputationMethods()
        {
            var providers = new List<IShippingRateComputationMethod>();

            //TODO search in all assemblies (use StructureMap assembly scanning - http://structuremap.net/structuremap/ScanningAssemblies.htm)
            //NOTE: currently it doesn't work until tax providers are saved into Nop.Services
            System.Type configType = typeof(ShippingService);
            var typesToRegister = Assembly.GetAssembly(configType).GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(IShippingRateComputationMethod)));

            foreach (var type in typesToRegister)
            {
                //TODO inject ISettingService into type.SettingService (and IMeasureService)
                dynamic provider = Activator.CreateInstance(type);
                providers.Add(provider);
            }

            //sort and return
            return providers.OrderBy(tp => tp.FriendlyName).ToList();
        }

        #endregion

        #region Shipping methods


        /// <summary>
        /// Deletes a shipping method
        /// </summary>
        /// <param name="shippingMethod">The shipping method</param>
        public void DeleteShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                return;

            _shippingMethodRepository.Delete(shippingMethod);
            _cacheManager.RemoveByPattern(SHIPPINGMETHODS_PATTERN_KEY);
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
            return _cacheManager.Get(key, () =>
            {
                var shippingMethod = _shippingMethodRepository.GetById(shippingMethodId);
                return shippingMethod;
            });
        }


        /// <summary>
        /// Gets all shipping methods
        /// </summary>
        /// <returns>Shipping method collection</returns>
        public List<ShippingMethod> GetAllShippingMethods()
        {
            var query = from sm in _shippingMethodRepository.Table
                        orderby sm.DisplayOrder
                        select sm;

            var shippingMethods = query.ToList();
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

            _shippingMethodRepository.Insert(shippingMethod);
            
            _cacheManager.RemoveByPattern(SHIPPINGMETHODS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        public void UpdateShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            _shippingMethodRepository.Update(shippingMethod);

            _cacheManager.RemoveByPattern(SHIPPINGMETHODS_PATTERN_KEY);
        }

        #endregion

        #region Workflow

        /// <summary>
        /// Gets shopping cart item total weight
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>Shopping cart item weight</returns>
        public decimal GetShoppingCartItemTotalWeight(ShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");
            decimal totalWeight = decimal.Zero;
            if (shoppingCartItem.ProductVariant != null)
            {
                decimal attributesTotalWeight = decimal.Zero;

                var pvaValues = _productAttributeParser.ParseProductVariantAttributeValues(shoppingCartItem.AttributesXml);
                foreach (var pvaValue in pvaValues)
                {
                    attributesTotalWeight += pvaValue.WeightAdjustment;
                }
                decimal unitWeight = shoppingCartItem.ProductVariant.Weight + attributesTotalWeight;
                totalWeight = unitWeight * shoppingCartItem.Quantity;
            }
            return totalWeight;
        }

        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>Shopping cart weight</returns>
        public decimal GetShoppingCartTotalWeight(IList<ShoppingCartItem> cart, Customer customer)
        {
            decimal totalWeight = decimal.Zero;
            //shopping cart items
            foreach (var shoppingCartItem in cart)
                totalWeight += GetShoppingCartItemTotalWeight(shoppingCartItem);

            //checkout attributes
            if (customer != null)
            {
                var caValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(customer.CheckoutAttributes);
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
        public decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart)
        {
            Customer customer = _workContext.CurrentCustomer;
            return GetShoppingCartShippingTotal(cart, customer);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>Shipping total</returns>
        public decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, Customer customer)
        {
            bool includingTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetShoppingCartShippingTotal(cart, customer, includingTax);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <returns>Shipping total</returns>
        public decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, Customer customer,
            bool includingTax)
        {
            decimal taxRate = decimal.Zero;
            return GetShoppingCartShippingTotal(cart, customer,
                includingTax, out taxRate);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <returns>Shipping total</returns>
        public decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, Customer customer, 
            bool includingTax, out decimal taxRate)
        {
            Discount appliedDiscount = null;
            return GetShoppingCartShippingTotal(cart, customer, 
                includingTax, out taxRate, out appliedDiscount);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Shipping total</returns>
        public decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, Customer customer,
            bool includingTax, out decimal taxRate, out Discount appliedDiscount)
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
                lastShippingOption = customer.GetAttribute<ShippingOption>("LastShippingOption");

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
                    shippingAddress = customer.ShippingAddress;

                var getShippingOptionRequest = CreateShippingOptionRequest(cart, customer, shippingAddress);
                var shippingRateComputationMethods = LoadActiveShippingRateComputationMethods();
                if (shippingRateComputationMethods.Count == 0)
                    throw new NopException("Shipping rate computation method could not be loaded");

                if (shippingRateComputationMethods.Count == 1)
                {
                    var shippingRateComputationMethod = shippingRateComputationMethods[0];

                    decimal? fixedRate = shippingRateComputationMethod.GetFixedRate(getShippingOptionRequest);
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

            if (shippingTotalWithDiscount.HasValue)
            {
                shippingTotalWithDiscountTaxed = _taxService.GetShippingPrice(shippingTotalWithDiscount.Value,
                    includingTax,
                    customer,
                    out taxRate);

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

            var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToShipping);
            var allowedDiscounts = new List<Discount>();
            foreach (var discount in allDiscounts)
            {
                if (_discountService.IsDiscountValid(discount, customer) &&
                           discount.DiscountType == DiscountType.AssignedToShipping &&
                           !allowedDiscounts.Contains(discount))
                {
                    allowedDiscounts.Add(discount);
                }
            }

            appliedDiscount = _discountService.GetPreferredDiscount(allowedDiscounts, shippingTotal);
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
        /// Gets shopping cart additional shipping charge
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>Additional shipping charge</returns>
        public decimal GetShoppingCartAdditionalShippingCharge(IList<ShoppingCartItem> cart, Customer customer)
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
        /// <param name="allowedShippingRateComputationMethodSystemName">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
        /// <returns>Shipping options</returns>
        public List<ShippingOption> GetShippingOptions(IList<ShoppingCartItem> cart,
            Customer customer, Address shippingAddress,
            string allowedShippingRateComputationMethodSystemName)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            var shippingOptions = new List<ShippingOption>();

            bool isFreeShipping = IsFreeShipping(cart, customer);

            //create a package
            var getShippingOptionRequest = CreateShippingOptionRequest(cart, customer, shippingAddress);
            var shippingRateComputationMethods = LoadActiveShippingRateComputationMethods();
            if (shippingRateComputationMethods.Count == 0)
                throw new NopException("Shipping rate computation method could not be loaded");

            //get shipping options
            foreach (var srcm in shippingRateComputationMethods)
            {
                if (!String.IsNullOrWhiteSpace(allowedShippingRateComputationMethodSystemName) &&
                   allowedShippingRateComputationMethodSystemName.Equals(srcm.SystemName, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var getShippingOptionResponse = srcm.GetShippingOptions(getShippingOptionRequest);
                foreach (var so2 in getShippingOptionResponse.ShippingOptions)
                {
                    so2.ShippingRateComputationMethodSystemName = srcm.SystemName;
                    shippingOptions.Add(so2);
                }

                //log errors
                if (!getShippingOptionResponse.Success)
                {
                    foreach (string error in getShippingOptionResponse.Errors)
                        _logger.Warning(string.Format("Shipping ({0}). {1}", srcm.FriendlyName, error));
                }
            }
            
            //additional shipping charges
            decimal additionalShippingCharge = GetShoppingCartAdditionalShippingCharge(cart, customer);
            shippingOptions.ForEach(so => so.Rate += additionalShippingCharge);
            
            //free shipping
            if (isFreeShipping)
                shippingOptions.ForEach(so => so.Rate = decimal.Zero);

            return shippingOptions;
        }

        #endregion

        #endregion
    }
}
