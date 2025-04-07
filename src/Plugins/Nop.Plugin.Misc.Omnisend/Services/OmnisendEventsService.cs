using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Misc.Omnisend.DTO;
using Nop.Plugin.Misc.Omnisend.DTO.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Misc.Omnisend.Services;

/// <summary>
/// Events based Platform integration service
/// </summary>
public class OmnisendEventsService(IActionContextAccessor actionContextAccessor,
        IAddressService addressService,
        ICategoryService categoryService,
        ICountryService countryService,
        ICustomerService customerService,
        IDiscountService discountService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IManufacturerService manufacturerService,
        IMeasureService measureService,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPaymentPluginManager paymentPluginManager,
        IPictureService pictureService,
        IProductService productService,
        IProductTagService productTagService,
        IShipmentService shipmentService,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        ITaxService taxService,
        IUrlHelperFactory urlHelperFactory,
        IWebHelper webHelper,
        IWorkContext workContext,
        OmnisendCustomerService omnisendCustomerService,
        OmnisendHelper omnisendHelper,
        OmnisendHttpClient omnisendHttpClient)
{
    #region Utilities

    private async Task SendEventAsync(CustomerEvents customerEvent)
    {
        if (customerEvent == null)
            return;

        var data = JsonConvert.SerializeObject(customerEvent);
        await omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CustomerEventsApiUrl, data, HttpMethod.Post);
    }

    private async Task<CustomerEvents> CreateAddedProductToCartEventAsync(ShoppingCartItem shoppingCartItem)
    {
        var customer = await customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

        var customerEvent = await omnisendCustomerService.CreateCustomerEventAsync(customer,
            await PrepareAddedProductToCartPropertyAsync(shoppingCartItem));

        return customerEvent;
    }

    private async Task<CustomerEvents> CreateStartedCheckoutEventAsync()
    {
        var customer = await workContext.GetCurrentCustomerAsync();
        var customerEvent =
            await omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PrepareStartedCheckoutPropertyAsync(customer));

        return customerEvent;
    }

    private async Task<CustomerEvents> CreateOrderPlacedEventAsync(Order order)
    {
        var customer = await customerService.GetCustomerByIdAsync(order.CustomerId);

        var customerEvent =
            await omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PreparePlacedOrderPropertyAsync(order));

        return customerEvent;
    }

    private async Task<CustomerEvents> CreateOrderPaidEventAsync(Order order)
    {
        var customer = await customerService.GetCustomerByIdAsync(order.CustomerId);

        var customerEvent =
            await omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PreparePlacedPaidPropertyAsync(order));

        return customerEvent;
    }

    private async Task<CustomerEvents> CreateOrderCanceledEventAsync(Order order)
    {
        var customer = await customerService.GetCustomerByIdAsync(order.CustomerId);

        var customerEvent =
            await omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PrepareOrderCanceledPropertyAsync(order));

        return customerEvent;
    }

    private async Task<CustomerEvents> CreateOrderFulfilledEventAsync(Order order)
    {
        var customer = await customerService.GetCustomerByIdAsync(order.CustomerId);

        var customerEvent =
            await omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PrepareOrderFulfilledPropertyAsync(order));

        return customerEvent;
    }

    private async Task<CustomerEvents> CreateOrderRefundedEventAsync(Order order)
    {
        var customer = await customerService.GetCustomerByIdAsync(order.CustomerId);

        var customerEvent =
            await omnisendCustomerService.CreateCustomerEventAsync(customer,
                await PrepareOrderRefundedPropertyAsync(order));

        return customerEvent;
    }

    private async Task<AddedProductToCartProperty> PrepareAddedProductToCartPropertyAsync(
        ShoppingCartItem shoppingCartItem)
    {
        var customer = await customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);
        var cart = await shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart,
            shoppingCartItem.StoreId);

        var cartId = await omnisendCustomerService.GetCartIdAsync(customer);

        var property = new AddedProductToCartProperty
        {
            AbandonedCheckoutURL = omnisendCustomerService.GetAbandonedCheckoutUrl(cartId),
            CartId = cartId,
            Currency = await omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            LineItems =
                await cart.SelectAwait(async sci => await ShoppingCartItemToProductItemAsync(sci))
                    .ToListAsync(),
            Value = (await GetShoppingCartItemPriceAsync(shoppingCartItem)).price,
            AddedItem = await ShoppingCartItemToProductItemAsync(shoppingCartItem)
        };

        return property;
    }

    private async Task<StartedCheckoutProperty> PrepareStartedCheckoutPropertyAsync(Customer customer)
    {
        var store = await storeContext.GetCurrentStoreAsync();
        var cart = await shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        var cartSum = (await orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal ?? 0;
        var cartId = await omnisendCustomerService.GetCartIdAsync(customer);

        var property = new StartedCheckoutProperty
        {
            AbandonedCheckoutURL = omnisendCustomerService.GetAbandonedCheckoutUrl(cartId),
            CartId = cartId,
            Currency = await omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            LineItems = await cart.SelectAwait(async sci => await ShoppingCartItemToProductItemAsync(sci))
                .ToListAsync(),
            Value = (float)cartSum
        };

        return property;
    }

    private async Task<PlacedOrderProperty> PreparePlacedOrderPropertyAsync(Order order)
    {
        var property = new PlacedOrderProperty();
        await FillOrderEventBaseAsync(property, order);

        return property;
    }

    private async Task<PaidForOrderProperty> PreparePlacedPaidPropertyAsync(Order order)
    {
        var property = new PaidForOrderProperty();
        await FillOrderEventBaseAsync(property, order);

        return property;
    }

    private async Task<OrderCanceledProperty> PrepareOrderCanceledPropertyAsync(Order order)
    {
        var property = new OrderCanceledProperty();
        await FillOrderEventBaseAsync(property, order);
        property.CancelReason = null;

        return property;
    }

    private async Task<OrderFulfilledProperty> PrepareOrderFulfilledPropertyAsync(Order order)
    {
        var property = new OrderFulfilledProperty();
        await FillOrderEventBaseAsync(property, order);

        return property;
    }

    private async Task<OrderRefundedProperty> PrepareOrderRefundedPropertyAsync(Order order)
    {
        var property = new OrderRefundedProperty();
        await FillOrderEventBaseAsync(property, order);
        property.TotalRefundedAmount = (float)order.RefundedAmount;

        return property;
    }

    private async Task<ProductItem> ShoppingCartItemToProductItemAsync(ShoppingCartItem shoppingCartItem)
    {
        var product = await productService.GetProductByIdAsync(shoppingCartItem.ProductId);

        var (sku, variantId) =
            await omnisendHelper.GetSkuAndVariantIdAsync(product, shoppingCartItem.AttributesXml);
        var (price, discount) = await GetShoppingCartItemPriceAsync(shoppingCartItem);
        var picture = await pictureService.GetProductPictureAsync(product, shoppingCartItem.AttributesXml);
        var (pictureUrl, _) = await pictureService.GetPictureUrlAsync(picture);

        var productItem = new ProductItem
        {
            ProductCategories = await GetProductCategoriesAsync(product),
            ProductDescription = product.ShortDescription,
            ProductDiscount = discount,
            ProductId = product.Id,
            ProductImageURL = pictureUrl,
            ProductPrice = price,
            ProductQuantity = shoppingCartItem.Quantity,
            ProductSku = sku,
            ProductStrikeThroughPrice = (float)product.OldPrice,
            ProductTitle = product.Name,
            ProductURL = await omnisendHelper.GetProductUrlAsync(product),
            ProductVariantId = variantId,
            ProductVariantImageURL = pictureUrl
        };

        return productItem;
    }

    private async Task FillOrderEventBaseAsync(OrderEventBaseProperty property, Order order)
    {
        var urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);

        var items = await orderService.GetOrderItemsAsync(order.Id);
        var appliedDiscounts = await discountService.GetAllDiscountUsageHistoryAsync(orderId: order.Id);

        var paymentMethodName = await paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName) is { } plugin
            ? await localizationService.GetLocalizedFriendlyNameAsync(plugin, order.CustomerLanguageId)
            : order.PaymentMethodSystemName;

        property.BillingAddress = await GetAddressItemDataAsync(order.BillingAddressId);
        property.CreatedAt = order.CreatedOnUtc.ToDtoString();
        property.Currency = await omnisendHelper.GetPrimaryStoreCurrencyCodeAsync();
        property.Discounts = await appliedDiscounts.SelectAwait(async duh =>
        {
            var discount = await discountService.GetDiscountByIdAsync(duh.DiscountId);

            return new DiscountItem
            {
                Amount = (float)discount.DiscountAmount,
                Code = discount.CouponCode,
                Type = discount.DiscountType.ToString()
            };
        }).ToListAsync();
        property.FulfillmentStatus = order.OrderStatus.ToString();
        property.LineItems =
            await items.SelectAwait(async oi => await OrderItemToProductItemAsync(oi)).ToListAsync();
        property.Note = null;
        property.OrderId = order.CustomOrderNumber;
        property.OrderNumber = order.Id;
        property.OrderStatusURL = urlHelper.RouteUrl("OrderDetails", new { orderId = order.Id }, webHelper.GetCurrentRequestProtocol());
        property.PaymentMethod = paymentMethodName;
        property.PaymentStatus = order.PaymentStatus.ToString();
        property.ShippingAddress = await GetAddressItemDataAsync(order.ShippingAddressId);
        property.ShippingMethod = order.ShippingMethod;
        property.ShippingPrice = (float)order.OrderShippingInclTax;
        property.SubTotalPrice = (float)order.OrderSubtotalInclTax;
        property.SubTotalTaxIncluded = true;
        property.Tags = null;
        property.TotalDiscount = (float)order.OrderDiscount;
        property.TotalPrice = (float)order.OrderTotal;
        property.TotalTax = (float)order.OrderTax;

        if ((await shipmentService.GetShipmentsByOrderIdAsync(order.Id)).LastOrDefault() is { } shipment &&
            await shipmentService.GetShipmentTrackerAsync(shipment) is { } shipmentTracker)
            property.Tracking = new TrackingItem
            {
                Code = shipment.TrackingNumber,
                CourierURL = await shipmentTracker.GetUrlAsync(shipment.TrackingNumber, shipment)
            };
    }

    private async Task<OrderProductItem> OrderItemToProductItemAsync(OrderItem orderItem)
    {
        var product = await productService.GetProductByIdAsync(orderItem.ProductId);

        var (sku, variantId) = await omnisendHelper.GetSkuAndVariantIdAsync(product, orderItem.AttributesXml);

        var picture = await pictureService.GetProductPictureAsync(product, orderItem.AttributesXml);
        var (pictureUrl, _) = await pictureService.GetPictureUrlAsync(picture);

        var productManufacturer = (await manufacturerService.GetProductManufacturersByProductIdAsync(orderItem.ProductId)).FirstOrDefault();
        var manufacturer = await manufacturerService.GetManufacturerByIdAsync(productManufacturer?.ManufacturerId ?? 0);
        var productsTags = await productTagService.GetAllProductTagsByProductIdAsync(product.Id);

        var weight = await measureService.GetMeasureWeightBySystemKeywordAsync("grams") is { } measureWeight
            ? await measureService.ConvertFromPrimaryMeasureWeightAsync(orderItem.ItemWeight ?? 0, measureWeight)
            : 0;

        float discount = 0;

        if (orderItem.DiscountAmountInclTax > 0 && orderItem.Quantity > 0)
            discount = (float)orderItem.DiscountAmountInclTax / orderItem.Quantity;

        var productItem = new OrderProductItem
        {
            ProductCategories = await GetProductCategoriesAsync(product),
            ProductDescription = product.ShortDescription,
            ProductDiscount = discount,
            ProductId = product.Id,
            ProductImageURL = pictureUrl,
            ProductPrice = (float)orderItem.UnitPriceInclTax + discount,
            ProductQuantity = orderItem.Quantity,
            ProductSku = sku,
            ProductStrikeThroughPrice = (float)product.OldPrice,
            ProductTags = productsTags.Select(tag => tag.Name).ToList(),
            ProductTitle = product.Name,
            ProductURL = await omnisendHelper.GetProductUrlAsync(product),
            ProductVariantId = variantId,
            ProductVariantImageURL = pictureUrl,
            ProductVendor = manufacturer?.Name,
            ProductWeight = (int)weight
        };

        return productItem;
    }

    private async Task<AddressItem> GetAddressItemDataAsync(int? addressId)
    {
        var address = await addressService.GetAddressByIdAsync(addressId ?? 0);

        if (address == null)
            return null;

        var country = await countryService.GetCountryByIdAsync(address.CountryId ?? 0);
        var state = await stateProvinceService.GetStateProvinceByIdAsync(address.StateProvinceId ?? 0);

        return new AddressItem
        {
            Address1 = address.Address1,
            Address2 = address.Address2,
            City = address.City,
            Company = address.Company,
            Country = country?.Name,
            CountryCode = country?.TwoLetterIsoCode,
            FirstName = address.FirstName,
            LastName = address.LastName,
            Phone = address.PhoneNumber,
            State = state?.Name,
            StateCode = state?.Abbreviation,
            Zip = address.ZipPostalCode,
        };
    }

    private async Task<(float price, float discountAmount)> GetShoppingCartItemPriceAsync(
        ShoppingCartItem shoppingCartItem)
    {
        var customer = await customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);
        var product = await productService.GetProductByIdAsync(shoppingCartItem.ProductId);

        var (scSubTotal, discountAmount, _, _) =
            await shoppingCartService.GetSubTotalAsync(shoppingCartItem, true);
        var price = (float)(await taxService.GetProductPriceAsync(product, scSubTotal, true, customer)).price;

        return (price, (float)discountAmount);
    }

    private async Task<List<ProductItem.ProductItemCategories>> GetProductCategoriesAsync(Product product)
    {
        var productCategories = await categoryService.GetProductCategoriesByProductIdAsync(product.Id);

        return await productCategories.SelectAwait(async pc => new ProductItem.ProductItemCategories
        {
            Id = pc.Id,
            Title = (await categoryService.GetCategoryByIdAsync(pc.CategoryId)).Name
        }).ToListAsync();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Send "added product to cart" event
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task SendAddedProductToCartEventAsync(ShoppingCartItem shoppingCartItem)
    {
        await SendEventAsync(await CreateAddedProductToCartEventAsync(shoppingCartItem));
    }

    /// <summary>
    /// Send "order placed" event
    /// </summary>
    /// <param name="order">Order</param>
    public async Task SendOrderPlacedEventAsync(Order order)
    {
        await SendEventAsync(await CreateOrderPlacedEventAsync(order));
    }

    /// <summary>
    /// Send "order paid" event
    /// </summary>
    /// <param name="eventMessage">Order paid event</param>
    public async Task SendOrderPaidEventAsync(OrderPaidEvent eventMessage)
    {
        await SendEventAsync(await CreateOrderPaidEventAsync(eventMessage.Order));
    }

    /// <summary>
    /// Send "order refunded" event
    /// </summary>
    /// <param name="eventMessage">Order refunded event</param>
    public async Task SendOrderRefundedEventAsync(OrderRefundedEvent eventMessage)
    {
        if (eventMessage.Order.PaymentStatus == PaymentStatus.Refunded)
            await SendEventAsync(await CreateOrderRefundedEventAsync(eventMessage.Order));
    }

    /// <summary>
    /// Send "order canceled" or "order fulfilled" events
    /// </summary>
    /// <param name="eventMessage">Order status changed event</param>
    public async Task SendOrderStatusChangedEventAsync(OrderStatusChangedEvent eventMessage)
    {
        var order = eventMessage.Order;

        if (eventMessage.PreviousOrderStatus == order.OrderStatus)
            return;

        switch (order.OrderStatus)
        {
            case OrderStatus.Cancelled:
            {
                var sent = await genericAttributeService.GetAttributeAsync<bool>(order, OmnisendDefaults.OrderCanceledAttribute);

                if (sent)
                    return;

                await SendEventAsync(await CreateOrderCanceledEventAsync(order));

                await genericAttributeService.SaveAttributeAsync(order, OmnisendDefaults.OrderCanceledAttribute, true);

                break;
            }
            case OrderStatus.Complete:
            {
                var sent = await genericAttributeService.GetAttributeAsync<bool>(order, OmnisendDefaults.OrderFulfilledAttribute);

                if (sent)
                    return;

                await SendEventAsync(await CreateOrderFulfilledEventAsync(order));

                await genericAttributeService.SaveAttributeAsync(order, OmnisendDefaults.OrderFulfilledAttribute, true);

                break;
            }
        }
    }

    /// <summary>
    /// Send "started checkout" event
    /// </summary>
    /// <param name="eventMessage">Page rendering event</param>
    public async Task SendStartedCheckoutEventAsync(PageRenderingEvent eventMessage)
    {
        var routeName = eventMessage.GetRouteName();
        if (!routeName.Equals("CheckoutOnePage", StringComparison.InvariantCultureIgnoreCase) &&
            !routeName.Equals("CheckoutBillingAddress", StringComparison.InvariantCultureIgnoreCase))
            return;

        await SendEventAsync(await CreateStartedCheckoutEventAsync());
    }

    #endregion
}