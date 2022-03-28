using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class AbcPromoService : IAbcPromoService
    {
        private readonly IRepository<AbcPromoProductMapping> _abcPromoProductMappingRepository;
        private readonly IRepository<AbcPromo> _abcPromoRepository;

        private readonly IProductService _productService;

        public AbcPromoService(
            IRepository<AbcPromoProductMapping> abcPromoProductMappingRepository,
            IRepository<AbcPromo> abcPromoRepository,
            IProductService productService
        )
        {
            _abcPromoProductMappingRepository = abcPromoProductMappingRepository;
            _abcPromoRepository = abcPromoRepository;
            _productService = productService;
        }

        public async Task<IList<AbcPromo>> GetAllPromosAsync()
        {
            return await _abcPromoRepository.Table.ToListAsync();
        }

        public async Task<IList<AbcPromo>> GetActivePromosAsync()
        {
            var promos = await GetAllPromosAsync();
            return promos.Where(p => p.IsActive()).ToList();
        }

        public async Task<IList<AbcPromo>> GetExpiredPromosAsync()
        {
            var promos = await GetAllPromosAsync();
            return promos.Where(p => p.IsExpired()).ToList();
        }

        public async Task<AbcPromo> GetPromoByIdAsync(int promoId)
        {
            var promos = await GetAllPromosAsync();
            return promos.Where(p => p.Id == promoId).FirstOrDefault();
        }

        // Will include promos expired by one month.
        public async Task<IList<AbcPromo>> GetAllPromosByProductIdAsync(int productId)
        {
            var productPromoIds = _abcPromoProductMappingRepository.Table
                                                            .Where(appm => appm.ProductId == productId)
                                                            .Select(appm => appm.AbcPromoId)
                                                            .ToList();
            var promos = await GetAllPromosAsync();
            return promos.Where(p => productPromoIds.Contains(p.Id))
                         .ToList();
        }

        public async Task<IList<AbcPromo>> GetActivePromosByProductIdAsync(int productId)
        {
            var promos = await GetAllPromosByProductIdAsync(productId);
            return promos.Where(p => p.IsActive()).ToList();
        }

        public async Task<IList<Product>> GetProductsByPromoIdAsync(int promoId)
        {
            var productIds = _abcPromoProductMappingRepository.Table
                                        .Where(appm => appm.AbcPromoId == promoId)
                                        .Select(appm => appm.ProductId)
                                        .ToArray();

            return await _productService.GetProductsByIdsAsync(productIds);
        }

        public async Task<IList<Product>> GetPublishedProductsByPromoIdAsync(int promoId)
        {
            var products = await GetProductsByPromoIdAsync(promoId);
            return products.Where(p => p.Published).ToList();
        }

        public void UpdatePromo(AbcPromo promo)
        {
            if (promo == null)
                throw new ArgumentNullException(nameof(promo));

            _abcPromoRepository.UpdateAsync(promo);
        }
    }
}
