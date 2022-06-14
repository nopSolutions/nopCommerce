using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Logging;
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
        private readonly IAbcProductAttributeService _abcProductAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IPriceFormatter _priceFormatter;

        private ProductAttribute _deliveryPickupOptionsProductAttribute;
        private ProductAttribute _haulAwayDeliveryProductAttribute;
        private ProductAttribute _haulAwayDeliveryInstallProductAttribute;

        public UpdateDeliveryOptionsTask(
            IAbcDeliveryService abcDeliveryService,
            IAbcProductAttributeService abcProductAttributeService,
            ICategoryService categoryService,
            ILogger logger,
            IPriceFormatter priceFormatter)
        {
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
                            $"Failure when updating delivery options for CategoryId {map.CategoryId}, Product ID {productId}",
                            e.StackTrace
                        );
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
                    new string[]
                    {
                        AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName,
                        AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName,
                    }
                )).SingleOrDefault();

            (int deliveryOptionsPamId,
             int? deliveryPavId,
             int? deliveryInstallPavId,
             decimal? deliveryPriceAdjustment,
             decimal? deliveryInstallPriceAdjustment) = await AddDeliveryOptionValuesAsync(deliveryOptionsPam.Id, productId, map);

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
            await _abcProductAttributeService.SaveProductAttributeMappingsAsync(
                productId,
                pams,
                new string[0]
            );

            await AddHaulAwayAsync(
                productId,
                map,
                deliveryOptionsPamId,
                deliveryPavId,
                deliveryInstallPavId,
                deliveryPriceAdjustment.HasValue ? deliveryPriceAdjustment.Value : 0M,
                deliveryInstallPriceAdjustment.HasValue ? deliveryInstallPriceAdjustment.Value : 0M
            );
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
            deliveryOnlyPav = await _abcDeliveryService.AddValueAsync(
                pamId,
                deliveryOnlyPav,
                map.DeliveryOnly,
                "Home Delivery ({0}, FREE With Mail-In Rebate)",
                10,
                true);

            var deliveryInstallationPav = values.Where(pav => pav.Name.Contains("Home Delivery and Installation (")).SingleOrDefault();
            deliveryInstallationPav = await _abcDeliveryService.AddValueAsync(
                pamId,
                deliveryInstallationPav,
                map.DeliveryInstall,
                "Home Delivery and Installation ({0})",
                20,
                deliveryOnlyPav == null);

            return (pamId, deliveryOnlyPav?.Id, deliveryInstallationPav?.Id, deliveryOnlyPav?.PriceAdjustment, deliveryInstallationPav?.PriceAdjustment);
        }

        private async System.Threading.Tasks.Task AddHaulAwayAsync(
            int productId,
            AbcDeliveryMap map,
            int deliveryOptionsPamId,
            int? deliveryPavId,
            int? deliveryInstallPavId,
            decimal deliveryPriceAdjustment,
            decimal deliveryInstallPriceAdjustment)
        {
            // Haulaway (Delivery)
            if (deliveryPavId.HasValue)
            {
                await AddHaulAwayAttributeAsync(
                    productId,
                    _haulAwayDeliveryProductAttribute.Id,
                    deliveryOptionsPamId,
                    deliveryPavId.Value,
                    map.DeliveryHaulway,
                    deliveryPriceAdjustment);
            }

            // Haulaway (Delivery/Install)
            if (deliveryInstallPavId.HasValue)
            {
                await AddHaulAwayAttributeAsync(
                    productId,
                    _haulAwayDeliveryInstallProductAttribute.Id,
                    deliveryOptionsPamId,
                    deliveryInstallPavId.Value,
                    map.DeliveryHaulwayInstall,
                    deliveryInstallPriceAdjustment);
            }
        }

        private async System.Threading.Tasks.Task AddHaulAwayAttributeAsync(
            int productId,
            int productAttributeId,
            int deliveryOptionsPamId,
            int deliveryOptionsPavId,
            int abcDeliveryMapItemNumber,
            decimal priceAdjustment)
        {
            var pam = await AddHaulAwayAttributeMappingAsync(
                productId,
                _haulAwayDeliveryProductAttribute.Id,
                deliveryOptionsPamId,
                deliveryOptionsPavId);
            if (pam == null)
            {
                return;
            }

            var pav = (await _abcProductAttributeService.GetProductAttributeValuesAsync(pam.Id)).FirstOrDefault();
            await _abcDeliveryService.AddValueAsync(
                pam.Id,
                pav,
                abcDeliveryMapItemNumber,
                "Remove Old Appliance ({0})",
                0,
                false,
                priceAdjustment);
        }

        private async System.Threading.Tasks.Task<ProductAttributeMapping> AddHaulAwayAttributeMappingAsync(
            int productId,
            int productAttributeId,
            int deliveryOptionsPamId,
            int? pavId)
        {
            if (pavId == null)
            {
                return null;
            }

            var pam = (await _abcProductAttributeService.GetProductAttributeMappingsByProductIdAsync(productId))
                                                    .SingleOrDefault(pam => pam.ProductAttributeId == productAttributeId);
            if (pam == null)
            {
                pam = new ProductAttributeMapping()
                {
                    ProductId = productId,
                    ProductAttributeId = productAttributeId,
                    AttributeControlType = AttributeControlType.Checkboxes,
                    TextPrompt = "Haul Away",
                    ConditionAttributeXml = $"<Attributes><ProductAttribute ID=\"{deliveryOptionsPamId}\"><ProductAttributeValue><Value>{pavId}</Value></ProductAttributeValue></ProductAttribute></Attributes>",
                };
                await _abcProductAttributeService.InsertProductAttributeMappingAsync(pam);
            }

            return pam;
        }
    }
}
