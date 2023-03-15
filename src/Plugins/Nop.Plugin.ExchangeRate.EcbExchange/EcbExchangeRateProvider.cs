using System.Globalization;
using System.Xml;
using Nop.Core;
using Nop.Core.Http;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;

namespace Nop.Plugin.ExchangeRate.EcbExchange
{
    public class EcbExchangeRateProvider : BasePlugin, IExchangeRateProvider
    {
        #region Fields

        protected readonly EcbExchangeRateSettings _ecbExchangeRateSettings;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ILocalizationService _localizationService;
        protected readonly ILogger _logger;
        protected readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public EcbExchangeRateProvider(EcbExchangeRateSettings ecbExchangeRateSettings,
            IHttpClientFactory httpClientFactory,
            ILocalizationService localizationService,
            ILogger logger,
            ISettingService settingService)
        {
            _ecbExchangeRateSettings = ecbExchangeRateSettings;
            _httpClientFactory = httpClientFactory;
            _localizationService = localizationService;
            _logger = logger;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the exchange rates
        /// </returns>
        public async Task<IList<Core.Domain.Directory.ExchangeRate>> GetCurrencyLiveRatesAsync(string exchangeRateCurrencyCode)
        {
            if (exchangeRateCurrencyCode == null)
                throw new ArgumentNullException(nameof(exchangeRateCurrencyCode));

            //add euro with rate 1
            var ratesToEuro = new List<Core.Domain.Directory.ExchangeRate>
            {
                new Core.Domain.Directory.ExchangeRate
                {
                    CurrencyCode = "EUR",
                    Rate = 1,
                    UpdatedOn = DateTime.UtcNow
                }
            };

            //get exchange rates to euro from European Central Bank
            try
            {
                var httpClient = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
                var stream = await httpClient.GetStreamAsync(_ecbExchangeRateSettings.EcbLink);

                //load XML document
                var document = new XmlDocument();
                document.Load(stream);

                //add namespaces
                var namespaces = new XmlNamespaceManager(document.NameTable);
                namespaces.AddNamespace("ns", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
                namespaces.AddNamespace("gesmes", "http://www.gesmes.org/xml/2002-08-01");

                //get daily rates
                var dailyRates = document.SelectSingleNode("gesmes:Envelope/ns:Cube/ns:Cube", namespaces);
                if (!DateTime.TryParseExact(dailyRates.Attributes["time"].Value, "yyyy-MM-dd", null, DateTimeStyles.None, out var updateDate))
                    updateDate = DateTime.UtcNow;

                foreach (XmlNode currency in dailyRates.ChildNodes)
                {
                    //get rate
                    if (!decimal.TryParse(currency.Attributes["rate"].Value, NumberStyles.Currency, CultureInfo.InvariantCulture, out var currencyRate))
                        continue;

                    ratesToEuro.Add(new Core.Domain.Directory.ExchangeRate()
                    {
                        CurrencyCode = currency.Attributes["currency"].Value,
                        Rate = currencyRate,
                        UpdatedOn = updateDate
                    });
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("ECB exchange rate provider", ex);
            }

            //return result for the euro
            if (exchangeRateCurrencyCode.Equals("eur", StringComparison.InvariantCultureIgnoreCase))
                return ratesToEuro;

            //use only currencies that are supported by ECB
            var exchangeRateCurrency = ratesToEuro.FirstOrDefault(rate => rate.CurrencyCode.Equals(exchangeRateCurrencyCode, StringComparison.InvariantCultureIgnoreCase));
            if (exchangeRateCurrency == null)
                throw new NopException(await _localizationService.GetResourceAsync("Plugins.ExchangeRate.EcbExchange.Error"));

            //return result for the selected (not euro) currency
            return ratesToEuro.Select(rate => new Core.Domain.Directory.ExchangeRate
            {
                CurrencyCode = rate.CurrencyCode,
                Rate = Math.Round(rate.Rate / exchangeRateCurrency.Rate, 4),
                UpdatedOn = rate.UpdatedOn
            }).ToList();
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            var defaultSettings = new EcbExchangeRateSettings
            {
                EcbLink = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"
            };
            await _settingService.SaveSettingAsync(defaultSettings);

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.ExchangeRate.EcbExchange.Error", "You can use ECB (European central bank) exchange rate provider only when the primary exchange rate currency is supported by ECB");

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<EcbExchangeRateSettings>();

            //locales
            await _localizationService.DeleteLocaleResourceAsync("Plugins.ExchangeRate.EcbExchange.Error");

            await base.UninstallAsync();
        }

        #endregion
    }
}