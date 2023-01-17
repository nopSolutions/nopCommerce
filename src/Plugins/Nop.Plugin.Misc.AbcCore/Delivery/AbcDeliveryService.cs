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

        public async Task<AbcDeliveryItem> GetAbcDeliveryItemByItemNumberAsync(int itemNumber)
        {
            // Mattresses
            if (itemNumber == 2)
            {
                return new AbcDeliveryItem();
            }

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

        // Calling async methods synchronously, if things go wrong check this:
        // https://stackoverflow.com/questions/22628087/calling-async-method-synchronously
        public ProductAttributeValue AddValue(
            int pamId,
            ProductAttributeValue pav,
            int itemNumber,
            string displayName,
            int displayOrder,
            bool isPreSelected,
            decimal priceAdjustment = 0)
        {
            if (pav != null || itemNumber == 0)
            {
                return pav;
            }

            // Special case for pickup in store
            var item = itemNumber == 1 ? new AbcDeliveryItem() : GetAbcDeliveryItemByItemNumberAsync(itemNumber).Result;
            var price = itemNumber == 1 ? 0M : item.Price - priceAdjustment;
            
            var priceDisplay = price == 0 ?
                "FREE" :
                _priceFormatter.FormatPriceAsync(price).Result;
            pav = new ProductAttributeValue()
            {
                ProductAttributeMappingId = pamId,
                Name = string.Format(displayName, priceDisplay),
                Cost = itemNumber,
                PriceAdjustment = price,
                IsPreSelected = isPreSelected,
                DisplayOrder = displayOrder,
            };

            _productAttributeService.InsertProductAttributeValueAsync(pav);

            return pav;
        }
    }
}