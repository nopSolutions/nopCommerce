using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Misc.AbcCore;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Plugin.Misc.AbcCore.Nop;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Tasks
{
    public class UpdateDeliveryOptionsTask : IScheduleTask
    {
        private readonly CoreSettings _coreSettings;
        private readonly IAbcDeliveryService _abcDeliveryService;
        private readonly IAbcProductAttributeService _abcProductAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IPriceFormatter _priceFormatter;
        

        private ProductAttribute _deliveryPickupOptionsProductAttribute;
        private ProductAttribute _haulAwayDeliveryProductAttribute;
        private ProductAttribute _haulAwayDeliveryInstallProductAttribute;

        public UpdateDeliveryOptionsTask(
            CoreSettings coreSettings,
            IAbcDeliveryService abcDeliveryService,
            IAbcProductAttributeService abcProductAttributeService,
            ICategoryService categoryService,
            ILogger logger,
            IPriceFormatter priceFormatter)
        {
            _coreSettings = coreSettings;
            _abcDeliveryService = abcDeliveryService;
            _abcProductAttributeService = abcProductAttributeService;
            _categoryService = categoryService;
            _logger = logger;
            _priceFormatter = priceFormatter;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            _deliveryPickupOptionsProductAttribute =
                await _abcProductAttributeService.GetProductAttributeByNameAsync(AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName);
            _haulAwayDeliveryProductAttribute =
                await _abcProductAttributeService.GetProductAttributeByNameAsync(AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName);
            _haulAwayDeliveryInstallProductAttribute =
                await _abcProductAttributeService.GetProductAttributeByNameAsync(AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName);

            var hasErrors = false;

            var abcDeliveryMaps = await _abcDeliveryService.GetAbcDeliveryMapsAsync();
            foreach (var map in abcDeliveryMaps)
            {
                var productIds = (await _categoryService.GetProductCategoriesByCategoryIdAsync(map.CategoryId)).Select(pc => pc.ProductId);
                foreach (var productId in productIds)
                {
                    try
                    {
                        await UpdateProductDeliveryOptionsAsync(map, productId);
                    }
                    catch (Exception e)
                    {
                        await _logger.InsertLogAsync(
                            LogLevel.Error,
                            $"Failure when updating delivery options for CategoryId {map.CategoryId}, ProductId {productId}",
                            e.ToString());
                        hasErrors = true;
                    }
                }
            }

            if (hasErrors)
            {
                throw new NopException("Failures occured when updating product delivery options.");
            }
        }

        private async System.Threading.Tasks.Task UpdateProductDeliveryOptionsAsync(
            AbcDeliveryMap map,
            int productId)
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

            var deliveryOptionsPam = (await _abcProductAttributeService.SaveProductAttributeMappingsAsync(
                    productId,
                    pams,
                    new string[] { })).SingleOrDefault();

            (int deliveryOptionsPamId,
             int? deliveryPavId,
             int? deliveryInstallPavId,
             decimal? deliveryPriceAdjustment,
             decimal? deliveryInstallPriceAdjustment) = await AddDeliveryOptionValuesAsync(deliveryOptionsPam.Id, productId, map);

             // Handle haul away here using the values above
        }

        private async System.Threading.Tasks.Task<(int pamId,
                                                   int? deliveryPavId,
                                                   int? deliveryInstallPavId,
                                                   decimal? deliveryPriceAdjustment,
                                                   decimal? deliveryInstallPriceAdjustment)> AddDeliveryOptionValuesAsync(
            int pamId,
            int productId,
            AbcDeliveryMap map)
        {
            var values = await _abcProductAttributeService.GetProductAttributeValuesAsync(pamId);

            var deliveryOnlyPav = values.Where(pav => pav.Name.Contains("Home Delivery (")).SingleOrDefault();
            deliveryOnlyPav = _abcDeliveryService.AddValue(
                pamId,
                deliveryOnlyPav,
                map.DeliveryOnly,
                "Home Delivery ({0}, FREE With Mail-In Rebate)",
                10,
                true);

            var deliveryInstallationPav = values.Where(pav => pav.Name.Contains("Home Delivery and Installation (")).SingleOrDefault();
            deliveryInstallationPav = _abcDeliveryService.AddValue(
                pamId,
                deliveryInstallationPav,
                map.DeliveryInstall,
                "Home Delivery and Installation ({0})",
                20,
                deliveryOnlyPav == null);

            return (pamId, deliveryOnlyPav?.Id, deliveryInstallationPav?.Id, deliveryOnlyPav?.PriceAdjustment, deliveryInstallationPav?.PriceAdjustment);
        }
    }
}
