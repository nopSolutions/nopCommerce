using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using System.Threading.Tasks;
using System.Linq;
using LinqToDB.Data;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class ProductAbcFinanceService : IProductAbcFinanceService
    {
        private readonly IRepository<ProductAbcFinance> _productAbcFinanceRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        public ProductAbcFinanceService(
            IRepository<ProductAbcFinance> productAbcFinanceRepository,
            IStaticCacheManager staticCacheManager
        )
        {
            _productAbcFinanceRepository = productAbcFinanceRepository;
            _staticCacheManager = staticCacheManager;
        }

        public async Task<ProductAbcFinance> GetProductAbcFinanceByAbcItemNumberAsync(string abcItemNumber)
        {
            var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(new CacheKey("AbcWarehouse.productabcfinance.byabcitemnumber.{0}"), abcItemNumber);
            var productAbcFinance = await _staticCacheManager.GetAsync(allCacheKey, async () =>
            {
                return await _productAbcFinanceRepository.Table.FirstOrDefaultAsync(paf => paf.AbcItemNumber == abcItemNumber);
            });
            return productAbcFinance;
        }
    }
}
