
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

        private readonly IRepository<ShippingMethod> _shippingMethodRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ShippingSettings _shippingSettings;
        
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="shippingMethodRepository">Shipping method repository</param>
        /// <param name="logger">Logger</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="shippingSettings">Shipping settings</param>
        public ShippingService(ICacheManager cacheManager, 
            IRepository<ShippingMethod> shippingMethodRepository,
            ILogger logger,
            IProductAttributeParser productAttributeParser,
            ICheckoutAttributeParser checkoutAttributeParser,
            ShippingSettings shippingSettings)
        {
            this._cacheManager = cacheManager;
            this._shippingMethodRepository = shippingMethodRepository;
            this._logger = logger;
            this._productAttributeParser = productAttributeParser;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._shippingSettings = shippingSettings;
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
        public IList<ShippingMethod> GetAllShippingMethods()
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
        /// <returns>Shopping cart weight</returns>
        public decimal GetShoppingCartTotalWeight(IList<ShoppingCartItem> cart)
        {
            Customer customer = cart.GetCustomer();

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
        /// Gets shopping cart additional shipping charge
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Additional shipping charge</returns>
        public decimal GetShoppingCartAdditionalShippingCharge(IList<ShoppingCartItem> cart)
        {
            decimal additionalShippingCharge = decimal.Zero;
            
            bool isFreeShipping = IsFreeShipping(cart);
            if (isFreeShipping)
                return decimal.Zero;

            foreach (var shoppingCartItem in cart)
                additionalShippingCharge += shoppingCartItem.AdditionalShippingCharge;

            return additionalShippingCharge;
        }

        /// <summary>
        /// Gets a value indicating whether shipping is free
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>A value indicating whether shipping is free</returns>
        public bool IsFreeShipping(IList<ShoppingCartItem> cart)
        {
            Customer customer = cart.GetCustomer();
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
        /// <param name="shippingAddress">Shipping address</param>
        /// <returns>Shipment package</returns>
        public GetShippingOptionRequest CreateShippingOptionRequest(IList<ShoppingCartItem> cart,
            Address shippingAddress)
        {
            var request = new GetShippingOptionRequest();
            request.Customer = cart.GetCustomer();
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

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="allowedShippingRateComputationMethodSystemName">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
        /// <returns>Shipping options</returns>
        public IList<ShippingOption> GetShippingOptions(IList<ShoppingCartItem> cart,
            Address shippingAddress, string allowedShippingRateComputationMethodSystemName)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            var shippingOptions = new List<ShippingOption>();

            bool isFreeShipping = IsFreeShipping(cart);

            //create a package
            var getShippingOptionRequest = CreateShippingOptionRequest(cart, shippingAddress);
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
            decimal additionalShippingCharge = GetShoppingCartAdditionalShippingCharge(cart);
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
