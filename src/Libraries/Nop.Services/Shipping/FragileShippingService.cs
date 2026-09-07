using Nop.Core;
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

namespace Nop.Services.Shipping
{
    public class FragileShippingService : ShippingService
    {
        public FragileShippingService(
            IAddressService addressService,
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
            ShoppingCartSettings shoppingCartSettings
        ) : base(
            addressService,
            checkoutAttributeParser,
            countryService,
            customerService,
            genericAttributeService,
            localizationService,
            logger,
            pickupPluginManager,
            priceCalculationService,
            productAttributeParser,
            productService,
            shippingPluginManager,
            stateProvinceService,
            storeContext,
            warehouseService,
            shippingSettings,
            shoppingCartSettings
        )
        {
        }

        public override async Task<GetShippingOptionResponse> GetShippingOptionsAsync(IList<ShoppingCartItem> cart,
    Address shippingAddress, Customer customer = null, string allowedShippingRateComputationMethodSystemName = "",
    int storeId = 0)
        {
            var result = new GetShippingOptionResponse();
            var shippingOption = new ShippingOption
            {
                Name = "Fragile Shipping",
                Rate = 1.1m,
                Description = "Specialized Oversize Courier"
            };
            result.ShippingOptions.Add(shippingOption);            
            return await Task.FromResult(result);
        }
    }
}
