using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the currency model factory implementation
/// </summary>
public partial class CurrencyModelFactory : ICurrencyModelFactory
{
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IExchangeRatePluginManager _exchangeRatePluginManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;

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
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareExchangeRateProviderModelAsync(CurrencyExchangeRateProviderModel model, bool prepareExchangeRates)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.AutoUpdateEnabled = _currencySettings.AutoUpdateEnabled;

        //prepare available exchange rate providers
        var availableExchangeRateProviders = await _exchangeRatePluginManager.LoadAllPluginsAsync();

        model.ExchangeRateProviders = availableExchangeRateProviders.Select(provider => new SelectListItem
        {
            Text = provider.PluginDescriptor.FriendlyName,
            Value = provider.PluginDescriptor.SystemName,
            Selected = _exchangeRatePluginManager.IsPluginActive(provider)
        }).ToList();

        //prepare exchange rates
        if (prepareExchangeRates)
            await PrepareExchangeRateModelsAsync(model.ExchangeRates);
    }

    /// <summary>
    /// Prepare exchange rate models
    /// </summary>
    /// <param name="models">List of currency exchange rate model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareExchangeRateModelsAsync(IList<CurrencyExchangeRateModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        //get exchange rates
        var exchangeRates = await _currencyService.GetCurrencyLiveRatesAsync();

        //filter by existing currencies
        var currencies = await _currencyService.GetAllCurrenciesAsync(true);
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the currency search model
    /// </returns>
    public virtual async Task<CurrencySearchModel> PrepareCurrencySearchModelAsync(CurrencySearchModel searchModel, bool prepareExchangeRates = false)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare exchange rate provider model
        await PrepareExchangeRateProviderModelAsync(searchModel.ExchangeRateProviderModel, prepareExchangeRates);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged currency list model
    /// </summary>
    /// <param name="searchModel">Currency search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the currency list model
    /// </returns>
    public virtual async Task<CurrencyListModel> PrepareCurrencyListModelAsync(CurrencySearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get currencies
        var currencies = (await _currencyService.GetAllCurrenciesAsync(showHidden: true)).ToPagedList(searchModel);

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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the currency model
    /// </returns>
    public virtual async Task<CurrencyModel> PrepareCurrencyModelAsync(CurrencyModel model, Currency currency, bool excludeProperties = false)
    {
        Func<CurrencyLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (currency != null)
        {
            //fill in model values from the entity
            model ??= currency.ToModel<CurrencyModel>();

            //convert dates to the user time
            model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(currency.CreatedOnUtc, DateTimeKind.Utc);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(currency, entity => entity.Name, languageId, false, false);
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
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare available stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, currency, excludeProperties);

        return model;
    }

    #endregion
}