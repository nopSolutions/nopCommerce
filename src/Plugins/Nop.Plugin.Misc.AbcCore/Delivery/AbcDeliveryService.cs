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
        private readonly IRepository<AbcDeliveryAccessory> _abcDeliveryAccessoryRepository;
        private readonly IRepository<AbcDeliveryItem> _abcDeliveryItemRepository;
        private readonly IRepository<AbcDeliveryMap> _abcDeliveryMapRepository;

        public AbcDeliveryService(
            IPriceFormatter priceFormatter,
            IProductAttributeService productAttributeService,
            ILogger logger,
            IRepository<AbcDeliveryAccessory> abcDeliveryAccessoryRepository,
            IRepository<AbcDeliveryItem> abcDeliveryItemRepository,
            IRepository<AbcDeliveryMap> abcDeliveryMapRepository)
        {
            _priceFormatter = priceFormatter;
            _productAttributeService = productAttributeService;
            _logger = logger;
            _abcDeliveryAccessoryRepository = abcDeliveryAccessoryRepository;
            _abcDeliveryItemRepository = abcDeliveryItemRepository;
            _abcDeliveryMapRepository = abcDeliveryMapRepository;
        }

        public async Task<AbcDeliveryItem> GetAbcDeliveryItemByItemNumberAsync(string itemNumber)
        {
            // Mattresses or FedEx
            if (itemNumber == "-2")
            {
                return new AbcDeliveryItem();
            }

            try
            {
                return await _abcDeliveryItemRepository.Table.SingleAsync(adi => adi.Item_Number == itemNumber);
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync($"Cannot find single AbcDeliveryItem with ItemNumber {itemNumber}");
                throw;
            }
        }

        public async Task<AbcDeliveryItem> GetAbcDeliveryItemByDescriptionAsync(string description)
        {
            try
            {
                return await _abcDeliveryItemRepository.Table.SingleAsync(adi => adi.Description == description);
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync($"Cannot find single AbcDeliveryItem with Description {description}");
                throw;
            }
        }

        public async Task<AbcDeliveryItem> GetAbcDeliveryItemByIdAsync(int id)
        {
            return await _abcDeliveryItemRepository.Table
                        .SingleAsync(adi => adi.Id == id);
        }

        public async Task<IList<AbcDeliveryMap>> GetAbcDeliveryMapsAsync()
        {
            var abcDeliveryMaps = await _abcDeliveryMapRepository.Table.Where(adm => adm != null).ToListAsync();

            return await abcDeliveryMaps.Where(adm => adm.HasDeliveryOptions()).ToListAsync();
        }

        public async Task<IList<AbcDeliveryAccessory>> GetAbcDeliveryAccessoriesByCategoryId(int categoryId)
        {
            var accessories = await _abcDeliveryAccessoryRepository.Table.ToListAsync();
            return accessories.Where(a => a.CategoryId == categoryId).ToList();
        }
    }
}