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
    public partial class UpdateDeliveryOptionsTask : IScheduleTask
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
        private ProductAttribute _pickupInStoreProductAttribute;

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
            _pickupInStoreProductAttribute =
                await _abcProductAttributeService.GetProductAttributeByNameAsync(AbcDeliveryConsts.LegacyPickupInStoreProductAttributeName);

            var hasErrors = false;

            var abcDeliveryMaps = await _abcDeliveryService.GetAbcDeliveryMapsAsync();
            foreach (var map in abcDeliveryMaps)
            {
                var productIds = (await _categoryService.GetProductCategoriesByCategoryIdAsync(map.CategoryId)).Select(pc => pc.ProductId);
                foreach (var productId in productIds)
                {
                    try
                    {
                        var deliveryOptionsPam = await UpdateDeliveryOptionsPamAsync(productId, map);
                        if (deliveryOptionsPam != null)
                        {
                            // update deliveryOptionsPav
                            var deliveryOptionPavs = await UpdateDeliveryOptionsPavAsync(deliveryOptionsPam.Id, map);

                            // update haulawayDeliveryPam/Pav

                            // update haulawayDeliveryInstallPam/Pav
                        }
                        // (int deliveryOptionsPamId,
                        //  int? deliveryPavId,
                        //  int? deliveryInstallPavId,
                        //  decimal? deliveryPriceAdjustment,
                        //  decimal? deliveryInstallPriceAdjustment) = await UpdateProductDeliveryOptionsAsync(map, productId);
                        // let's try doing haulaway in here instead
                        // await AddHaulAwayAsync(
                        //     productId,
                        //     map,
                        //     deliveryOptionsPamId,
                        //     deliveryPavId,
                        //     deliveryInstallPavId,
                        //     deliveryPriceAdjustment.HasValue ? deliveryPriceAdjustment.Value : 0M,
                        //     deliveryInstallPriceAdjustment.HasValue ? deliveryInstallPriceAdjustment.Value : 0M
                        // );
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

        private async System.Threading.Tasks.Task<ProductAttributeMapping> UpdateDeliveryOptionsPamAsync(
            int productId,
            AbcDeliveryMap map)
        {
            var existingPam =
                (await _abcProductAttributeService.GetProductAttributeMappingsByProductIdAsync(productId)).FirstOrDefault(
                    pam => pam.ProductAttributeId == _deliveryPickupOptionsProductAttribute.Id
                );
            // Only create if either delivery or delivery/install enabled for product
            var newPam = map.DeliveryOnly != 0 || map.DeliveryInstall != 0 ? new ProductAttributeMapping()
            {
                ProductId = productId,
                ProductAttributeId = _deliveryPickupOptionsProductAttribute.Id,
                AttributeControlType = AttributeControlType.RadioList,
            } : null;

            if (existingPam == null && newPam != null)
            {
                await _abcProductAttributeService.InsertProductAttributeMappingAsync(newPam);
                return newPam;
            }
            else if (existingPam != null && newPam == null)
            {
                await _abcProductAttributeService.DeleteProductAttributeMappingAsync(existingPam);
                return null;
            }
            
            return existingPam;
        }

        private async System.Threading.Tasks.Task<List<ProductAttributeValue>> UpdateDeliveryOptionsPavAsync(
            int deliveryOptionsPamId,
            AbcDeliveryMap map
        )
        {
            var existingValues = await _abcProductAttributeService.GetProductAttributeValuesAsync(deliveryOptionsPamId);
            var results = new List<ProductAttributeValue>();

            // Delivery only
            var deliveryOnlyItem = map.DeliveryOnly == 0 ? new AbcDeliveryItem() : await _abcDeliveryService.GetAbcDeliveryItemByItemNumberAsync(map.DeliveryOnly);
            var deliveryOnlyPriceFormatted = await _priceFormatter.FormatPriceAsync(deliveryOnlyItem.Price);

            var existingDeliveryOnlyPav = existingValues.Where(pav => pav.Name.Contains("Home Delivery (")).SingleOrDefault();
            var newDeliveryOnlyPav = map.DeliveryOnly != 0 ? new ProductAttributeValue()
            {
                ProductAttributeMappingId = deliveryOptionsPamId,
                Name = string.Format("Home Delivery ({0}, FREE With Mail-In Rebate)", deliveryOnlyPriceFormatted),
                Cost = map.DeliveryOnly,
                PriceAdjustment = deliveryOnlyItem.Price,
                IsPreSelected = true,
                DisplayOrder = 10,
            } : null;

            var resultDeliveryOnlyPav = await SaveProductAttributeValueAsync(existingDeliveryOnlyPav, newDeliveryOnlyPav)
            if (resultDeliveryOnlyPav != null) { results.Add(resultDeliveryOnlyPav) };

            // Delivery/Install

            // Pickup

            return results;
        }

        // private async System.Threading.Tasks.Task<(int pamId,
        //                                            int? deliveryPavId,
        //                                            int? deliveryInstallPavId,
        //                                            decimal? deliveryPriceAdjustment,
        //                                            decimal? deliveryInstallPriceAdjustment)> UpdateProductDeliveryOptionsAsync(
        //     AbcDeliveryMap map,
        //     int productId)
        // {
        //     (int deliveryOptionsPamId,
        //      int? deliveryPavId,
        //      int? deliveryInstallPavId,
        //      decimal? deliveryPriceAdjustment,
        //      decimal? deliveryInstallPriceAdjustment) = await AddDeliveryOptionValuesAsync(deliveryOptionsPam.Id, productId, map);
        // }

        // private async System.Threading.Tasks.Task<(int pamId,
        //                                            int? deliveryPavId,
        //                                            int? deliveryInstallPavId,
        //                                            decimal? deliveryPriceAdjustment,
        //                                            decimal? deliveryInstallPriceAdjustment)> AddDeliveryOptionValuesAsync(
        //     int pamId,
        //     int productId,
        //     AbcDeliveryMap map)
        // {
        //     // var values = await _abcProductAttributeService.GetProductAttributeValuesAsync(pamId);

        //     // var deliveryOnlyPav = values.Where(pav => pav.Name.Contains("Home Delivery (")).SingleOrDefault();
        //     // deliveryOnlyPav = _abcDeliveryService.AddValue(
        //     //     pamId,
        //     //     deliveryOnlyPav,
        //     //     map.DeliveryOnly,
        //     //     "Home Delivery ({0}, FREE With Mail-In Rebate)",
        //     //     10,
        //     //     true);

        //     // var deliveryInstallationPav = values.Where(pav => pav.Name.Contains("Home Delivery and Installation (")).SingleOrDefault();
        //     // deliveryInstallationPav = _abcDeliveryService.AddValue(
        //     //     pamId,
        //     //     deliveryInstallationPav,
        //     //     map.DeliveryInstall,
        //     //     "Home Delivery and Installation ({0})",
        //     //     20,
        //     //     deliveryOnlyPav == null);

        //     // Handle pickup in store via legacy product attribute, no need to return
        //     var pams = await _abcProductAttributeService.GetProductAttributeMappingsByProductIdAsync(productId);
        //     var pickupPam = await pams.WhereAwait(
        //         async pam => (await _abcProductAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId)).Name ==
        //                       AbcDeliveryConsts.LegacyPickupInStoreProductAttributeName).FirstOrDefaultAsync();
        //     if (pickupPam != null)
        //     {
        //         var pickupPav = values.Where(pav => pav.Name.Contains("Pickup In-Store")).SingleOrDefault();
        //         _abcDeliveryService.AddValue(
        //             pamId,
        //             pickupPav,
        //             // I think we'll need this info later, but not sure what it should be
        //             1,
        //             "Pickup In-Store Or Curbside (FREE)",
        //             0,
        //             false);
        //     }

        //     return (pamId,
        //             deliveryOnlyPav?.Id,
        //             deliveryInstallationPav?.Id,
        //             deliveryOnlyPav?.PriceAdjustment,
        //             deliveryInstallationPav?.PriceAdjustment);
        // }

        private bool DoProductAttributeValuesMatch(ProductAttributeValue existingPav, ProductAttributeValue newPav)
        {
            return existingPav.Name == newPav.Name;
        }

        private async Task<ProductAttributeValue> SaveProductAttributeValueAsync(ProductAttributeValue existingPav, ProductAttributeValue newPav)
        {
            if (existingPav == null && newPav != null)
            {
                await _abcProductAttributeService.InsertProductAttributeValueAsync(newPav);
                return newPav;
            }
            else if (existingPav != null && newPav == null)
            {
                await _abcProductAttributeService.DeleteProductAttributeValueAsync(existingPav);
            }
            else if (!existingPav.Equals(newPav))
            {
                existingPav.Name = newPav.Name;
                await _abcProductAttributeService.UpdateProductAttributeValueAsync(existingPav);
                return existingPav;
            }

            return existingPav;
        }
    }
}
