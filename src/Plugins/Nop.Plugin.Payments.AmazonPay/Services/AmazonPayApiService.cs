using Amazon.Pay.API;
using Amazon.Pay.API.Types;
using Amazon.Pay.API.WebStore;
using Amazon.Pay.API.WebStore.Buyer;
using Amazon.Pay.API.WebStore.CheckoutSession;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Payments.AmazonPay.Domain;
using Nop.Plugin.Payments.AmazonPay.Enums;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Web.Framework.Mvc.Routing;
using AmazonCurrency = Amazon.Pay.API.Types.Currency;
using Currency = Nop.Core.Domain.Directory.Currency;
using Environment = Amazon.Pay.API.Types.Environment;

namespace Nop.Plugin.Payments.AmazonPay.Services;

/// <summary>
/// Represents the service to call Amazon API
/// </summary>
public class AmazonPayApiService
{
    #region Fields

    private WebStoreClient _client;

    private readonly AmazonPaySettings _amazonPaySettings;
    private readonly CurrencySettings _currencySettings;
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly ICurrencyService _currencyService;
    private readonly ILogger _logger;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IPaymentPluginManager _paymentPluginManager;
    private readonly IStoreContext _storeContext;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AmazonPayApiService(AmazonPaySettings amazonPaySettings,
        CurrencySettings currencySettings,
        IActionContextAccessor actionContextAccessor,
        ICurrencyService currencyService,
        ILogger logger,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPaymentPluginManager paymentPluginManager,
        IStoreContext storeContext,
        IUrlHelperFactory urlHelperFactory,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
        _client = null;

        _amazonPaySettings = amazonPaySettings;
        _currencySettings = currencySettings;
        _actionContextAccessor = actionContextAccessor;
        _currencyService = currencyService;
        _logger = logger;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _paymentPluginManager = paymentPluginManager;
        _storeContext = storeContext;
        _urlHelperFactory = urlHelperFactory;
        _webHelper = webHelper;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets primary store (for US and JP regions) or customer (EU and UK regions) currency
    /// </summary>
    /// <param name="currencyCode">Currency code; pass null to get working currency</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the code, currency
    /// </returns>
    public async Task<(AmazonCurrency Code, Currency Currency)> GetCurrencyAsync(string currencyCode = null)
    {
        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        _ = Enum.GetValues(typeof(AmazonCurrency)).OfType<AmazonCurrency?>()
            .FirstOrDefault(p => p.ToString()!.Equals(primaryStoreCurrency.CurrencyCode, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new NopException($"'{primaryStoreCurrency.CurrencyCode}' currency is not supported");

        var currency = await _currencyService.GetCurrencyByCodeAsync(currencyCode)
            ?? primaryStoreCurrency;
        if (_amazonPaySettings.PaymentRegion == PaymentRegion.Eu || _amazonPaySettings.PaymentRegion == PaymentRegion.Uk)
            currency = await _workContext.GetWorkingCurrencyAsync();

        var code = Enum.GetValues(typeof(AmazonCurrency)).OfType<AmazonCurrency?>()
            .FirstOrDefault(p => p.ToString()!.Equals(currency.CurrencyCode, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new NopException($"'{currency.CurrencyCode}' currency is not supported");

        return (code, currency);
    }

    /// <summary>
    /// Ensure primary store currency is valid
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task EnsureCurrencyIsValidAsync()
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        try
        {
            var (currencyCode, _) = await GetCurrencyAsync();
            var incorrectCurrency =
                _amazonPaySettings.PaymentRegion == PaymentRegion.Us && currencyCode != AmazonCurrency.USD ||
                _amazonPaySettings.PaymentRegion == PaymentRegion.Jp && currencyCode != AmazonCurrency.JPY;
            if (incorrectCurrency)
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.Currency.Incorrect.Warning");
                var region = await _localizationService.GetLocalizedEnumAsync(_amazonPaySettings.PaymentRegion);
                var warning = string.Format(locale, urlHelper.Action("List", "Currency"), currencyCode, region);
                _notificationService.WarningNotification(warning, false);
            }
        }
        catch
        {
            var locale = await _localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.Currency.Warning");
            var warning = string.Format(locale, urlHelper.Action("List", "Currency"));
            _notificationService.WarningNotification(warning, false);
        }
    }

    /// <summary>
    /// Performs API request
    /// </summary>
    /// <typeparam name="TResponse">Amazon pay response type</typeparam>
    /// <param name="action">Action to perform request</param>
    /// <param name="badRequestHandler">Bad request handler to replace default one</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the Amazon pay response
    /// </returns>
    public async Task<TResponse> PerformRequestAsync<TResponse>(Func<WebStoreClient, TResponse> action, Func<RequestErrorMessage, Task> badRequestHandler = null)
        where TResponse : AmazonPayResponse
    {
        // send the request
        var result = action(ApiClient);

        if (_amazonPaySettings.EnableLogging)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            var logMessage = $"{AmazonPayDefaults.PluginSystemName} request to {result.Url} ({result.Method})";

            if (!string.IsNullOrEmpty(result.RawRequest))
                logMessage += $"{System.Environment.NewLine}{result.RawRequest}";

            await _logger.InsertLogAsync(LogLevel.Debug, $"{AmazonPayDefaults.PluginSystemName} request details", logMessage, customer);
            logMessage = $"{System.Environment.NewLine}{result.RawResponse}";
            await _logger.InsertLogAsync(LogLevel.Debug, $"{AmazonPayDefaults.PluginSystemName} response details", logMessage, customer);
        }

        // check if API call was successful
        if (!result.Success)
        {
            var message = RequestErrorMessage.Create(result.RawResponse);

            if (badRequestHandler != null)
                await badRequestHandler(message);
            else
                throw new NopException(message.Message);
        }

        return result;
    }

    /// <summary>
    /// Check whether the plugin is active and configured
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result of check
    /// </returns>
    public async Task<bool> IsActiveAndConfiguredAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        if (!await _paymentPluginManager.IsPluginActiveAsync(AmazonPayDefaults.PluginSystemName, customer, store?.Id ?? 0))
            return false;

        if (string.IsNullOrEmpty(_amazonPaySettings.PrivateKey)
            || string.IsNullOrEmpty(_amazonPaySettings.StoreId)
            || string.IsNullOrEmpty(_amazonPaySettings.PublicKeyId)
            || string.IsNullOrEmpty(_amazonPaySettings.MerchantId))
            return false;

        return true;
    }

    /// <summary>
    /// Gets URL address by route name
    /// </summary>
    /// <param name="routeName">Route name</param>
    /// <returns>URL address</returns>
    public string GetUrl(string routeName)
    {
        if (_actionContextAccessor.ActionContext == null)
            return null;

        return _urlHelperFactory
            .GetUrlHelper(_actionContextAccessor.ActionContext)
            .RouteUrl(routeName, null, _webHelper.GetCurrentRequestProtocol());
    }

    /// <summary>
    /// Generate button signature
    /// </summary>
    /// <param name="request">Request to generate signature</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the button signature
    /// </returns>
    public async Task<string> GenerateButtonSignatureAsync(CreateCheckoutSessionRequest request)
    {
        try
        {
            return ApiClient.GenerateButtonSignature(request);
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());

            return null;
        }
    }

    /// <summary>
    /// Generate button signature
    /// </summary>
    /// <param name="request">Request to generate signature</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the button signature
    /// </returns>
    public async Task<string> GenerateButtonSignatureAsync(SignInRequest request)
    {
        try
        {
            return ApiClient.GenerateButtonSignature(request);
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());

            return null;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets API client
    /// </summary>
    private WebStoreClient ApiClient
    {
        get
        {
            if (_client != null)
                return _client;

            var region = Region.UnitedStates;

            switch (_amazonPaySettings.PaymentRegion)
            {
                case PaymentRegion.Eu:
                case PaymentRegion.Uk:
                    region = Region.Europe;
                    break;
                case PaymentRegion.Jp:
                    region = Region.Japan;
                    break;
            }

            // set up config
            var payConfiguration = new ApiConfiguration
            (
                region: region,
                environment: _amazonPaySettings.UseSandbox ? Environment.Sandbox : Environment.Live,
                publicKeyId: _amazonPaySettings.PublicKeyId,
                privateKey: _amazonPaySettings.PrivateKey,
                algorithm: AmazonSignatureAlgorithm.V2
            );

            // init API client
            _client = new WebStoreClient(payConfiguration);

            return _client;
        }
    }

    /// <summary>
    /// Gets Amazon Pay script URL
    /// </summary>
    public string AmazonPayScriptUrl => _amazonPaySettings.PaymentRegion switch
    {
        PaymentRegion.Us => "https://static-na.payments-amazon.com/checkout.js",
        PaymentRegion.Jp => "https://static-fe.payments-amazon.com/checkout.js",
        PaymentRegion.Eu or
        PaymentRegion.Uk => "https://static-eu.payments-amazon.com/checkout.js",
        _ => string.Empty
    };

    /// <summary>
    /// Gets ledger currency
    /// </summary>
    public AmazonCurrency? LedgerCurrency => _amazonPaySettings.PaymentRegion switch
    {
        PaymentRegion.Us => AmazonCurrency.USD,
        PaymentRegion.Jp => AmazonCurrency.JPY,
        PaymentRegion.Eu => AmazonCurrency.EUR,
        PaymentRegion.Uk => AmazonCurrency.GBP,
        _ => null
    };

    /// <summary>
    /// Gets custom request headers
    /// </summary>
    public Dictionary<string, string> Headers => new()
    {
        ["x-amz-pay-sdk-type"] = Constants.SdkName,
        //["x-amz-pay-language-version"] = "",
        ["x-amz-pay-sdk-version"] = Constants.SdkVersion,
        ["x-amz-pay-integrator-id"] = AmazonPayDefaults.SpId,
        ["x-amz-pay-integrator-version"] = AmazonPayDefaults.PluginVersion,
        ["x-amz-pay-platform-version"] = NopVersion.FULL_VERSION,
    };

    #endregion
}