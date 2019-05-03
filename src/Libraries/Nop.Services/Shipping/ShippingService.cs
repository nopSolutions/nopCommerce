using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping.Pickup;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipping service
    /// </summary>
    public partial class ShippingService : IShippingService
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICacheManager _cacheManager;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IPickupPluginManager _pickupPluginManager;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IRepository<ShippingMethod> _shippingMethodRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly ShippingSettings _shippingSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public ShippingService(IAddressService addressService,
            ICacheManager cacheManager,
            ICheckoutAttributeParser checkoutAttributeParser,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IPickupPluginManager pickupPluginManager,
            IPriceCalculationService priceCalculationService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IRepository<ShippingMethod> shippingMethodRepository,
            IRepository<Warehouse> warehouseRepository,
            IShippingPluginManager shippingPluginManager,
            IStoreContext storeContext,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings)
        {
            _addressService = addressService;
            _cacheManager = cacheManager;
            _checkoutAttributeParser = checkoutAttributeParser;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _pickupPluginManager = pickupPluginManager;
            _priceCalculationService = priceCalculationService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _shippingMethodRepository = shippingMethodRepository;
            _warehouseRepository = warehouseRepository;
            _shippingPluginManager = shippingPluginManager;
            _storeContext = storeContext;
            _shippingSettings = shippingSettings;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether there are multiple package items in the cart for the delivery
        /// </summary>
        /// <param name="items">Package items</param>
        /// <returns>True if there are multiple items; otherwise false</returns>
        protected bool AreMultipleItems(IList<GetShippingOptionRequest.PackageItem> items)
        {
            //no items
            if (!items.Any())
                return false;

            //more than one
            if (items.Count > 1)
                return true;

            //or single item
            var singleItem = items.First();

            //but quantity more than one
            if (singleItem.GetQuantity() > 1)
                return true;

            //one item with quantity is one and without attributes
            if (string.IsNullOrEmpty(singleItem.ShoppingCartItem.AttributesXml))
                return false;

            //find associated products of item
            var associatedAttributeValues = _productAttributeParser.ParseProductAttributeValues(singleItem.ShoppingCartItem.AttributesXml)
                .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct);

            //whether to ship associated products
            return associatedAttributeValues.Any(attributeValue =>
                _productService.GetProductById(attributeValue.AssociatedProductId)?.IsShipEnabled ?? false);
        }

        #endregion

        #region Methods

        #region Shipping methods

        /// <summary>
        /// Deletes a shipping method
        /// </summary>
        /// <param name="shippingMethod">The shipping method</param>
        public virtual void DeleteShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException(nameof(shippingMethod));

            _shippingMethodRepository.Delete(shippingMethod);

            _cacheManager.RemoveByPrefix(NopShippingDefaults.ShippingMethodsPrefixCacheKey);

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
        /// <param name="filterByCountryId">The country identifier to filter by</param>
        /// <returns>Shipping methods</returns>
        public virtual IList<ShippingMethod> GetAllShippingMethods(int? filterByCountryId = null)
        {
            var key = string.Format(NopShippingDefaults.ShippingMethodsAllCacheKey, filterByCountryId ?? 0);

            return _cacheManager.Get(key, () =>
            {
                if (filterByCountryId.HasValue && filterByCountryId.Value > 0)
                {
                    var query1 = from sm in _shippingMethodRepository.Table
                                 where sm.ShippingMethodCountryMappings.Select(mapping => mapping.CountryId).Contains(filterByCountryId.Value)
                                 select sm.Id;

                    var query2 = from sm in _shippingMethodRepository.Table
                                 where !query1.Contains(sm.Id)
                                 orderby sm.DisplayOrder, sm.Id
                                 select sm;

                    return query2.ToList();
                }
                else
                {
                    var query = from sm in _shippingMethodRepository.Table
                                orderby sm.DisplayOrder, sm.Id
                                select sm;
                    return query.ToList();
                }
            });
        }

        /// <summary>
        /// Inserts a shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        public virtual void InsertShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException(nameof(shippingMethod));

            _shippingMethodRepository.Insert(shippingMethod);

            _cacheManager.RemoveByPrefix(NopShippingDefaults.ShippingMethodsPrefixCacheKey);

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
                throw new ArgumentNullException(nameof(shippingMethod));

            _shippingMethodRepository.Update(shippingMethod);

            _cacheManager.RemoveByPrefix(NopShippingDefaults.ShippingMethodsPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(shippingMethod);
        }

        /// <summary>
        /// Does country restriction exist
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Result</returns>
        public virtual bool CountryRestrictionExists(ShippingMethod shippingMethod, int countryId)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException(nameof(shippingMethod));

            var result = shippingMethod.ShippingMethodCountryMappings.Any(c => c.CountryId == countryId);
            return result;
        }

        #endregion

        #region Warehouses

        /// <summary>
        /// Deletes a warehouse
        /// </summary>
        /// <param name="warehouse">The warehouse</param>
        public virtual void DeleteWarehouse(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException(nameof(warehouse));

            _warehouseRepository.Delete(warehouse);

            //clear cache
            _cacheManager.RemoveByPrefix(NopShippingDefaults.WarehousesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(warehouse);
        }

        /// <summary>
        /// Gets a warehouse
        /// </summary>
        /// <param name="warehouseId">The warehouse identifier</param>
        /// <returns>Warehouse</returns>
        public virtual Warehouse GetWarehouseById(int warehouseId)
        {
            if (warehouseId == 0)
                return null;

            var key = string.Format(NopShippingDefaults.WarehousesByIdCacheKey, warehouseId);
            return _cacheManager.Get(key, () => _warehouseRepository.GetById(warehouseId));
        }

        /// <summary>
        /// Gets all warehouses
        /// </summary>
        /// <returns>Warehouses</returns>
        public virtual IList<Warehouse> GetAllWarehouses()
        {
            var query = from wh in _warehouseRepository.Table
                        orderby wh.Name
                        select wh;
            var warehouses = query.ToList();
            return warehouses;
        }

        /// <summary>
        /// Inserts a warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        public virtual void InsertWarehouse(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException(nameof(warehouse));

            _warehouseRepository.Insert(warehouse);

            //clear cache
            _cacheManager.RemoveByPrefix(NopShippingDefaults.WarehousesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(warehouse);
        }

        /// <summary>
        /// Updates the warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        public virtual void UpdateWarehouse(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException(nameof(warehouse));

            _warehouseRepository.Update(warehouse);

            //clear cache
            _cacheManager.RemoveByPrefix(NopShippingDefaults.WarehousesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(warehouse);
        }

        #endregion

        #region Workflow

        /// <summary>
        /// Gets shopping cart item weight (of one item)
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        /// <returns>Shopping cart item weight</returns>
        public virtual decimal GetShoppingCartItemWeight(ShoppingCartItem shoppingCartItem, bool ignoreFreeShippedItems = false)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            return GetShoppingCartItemWeight(shoppingCartItem.Product, shoppingCartItem.AttributesXml, ignoreFreeShippedItems);
        }

        /// <summary>
        /// Gets product item weight (of one item)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Selected product attributes in XML</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        /// <returns>Item weight</returns>
        public virtual decimal GetShoppingCartItemWeight(Product product, string attributesXml, bool ignoreFreeShippedItems = false)
        {
            if (product == null)
                return decimal.Zero;

            //product weight
            var productWeight = !product.IsFreeShipping || !ignoreFreeShippedItems ? product.Weight : decimal.Zero;

            //attribute weight
            var attributesTotalWeight = decimal.Zero;

            if (!_shippingSettings.ConsiderAssociatedProductsDimensions || string.IsNullOrEmpty(attributesXml))
                return productWeight + attributesTotalWeight;

            var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                switch (attributeValue.AttributeValueType)
                {
                    case AttributeValueType.Simple:
                        //simple attribute
                        attributesTotalWeight += attributeValue.WeightAdjustment;
                        break;
                    case AttributeValueType.AssociatedToProduct:
                        //bundled product
                        var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                        if (associatedProduct != null && associatedProduct.IsShipEnabled && (!associatedProduct.IsFreeShipping || !ignoreFreeShippedItems))
                            attributesTotalWeight += associatedProduct.Weight * attributeValue.Quantity;
                        break;
                }
            }

            return productWeight + attributesTotalWeight;
        }

        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="includeCheckoutAttributes">A value indicating whether we should calculate weights of selected checkotu attributes</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        /// <returns>Total weight</returns>
        public virtual decimal GetTotalWeight(GetShippingOptionRequest request,
            bool includeCheckoutAttributes = true, bool ignoreFreeShippedItems = false)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var totalWeight = decimal.Zero;

            //shopping cart items
            foreach (var packageItem in request.Items)
                totalWeight += GetShoppingCartItemWeight(packageItem.ShoppingCartItem, ignoreFreeShippedItems) * packageItem.GetQuantity();

            //checkout attributes
            if (request.Customer == null || !includeCheckoutAttributes)
                return totalWeight;
            var checkoutAttributesXml = _genericAttributeService.GetAttribute<string>(request.Customer, NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);
            if (string.IsNullOrEmpty(checkoutAttributesXml))
                return totalWeight;
            var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);
            foreach (var attributeValue in attributeValues)
                totalWeight += attributeValue.WeightAdjustment;

            return totalWeight;
        }

        /// <summary>
        /// Get dimensions of associated products (for quantity 1)
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="width">Width</param>
        /// <param name="length">Length</param>
        /// <param name="height">Height</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        public virtual void GetAssociatedProductDimensions(ShoppingCartItem shoppingCartItem,
            out decimal width, out decimal length, out decimal height, bool ignoreFreeShippedItems = false)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            width = length = height = decimal.Zero;

            //don't consider associated products dimensions
            if (!_shippingSettings.ConsiderAssociatedProductsDimensions)
                return;

            //attributes
            if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
                return;

            //bundled products (associated attributes)
            var attributeValues = _productAttributeParser.ParseProductAttributeValues(shoppingCartItem.AttributesXml)
                .Where(x => x.AttributeValueType == AttributeValueType.AssociatedToProduct).ToList();
            foreach (var attributeValue in attributeValues)
            {
                var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                if (associatedProduct == null || !associatedProduct.IsShipEnabled || (associatedProduct.IsFreeShipping && ignoreFreeShippedItems))
                    continue;

                width += associatedProduct.Width * attributeValue.Quantity;
                length += associatedProduct.Length * attributeValue.Quantity;
                height += associatedProduct.Height * attributeValue.Quantity;
            }
        }

        /// <summary>
        /// Get total dimensions
        /// </summary>
        /// <param name="packageItems">Package items</param>
        /// <param name="width">Width</param>
        /// <param name="length">Length</param>
        /// <param name="height">Height</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        public virtual void GetDimensions(IList<GetShippingOptionRequest.PackageItem> packageItems,
            out decimal width, out decimal length, out decimal height, bool ignoreFreeShippedItems = false)
        {
            if (packageItems == null)
                throw new ArgumentNullException(nameof(packageItems));

            //calculate cube root of volume, in case if the number of items more than 1
            if (_shippingSettings.UseCubeRootMethod && AreMultipleItems(packageItems))
            {
                //find max dimensions of the shipped items
                var maxWidth = packageItems.Max(item => !item.ShoppingCartItem.Product.IsFreeShipping || !ignoreFreeShippedItems
                    ? item.ShoppingCartItem.Product.Width : decimal.Zero);
                var maxLength = packageItems.Max(item => !item.ShoppingCartItem.Product.IsFreeShipping || !ignoreFreeShippedItems
                    ? item.ShoppingCartItem.Product.Length : decimal.Zero);
                var maxHeight = packageItems.Max(item => !item.ShoppingCartItem.Product.IsFreeShipping || !ignoreFreeShippedItems
                    ? item.ShoppingCartItem.Product.Height : decimal.Zero);

                //get total volume of the shipped items
                var totalVolume = packageItems.Sum(packageItem =>
                {
                    //product volume
                    var productVolume = !packageItem.ShoppingCartItem.Product.IsFreeShipping || !ignoreFreeShippedItems ?
                        packageItem.ShoppingCartItem.Product.Width * packageItem.ShoppingCartItem.Product.Length * packageItem.ShoppingCartItem.Product.Height : decimal.Zero;

                    //associated products volume
                    if (_shippingSettings.ConsiderAssociatedProductsDimensions && !string.IsNullOrEmpty(packageItem.ShoppingCartItem.AttributesXml))
                    {
                        productVolume += _productAttributeParser.ParseProductAttributeValues(packageItem.ShoppingCartItem.AttributesXml)
                            .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct).Sum(attributeValue =>
                            {
                                var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                                if (associatedProduct == null || !associatedProduct.IsShipEnabled || (associatedProduct.IsFreeShipping && ignoreFreeShippedItems))
                                    return 0;

                                //adjust max dimensions
                                maxWidth = Math.Max(maxWidth, associatedProduct.Width);
                                maxLength = Math.Max(maxLength, associatedProduct.Length);
                                maxHeight = Math.Max(maxHeight, associatedProduct.Height);

                                return attributeValue.Quantity * associatedProduct.Width * associatedProduct.Length * associatedProduct.Height;
                            });
                    }

                    //total volume of item
                    return productVolume * packageItem.GetQuantity();
                });

                //set dimensions as cube root of volume
                width = length = height = Convert.ToDecimal(Math.Pow(Convert.ToDouble(totalVolume), 1.0 / 3.0));

                //sometimes we have products with sizes like 1x1x20
                //that's why let's ensure that a maximum dimension is always preserved
                //otherwise, shipping rate computation methods can return low rates
                width = Math.Max(width, maxWidth);
                length = Math.Max(length, maxLength);
                height = Math.Max(height, maxHeight);
            }
            else
            {
                //summarize all values (very inaccurate with multiple items)
                width = length = height = decimal.Zero;
                foreach (var packageItem in packageItems)
                {
                    var productWidth = decimal.Zero;
                    var productLength = decimal.Zero;
                    var productHeight = decimal.Zero;
                    if (!packageItem.ShoppingCartItem.Product.IsFreeShipping || !ignoreFreeShippedItems)
                    {
                        productWidth = packageItem.ShoppingCartItem.Product.Width;
                        productLength = packageItem.ShoppingCartItem.Product.Length;
                        productHeight = packageItem.ShoppingCartItem.Product.Height;
                    }

                    //associated products
                    GetAssociatedProductDimensions(packageItem.ShoppingCartItem, out var associatedProductsWidth, out var associatedProductsLength, out var associatedProductsHeight);

                    var quantity = packageItem.GetQuantity();
                    width += (productWidth + associatedProductsWidth) * quantity;
                    length += (productLength + associatedProductsLength) * quantity;
                    height += (productHeight + associatedProductsHeight) * quantity;
                }
            }
        }

        /// <summary>
        /// Get the nearest warehouse for the specified address
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="warehouses">List of warehouses, if null all warehouses are used.</param>
        /// <returns></returns>
        public virtual Warehouse GetNearestWarehouse(Address address, IList<Warehouse> warehouses = null)
        {
            warehouses = warehouses ?? GetAllWarehouses();

            //no address specified. return any
            if (address == null)
                return warehouses.FirstOrDefault();

            //of course, we should use some better logic to find nearest warehouse
            //but we don't have a built-in geographic database which supports "distance" functionality
            //that's why we simply look for exact matches

            //find by country
            var matchedByCountry = new List<Warehouse>();
            foreach (var warehouse in warehouses)
            {
                var warehouseAddress = _addressService.GetAddressById(warehouse.AddressId);
                if (warehouseAddress == null)
                    continue;

                if (warehouseAddress.CountryId == address.CountryId)
                    matchedByCountry.Add(warehouse);
            }
            //no country matches. return any
            if (!matchedByCountry.Any())
                return warehouses.FirstOrDefault();

            //find by state
            var matchedByState = new List<Warehouse>();
            foreach (var warehouse in matchedByCountry)
            {
                var warehouseAddress = _addressService.GetAddressById(warehouse.AddressId);
                if (warehouseAddress == null)
                    continue;

                if (warehouseAddress.StateProvinceId == address.StateProvinceId)
                    matchedByState.Add(warehouse);
            }

            if (matchedByState.Any())
                return matchedByState.FirstOrDefault();

            //no state matches. return any
            return matchedByCountry.FirstOrDefault();
        }

        /// <summary>
        /// Create shipment packages (requests) from shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="shippingFromMultipleLocations">Value indicating whether shipping is done from multiple locations (warehouses)</param>
        /// <returns>Shipment packages (requests)</returns>
        public virtual IList<GetShippingOptionRequest> CreateShippingOptionRequests(IList<ShoppingCartItem> cart,
            Address shippingAddress, int storeId, out bool shippingFromMultipleLocations)
        {
            //if we always ship from the default shipping origin, then there's only one request
            //if we ship from warehouses ("ShippingSettings.UseWarehouseLocation" enabled),
            //then there could be several requests

            //key - warehouse identifier (0 - default shipping origin)
            //value - request
            var requests = new Dictionary<int, GetShippingOptionRequest>();

            //a list of requests with products which should be shipped separately
            var separateRequests = new List<GetShippingOptionRequest>();

            foreach (var sci in cart)
            {
                if (!IsShipEnabled(sci))
                    continue;

                var product = sci.Product;

                //TODO properly create requests for the associated products
                if (product == null || !product.IsShipEnabled)
                {
                    var associatedProducts = _productAttributeParser.ParseProductAttributeValues(sci.AttributesXml)
                        .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                        .Select(attributeValue => _productService.GetProductById(attributeValue.AssociatedProductId));
                    product = associatedProducts.FirstOrDefault(associatedProduct => associatedProduct != null && associatedProduct.IsShipEnabled);
                }

                if (product == null)
                    continue;

                //warehouses
                Warehouse warehouse = null;
                if (_shippingSettings.UseWarehouseLocation)
                {
                    if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                        product.UseMultipleWarehouses)
                    {
                        var allWarehouses = new List<Warehouse>();
                        //multiple warehouses supported
                        foreach (var pwi in product.ProductWarehouseInventory)
                        {
                            //TODO validate stock quantity when backorder is not allowed?
                            var tmpWarehouse = GetWarehouseById(pwi.WarehouseId);
                            if (tmpWarehouse != null)
                                allWarehouses.Add(tmpWarehouse);
                        }

                        warehouse = GetNearestWarehouse(shippingAddress, allWarehouses);
                    }
                    else
                    {
                        //multiple warehouses are not supported
                        warehouse = GetWarehouseById(product.WarehouseId);
                    }
                }

                var warehouseId = warehouse?.Id ?? 0;

                if (requests.ContainsKey(warehouseId) && !product.ShipSeparately)
                {
                    //add item to existing request
                    requests[warehouseId].Items.Add(new GetShippingOptionRequest.PackageItem(sci));
                }
                else
                {
                    //create a new request
                    var request = new GetShippingOptionRequest
                    {
                        //store
                        StoreId = storeId
                    };
                    //customer
                    request.Customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;
                    //ship to
                    request.ShippingAddress = shippingAddress;
                    //ship from
                    Address originAddress = null;
                    if (warehouse != null)
                    {
                        //warehouse address
                        originAddress = _addressService.GetAddressById(warehouse.AddressId);
                        request.WarehouseFrom = warehouse;
                    }

                    if (originAddress == null)
                    {
                        //no warehouse address. in this case use the default shipping origin
                        originAddress = _addressService.GetAddressById(_shippingSettings.ShippingOriginAddressId);
                    }

                    if (originAddress != null)
                    {
                        request.CountryFrom = originAddress.Country;
                        request.StateProvinceFrom = originAddress.StateProvince;
                        request.ZipPostalCodeFrom = originAddress.ZipPostalCode;
                        request.CountyFrom = originAddress.County;
                        request.CityFrom = originAddress.City;
                        request.AddressFrom = originAddress.Address1;
                    }

                    //whether this product should be shipped separately from other ones
                    if (product.ShipSeparately)
                    {
                        //whether product items should be shipped separately
                        if (_shippingSettings.ShipSeparatelyOneItemEach)
                        {
                            //add item with overridden quantity 1
                            request.Items.Add(new GetShippingOptionRequest.PackageItem(sci, 1));

                            //create separate requests for all product quantity
                            for (var i = 0; i < sci.Quantity; i++)
                            {
                                separateRequests.Add(request);
                            }
                        }
                        else
                        {
                            //all of product items should be shipped in a single box, so create the single separate request 
                            request.Items.Add(new GetShippingOptionRequest.PackageItem(sci));
                            separateRequests.Add(request);
                        }
                    }
                    else
                    {
                        //usual request
                        request.Items.Add(new GetShippingOptionRequest.PackageItem(sci));
                        requests.Add(warehouseId, request);
                    }
                }
            }

            //multiple locations?
            //currently we just compare warehouses
            //but we should also consider cases when several warehouses are located in the same address
            shippingFromMultipleLocations = requests.Select(x => x.Key).Distinct().Count() > 1;

            var result = requests.Values.ToList();
            result.AddRange(separateRequests);

            return result;
        }

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="allowedShippingRateComputationMethodSystemName">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Shipping options</returns>
        public virtual GetShippingOptionResponse GetShippingOptions(IList<ShoppingCartItem> cart,
            Address shippingAddress, Customer customer = null, string allowedShippingRateComputationMethodSystemName = "",
            int storeId = 0)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var result = new GetShippingOptionResponse();

            //create a package
            var shippingOptionRequests = CreateShippingOptionRequests(cart, shippingAddress, storeId, out var shippingFromMultipleLocations);
            result.ShippingFromMultipleLocations = shippingFromMultipleLocations;

            var shippingRateComputationMethods = _shippingPluginManager
                .LoadActivePlugins(customer, storeId, allowedShippingRateComputationMethodSystemName);
            if (!shippingRateComputationMethods.Any())
                return result;

            //request shipping options from each shipping rate computation methods
            foreach (var srcm in shippingRateComputationMethods)
            {
                //request shipping options (separately for each package-request)
                IList<ShippingOption> srcmShippingOptions = null;
                foreach (var shippingOptionRequest in shippingOptionRequests)
                {
                    var getShippingOptionResponse = srcm.GetShippingOptions(shippingOptionRequest);

                    if (getShippingOptionResponse.Success)
                    {
                        //success
                        if (srcmShippingOptions == null)
                        {
                            //first shipping option request
                            srcmShippingOptions = getShippingOptionResponse.ShippingOptions;
                        }
                        else
                        {
                            //get shipping options which already exist for prior requested packages for this scrm (i.e. common options)
                            srcmShippingOptions = srcmShippingOptions
                                .Where(existingso => getShippingOptionResponse.ShippingOptions.Any(newso => newso.Name == existingso.Name))
                                .ToList();

                            //and sum the rates
                            foreach (var existingso in srcmShippingOptions)
                            {
                                existingso.Rate += getShippingOptionResponse
                                    .ShippingOptions
                                    .First(newso => newso.Name == existingso.Name)
                                    .Rate;
                            }
                        }
                    }
                    else
                    {
                        //errors
                        foreach (var error in getShippingOptionResponse.Errors)
                        {
                            result.AddError(error);
                            _logger.Warning($"Shipping ({srcm.PluginDescriptor.FriendlyName}). {error}");
                        }
                        //clear the shipping options in this case
                        srcmShippingOptions = new List<ShippingOption>();
                        break;
                    }
                }

                //add this scrm's options to the result
                if (srcmShippingOptions == null)
                    continue;

                foreach (var so in srcmShippingOptions)
                {
                    //set system name if not set yet
                    if (string.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName))
                        so.ShippingRateComputationMethodSystemName = srcm.PluginDescriptor.SystemName;
                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                        so.Rate = _priceCalculationService.RoundPrice(so.Rate);
                    result.ShippingOptions.Add(so);
                }
            }

            if (_shippingSettings.ReturnValidOptionsIfThereAreAny)
            {
                //return valid options if there are any (no matter of the errors returned by other shipping rate computation methods).
                if (result.ShippingOptions.Any() && result.Errors.Any())
                    result.Errors.Clear();
            }

            //no shipping options loaded
            if (!result.ShippingOptions.Any() && !result.Errors.Any())
                result.Errors.Add(_localizationService.GetResource("Checkout.ShippingOptionCouldNotBeLoaded"));

            return result;
        }

        /// <summary>
        /// Gets available pickup points
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="providerSystemName">Filter by provider identifier; null to load pickup points of all providers</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Pickup points</returns>
        public virtual GetPickupPointsResponse GetPickupPoints(Address address, Customer customer = null,
            string providerSystemName = null, int storeId = 0)
        {
            var result = new GetPickupPointsResponse();

            var pickupPointsProviders = _pickupPluginManager.LoadActivePlugins(customer, storeId, providerSystemName);
            if (!pickupPointsProviders.Any())
                return result;

            var allPickupPoints = new List<PickupPoint>();
            foreach (var provider in pickupPointsProviders)
            {
                var pickPointsResponse = provider.GetPickupPoints(address);
                if (pickPointsResponse.Success)
                    allPickupPoints.AddRange(pickPointsResponse.PickupPoints);
                else
                {
                    foreach (var error in pickPointsResponse.Errors)
                    {
                        result.AddError(error);
                        _logger.Warning($"PickupPoints ({provider.PluginDescriptor.FriendlyName}). {error}");
                    }
                }
            }

            //any pickup points is enough
            if (allPickupPoints.Count <= 0)
                return result;

            result.Errors.Clear();
            result.PickupPoints = allPickupPoints.OrderBy(point => point.DisplayOrder).ThenBy(point => point.Name).ToList();

            return result;
        }

        /// <summary>
        /// Whether the shopping cart item is ship enabled
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>True if the shopping cart item requires shipping; otherwise false</returns>
        public virtual bool IsShipEnabled(ShoppingCartItem shoppingCartItem)
        {
            //whether the product requires shipping
            if (shoppingCartItem.Product != null && shoppingCartItem.Product.IsShipEnabled)
                return true;

            if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
                return false;

            //or whether associated products of the shopping cart item require shipping
            return _productAttributeParser.ParseProductAttributeValues(shoppingCartItem.AttributesXml)
                .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                .Any(attributeValue => _productService.GetProductById(attributeValue.AssociatedProductId)?.IsShipEnabled ?? false);
        }

        /// <summary>
        /// Whether the shopping cart item is free shipping
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>True if the shopping cart item is free shipping; otherwise false</returns>
        public virtual bool IsFreeShipping(ShoppingCartItem shoppingCartItem)
        {
            //first, check whether shipping is required
            if (!IsShipEnabled(shoppingCartItem))
                return true;

            //then whether the product is free shipping
            if (shoppingCartItem.Product != null && !shoppingCartItem.Product.IsFreeShipping)
                return false;

            if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
                return true;

            //and whether associated products of the shopping cart item is free shipping
            return _productAttributeParser.ParseProductAttributeValues(shoppingCartItem.AttributesXml)
                .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                .All(attributeValue => _productService.GetProductById(attributeValue.AssociatedProductId)?.IsFreeShipping ?? true);
        }

        /// <summary>
        /// Get the additional shipping charge
        /// </summary> 
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>The additional shipping charge of the shopping cart item</returns>
        public virtual decimal GetAdditionalShippingCharge(ShoppingCartItem shoppingCartItem)
        {
            //first, check whether shipping is free
            if (IsFreeShipping(shoppingCartItem))
                return decimal.Zero;

            //get additional shipping charge of the product
            var additionalShippingCharge = (shoppingCartItem.Product?.AdditionalShippingCharge ?? decimal.Zero) * shoppingCartItem.Quantity;

            if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
                return additionalShippingCharge;

            //and sum with associated products additional shipping charges
            additionalShippingCharge += _productAttributeParser.ParseProductAttributeValues(shoppingCartItem.AttributesXml)
                .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                .Sum(attributeValue => _productService.GetProductById(attributeValue.AssociatedProductId)?.AdditionalShippingCharge ?? decimal.Zero);

            return additionalShippingCharge;
        }

        #endregion

        #endregion
    }
}