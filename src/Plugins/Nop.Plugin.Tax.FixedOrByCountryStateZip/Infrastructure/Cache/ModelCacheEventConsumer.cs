using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
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
        IConsumer<EntityInsertedEvent<TaxRate>>,
        IConsumer<EntityUpdatedEvent<TaxRate>>,
        IConsumer<EntityDeletedEvent<TaxRate>>,
        //tax category
        IConsumer<EntityDeletedEvent<TaxCategory>>
    {
        #region Constants

        /// <summary>
        /// Key for caching all tax rates
        /// </summary>
        public const string ALL_TAX_RATES_MODEL_KEY = "Nop.plugins.tax.fixedorbycountrystateziptaxrate.all";
        public const string TAXRATE_ALL_KEY = "Nop.plugins.tax.fixedorbycountrystateziptaxrate.all-{0}-{1}";
        public const string TAXRATE_PATTERN_KEY = "Nop.plugins.tax.fixedorbycountrystateziptaxrate.";

        #endregion

        #region Fields

        private readonly ICountryStateZipService _taxRateService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(ICountryStateZipService taxRateService,
            ISettingService settingService,
            IStaticCacheManager cacheManager)
        {
            this._taxRateService = taxRateService;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle tax rate inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(EntityInsertedEvent<TaxRate> eventMessage)
        {
            //clear cache
            _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
        }

        /// <summary>
        /// Handle tax rate updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(EntityUpdatedEvent<TaxRate> eventMessage)
        {
            //clear cache
            _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
        }

        /// <summary>
        /// Handle tax rate deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(EntityDeletedEvent<TaxRate> eventMessage)
        {
            //clear cache
            _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
        }

        /// <summary>
        /// Handle tax category deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(EntityDeletedEvent<TaxCategory> eventMessage)
        {
            var taxCategory = eventMessage?.Entity;
            if (taxCategory == null)
                return;

            //delete an appropriate record when tax category is deleted
            var recordsToDelete = _taxRateService.GetAllTaxRates().Where(taxRate => taxRate.TaxCategoryId == taxCategory.Id).ToList();
            foreach (var taxRate in recordsToDelete)
            {
                _taxRateService.DeleteTaxRate(taxRate);
            }

            //delete saved fixed rate if exists
            var setting = _settingService.GetSetting(string.Format(FixedOrByCountryStateZipDefaults.FixedRateSettingsKey, taxCategory.Id));
            if (setting != null)
                _settingService.DeleteSetting(setting);
        }

        #endregion
    }
}