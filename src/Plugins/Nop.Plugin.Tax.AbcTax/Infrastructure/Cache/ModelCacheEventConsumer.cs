using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Plugin.Tax.AbcTax.Domain;
using Nop.Plugin.Tax.AbcTax.Services;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.Tax.AbcTax.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer : 
        //tax rates
        IConsumer<EntityInsertedEvent<AbcTaxRate>>,
        IConsumer<EntityUpdatedEvent<AbcTaxRate>>,
        IConsumer<EntityDeletedEvent<AbcTaxRate>>,
        //tax category
        IConsumer<EntityDeletedEvent<TaxCategory>>
    {
        #region Constants

        /// <summary>
        /// Key for caching all tax rates
        /// </summary>
        public static CacheKey ALL_TAX_RATES_MODEL_KEY = new CacheKey("Nop.plugins.tax.AbcTaxRate.all", TAXRATE_PATTERN_KEY);
        public static CacheKey TAXRATE_ALL_KEY = new CacheKey("Nop.plugins.tax.AbcTaxRate.taxrate.all", TAXRATE_PATTERN_KEY);
        
        public const string TAXRATE_PATTERN_KEY = "Nop.plugins.tax.AbcTaxRate.";

        #endregion

        #region Fields

        private readonly IAbcTaxService _abcTaxService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(IAbcTaxService abcTaxService,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager)
        {
            _abcTaxService = abcTaxService;
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
        public async Task HandleEventAsync(EntityInsertedEvent<AbcTaxRate> eventMessage)
        {
            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(TAXRATE_PATTERN_KEY);
        }

        /// <summary>
        /// Handle tax rate updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<AbcTaxRate> eventMessage)
        {
            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(TAXRATE_PATTERN_KEY);
        }

        /// <summary>
        /// Handle tax rate deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<AbcTaxRate> eventMessage)
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
            var recordsToDelete = (await _abcTaxService.GetAllTaxRatesAsync()).Where(taxRate => taxRate.TaxCategoryId == taxCategory.Id).ToList();
            foreach (var taxRate in recordsToDelete)
            {
                await _abcTaxService.DeleteTaxRateAsync(taxRate);
            }
        }

        #endregion
    }
}