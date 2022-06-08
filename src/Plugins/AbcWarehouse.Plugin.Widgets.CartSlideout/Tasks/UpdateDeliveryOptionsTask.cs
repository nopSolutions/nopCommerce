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

                    var deliveryOptionsPam = (await _abcProductAttributeService.SaveProductAttributeMappingsAsync(
                            productId,
                            pams,
                            new string[]
                            {
                                AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName,
                                AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName,
                            }
                        )).SingleOrDefault();

                    await AddDeliveryOptionValuesAsync(deliveryOptionsPam.Id, productId, map);

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
            await _abcProductAttributeService.SaveProductAttributeAsync(attribute);

            return attribute;
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
            deliveryOnlyPav = await AddValueAsync(
                pamId,
                deliveryOnlyPav,
                map.DeliveryOnly,
                "Home Delivery ({0}, FREE With Mail-In Rebate)",
                10,
                true);

            var deliveryInstallationPav = values.Where(pav => pav.Name.Contains("Home Delivery and Installation (")).SingleOrDefault();
            deliveryInstallationPav = await AddValueAsync(
                pamId,
                deliveryInstallationPav,
                map.DeliveryInstall,
                "Home Delivery and Installation ({0})",
                20,
                deliveryOnlyPav == null);

            // if (allowInStorePickup)
            // {
            //     var pickupPav = values.Where(pav => pav.Name.Contains("Pickup In-Store")).SingleOrDefault();
            //     if (pickupPav == null)
            //     {
            //         var newPickupPav = new ProductAttributeValue()
            //         {
            //             ProductAttributeMappingId = pam.Id,
            //             Name = "Pickup In-Store Or Curbside (FREE)",
            //             DisplayOrder = 0,
            //         };
            //         await _productAttributeService.InsertProductAttributeValueAsync(newPickupPav);
            //     }
            // }

            return (pamId, deliveryOnlyPav?.Id, deliveryInstallationPav?.Id, deliveryOnlyPav?.PriceAdjustment, deliveryInstallationPav?.PriceAdjustment);
        }

        private async System.Threading.Tasks.Task<ProductAttributeValue> AddValueAsync(
            int pamId,
            ProductAttributeValue pav,
            int itemNumber,
            string displayName,
            int displayOrder,
            bool isPreSelected,
            decimal priceAdjustment = 0)
        {
            if (pav == null && itemNumber != 0)
            {
                var item = await _abcDeliveryService.GetAbcDeliveryItemByItemNumberAsync(itemNumber);
                var price = item.Price - priceAdjustment;
                var priceDisplay = price == 0 ?
                    "FREE" :
                    await _priceFormatter.FormatPriceAsync(price);
                pav = new ProductAttributeValue()
                {
                    ProductAttributeMappingId = pamId,
                    Name = string.Format(displayName, priceDisplay),
                    Cost = itemNumber,
                    PriceAdjustment = price,
                    IsPreSelected = isPreSelected,
                    DisplayOrder = displayOrder,
                };

                await _abcProductAttributeService.InsertProductAttributeValueAsync(pav);
            }

            return pav;
        }
    }
}
