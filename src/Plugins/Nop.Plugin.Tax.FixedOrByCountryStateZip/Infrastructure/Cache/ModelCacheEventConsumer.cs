using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Services;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer : 
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
        public const string ALL_TAX_RATES_MODEL_KEY = "Nop.plugins.tax.fixedorbycountrystateziptaxrate.all";
        public const string TAXRATE_ALL_KEY = "Nop.plugins.tax.fixedorbycountrystateziptaxrate.all-{0}-{1}";
        public const string TAXRATE_PATTERN_KEY = "Nop.plugins.tax.fixedorbycountrystateziptaxrate.";

        private readonly ICacheManager _cacheManager;
        private readonly ICountryStateZipService _taxRateService;
        private readonly ISettingService _settingService;

        public ModelCacheEventConsumer(ICountryStateZipService taxRateService, ISettingService settingService)
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");

            this._taxRateService = taxRateService;
            this._settingService = settingService;
        }

        //tax rates
        public void HandleEvent(EntityInserted<TaxRate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<TaxRate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<TaxRate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
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

            var settingKey = string.Format("Tax.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}", eventMessage.Entity.Id);
            var setting = _settingService.GetSetting(settingKey);
            if (setting != null)
                _settingService.DeleteSetting(setting);
        }
    }
}
