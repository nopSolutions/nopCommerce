using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Plugin.Tax.CountryStateZip.Domain;
using Nop.Plugin.Tax.CountryStateZip.Services;
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
        IConsumer<EntityDeleted<TaxRate>>,
        //tax category
        IConsumer<EntityDeleted<TaxCategory>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        public const string ALL_TAX_RATES_MODEL_KEY = "Nop.plugins.tax.countrystatezip.all";
        public const string ALL_TAX_RATES_PATTERN_KEY = "Nop.plugins.tax.countrystatezip";

        private readonly ICacheManager _cacheManager;
        private readonly ITaxRateService _taxRateService;

        public ModelCacheEventConsumer(ITaxRateService taxRateService)
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");

            this._taxRateService = taxRateService;
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

        //tax category
        public void HandleEvent(EntityDeleted<TaxCategory> eventMessage)
        {
            if (eventMessage.Entity == null)
                return;

            //delete an appropriate record when tax category is deleted
            var recordsToDelete = _taxRateService.GetAllTaxRates().Where(tr => tr.TaxCategoryId == eventMessage.Entity.Id).ToList();
            foreach (var taxRate in recordsToDelete)
            {
                _taxRateService.DeleteTaxRate(taxRate);
            }
        }
    }
}
