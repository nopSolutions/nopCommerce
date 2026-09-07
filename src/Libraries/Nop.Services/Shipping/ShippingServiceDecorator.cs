using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;

public class ShippingServiceDecorator : IShippingService
{
    private readonly IShippingService _original;
    private readonly FragileShippingService _fragile;

    private readonly IProductService _productService;

    public ShippingServiceDecorator(IShippingService original, FragileShippingService custom, IProductService productService)
    {
        _original = original;
        _fragile = custom;
        _productService = productService;
    }

    public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(IList<ShoppingCartItem> cart,
    Address shippingAddress, Customer customer = null, string allowedShippingRateComputationMethodSystemName = "",
    int storeId = 0)
    {
        var handler = await RequiresFragileHandlingAsync(cart) ? _fragile : _original;

        return await handler.GetShippingOptionsAsync(cart, shippingAddress, customer,
            allowedShippingRateComputationMethodSystemName, storeId);
    }

    private async Task<bool> RequiresFragileHandlingAsync(IList<ShoppingCartItem> cart)
    {
        decimal weight = 0;
        var anyFragile = false;

        foreach (var item in cart)
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId);
            weight += product.Weight * item.Quantity;
            anyFragile |= product.IsFragile;

            if (weight > 15 && anyFragile)
                return true;
        }

        return false;
    }

    public Task<decimal> GetShoppingCartItemWeightAsync(ShoppingCartItem shoppingCartItem, bool ignoreFreeShippedItems = false)
        => _original.GetShoppingCartItemWeightAsync(shoppingCartItem, ignoreFreeShippedItems);

    public Task<decimal> GetShoppingCartItemWeightAsync(Product product, string attributesXml, bool ignoreFreeShippedItems = false)
        => _original.GetShoppingCartItemWeightAsync(product, attributesXml, ignoreFreeShippedItems);

    public Task<decimal> GetTotalWeightAsync(GetShippingOptionRequest request, bool includeCheckoutAttributes = true, bool ignoreFreeShippedItems = false)
        => _original.GetTotalWeightAsync(request, includeCheckoutAttributes, ignoreFreeShippedItems);

    public Task<(decimal width, decimal length, decimal height)> GetDimensionsAsync(IList<GetShippingOptionRequest.PackageItem> packageItems, bool ignoreFreeShippedItems = false)
        => _original.GetDimensionsAsync(packageItems, ignoreFreeShippedItems);

    public Task<(IList<GetShippingOptionRequest> shipmentPackages, bool shippingFromMultipleLocations)> CreateShippingOptionRequestsAsync(IList<ShoppingCartItem> cart, Address shippingAddress, int storeId)
        => _original.CreateShippingOptionRequestsAsync(cart, shippingAddress, storeId);

    public Task<GetPickupPointsResponse> GetPickupPointsAsync(IList<ShoppingCartItem> cart, Address address, Customer customer = null, string providerSystemName = null, int storeId = 0)
        => _original.GetPickupPointsAsync(cart, address, customer, providerSystemName, storeId);

    public Task<bool> IsShipEnabledAsync(ShoppingCartItem shoppingCartItem)
        => _original.IsShipEnabledAsync(shoppingCartItem);

    public Task<bool> IsFreeShippingAsync(ShoppingCartItem shoppingCartItem)
        => _original.IsFreeShippingAsync(shoppingCartItem);

    public Task<decimal> GetAdditionalShippingChargeAsync(ShoppingCartItem shoppingCartItem)
        => _original.GetAdditionalShippingChargeAsync(shoppingCartItem);
}