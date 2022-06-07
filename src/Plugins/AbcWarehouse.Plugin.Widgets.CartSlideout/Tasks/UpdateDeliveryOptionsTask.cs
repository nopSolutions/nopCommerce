using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Plugin.Misc.AbcCore.Nop;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Tasks
{
    public class UpdateDeliveryOptionsTask : IScheduleTask
    {
        private readonly IAbcDeliveryService _abcDeliveryService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IAbcProductAttributeService _productAttributeService;

        private ProductAttribute _deliveryPickupOptionsProductAttribute;
        private ProductAttribute _haulAwayDeliveryProductAttribute;
        private ProductAttribute _haulAwayDeliveryInstallProductAttribute;

        public UpdateDeliveryOptionsTask(
            IAbcDeliveryService abcDeliveryService,
            ICategoryService categoryService,
            ILogger logger,
            IAbcProductAttributeService productAttributeService)
        {
            _abcDeliveryService = abcDeliveryService;
            _categoryService = categoryService;
            _logger = logger;
            _productAttributeService = productAttributeService;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await InitializeDeliveryOptionsProductAttributesAsync();

            var abcDeliveryMaps = await _abcDeliveryService.GetAbcDeliveryMapsAsync();
            foreach (var map in abcDeliveryMaps)
            {
                var productIds = (await _categoryService.GetProductCategoriesByCategoryIdAsync(map.CategoryId)).Select(pc => pc.ProductId);
                foreach (var productId in productIds)
                {
                    var pams = new List<ProductAttributeMapping>();
                    if (map.DeliveryOnly != 0 || map.DeliveryInstall != 0)
                    {
                        pams.Add(new ProductAttributeMapping()
                        {
                            ProductId = productId,
                            ProductAttributeId = _deliveryPickupOptionsProductAttribute.Id,
                            AttributeControlType = AttributeControlType.RadioList,
                        });
                    }

                    var deliveryOptionsPam = (await _productAttributeService.SaveProductAttributeMappingsAsync(
                            productId,
                            pams,
                            new string[]
                            {
                                AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName,
                                AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName,
                            }
                        )).SingleOrDefault();

                    if (map.DeliveryHaulway != 0)
                    {
                        pams.Add(new ProductAttributeMapping()
                        {
                            ProductId = productId,
                            ProductAttributeId = _haulAwayDeliveryProductAttribute.Id,
                            AttributeControlType = AttributeControlType.Checkboxes,
                            DisplayOrder = 10,
                            TextPrompt = AbcDeliveryConsts.HaulAwayTextPrompt

                            // Needs condition - need ID from delivery options
                        });
                    }

                    if (map.DeliveryHaulwayInstall != 0)
                    {
                        pams.Add(new ProductAttributeMapping()
                        {
                            ProductId = productId,
                            ProductAttributeId = _haulAwayDeliveryInstallProductAttribute.Id,
                            AttributeControlType = AttributeControlType.Checkboxes,
                            DisplayOrder = 20,
                            TextPrompt = AbcDeliveryConsts.HaulAwayTextPrompt

                            // Needs condition - need ID from delivery options
                        });
                    }

                    // Need to save now without Delivery Options
                    await _productAttributeService.SaveProductAttributeMappingsAsync(
                        productId,
                        pams,
                        new string[0]
                    );
                }
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
