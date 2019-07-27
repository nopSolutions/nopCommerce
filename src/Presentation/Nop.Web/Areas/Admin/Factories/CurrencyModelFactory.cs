using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the currency model factory implementation
    /// </summary>
    public partial class CurrencyModelFactory : ICurrencyModelFactory
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExchangeRatePluginManager _exchangeRatePluginManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;

        #endregion

        #region Ctor

        public CurrencyModelFactory(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IExchangeRatePluginManager exchangeRatePluginManager,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory)
        {
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _exchangeRatePluginManager = exchangeRatePluginManager;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare exchange rate provider model
        /// </summary>
        /// <param name="model">Currency exchange rate provider model</param>
        /// <param name="prepareExchangeRates">Whether to prepare exchange rate models</param>
        protected virtual void PrepareExchangeRateProviderModel(CurrencyExchangeRateProviderModel model, bool prepareExchangeRates)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AutoUpdateEnabled = _currencySettings.AutoUpdateEnabled;

            //prepare available exchange rate providers
            var availableExchangeRateProviders = _exchangeRatePluginManager.LoadAllPlugins();

            model.ExchangeRateProviders = availableExchangeRateProviders.Select(provider => new SelectListItem
            {
                Text = provider.PluginDescriptor.FriendlyName,
                Value = provider.PluginDescriptor.SystemName,
                Selected = _exchangeRatePluginManager.IsPluginActive(provider)
            }).ToList();

            //prepare exchange rates
            if (prepareExchangeRates)
                PrepareExchangeRateModels(model.ExchangeRates);
        }

        /// <summary>
        /// Prepare exchange rate models
        /// </summary>
        /// <param name="models">List of currency exchange rate model</param>
        protected virtual void PrepareExchangeRateModels(IList<CurrencyExchangeRateModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //get exchange rates
            var exchangeRates = _currencyService.GetCurrencyLiveRates();

            //filter by existing currencies
            var currencies = _currencyService.GetAllCurrencies(true, loadCacheableCopy: false);
            exchangeRates = exchangeRates
                .Where(rate => currencies
                    .Any(currency => currency.CurrencyCode.Equals(rate.CurrencyCode, StringComparison.InvariantCultureIgnoreCase))).ToList();

            //prepare models
            foreach (var rate in exchangeRates)
            {
                models.Add(new CurrencyExchangeRateModel { CurrencyCode = rate.CurrencyCode, Rate = rate.Rate });
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare currency search model
        /// </summary>
        /// <param name="searchModel">Currency search model</param>
        /// <param name="prepareExchangeRates">Whether to prepare exchange rate models</param>
        /// <returns>Currency search model</returns>
        public virtual CurrencySearchModel PrepareCurrencySearchModel(CurrencySearchModel searchModel, bool prepareExchangeRates = false)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare exchange rate provider model
            PrepareExchangeRateProviderModel(searchModel.ExchangeRateProviderModel, prepareExchangeRates);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged currency list model
        /// </summary>
        /// <param name="searchModel">Currency search model</param>
        /// <returns>Currency list model</returns>
        public virtual CurrencyListModel PrepareCurrencyListModel(CurrencySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get currencies
            var currencies = _currencyService.GetAllCurrencies(showHidden: true, loadCacheableCopy: false).ToPagedList(searchModel);

            //prepare list model
            var model = new CurrencyListModel().PrepareToGrid(searchModel, currencies, () =>
            {
                return currencies.Select(currency =>
                {
                    //fill in model values from the entity
                    var currencyModel = currency.ToModel<CurrencyModel>();

                    //fill in additional values (not existing in the entity)
                    currencyModel.IsPrimaryExchangeRateCurrency = currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId;
                    currencyModel.IsPrimaryStoreCurrency = currency.Id == _currencySettings.PrimaryStoreCurrencyId;

                    return currencyModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare currency model
        /// </summary>
        /// <param name="model">Currency model</param>
        /// <param name="currency">Currency</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Currency model</returns>
        public virtual CurrencyModel PrepareCurrencyModel(CurrencyModel model, Currency currency, bool excludeProperties = false)
        {
            Action<CurrencyLocalizedModel, int> localizedModelConfiguration = null;

            if (currency != null)
            {
                //fill in model values from the entity
                model = model ?? currency.ToModel<CurrencyModel>();

                //convert dates to the user time
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(currency.CreatedOnUtc, DateTimeKind.Utc);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(currency, entity => entity.Name, languageId, false, false);
                };
            }

            //set default values for the new model
            if (currency == null)
            {
                model.Published = true;
                model.Rate = 1;
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            //prepare available stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, currency, excludeProperties);

            return model;
        }

        #endregion
    }
}