using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Plugin.Misc.AbcCore.Nop;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Tasks
{
    public class UpdateDeliveryOptionsTask : IScheduleTask
    {
        private readonly IAbcDeliveryService _abcDeliveryService;
        private readonly ILogger _logger;
        private readonly IAbcProductAttributeService _productAttributeService;

        private ProductAttribute _deliveryPickupOptionsProductAttribute;
        private ProductAttribute _haulAwayDeliveryProductAttribute;
        private ProductAttribute _haulAwayDeliveryInstallProductAttribute;

        public UpdateDeliveryOptionsTask(
            IAbcDeliveryService abcDeliveryService,
            ILogger logger,
            IAbcProductAttributeService productAttributeService)
        {
            _abcDeliveryService = abcDeliveryService;
            _logger = logger;
            _productAttributeService = productAttributeService;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await InitializeDeliveryOptionsProductAttributesAsync();

            var abcDeliveryMaps = await _abcDeliveryService.GetAbcDeliveryMapsAsync();
            foreach (var abcDeliveryMap in abcDeliveryMaps)
            {
                await _logger.InformationAsync($"AbcDeliveryMap Category ID: {abcDeliveryMap.CategoryId}");

                // Get products from a category

                // Apply to each product
            }
        }

        private async System.Threading.Tasks.Task InitializeDeliveryOptionsProductAttributesAsync()
        {
            _deliveryPickupOptionsProductAttribute = await SaveProductAttributeAsync(AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName);
            _haulAwayDeliveryProductAttribute = await SaveProductAttributeAsync(AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName);
            _haulAwayDeliveryInstallProductAttribute = await SaveProductAttributeAsync(AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName);
        }

        private async System.Threading.Tasks.Task<ProductAttribute> SaveProductAttributeAsync(string name)
        {
            var attribute = new ProductAttribute()
            {
                Name = name
            };
            await _productAttributeService.SaveProductAttributeAsync(attribute);

            return attribute;
        }
    }
}
