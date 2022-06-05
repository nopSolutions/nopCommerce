using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Data;
using Nop.Services.Logging;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Delivery
{
    public class AbcDeliveryService : IAbcDeliveryService
    {
        private readonly ICategoryService _categoryService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ILogger _logger;
        private readonly IRepository<AbcDeliveryItem> _abcDeliveryItemRepository;
        private readonly IRepository<AbcDeliveryMap> _abcDeliveryMapRepository;

        public AbcDeliveryService(
            ICategoryService categoryService,
            IPriceFormatter priceFormatter,
            IProductAttributeService productAttributeService,
            ILogger logger,
            IRepository<AbcDeliveryItem> abcDeliveryItemRepository,
            IRepository<AbcDeliveryMap> abcDeliveryMapRepository)
        {
            _categoryService = categoryService;
            _priceFormatter = priceFormatter;
            _productAttributeService = productAttributeService;
            _logger = logger;
            _abcDeliveryItemRepository = abcDeliveryItemRepository;
            _abcDeliveryMapRepository = abcDeliveryMapRepository;
        }

        private async System.Threading.Tasks.Task<(int deliveryPickupOptionsProductAttributeId,
                                                   int haulAwayDeliveryProductAttributeId,
                                                   int haulAwayDeliveryInstallProductAttributeId)> GetAbcDeliveryProductAttributesAsync()
        {
            var productAttributes = await _productAttributeService.GetAllProductAttributesAsync();

            int _deliveryPickupOptionsProductAttributeId = productAttributes
                .Where(p => p.Name == AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName)
                .Select(p => p.Id)
                .Single();

            int _haulAwayDeliveryProductAttributeId = productAttributes
                .Where(p => p.Name == AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName)
                .Select(p => p.Id)
                .Single();

            int _haulAwayDeliveryInstallProductAttributeId = productAttributes
                .Where(p => p.Name == AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName)
                .Select(p => p.Id)
                .Single();

            return (
                _deliveryPickupOptionsProductAttributeId,
                _haulAwayDeliveryProductAttributeId,
                _haulAwayDeliveryInstallProductAttributeId
            );
        }

        public async Task<AbcDeliveryItem> GetAbcDeliveryItemByItemNumberAsync(int itemNumber)
        {
            try
            {
                return await _abcDeliveryItemRepository.Table.SingleAsync(adi => adi.Item_Number == itemNumber);
            }
            catch (Exception)
            {
                await _logger.ErrorAsync($"Cannot find single AbcDeliveryItem with ItemNumber {itemNumber}");
                throw;
            }
        }

        // Will only return options with mapped delivery options
        // May not need to be made public
        public async Task<AbcDeliveryMap> GetAbcDeliveryMapByCategoryIdAsync(int categoryId)
        {
            var adm = await _abcDeliveryMapRepository.Table.FirstOrDefaultAsync(adm => adm.CategoryId == categoryId);

            return adm != null && adm.HasDeliveryOptions() ? adm : null;
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
                    (await GetAbcDeliveryProductAttributesAsync()).haulAwayDeliveryProductAttributeId,
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
                    (await GetAbcDeliveryProductAttributesAsync()).haulAwayDeliveryInstallProductAttributeId,
                    deliveryOptionsPamId,
                    deliveryInstallPavId.Value,
                    map.DeliveryHaulwayInstall,
                    deliveryInstallPriceAdjustment);
            }
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

            var pam = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productId))
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
                await _productAttributeService.InsertProductAttributeMappingAsync(pam);
            }

            return pam;
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
                (await GetAbcDeliveryProductAttributesAsync()).haulAwayDeliveryProductAttributeId,
                deliveryOptionsPamId,
                deliveryOptionsPavId);
            if (pam == null)
            {
                return;
            }

            var pav = (await _productAttributeService.GetProductAttributeValuesAsync(pam.Id)).FirstOrDefault();
            await AddValueAsync(
                pam.Id,
                pav,
                abcDeliveryMapItemNumber,
                "Remove Old Appliance ({0})",
                0,
                false,
                priceAdjustment);
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
                var item = await GetAbcDeliveryItemByItemNumberAsync(itemNumber);
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

                await _productAttributeService.InsertProductAttributeValueAsync(pav);
            }

            return pav;
        }
    }
}