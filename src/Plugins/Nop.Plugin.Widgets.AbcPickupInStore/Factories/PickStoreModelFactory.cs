using Nop.Plugin.Misc.AbcCore.Domain;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.AbcPickupInStore.Models;
using Nop.Plugin.Misc.AbcCore.Services;
using SevenSpikes.Nop.Plugins.StoreLocator.Services;
using Nop.Services.Orders;
using Nop.Core;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain;

namespace Nop.Plugin.Widgets.AbcPickupInStore.Factories
{
    public class PickStoreModelFactory : IPickStoreModelFactory
    {
        private readonly ICustomerShopService _customerShopService;
        private readonly IShopService _shopService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;

        private readonly PickupInStoreSettings _pickupInStoreSettings;
        private readonly StoreLocatorSettings _storeLocatorSettings;

        public PickStoreModelFactory(
            ICustomerShopService customerShopService,
            IShopService shopService,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext,
            PickupInStoreSettings pickupInStoreSettings,
            StoreLocatorSettings storeLocatorSettings
        ) {
            _customerShopService = customerShopService;
            _shopService = shopService;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _pickupInStoreSettings = pickupInStoreSettings;
            _storeLocatorSettings = storeLocatorSettings;
        }

        public async Task<PickStoreModel> InitializePickStoreModelAsync()
        {
            // initialize model for the view
            PickStoreModel model = new PickStoreModel();

            // get the store that the customer selected previously if selected at all
            CustomerShopMapping currentCustomerShopMapping
                = _customerShopService.GetCurrentCustomerShopMapping((await _workContext.GetCurrentCustomerAsync()).Id);

            var shoppingCartItems = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync());

            // if selected before, add it to the model, else -1
            if (currentCustomerShopMapping != null)
            {
                model.SelectedShop = await _shopService.GetShopByIdAsync(currentCustomerShopMapping.ShopId);
            }

            model.PickupInStoreText = _pickupInStoreSettings.PickupInStoreText;
            model.GoogleMapsAPIKey = _storeLocatorSettings.GoogleApiKey;

            return model;
        }
    }
}