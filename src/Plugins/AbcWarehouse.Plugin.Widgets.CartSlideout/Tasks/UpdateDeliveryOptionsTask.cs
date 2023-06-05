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
using Nop.Plugin.Misc.AbcCore.Mattresses;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Tasks
{
    public partial class UpdateDeliveryOptionsTask : IScheduleTask
    {
        private readonly CoreSettings _coreSettings;
        private readonly IAbcDeliveryService _abcDeliveryService;
        private readonly IAbcMattressModelService _abcMattressModelService;
        private readonly IAbcProductAttributeService _abcProductAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IPriceFormatter _priceFormatter;

        private ProductAttribute _deliveryPickupOptionsProductAttribute;
        private ProductAttribute _haulAwayDeliveryProductAttribute;
        private ProductAttribute _haulAwayDeliveryInstallProductAttribute;
        private ProductAttribute _pickupInStoreProductAttribute;
        private ProductAttribute _deliveryAccessoriesProductAttribute;
        private ProductAttribute _deliveryInstallAccessoriesProductAttribute;
        private ProductAttribute _pickupAccessoriesProductAttribute;

        public UpdateDeliveryOptionsTask(
            CoreSettings coreSettings,
            IAbcDeliveryService abcDeliveryService,
            IAbcMattressModelService abcMattressModelService,
            IAbcProductAttributeService abcProductAttributeService,
            ICategoryService categoryService,
            ILogger logger,
            IPriceFormatter priceFormatter)
        {
            _coreSettings = coreSettings;
            _abcDeliveryService = abcDeliveryService;
            _abcMattressModelService = abcMattressModelService;
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
            _deliveryAccessoriesProductAttribute =
                await _abcProductAttributeService.GetProductAttributeByNameAsync(AbcDeliveryConsts.DeliveryAccessoriesProductAttributeName);
            _deliveryInstallAccessoriesProductAttribute =
                await _abcProductAttributeService.GetProductAttributeByNameAsync(AbcDeliveryConsts.DeliveryInstallAccessoriesProductAttributeName);
            _pickupAccessoriesProductAttribute =
                await _abcProductAttributeService.GetProductAttributeByNameAsync(AbcDeliveryConsts.PickupAccessoriesProductAttributeName);

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
                            var deliveryOptionPavs = await UpdateDeliveryOptionsPavAsync(deliveryOptionsPam, map);

                            // Try running these synchronously to prevent the PAV collision issues
                            var deliveryOnlyPav = deliveryOptionPavs.SingleOrDefault(pav => pav.Name.Contains("Home Delivery ("));
                            if (deliveryOnlyPav != null)
                            {
                                UpdateHaulawayAsync(productId, map.DeliveryHaulway, _haulAwayDeliveryProductAttribute.Id, deliveryOptionsPam.Id, deliveryOnlyPav);
                                UpdateAccessoriesAsync(
                                    productId,
                                    map,
                                    _deliveryAccessoriesProductAttribute.Id,
                                    deliveryOptionsPam.Id,
                                    deliveryOnlyPav,
                                    "Recommended Accessories");
                            }

                            var deliveryInstallPav = deliveryOptionPavs.SingleOrDefault(pav => pav.Name.Contains("Home Delivery and Installation ("));
                            if (deliveryInstallPav != null)
                            {
                                UpdateHaulawayAsync(productId, map.DeliveryHaulwayInstall, _haulAwayDeliveryInstallProductAttribute.Id, deliveryOptionsPam.Id, deliveryInstallPav);
                                UpdateAccessoriesAsync(
                                    productId,
                                    map,
                                    _deliveryInstallAccessoriesProductAttribute.Id,
                                    deliveryOptionsPam.Id,
                                    deliveryInstallPav,
                                    "Required Accessories");
                            }

                            var pickupPav = deliveryOptionPavs.SingleOrDefault(pav => pav.Name.Contains("Pickup In-Store Or Curbside ("));
                            if (pickupPav != null)
                            {
                                UpdateAccessoriesAsync(
                                    productId,
                                    map,
                                    _pickupAccessoriesProductAttribute.Id,
                                    deliveryOptionsPam.Id,
                                    pickupPav,
                                    "Recommended Accessories");
                            }
                        }
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

            // Mattresses
            var mattressProductIds = _abcMattressModelService.GetAllAbcMattressModels()
                                                             .Where(amm => amm.ProductId != null)
                                                             .Select(amm => (int)amm.ProductId);
            foreach (var mattressProductId in mattressProductIds)
            {
                try
                {
                    // Hardcoding 2 as the "Mattress Delivery Only" option
                    var mattressAbcDeliveryMap = new AbcDeliveryMap() { DeliveryOnly = 2 };
                    var deliveryOptionsPam = await UpdateDeliveryOptionsPamAsync(mattressProductId, mattressAbcDeliveryMap);
                    await UpdateDeliveryOptionsPavAsync(deliveryOptionsPam, mattressAbcDeliveryMap);
                }
                catch (Exception e)
                {
                    await _logger.InsertLogAsync(
                        LogLevel.Error,
                        $"Failure when updating delivery options for Mattress ProductId {mattressProductId}",
                        e.ToString());
                    hasErrors = true;
                }
            }

            if (hasErrors)
            {
                throw new NopException("Failures occured when updating product delivery options.");
            }
        }

        private async System.Threading.Tasks.Task UpdateAccessoriesAsync(
            int productId,
            AbcDeliveryMap map,
            int accessoriesProductAttributeId,
            int deliveryOptionsPamId,
            ProductAttributeValue deliveryPav,
            string textPrompt)
        {
            var abcDeliveryAccessories = await _abcDeliveryService.GetAbcDeliveryAccessoriesByCategoryId(map.CategoryId);
            if (!abcDeliveryAccessories.Any()) { return; }

            var isDeliveryInstall = accessoriesProductAttributeId == _deliveryInstallAccessoriesProductAttribute.Id;
            if (isDeliveryInstall)
            {
                abcDeliveryAccessories = abcDeliveryAccessories.Where(da => da.IsDeliveryInstall).ToList();
            }
            if (!abcDeliveryAccessories.Any()) { return; }

            var existingPam =
                (await _abcProductAttributeService.GetProductAttributeMappingsByProductIdAsync(productId)).FirstOrDefault(
                    pam => pam.ProductAttributeId == accessoriesProductAttributeId
                );
            var newPam = new ProductAttributeMapping()
            {
                ProductId = productId,
                ProductAttributeId = accessoriesProductAttributeId,
                AttributeControlType = AttributeControlType.RadioList,
                TextPrompt = textPrompt,
                DisplayOrder = 30,
                ConditionAttributeXml = $"<Attributes><ProductAttribute ID=\"{deliveryOptionsPamId}\"><ProductAttributeValue><Value>{deliveryPav.Id}</Value></ProductAttributeValue></ProductAttribute></Attributes>",
            };

            var resultPam = await SaveProductAttributeMappingAsync(existingPam, newPam);
            if (resultPam != null)
            {
                foreach (var accessory in abcDeliveryAccessories)
                {
                    var item = await _abcDeliveryService.GetAbcDeliveryItemByItemNumberAsync(accessory.AccessoryItemNumber);
                    var existingPav = (await _abcProductAttributeService.GetProductAttributeValuesAsync(resultPam.Id)).SingleOrDefault(pav => pav.Name == item.Description);
                    var newPav = new ProductAttributeValue()
                    {
                        ProductAttributeMappingId = resultPam.Id,
                        Name = item.Description,
                        // use 3 to hardcode NO options
                        Cost = accessory.AccessoryItemNumber.All(Char.IsDigit) ?
                            item.Price :
                            3M,
                        PriceAdjustment = item.Price,
                        IsPreSelected = item.Item_Number.Contains("NO"),
                        DisplayOrder = item.Item_Number.Contains("NO") ? 10 : 0,
                    };
                    await SaveProductAttributeValueAsync(existingPav, newPav);
                }
            }
        }

        private async System.Threading.Tasks.Task UpdateHaulawayAsync(
            int productId,
            int mapItemNumber,
            int haulAwayProductAttributeId,
            int deliveryOptionsPamId,
            ProductAttributeValue deliveryPav)
        {
            var existingPam =
                (await _abcProductAttributeService.GetProductAttributeMappingsByProductIdAsync(productId)).FirstOrDefault(
                    pam => pam.ProductAttributeId == haulAwayProductAttributeId
                );
            var newPam = mapItemNumber != 0 ? new ProductAttributeMapping()
            {
                ProductId = productId,
                ProductAttributeId = haulAwayProductAttributeId,
                AttributeControlType = AttributeControlType.Checkboxes,
                TextPrompt = "Haul Away",
                ConditionAttributeXml = $"<Attributes><ProductAttribute ID=\"{deliveryOptionsPamId}\"><ProductAttributeValue><Value>{deliveryPav.Id}</Value></ProductAttributeValue></ProductAttribute></Attributes>",
            } : null;

            var resultPam = await SaveProductAttributeMappingAsync(existingPam, newPam);

            if (resultPam != null)
            {
                var existingPav = (await _abcProductAttributeService.GetProductAttributeValuesAsync(resultPam.Id)).FirstOrDefault();

                var item = await _abcDeliveryService.GetAbcDeliveryItemByItemNumberAsync(mapItemNumber.ToString());
                var price = item.Price - deliveryPav.PriceAdjustment;
                var priceFormatted = price == 0 ? "FREE" : await _priceFormatter.FormatPriceAsync(price);
                var newPav = new ProductAttributeValue()
                {
                    ProductAttributeMappingId = resultPam.Id,
                    Name = await GetHaulawayMessageAsync(priceFormatted, productId),
                    Cost = mapItemNumber,
                    PriceAdjustment = price,
                    IsPreSelected = false,
                    DisplayOrder = 0,
                };

                await SaveProductAttributeValueAsync(existingPav, newPav);
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

            return await SaveProductAttributeMappingAsync(existingPam, newPam);
        }

        private async System.Threading.Tasks.Task<List<ProductAttributeValue>> UpdateDeliveryOptionsPavAsync(
            ProductAttributeMapping deliveryOptionsPam,
            AbcDeliveryMap map
        )
        {
            var existingValues = await _abcProductAttributeService.GetProductAttributeValuesAsync(deliveryOptionsPam.Id);
            var results = new List<ProductAttributeValue>();

            // TODO: This could be refactored to clean up the repeating
            // Delivery only
            var deliveryOnlyItem = map.DeliveryOnly == 0 ?
                new AbcDeliveryItem() :
                await _abcDeliveryService.GetAbcDeliveryItemByItemNumberAsync(map.DeliveryOnly.ToString());
            var deliveryOnlyPriceFormatted = map.DeliveryOnly == 2 ?
                "MATTRESS" : 
                await _priceFormatter.FormatPriceAsync(deliveryOnlyItem.Price);

            // Need to get the category to determine if furniture
            string message = await GetHomeDeliveryMessageAsync(
                deliveryOnlyPriceFormatted,
                deliveryOptionsPam.ProductId);

            var existingDeliveryOnlyPav = existingValues.Where(pav => pav.Name.Contains("Home Delivery (")).SingleOrDefault();
            var newDeliveryOnlyPav = map.DeliveryOnly != 0 ? new ProductAttributeValue()
            {
                ProductAttributeMappingId = deliveryOptionsPam.Id,
                Name = message,
                Cost = map.DeliveryOnly,
                PriceAdjustment = deliveryOnlyItem.Price,
                IsPreSelected = true,
                DisplayOrder = 10,
            } : null;

            var resultDeliveryOnlyPav = await SaveProductAttributeValueAsync(existingDeliveryOnlyPav, newDeliveryOnlyPav);
            if (resultDeliveryOnlyPav != null) { results.Add(resultDeliveryOnlyPav); }

            // Delivery/Install
            var deliveryInstallItem = map.DeliveryInstall == 0 ?
                new AbcDeliveryItem() :
                await _abcDeliveryService.GetAbcDeliveryItemByItemNumberAsync(map.DeliveryInstall.ToString());
            var deliveryInstallPriceFormatted = await _priceFormatter.FormatPriceAsync(deliveryInstallItem.Price);

            var existingDeliveryInstallPav = existingValues.Where(pav => pav.Name.Contains("Home Delivery and Installation (")).SingleOrDefault();
            var newDeliveryInstallPav = map.DeliveryInstall != 0 ? new ProductAttributeValue()
            {
                ProductAttributeMappingId = deliveryOptionsPam.Id,
                Name = string.Format("Home Delivery and Installation ({0})", deliveryInstallPriceFormatted),
                Cost = map.DeliveryInstall,
                PriceAdjustment = deliveryInstallItem.Price,
                IsPreSelected = false,
                DisplayOrder = 20,
            } : null;

            var resultDeliveryInstallPav = await SaveProductAttributeValueAsync(existingDeliveryInstallPav, newDeliveryInstallPav);
            if (resultDeliveryInstallPav != null) { results.Add(resultDeliveryInstallPav); }

            // Pickup
            var existingPickupPav = existingValues.Where(pav => pav.Name.Contains("Pickup In-Store")).SingleOrDefault();
            ProductAttributeValue newPickupPav = null;
            var pams = await _abcProductAttributeService.GetProductAttributeMappingsByProductIdAsync(deliveryOptionsPam.ProductId);
            var legacyPickupPam = await pams.WhereAwait(
                async pam => (await _abcProductAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId)).Name ==
                              AbcDeliveryConsts.LegacyPickupInStoreProductAttributeName).FirstOrDefaultAsync();
            if (legacyPickupPam != null)
            {
                newPickupPav = new ProductAttributeValue()
                {
                    ProductAttributeMappingId = deliveryOptionsPam.Id,
                    Name = AbcDeliveryConsts.PickupProductAttributeValueName,
                    // Used as the current placeholder for pickup in store
                    Cost = 1,
                    PriceAdjustment = 0,
                    IsPreSelected = false,
                    DisplayOrder = 0,
                };
            }
            var resultPickupPav = await SaveProductAttributeValueAsync(existingPickupPav, newPickupPav);
            if (resultPickupPav != null) { results.Add(resultPickupPav); }

            return results;
        }

        private bool DoProductAttributeValuesMatch(ProductAttributeValue existingPav, ProductAttributeValue newPav)
        {
            return existingPav.Name == newPav.Name;
        }

        private async System.Threading.Tasks.Task<ProductAttributeMapping> SaveProductAttributeMappingAsync(ProductAttributeMapping existingPam, ProductAttributeMapping newPam)
        {
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

        // Try making these synchronous to prevent collision issues
        private async System.Threading.Tasks.Task<ProductAttributeValue> SaveProductAttributeValueAsync(ProductAttributeValue existingPav, ProductAttributeValue newPav)
        {
            if (existingPav == null && newPav == null)
            {
                return null;
            }
            else if (existingPav == null && newPav != null)
            {
                await _abcProductAttributeService.InsertProductAttributeValueAsync(newPav);
                return newPav;
            }
            else if (existingPav != null && newPav == null)
            {
                await _abcProductAttributeService.DeleteProductAttributeValueAsync(existingPav);
                return null;
            }
            else if (!ArePavsEqual(existingPav, newPav))
            {
                existingPav.Name = newPav.Name;
                existingPav.PriceAdjustment = newPav.PriceAdjustment;
                existingPav.Cost = newPav.Cost;
                await _abcProductAttributeService.UpdateProductAttributeValueAsync(existingPav);
                return existingPav;
            }

            return existingPav;
        }

        private bool ArePavsEqual(ProductAttributeValue existingPav, ProductAttributeValue newPav)
        {
            return existingPav.Name == newPav.Name
                && existingPav.PriceAdjustment == newPav.PriceAdjustment
                && existingPav.Cost == newPav.Cost;
        }

        private async System.Threading.Tasks.Task<string> GetHomeDeliveryMessageAsync(string deliveryOnlyPriceFormatted, int productId)
        {
            if (deliveryOnlyPriceFormatted == "MATTRESS")
            {
                return "Home Delivery (Price in Cart)";
            }

            // If Furniture, no mail-in rebate
            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(productId);
            foreach (var pc in productCategories)
            {
                var category = await _categoryService.GetCategoryByIdAsync(pc.CategoryId);
                var fullCategoryListNames = (await _categoryService.GetCategoryBreadCrumbAsync(category)).Select(c => c.Name);
                if (fullCategoryListNames.Contains("Furniture"))
                {
                    return string.Format("Home Delivery ({0})", deliveryOnlyPriceFormatted);
                }
            }

            return string.Format("Home Delivery (FREE With Mail-In Rebate)", deliveryOnlyPriceFormatted);
        }

        private async System.Threading.Tasks.Task<string> GetHaulawayMessageAsync(string price, int productId)
        {
            // If TV, change messaging
            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(productId);
            foreach (var pc in productCategories)
            {
                var category = await _categoryService.GetCategoryByIdAsync(pc.CategoryId);
                var fullCategoryListNames = (await _categoryService.GetCategoryBreadCrumbAsync(category)).Select(c => c.Name);
                if (fullCategoryListNames.Contains("TV - Video"))
                {
                    return string.Format("Remove Old Television ({0})", price);
                }
            }

            return string.Format("Remove Old Appliance ({0})", price);
        }
    }
}
