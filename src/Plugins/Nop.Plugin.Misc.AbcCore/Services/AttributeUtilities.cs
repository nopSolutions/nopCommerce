using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Misc.AbcCore.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;
using SevenSpikes.Nop.Plugins.StoreLocator.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class AttributeUtilities : IAttributeUtilities
    {
        public readonly string PICKUP_ATTRIBUTE_NAME = "Pickup";
        public readonly string PICKUP_MSG_ATTRIBUTE_NAME = "Pickup Message";
        public readonly string HOME_DELIVERY_ATTRIBUTE_NAME = "Home Delivery";
        public readonly string WARRANTY_ATTRIBUTE_NAME = "Warranty";

        public readonly string HOME_DELIVERY_MESSAGE_ABC = "This item will be delivered to you by ABC";
        public readonly string HOME_DELIVERY_MESSAGE_HAWTHORNE = "This item will be delivered to you by Hawthorne";

        ISettingService _settingService;
        IShopService _shopService;
        IProductAttributeService _productAttributeService;
        IProductAttributeParser _productAttributeParser;

        private readonly IStoreService _storeService;

        IRepository<CustomerShopMapping> _customerShopMappingRepository;

        IWorkContext _workContext;

        public AttributeUtilities(ISettingService settingService,
            IShopService shopService,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            IRepository<CustomerShopMapping> customerShopMappingRepository,
            IWorkContext workContext,
            IStoreService storeService)
        {
            _settingService = settingService;
            _shopService = shopService;
            _productAttributeService = productAttributeService;
            _productAttributeParser = productAttributeParser;
            _customerShopMappingRepository = customerShopMappingRepository;
            _workContext = workContext;
            _storeService = storeService;
        }

        public async Task<string> InsertPickupAttributeAsync(
            Product product,
            StockResponse stockResponse,
            string attributes,
            Shop customerShop = null
        )
        {
            Shop shop = customerShop ?? await GetCustomerShop();
            string shopName = shop != null ? shop.Name : "Select Store";

            ProductAttribute pickupAttribute = await GetPickupAttributeAsync();

            // current product potential attribute mappings
            var pams =
                await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            var pickupAttributeMapping = pams
                .Where(pam => pam.ProductAttributeId == pickupAttribute.Id)
                .Select(pam => pam).FirstOrDefault();

            if (pickupAttributeMapping != null)
            {
                string pickupMsg = stockResponse.ProductStocks
                    .Where(ps => ps.Shop.Id == shop.Id)
                    .Select(ps => ps.Message).FirstOrDefault();

                attributes = _productAttributeParser.AddProductAttribute(attributes, pickupAttributeMapping, shop.Name + "\nAvailable: " + pickupMsg);

                // remove home delivery attributes
                var homeDeliveryAttributeMapping = await GetHomeDeliveryAttributeMappingAsync(attributes);

                if (homeDeliveryAttributeMapping != null)
                {
                    attributes = _productAttributeParser.RemoveProductAttribute(attributes, homeDeliveryAttributeMapping);
                }
            }
            return attributes;
        }

        public async Task<string> InsertHomeDeliveryAttributeAsync(Product product, string attributes)
        {
            var pams = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            ProductAttributeMapping hdProductAttributeMapping = null;
            foreach (var pam in pams)
            {
                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
                if (productAttribute != null && productAttribute.Name == HOME_DELIVERY_ATTRIBUTE_NAME)
                {
                    hdProductAttributeMapping = pam;
                    break;
                }
            }

            if (hdProductAttributeMapping != null)
            {
                string homeDeliveryMessage = await DetermineHomeDeliveryMessage();

                attributes = _productAttributeParser.AddProductAttribute(attributes, hdProductAttributeMapping,
                    homeDeliveryMessage);
            }
            // remove pickup attributes
            attributes = await RemovePickupAttributesAsync(attributes);
            return attributes;
        }

        public async Task<ProductAttributeMapping> GetPickupAttributeMappingAsync(string attributesXml)
        {
            return await GetAttributeMappingByNameAsync(attributesXml, PICKUP_ATTRIBUTE_NAME);
        }

        public async Task<ProductAttributeMapping> GetPickupMsgAttributeMapping(string attributesXml)
        {
            return await GetAttributeMappingByNameAsync(attributesXml, PICKUP_MSG_ATTRIBUTE_NAME);
        }

        public async Task<ProductAttributeMapping> GetHomeDeliveryAttributeMappingAsync(string attributesXml)
        {
            return await GetAttributeMappingByNameAsync(attributesXml, HOME_DELIVERY_ATTRIBUTE_NAME);
        }

        public async Task<ProductAttributeMapping> GetWarrantyAttributeMappingAsync(string attributesXml)
        {
            return await GetAttributeMappingByNameAsync(attributesXml, WARRANTY_ATTRIBUTE_NAME);
        }

        public async Task<string> RemovePickupAttributesAsync(string attributes)
        {
            var pickupAttributeMapping = await GetPickupAttributeMappingAsync(attributes);

            if (pickupAttributeMapping != null)
            {
                attributes = _productAttributeParser.RemoveProductAttribute(attributes, pickupAttributeMapping);
            }

            var pickupMsgAttributeMapping = await GetPickupMsgAttributeMapping(attributes);
            if (pickupMsgAttributeMapping != null)
            {
                attributes = _productAttributeParser.RemoveProductAttribute(attributes, pickupMsgAttributeMapping);
            }

            return attributes;
        }

        public async Task<ProductAttributeMapping> GetAttributeMappingByNameAsync(
            string attributesXml,
            string name
        ) {
            var productAttributeMappings =
                await _productAttributeParser.ParseProductAttributeMappingsAsync(attributesXml);

            foreach (var pam in productAttributeMappings)
            {
                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
                if (productAttribute == null) continue;

                if (productAttribute.Name == name) return pam;
            }
            
            return null;
        }

        private async Task<string> DetermineHomeDeliveryMessage()
        {
            var storeList = await _storeService.GetAllStoresAsync();
            Store abcWarehouseStore
                = storeList.Where(s => s.Name == "ABC Warehouse")
                .Select(s => s).FirstOrDefault();
            Store hawthorneStore
                = storeList.Where(s => s.Name == "Hawthorne Online")
                .Select(s => s).FirstOrDefault();

            if (abcWarehouseStore != null)
            {
                return HOME_DELIVERY_MESSAGE_ABC;
            }

            if (hawthorneStore != null
            {
                return HOME_DELIVERY_MESSAGE_HAWTHORNE;
            }

            return "";
        }

        // this adds the pickup in store attribute to the attribute string
        private async Task<ProductAttribute> GetPickupAttributeAsync()
        {
            var productAttributes =
                await _productAttributeService.GetAllProductAttributesAsync();
            var pickupAttribute = productAttributes
                .Where(pa => pa.Name == PICKUP_ATTRIBUTE_NAME)
                .Select(pa => pa).SingleOrDefault();

            if (pickupAttribute == null)
            {
                pickupAttribute = new ProductAttribute();
                pickupAttribute.Name = PICKUP_ATTRIBUTE_NAME;
                await _productAttributeService.InsertProductAttributeAsync(pickupAttribute);
            }

            return pickupAttribute;
        }

        private async Task<Shop> GetCustomerShop()
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            CustomerShopMapping csm = _customerShopMappingRepository.Table
                .Where(c => c.CustomerId == currentCustomer.Id)
                .Select(c => c).FirstOrDefault();

            // get the customer shop mapping, get the latest information about the store customer selected
            return await _shopService.GetShopByIdAsync(csm.ShopId);
        }
    }
}
