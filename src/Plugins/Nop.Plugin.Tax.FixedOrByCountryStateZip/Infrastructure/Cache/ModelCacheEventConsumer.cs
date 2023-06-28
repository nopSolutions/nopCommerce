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
    public class ModelCacheEventConsumer :
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
        public static CacheKey ALL_TAX_RATES_MODEL_KEY = new("Nop.plugins.tax.fixedorbycountrystateziptaxrate.all", TAXRATE_PATTERN_KEY);
        public static CacheKey TAXRATE_ALL_KEY = new("Nop.plugins.tax.fixedorbycountrystateziptaxrate.taxrate.all", TAXRATE_PATTERN_KEY);

        public const string TAXRATE_PATTERN_KEY = "Nop.plugins.tax.fixedorbycountrystateziptaxrate.";

        #endregion

        #region Fields

        protected readonly ICountryStateZipService _taxRateService;
        protected readonly ISettingService _settingService;
        protected readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(ICountryStateZipService taxRateService,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager)
        {
            _taxRateService = taxRateService;
            _settingService = settingService;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle tax rate inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<TaxRate> eventMessage)
        {
            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(TAXRATE_PATTERN_KEY);
        }

        /// <summary>
        /// Handle tax rate updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<TaxRate> eventMessage)
        {
            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(TAXRATE_PATTERN_KEY);
        }

        /// <summary>
        /// Handle tax rate deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TaxRate> eventMessage)
        {
            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(TAXRATE_PATTERN_KEY);
        }

        /// <summary>
        /// Handle tax category deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TaxCategory> eventMessage)
        {
            var taxCategory = eventMessage?.Entity;
            if (taxCategory == null)
                return;

            //delete an appropriate record when tax category is deleted
            var recordsToDelete = (await _taxRateService.GetAllTaxRatesAsync()).Where(taxRate => taxRate.TaxCategoryId == taxCategory.Id).ToList();
            foreach (var taxRate in recordsToDelete)
            {
                await _taxRateService.DeleteTaxRateAsync(taxRate);
            }

            //delete saved fixed rate if exists
            var setting = await _settingService.GetSettingAsync(string.Format(FixedOrByCountryStateZipDefaults.FixedRateSettingsKey, taxCategory.Id));
            if (setting != null)
                await _settingService.DeleteSettingAsync(setting);
        }

        #endregion
    }
}