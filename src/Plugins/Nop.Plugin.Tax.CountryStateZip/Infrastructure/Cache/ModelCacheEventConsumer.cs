using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Plugin.Tax.CountryStateZip.Domain;
using Nop.Services.Events;

namespace Nop.Plugin.Tax.CountryStateZip.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer: 
        //tax rates
        IConsumer<EntityInserted<TaxRate>>,
        IConsumer<EntityUpdated<TaxRate>>,
        IConsumer<EntityDeleted<TaxRate>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        public const string ALL_TAX_RATES_MODEL_KEY = "Nop.plugins.tax.countrystatezip.all";
        public const string ALL_TAX_RATES_PATTERN_KEY = "Nop.plugins.tax.countrystatezip";

        private readonly ICacheManager _cacheManager;
        
        public ModelCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        }

        //tax rates
        public void HandleEvent(EntityInserted<TaxRate> eventMessage)
        {
            _cacheManager.RemoveByPattern(ALL_TAX_RATES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<TaxRate> eventMessage)
        {
            _cacheManager.RemoveByPattern(ALL_TAX_RATES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<TaxRate> eventMessage)
        {
            _cacheManager.RemoveByPattern(ALL_TAX_RATES_PATTERN_KEY);
        }
    }
}
