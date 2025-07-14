﻿using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Shipping.Pickup;

namespace Nop.Services.Shipping;

/// <summary>
/// Shipping service
/// </summary>
public partial class ShippingService : IShippingService
{
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IPickupPluginManager _pickupPluginManager;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductService _productService;
    protected readonly IShippingPluginManager _shippingPluginManager;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWarehouseService _warehouseService;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly ShoppingCartSettings _shoppingCartSettings;

    #endregion

    #region Ctor

    public ShippingService(IAddressService addressService,
        IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        ICountryService countryService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILogger logger,
        IPickupPluginManager pickupPluginManager,
        IPriceCalculationService priceCalculationService,
        IProductAttributeParser productAttributeParser,
        IProductService productService,
        IShippingPluginManager shippingPluginManager,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IWarehouseService warehouseService,
        ShippingSettings shippingSettings,
        ShoppingCartSettings shoppingCartSettings)
    {
        _addressService = addressService;
        _checkoutAttributeParser = checkoutAttributeParser;
        _countryService = countryService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _logger = logger;
        _pickupPluginManager = pickupPluginManager;
        _priceCalculationService = priceCalculationService;
        _productAttributeParser = productAttributeParser;
        _productService = productService;
        _shippingPluginManager = shippingPluginManager;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _warehouseService = warehouseService;
        _shippingSettings = shippingSettings;
        _shoppingCartSettings = shoppingCartSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Check whether there are multiple package items in the cart for the delivery
    /// </summary>
    /// <param name="items">Package items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if there are multiple items; otherwise false
    /// </returns>
    protected virtual async Task<bool> AreMultipleItemsAsync(IList<GetShippingOptionRequest.PackageItem> items)
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
        var associatedAttributeValues = (await _productAttributeParser.ParseProductAttributeValuesAsync(singleItem.ShoppingCartItem.AttributesXml))
            .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct);

        //whether to ship associated products
        return await associatedAttributeValues.AnyAwaitAsync(async attributeValue =>
            (await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId))?.IsShipEnabled ?? false);
    }

    /// <summary>
    /// Get dimensions of associated products (for quantity 1)
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the width. Length. Height
    /// </returns>
    protected virtual async Task<(decimal width, decimal length, decimal height)> GetAssociatedProductDimensionsAsync(ShoppingCartItem shoppingCartItem,
        bool ignoreFreeShippedItems = false)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartItem);

        decimal length;
        decimal height;

        var width = length = height = decimal.Zero;

        //don't consider associated products dimensions
        if (!_shippingSettings.ConsiderAssociatedProductsDimensions)
            return (width, length, height);

        //attributes
        if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
            return (width, length, height);

        //bundled products (associated attributes)
        var attributeValues = (await _productAttributeParser.ParseProductAttributeValuesAsync(shoppingCartItem.AttributesXml))
            .Where(x => x.AttributeValueType == AttributeValueType.AssociatedToProduct).ToList();
        foreach (var attributeValue in attributeValues)
        {
            var associatedProduct = await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId);
            if (associatedProduct == null || !associatedProduct.IsShipEnabled || (associatedProduct.IsFreeShipping && ignoreFreeShippedItems))
                continue;

            width += associatedProduct.Width * attributeValue.Quantity;
            length += associatedProduct.Length * attributeValue.Quantity;
            height += associatedProduct.Height * attributeValue.Quantity;
        }

        return (width, length, height);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets shopping cart item weight (of one item)
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart item weight
    /// </returns>
    public virtual async Task<decimal> GetShoppingCartItemWeightAsync(ShoppingCartItem shoppingCartItem, bool ignoreFreeShippedItems = false)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartItem);

        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);

        return await GetShoppingCartItemWeightAsync(product, shoppingCartItem.AttributesXml, ignoreFreeShippedItems);
    }

    /// <summary>
    /// Gets product item weight (of one item)
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Selected product attributes in XML</param>
    /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the item weight
    /// </returns>
    public virtual async Task<decimal> GetShoppingCartItemWeightAsync(Product product, string attributesXml, bool ignoreFreeShippedItems = false)
    {
        if (product == null)
            return decimal.Zero;

        //product weight
        var productWeight = !product.IsFreeShipping || !ignoreFreeShippedItems ? product.Weight : decimal.Zero;

        //attribute weight
        var attributesTotalWeight = decimal.Zero;

        if (!_shippingSettings.ConsiderAssociatedProductsDimensions || string.IsNullOrEmpty(attributesXml))
            return productWeight + attributesTotalWeight;

        var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
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
                    var associatedProduct = await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId);
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the otal weight
    /// </returns>
    public virtual async Task<decimal> GetTotalWeightAsync(GetShippingOptionRequest request,
        bool includeCheckoutAttributes = true, bool ignoreFreeShippedItems = false)
    {
        ArgumentNullException.ThrowIfNull(request);

        var totalWeight = decimal.Zero;

        //shopping cart items
        foreach (var packageItem in request.Items)
            totalWeight += await GetShoppingCartItemWeightAsync(packageItem.ShoppingCartItem, ignoreFreeShippedItems) * packageItem.GetQuantity();

        //checkout attributes
        if (request.Customer is null || !includeCheckoutAttributes)
            return totalWeight;
        var store = await _storeContext.GetCurrentStoreAsync();
        var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(request.Customer, NopCustomerDefaults.CheckoutAttributes, store.Id);
        if (string.IsNullOrEmpty(checkoutAttributesXml))
            return totalWeight;
        var attributeValues = _checkoutAttributeParser.ParseAttributeValues(checkoutAttributesXml);
        foreach (var attributeValue in await attributeValues.SelectMany(x => x.values).ToListAsync())
            totalWeight += attributeValue.WeightAdjustment;

        return totalWeight;
    }

    /// <summary>
    /// Get total dimensions
    /// </summary>
    /// <param name="packageItems">Package items</param>
    /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the width. Length. Height
    /// </returns>
    public virtual async Task<(decimal width, decimal length, decimal height)> GetDimensionsAsync(IList<GetShippingOptionRequest.PackageItem> packageItems, bool ignoreFreeShippedItems = false)
    {
        ArgumentNullException.ThrowIfNull(packageItems);

        decimal length;
        decimal height;
        decimal width;

        //calculate cube root of volume, in case if the number of items more than 1
        if (_shippingSettings.UseCubeRootMethod && await AreMultipleItemsAsync(packageItems))
        {
            //find max dimensions of the shipped items
            var maxWidth = packageItems.Max(item => !item.Product.IsFreeShipping || !ignoreFreeShippedItems
                ? item.Product.Width : decimal.Zero);
            var maxLength = packageItems.Max(item => !item.Product.IsFreeShipping || !ignoreFreeShippedItems
                ? item.Product.Length : decimal.Zero);
            var maxHeight = packageItems.Max(item => !item.Product.IsFreeShipping || !ignoreFreeShippedItems
                ? item.Product.Height : decimal.Zero);

            //get total volume of the shipped items
            var totalVolume = await packageItems.SumAwaitAsync(async packageItem =>
            {
                //product volume
                var productVolume = !packageItem.Product.IsFreeShipping || !ignoreFreeShippedItems ?
                    packageItem.Product.Width * packageItem.Product.Length * packageItem.Product.Height : decimal.Zero;

                //associated products volume
                if (_shippingSettings.ConsiderAssociatedProductsDimensions && !string.IsNullOrEmpty(packageItem.ShoppingCartItem.AttributesXml))
                {
                    productVolume += await (await _productAttributeParser.ParseProductAttributeValuesAsync(packageItem.ShoppingCartItem.AttributesXml))
                        .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct).SumAwaitAsync(async attributeValue =>
                        {
                            var associatedProduct = await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId);
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
                if (!packageItem.Product.IsFreeShipping || !ignoreFreeShippedItems)
                {
                    productWidth = packageItem.Product.Width;
                    productLength = packageItem.Product.Length;
                    productHeight = packageItem.Product.Height;
                }

                //associated products
                var (associatedProductsWidth, associatedProductsLength, associatedProductsHeight) = await GetAssociatedProductDimensionsAsync(packageItem.ShoppingCartItem);

                var quantity = packageItem.GetQuantity();
                width += (productWidth + associatedProductsWidth) * quantity;
                length += (productLength + associatedProductsLength) * quantity;
                height += (productHeight + associatedProductsHeight) * quantity;
            }
        }

        return (width, length, height);
    }

    /// <summary>
    /// Create shipment packages (requests) from shopping cart
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <param name="shippingAddress">Shipping address</param>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment packages (requests). Value indicating whether shipping is done from multiple locations (warehouses)
    /// </returns>
    public virtual async Task<(IList<GetShippingOptionRequest> shipmentPackages, bool shippingFromMultipleLocations)> CreateShippingOptionRequestsAsync(IList<ShoppingCartItem> cart,
        Address shippingAddress, int storeId)
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
            if (!await IsShipEnabledAsync(sci))
                continue;

            var product = await _productService.GetProductByIdAsync(sci.ProductId);

            if (product == null || !product.IsShipEnabled)
            {
                var associatedProducts = await (await _productAttributeParser.ParseProductAttributeValuesAsync(sci.AttributesXml))
                    .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    .SelectAwait(async attributeValue => await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId)).ToListAsync();
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
                    foreach (var pwi in await _productService.GetAllProductWarehouseInventoryRecordsAsync(product.Id))
                    {
                        var tmpWarehouse = await _warehouseService.GetWarehouseByIdAsync(pwi.WarehouseId);
                        if (tmpWarehouse != null)
                            allWarehouses.Add(tmpWarehouse);
                    }

                    warehouse = await _warehouseService.GetNearestWarehouseAsync(shippingAddress, allWarehouses);
                }
                else
                {
                    //multiple warehouses are not supported
                    warehouse = await _warehouseService.GetWarehouseByIdAsync(product.WarehouseId);
                }
            }

            var warehouseId = warehouse?.Id ?? 0;

            //add item to existing request
            if (requests.TryGetValue(warehouseId, out var value) && !product.ShipSeparately)
            {
                value.Items.Add(new GetShippingOptionRequest.PackageItem(sci, product));
            }
            else
            {
                //create a new request
                var request = new GetShippingOptionRequest
                {
                    //store
                    StoreId = storeId,
                    //customer
                    Customer = await _customerService.GetShoppingCartCustomerAsync(cart),

                    //ship to
                    ShippingAddress = shippingAddress
                };
                //ship from
                Address originAddress = null;
                if (warehouse != null)
                {
                    //warehouse address
                    originAddress = await _addressService.GetAddressByIdAsync(warehouse.AddressId);
                    request.WarehouseFrom = warehouse;
                }

                //no warehouse address. in this case use the default shipping origin
                originAddress ??= await _addressService.GetAddressByIdAsync(_shippingSettings.ShippingOriginAddressId);

                if (originAddress != null)
                {
                    request.CountryFrom = await _countryService.GetCountryByAddressAsync(originAddress);
                    request.StateProvinceFrom = await _stateProvinceService.GetStateProvinceByAddressAsync(originAddress);
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
                        request.Items.Add(new GetShippingOptionRequest.PackageItem(sci, product, 1));

                        //create separate requests for all product quantity
                        for (var i = 0; i < sci.Quantity; i++)
                        {
                            separateRequests.Add(request);
                        }
                    }
                    else
                    {
                        //all of product items should be shipped in a single box, so create the single separate request 
                        request.Items.Add(new GetShippingOptionRequest.PackageItem(sci, product));
                        separateRequests.Add(request);
                    }
                }
                else
                {
                    //usual request
                    request.Items.Add(new GetShippingOptionRequest.PackageItem(sci, product));
                    requests.Add(warehouseId, request);
                }
            }
        }

        //multiple locations?
        //currently we just compare warehouses
        //but we should also consider cases when several warehouses are located in the same address
        var shippingFromMultipleLocations = requests.Select(x => x.Key).Distinct().Count() > 1;

        var result = requests.Values.ToList();
        result.AddRange(separateRequests);

        return (result, shippingFromMultipleLocations);
    }

    /// <summary>
    ///  Gets available shipping options
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <param name="shippingAddress">Shipping address</param>
    /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
    /// <param name="allowedShippingRateComputationMethodSystemName">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping options
    /// </returns>
    public virtual async Task<GetShippingOptionResponse> GetShippingOptionsAsync(IList<ShoppingCartItem> cart,
        Address shippingAddress, Customer customer = null, string allowedShippingRateComputationMethodSystemName = "",
        int storeId = 0)
    {
        ArgumentNullException.ThrowIfNull(cart);

        var result = new GetShippingOptionResponse();

        //create a package
        var (shippingOptionRequests, shippingFromMultipleLocations) = await CreateShippingOptionRequestsAsync(cart, shippingAddress, storeId);
        result.ShippingFromMultipleLocations = shippingFromMultipleLocations;

        var shippingRateComputationMethods = await _shippingPluginManager
            .LoadActivePluginsAsync(customer, storeId, allowedShippingRateComputationMethodSystemName);
        if (!shippingRateComputationMethods.Any())
            return result;

        //request shipping options from each shipping rate computation methods
        foreach (var srcm in shippingRateComputationMethods)
        {
            //request shipping options (separately for each package-request)
            IList<ShippingOption> srcmShippingOptions = null;
            foreach (var shippingOptionRequest in shippingOptionRequests)
            {
                var getShippingOptionResponse = await srcm.GetShippingOptionsAsync(shippingOptionRequest);

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
                        await _logger.WarningAsync($"Shipping ({srcm.PluginDescriptor.FriendlyName}). {error}");
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
                    so.Rate = await _priceCalculationService.RoundPriceAsync(so.Rate);
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
            result.Errors.Add(await _localizationService.GetResourceAsync("Checkout.ShippingOptionCouldNotBeLoaded"));

        return result;
    }

    /// <summary>
    /// Gets available pickup points
    /// </summary>
    /// <param name="cart">Shopping Cart</param>
    /// <param name="address">Address</param>
    /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
    /// <param name="providerSystemName">Filter by provider identifier; null to load pickup points of all providers</param>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pickup points
    /// </returns>
    public virtual async Task<GetPickupPointsResponse> GetPickupPointsAsync(IList<ShoppingCartItem> cart, Address address,
        Customer customer = null, string providerSystemName = null, int storeId = 0)
    {
        var result = new GetPickupPointsResponse();

        var pickupPointsProviders = await _pickupPluginManager.LoadActivePluginsAsync(customer, storeId, providerSystemName);
        if (!pickupPointsProviders.Any())
            return result;

        var allPickupPoints = new List<PickupPoint>();
        foreach (var provider in pickupPointsProviders)
        {
            var pickPointsResponse = await provider.GetPickupPointsAsync(cart, address);
            if (pickPointsResponse.Success)
                allPickupPoints.AddRange(pickPointsResponse.PickupPoints);
            else
            {
                foreach (var error in pickPointsResponse.Errors)
                {
                    result.AddError(error);
                    await _logger.WarningAsync($"PickupPoints ({provider.PluginDescriptor.FriendlyName}). {error}");
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if the shopping cart item requires shipping; otherwise false
    /// </returns>
    public virtual async Task<bool> IsShipEnabledAsync(ShoppingCartItem shoppingCartItem)
    {
        //whether the product requires shipping
        if (shoppingCartItem.ProductId != 0 && (await _productService.GetProductByIdAsync(shoppingCartItem.ProductId))?.IsShipEnabled == true)
            return true;

        if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
            return false;

        //or whether associated products of the shopping cart item require shipping
        return await (await _productAttributeParser.ParseProductAttributeValuesAsync(shoppingCartItem.AttributesXml))
            .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
            .AnyAwaitAsync(async attributeValue => (await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId))?.IsShipEnabled ?? false);
    }

    /// <summary>
    /// Whether the shopping cart item is free shipping
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if the shopping cart item is free shipping; otherwise false
    /// </returns>
    public virtual async Task<bool> IsFreeShippingAsync(ShoppingCartItem shoppingCartItem)
    {
        //first, check whether shipping is required
        if (!await IsShipEnabledAsync(shoppingCartItem))
            return true;

        //then whether the product is free shipping
        if (shoppingCartItem.ProductId != 0 && !(await _productService.GetProductByIdAsync(shoppingCartItem.ProductId)).IsFreeShipping)
            return false;

        if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
            return true;

        //and whether associated products of the shopping cart item is free shipping
        return await (await _productAttributeParser.ParseProductAttributeValuesAsync(shoppingCartItem.AttributesXml))
            .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
            .AllAwaitAsync(async attributeValue => (await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId))?.IsFreeShipping ?? true);
    }

    /// <summary>
    /// Get the additional shipping charge
    /// </summary> 
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the additional shipping charge of the shopping cart item
    /// </returns>
    public virtual async Task<decimal> GetAdditionalShippingChargeAsync(ShoppingCartItem shoppingCartItem)
    {
        //first, check whether shipping is free
        if (await IsFreeShippingAsync(shoppingCartItem))
            return decimal.Zero;

        //get additional shipping charge of the product
        var additionalShippingCharge = ((await _productService.GetProductByIdAsync(shoppingCartItem.ProductId))?.AdditionalShippingCharge ?? decimal.Zero) * shoppingCartItem.Quantity;

        if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
            return additionalShippingCharge;

        //and sum with associated products additional shipping charges
        additionalShippingCharge += await (await _productAttributeParser.ParseProductAttributeValuesAsync(shoppingCartItem.AttributesXml))
            .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
            .SumAwaitAsync(async attributeValue => (await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId))?.AdditionalShippingCharge ?? decimal.Zero);

        return additionalShippingCharge;
    }

    #endregion
}