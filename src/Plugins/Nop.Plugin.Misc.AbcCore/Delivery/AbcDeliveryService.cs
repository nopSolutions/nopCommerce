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
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ILogger _logger;
        private readonly IRepository<AbcDeliveryItem> _abcDeliveryItemRepository;
        private readonly IRepository<AbcDeliveryMap> _abcDeliveryMapRepository;

        public AbcDeliveryService(
            IPriceFormatter priceFormatter,
            IProductAttributeService productAttributeService,
            ILogger logger,
            IRepository<AbcDeliveryItem> abcDeliveryItemRepository,
            IRepository<AbcDeliveryMap> abcDeliveryMapRepository)
        {
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

        public async Task<IList<AbcDeliveryMap>> GetAbcDeliveryMapsAsync()
        {

            var abcDeliveryMaps = await _abcDeliveryMapRepository.Table.Where(adm => adm != null).ToListAsync();

            return await abcDeliveryMaps.Where(adm => adm.HasDeliveryOptions()).ToListAsync();
        }

        public async System.Threading.Tasks.Task<ProductAttributeValue> AddValueAsync(
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