using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipping service
    /// </summary>
    public partial class ShippingService : IShippingService
    {
        #region Fields

        private readonly IRepository<ShippingMethod> _shippingMethodRepository;
        private readonly ILogger _logger;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ShippingSettings _shippingSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IEventPublisher _eventPublisher;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shippingMethodRepository">Shipping method repository</param>
        /// <param name="logger">Logger</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        public ShippingService(IRepository<ShippingMethod> shippingMethodRepository,
            ILogger logger,
            IProductAttributeParser productAttributeParser,
            ICheckoutAttributeParser checkoutAttributeParser,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ShippingSettings shippingSettings,
            IPluginFinder pluginFinder,
            IEventPublisher eventPublisher,
            ShoppingCartSettings shoppingCartSettings)
        {
            this._shippingMethodRepository = shippingMethodRepository;
            this._logger = logger;
            this._productAttributeParser = productAttributeParser;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._shippingSettings = shippingSettings;
            this._pluginFinder = pluginFinder;
            this._eventPublisher = eventPublisher;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        #endregion
        
        #region Methods

        #region Shipping rate computation methods

        /// <summary>
        /// Load active shipping rate computation methods
        /// </summary>
        /// <returns>Shipping rate computation methods</returns>
        public virtual IList<IShippingRateComputationMethod> LoadActiveShippingRateComputationMethods()
        {
            return LoadAllShippingRateComputationMethods()
                   .Where(provider => _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase))
                   .ToList();
        }

        /// <summary>
        /// Load shipping rate computation method by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found Shipping rate computation method</returns>
        public virtual IShippingRateComputationMethod LoadShippingRateComputationMethodBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IShippingRateComputationMethod>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IShippingRateComputationMethod>();

            return null;
        }

        /// <summary>
        /// Load all shipping rate computation methods
        /// </summary>
        /// <returns>Shipping rate computation methods</returns>
        public virtual IList<IShippingRateComputationMethod> LoadAllShippingRateComputationMethods()
        {
            return _pluginFinder.GetPlugins<IShippingRateComputationMethod>().ToList();
        }

        #endregion

        #region Shipping methods


        /// <summary>
        /// Deletes a shipping method
        /// </summary>
        /// <param name="shippingMethod">The shipping method</param>
        public virtual void DeleteShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            _shippingMethodRepository.Delete(shippingMethod);

            //event notification
            _eventPublisher.EntityDeleted(shippingMethod);
        }

        /// <summary>
        /// Gets a shipping method
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <returns>Shipping method</returns>
        public virtual ShippingMethod GetShippingMethodById(int shippingMethodId)
        {
            if (shippingMethodId == 0)
                return null;

            return _shippingMethodRepository.GetById(shippingMethodId);
        }
        
        /// <summary>
        /// Gets all shipping methods
        /// </summary>
        /// <param name="filterByCountryId">The country indentifier to filter by</param>
        /// <returns>Shipping method collection</returns>
        public virtual IList<ShippingMethod> GetAllShippingMethods(int? filterByCountryId = null)
        {
            if (filterByCountryId.HasValue && filterByCountryId.Value > 0)
            {
                var query1 = from sm in _shippingMethodRepository.Table
                             where
                             sm.RestrictedCountries.Select(c => c.Id).Contains(filterByCountryId.Value)
                             select sm.Id;

                var query2 = from sm in _shippingMethodRepository.Table
                             where !query1.Contains(sm.Id)
                             orderby sm.DisplayOrder
                             select sm;

                var shippingMethods = query2.ToList();
                return shippingMethods;
            }
            else
            {
                var query = from sm in _shippingMethodRepository.Table
                            orderby sm.DisplayOrder
                            select sm;
                var shippingMethods = query.ToList();
                return shippingMethods;
            }
        }

        /// <summary>
        /// Inserts a shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        public virtual void InsertShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            _shippingMethodRepository.Insert(shippingMethod);

            //event notification
            _eventPublisher.EntityInserted(shippingMethod);
        }

        /// <summary>
        /// Updates the shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        public virtual void UpdateShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            _shippingMethodRepository.Update(shippingMethod);

            //event notification
            _eventPublisher.EntityUpdated(shippingMethod);
        }

        #endregion

        #region Workflow

        /// <summary>
        /// Gets shopping cart item weight (of one item)
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>Shopping cart item weight</returns>
        public virtual decimal GetShoppingCartItemWeight(ShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");
            decimal weight = decimal.Zero;
            if (shoppingCartItem.ProductVariant != null)
            {
                decimal attributesTotalWeight = decimal.Zero;

                if (!String.IsNullOrEmpty(shoppingCartItem.AttributesXml))
                {
                    var pvaValues = _productAttributeParser.ParseProductVariantAttributeValues(shoppingCartItem.AttributesXml);
                    foreach (var pvaValue in pvaValues)
                        attributesTotalWeight += pvaValue.WeightAdjustment;
                }
                weight = shoppingCartItem.ProductVariant.Weight + attributesTotalWeight;
            }
            return weight;
        }

        /// <summary>
        /// Gets shopping cart item total weight
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>Shopping cart item weight</returns>
        public virtual decimal GetShoppingCartItemTotalWeight(ShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");

            decimal totalWeight = GetShoppingCartItemWeight(shoppingCartItem) * shoppingCartItem.Quantity;
            return totalWeight;
        }

        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Shopping cart weight</returns>
        public virtual decimal GetShoppingCartTotalWeight(IList<ShoppingCartItem> cart)
        {
            Customer customer = cart.GetCustomer();

            decimal totalWeight = decimal.Zero;
            //shopping cart items
            foreach (var shoppingCartItem in cart)
                totalWeight += GetShoppingCartItemTotalWeight(shoppingCartItem);

            //checkout attributes
            if (customer != null)
            {
                var checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService);
                if (!String.IsNullOrEmpty(checkoutAttributesXml))
                {
                    var caValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);
                    foreach (var caValue in caValues)
                        totalWeight += caValue.WeightAdjustment;
                }
            }
            return totalWeight;
        }

        /// <summary>
        /// Create shipment package from shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <returns>Shipment package</returns>
        public virtual GetShippingOptionRequest CreateShippingOptionRequest(IList<ShoppingCartItem> cart,
            Address shippingAddress)
        {
            var request = new GetShippingOptionRequest();
            request.Customer = cart.GetCustomer();
            request.Items = new List<ShoppingCartItem>();
            foreach (var sc in cart)
                if (sc.IsShipEnabled)
                    request.Items.Add(sc);
            request.ShippingAddress = shippingAddress;
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
        public virtual GetShippingOptionResponse GetShippingOptions(IList<ShoppingCartItem> cart,
            Address shippingAddress, string allowedShippingRateComputationMethodSystemName = "")
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            var result = new GetShippingOptionResponse();
            
            //create a package
            var getShippingOptionRequest = CreateShippingOptionRequest(cart, shippingAddress);
            var shippingRateComputationMethods = LoadActiveShippingRateComputationMethods()
                .Where(srcm => 
                    String.IsNullOrWhiteSpace(allowedShippingRateComputationMethodSystemName) || 
                    allowedShippingRateComputationMethodSystemName.Equals(srcm.PluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            if (shippingRateComputationMethods.Count == 0)
                throw new NopException("Shipping rate computation method could not be loaded");

            //get shipping options
            foreach (var srcm in shippingRateComputationMethods)
            {
                var getShippingOptionResponse = srcm.GetShippingOptions(getShippingOptionRequest);
                foreach (var so2 in getShippingOptionResponse.ShippingOptions)
                {
                    //system name
                    so2.ShippingRateComputationMethodSystemName = srcm.PluginDescriptor.SystemName;
                    //round
                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                        so2.Rate = Math.Round(so2.Rate, 2);
                    result.ShippingOptions.Add(so2);
                }

                //log errors
                if (!getShippingOptionResponse.Success)
                {
                    foreach (string error in getShippingOptionResponse.Errors)
                    {
                        result.AddError(error);
                        _logger.Warning(string.Format("Shipping ({0}). {1}", srcm.PluginDescriptor.FriendlyName, error));
                    }
                }
            }

            if (_shippingSettings.ReturnValidOptionsIfThereAreAny)
            {
                //return valid options if there are any (no matter of the errors returned by other shipping rate compuation methods).
                if (result.ShippingOptions.Count > 0 && result.Errors.Count > 0)
                    result.Errors.Clear();
            }
            
            //no shipping options loaded
            if (result.ShippingOptions.Count == 0 && result.Errors.Count == 0)
                result.Errors.Add(_localizationService.GetResource("Checkout.ShippingOptionCouldNotBeLoaded"));
            
            return result;
        }

        #endregion

        #endregion
    }
}
