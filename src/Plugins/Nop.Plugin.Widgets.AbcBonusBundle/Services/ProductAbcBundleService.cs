using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Widgets.AbcBonusBundle.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.AbcBonusBundle.Services
{
    public class ProductAbcBundleService : IProductAbcBundleService
    {
        private const string PRODUCT_ABC_BUNDLE = "AbcWarehouse.productabcbundle.{0}";

        private readonly IStaticCacheManager _staticCacheManager;

        public ProductAbcBundleService(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        public IList<ProductAbcBundle> GetBundles(string sku)
        {
            IRepository<ProductAbcBundle> repo = EngineContext.Current.Resolve<IRepository<ProductAbcBundle>>();
            return _staticCacheManager.Get(new CacheKey(string.Format(PRODUCT_ABC_BUNDLE, sku), "Abc."),
                ProductAbcBundle.GetBySkuFunc(repo, sku));
        }
    }
}
