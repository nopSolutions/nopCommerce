using CyberSource.Api;
using CyberSource.Client;
using CyberSource.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Security;
using Nop.Plugin.Payments.CyberSource.Domain;
using Nop.Plugin.Payments.CyberSource.Services.Helpers;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Security;

namespace Nop.Plugin.Payments.CyberSource.Services
{
    /// <summary>
    /// Represents the plugin service manager
    /// </summary>
    public class CyberSourceService
    {
        #region Fields

        protected readonly CurrencySettings _currencySettings;
        protected readonly CustomerTokenService _customerTokenService;
        protected readonly CyberSourceSettings _cyberSourceSettings;
        protected readonly IAddressService _addressService;
        protected readonly ICountryService _countryService;
        protected readonly ICurrencyService _currencyService;
        protected readonly ICustomerService _customerService;
        protected readonly IEncryptionService _encryptionService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger _logger;
        protected readonly IStateProvinceService _stateProvinceService;
        protected readonly IStoreContext _storeContext;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;
        protected readonly ProxySettings _proxySettings;

        #endregion

        #region Ctor

        public CyberSourceService(CurrencySettings currencySettings,
            CustomerTokenService customerTokenService,
            CyberSourceSettings cyberSourceSettings,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IEncryptionService encryptionService,
            IHttpContextAccessor httpContextAccessor,
            ILogger logger,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            ProxySettings proxySettings)
        {
            _currencySettings = currencySettings;
            _customerTokenService = customerTokenService;
            _cyberSourceSettings = cyberSourceSettings;
            _addressService = addressService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _encryptionService = encryptionService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _proxySettings = proxySettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function</param>
        /// <param name="logErrors">Whether to log errors</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result; error message if exists
        /// </returns>
        protected async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function, bool logErrors = true)
        {
            try
            {
                //ensure that plugin is configured
                if (!IsConfigured(_cyberSourceSettings))
                    throw new NopException("Plugin not configured");

                //invoke function
                return (await function(), default);
            }
            catch (Exception exception)
            {
                var message = exception.Message;
                if (exception is ApiException apiException)
                {
                    try
                    {
                        var errorContent = JsonConvert.DeserializeObject<ErrorContent>(Convert.ToString(apiException.ErrorContent));
                        if (!string.IsNullOrEmpty(errorContent.Message))
                            message = errorContent.Message;
                    }
                    catch { }
                }

                if (logErrors)
                {
                    var logMessage = $"{CyberSourceDefaults.SystemName} error: {Environment.NewLine}{message}";
                    await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());
                }

                return (default, message);
            }
        }

        /// <summary>
        /// Get API client configuration
        /// </summary>
        /// <returns>
        /// Client configuration
        /// </returns>
        protected Configuration GetConfiguration()
        {
            var config = new Dictionary<string, string>
            {
                //authentication config
                ["authenticationType"] = "HTTP_SIGNATURE",
                ["merchantID"] = _cyberSourceSettings.MerchantId,
                ["merchantsecretKey"] = _cyberSourceSettings.SecretKey,
                ["merchantKeyId"] = _cyberSourceSettings.KeyId,
                ["runEnvironment"] = _cyberSourceSettings.UseSandbox ? CyberSourceDefaults.TestApiBaseUrl : CyberSourceDefaults.LiveApiBaseUrl,
                ["timeout"] = ((_cyberSourceSettings.RequestTimeout ?? CyberSourceDefaults.RequestTimeout) * 1000).ToString(),

                //meta config
                ["useMetaKey"] = false.ToString().ToLower(),
                ["portfolioID"] = string.Empty,

                //OAuth config
                ["enableClientCert"] = false.ToString().ToLower(),
                ["clientCertDirectory"] = string.Empty,
                ["clientCertFile"] = string.Empty,
                ["clientCertPassword"] = string.Empty,
                ["clientId"] = string.Empty,
                ["clientSecret"] = string.Empty,

                //proxy config
                ["useProxy"] = _proxySettings.Enabled.ToString().ToLower(),
                ["proxyAddress"] = _proxySettings.Enabled ? _proxySettings.Address : string.Empty,
                ["proxyPort"] = _proxySettings.Enabled ? _proxySettings.Port : string.Empty,
                ["proxyUsername"] = _proxySettings.Enabled ? _proxySettings.Username : string.Empty,
                ["proxyPassword"] = _proxySettings.Enabled ? _proxySettings.Password : string.Empty,

                //solution config
                ["solutionID"] = CyberSourceDefaults.SolutionId
            };

            return new Configuration(merchConfigDictObj: config, userAgent: CyberSourceDefaults.UserAgent);
        }

        #region Payments

        /// <summary>
        /// Authorize a payment request
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="capture">Authorization with capture (Sale)</param>
        /// <param name="saveCardOnFile">Save card on file</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the authorized response
        /// </returns>
        protected async Task<PtsV2PaymentsPost201Response> AuthorizeAsync(Nop.Services.Payments.ProcessPaymentRequest processPaymentRequest,
            bool capture, bool saveCardOnFile)
        {
            var cardNumber = CreditCardHelper.RemoveSpecialCharacters(processPaymentRequest.CreditCardNumber);

            var clientReferenceInformation = new Ptsv2paymentsClientReferenceInformation(Code: processPaymentRequest.OrderGuid.ToString(),
                Partner: new Ptsv2paymentsClientReferenceInformationPartner { SolutionId = CyberSourceDefaults.SolutionId });
            var authorizationOptions = new Ptsv2paymentsProcessingInformationAuthorizationOptions(
                IgnoreAvsResult: _cyberSourceSettings.AvsActionType == AvsActionType.Ignore,
                IgnoreCvResult: _cyberSourceSettings.CvnActionType == CvnActionType.Ignore);
            var processingInformation = new Ptsv2paymentsProcessingInformation(Capture: capture,
                AuthorizationOptions: authorizationOptions,
                ActionList: !_cyberSourceSettings.DecisionManagerEnabled ? new() { CyberSourceDefaults.DecisionSkipActionName } : new());
            var paymentInformationCard = new Ptsv2paymentsPaymentInformationCard(Number: cardNumber,
                ExpirationMonth: processPaymentRequest.CreditCardExpireMonth.ToString(),
                ExpirationYear: processPaymentRequest.CreditCardExpireYear.ToString(),
                SecurityCode: processPaymentRequest.CreditCardCvv2);
            var paymentInformation = new Ptsv2paymentsPaymentInformation(Card: paymentInformationCard);
            var orderInformation = await PrepareOrderInformationAsync(processPaymentRequest.CustomerId, processPaymentRequest.OrderTotal);
            var deviceInformation = new Ptsv2paymentsDeviceInformation(HostName: _webHelper.GetStoreHost(false),
                IpAddress: _webHelper.GetCurrentIpAddress(),
                UserAgent: _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent]);
            var requestObj = new CreatePaymentRequest(ClientReferenceInformation: clientReferenceInformation,
                ProcessingInformation: processingInformation,
                PaymentInformation: paymentInformation,
                OrderInformation: orderInformation,
                DeviceInformation: deviceInformation);

            if (saveCardOnFile)
            {
                requestObj.ProcessingInformation.ActionList.Add(CyberSourceDefaults.TokenCreateActionName);
                requestObj.ProcessingInformation.ActionTokenTypes = new List<string>
                {
                    CyberSourceDefaults.CustomerActionTokenTypeName,
                    CyberSourceDefaults.PaymentInstrumentActionTokenTypeName
                };
            }

            var apiInstance = new PaymentsApi(GetConfiguration());
            var result = await apiInstance.CreatePaymentAsync(requestObj);

            return result;
        }

        /// <summary>
        /// Authorize a payment request with token
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="customerTokenId">Cybersource customer token id</param>
        /// <param name="capture">Authorization with capture (Sale)</param>
        /// <param name="authenticationTransactionId">Payer authentication transaction id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the authorized response
        /// </returns>
        protected async Task<PtsV2PaymentsPost201Response> AuthorizeWithTokenAsync(Nop.Services.Payments.ProcessPaymentRequest processPaymentRequest,
            int customerTokenId, bool capture, string authenticationTransactionId)
        {
            if (_cyberSourceSettings.PaymentConnectionMethod == ConnectionMethodType.FlexMicroForm &&
                _cyberSourceSettings.PayerAuthenticationEnabled &&
                string.IsNullOrEmpty(authenticationTransactionId))
            {
                throw new NopException("Payer authentication failed");
            }

            //ensure that tokenization is enabled
            if (!_cyberSourceSettings.TokenizationEnabled)
                throw new NopException("Tokenization not enabled");

            var customerToken = await _customerTokenService.GetByIdAsync(customerTokenId)
                ?? throw new NopException("Saved card data not found");

            if (customerToken.CustomerId != processPaymentRequest.CustomerId)
                throw new NopException("Saved card data is incorrect");

            //ensure that token is valid
            if (string.IsNullOrEmpty(customerToken.SubscriptionId))
                throw new NopException("Invalid token");

            var clientReferenceInformation = new Ptsv2paymentsClientReferenceInformation(Code: processPaymentRequest.OrderGuid.ToString(),
                Partner: new Ptsv2paymentsClientReferenceInformationPartner { SolutionId = CyberSourceDefaults.SolutionId });
            var authorizationOptions = new Ptsv2paymentsProcessingInformationAuthorizationOptions(
                IgnoreAvsResult: _cyberSourceSettings.AvsActionType == AvsActionType.Ignore,
                IgnoreCvResult: _cyberSourceSettings.CvnActionType == CvnActionType.Ignore);
            var processingInformation = new Ptsv2paymentsProcessingInformation(Capture: capture,
                AuthorizationOptions: authorizationOptions,
                ActionList: !_cyberSourceSettings.DecisionManagerEnabled ? new() { CyberSourceDefaults.DecisionSkipActionName } : new());
            var paymentInformationCard = new Ptsv2paymentsPaymentInformationCard(ExpirationMonth: customerToken.CardExpirationMonth,
                ExpirationYear: customerToken.CardExpirationYear);
            var paymentInstrument = new Ptsv2paymentsPaymentInformationPaymentInstrument(Id: customerToken.SubscriptionId);
            var paymentInformation = new Ptsv2paymentsPaymentInformation(Card: paymentInformationCard,
                PaymentInstrument: paymentInstrument);
            var orderInformation = await PrepareOrderInformationAsync(processPaymentRequest.CustomerId, processPaymentRequest.OrderTotal);
            var deviceInformation = new Ptsv2paymentsDeviceInformation(HostName: _webHelper.GetStoreHost(false),
                IpAddress: _webHelper.GetCurrentIpAddress(),
                UserAgent: _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent]);
            var requestObj = new CreatePaymentRequest(ClientReferenceInformation: clientReferenceInformation,
                ProcessingInformation: processingInformation,
                PaymentInformation: paymentInformation,
                OrderInformation: orderInformation,
                DeviceInformation: deviceInformation);

            if (_cyberSourceSettings.PaymentConnectionMethod == ConnectionMethodType.FlexMicroForm && _cyberSourceSettings.PayerAuthenticationEnabled)
            {
                requestObj.ProcessingInformation.ActionList.Add(CyberSourceDefaults.ValidatePayerAuthActionName);
                requestObj.ConsumerAuthenticationInformation =
                    new Ptsv2paymentsConsumerAuthenticationInformation(AuthenticationTransactionId: authenticationTransactionId);
            }

            var apiInstance = new PaymentsApi(GetConfiguration());
            var result = await apiInstance.CreatePaymentAsync(requestObj);

            return result;
        }

        /// <summary>
        /// Authorize a payment request with flex token
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="transientToken">Flex microform transient token</param>
        /// <param name="capture">Authorization with capture (Sale)</param>
        /// <param name="saveCardOnFile">Save card on file</param>
        /// <param name="authenticationTransactionId">Payer authentication transaction id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the authorized response
        /// </returns>
        protected async Task<PtsV2PaymentsPost201Response> AuthorizeWithTransientTokenAsync(Nop.Services.Payments.ProcessPaymentRequest processPaymentRequest,
            string transientToken, bool capture, bool saveCardOnFile, string authenticationTransactionId)
        {
            //ensure that flex microform flow is enabled
            if (_cyberSourceSettings.PaymentConnectionMethod != ConnectionMethodType.FlexMicroForm)
                throw new NopException("Flex microform flow not enabled");

            if (_cyberSourceSettings.PayerAuthenticationEnabled && string.IsNullOrEmpty(authenticationTransactionId))
                throw new NopException("Payer authentication failed");

            //ensure that token is valid
            if (string.IsNullOrEmpty(transientToken))
                throw new NopException("Invalid transient token");

            var clientReferenceInformation = new Ptsv2paymentsClientReferenceInformation(Code: processPaymentRequest.OrderGuid.ToString(),
                Partner: new Ptsv2paymentsClientReferenceInformationPartner { SolutionId = CyberSourceDefaults.SolutionId });
            var authorizationOptions = new Ptsv2paymentsProcessingInformationAuthorizationOptions(
                IgnoreAvsResult: _cyberSourceSettings.AvsActionType == AvsActionType.Ignore,
                IgnoreCvResult: _cyberSourceSettings.CvnActionType == CvnActionType.Ignore);
            var processingInformation = new Ptsv2paymentsProcessingInformation(Capture: capture,
                AuthorizationOptions: authorizationOptions,
                ActionList: !_cyberSourceSettings.DecisionManagerEnabled ? new() { CyberSourceDefaults.DecisionSkipActionName } : new());
            var orderInformation = await PrepareOrderInformationAsync(processPaymentRequest.CustomerId, processPaymentRequest.OrderTotal);
            var deviceInformation = new Ptsv2paymentsDeviceInformation(HostName: _webHelper.GetStoreHost(false),
                IpAddress: _webHelper.GetCurrentIpAddress(),
                UserAgent: _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent]);
            var tokenInformation = new Ptsv2paymentsTokenInformation { TransientTokenJwt = transientToken };
            var requestObj = new CreatePaymentRequest(ClientReferenceInformation: clientReferenceInformation,
                ProcessingInformation: processingInformation,
                TokenInformation: tokenInformation,
                OrderInformation: orderInformation,
                DeviceInformation: deviceInformation);

            if (saveCardOnFile)
            {
                requestObj.ProcessingInformation.ActionList.Add(CyberSourceDefaults.TokenCreateActionName);
                requestObj.ProcessingInformation.ActionTokenTypes = new List<string>
                {
                    CyberSourceDefaults.CustomerActionTokenTypeName,
                    CyberSourceDefaults.PaymentInstrumentActionTokenTypeName
                };
            }

            if (_cyberSourceSettings.PayerAuthenticationEnabled)
            {
                requestObj.ProcessingInformation.ActionList.Add(CyberSourceDefaults.ValidatePayerAuthActionName);
                requestObj.ConsumerAuthenticationInformation =
                    new Ptsv2paymentsConsumerAuthenticationInformation(AuthenticationTransactionId: authenticationTransactionId);
            }

            var apiInstance = new PaymentsApi(GetConfiguration());
            var result = await apiInstance.CreatePaymentAsync(requestObj);

            return result;
        }

        /// <summary>
        /// Prepare order information
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="orderTotal">Order total</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order information
        /// </returns>
        protected async Task<Ptsv2paymentsOrderInformation> PrepareOrderInformationAsync(int customerId, decimal orderTotal)
        {
            var currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            if (string.IsNullOrEmpty(currency))
                throw new NopException("Primary store currency not set");

            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            var billingAddress = await _addressService.GetAddressByIdAsync(customer.BillingAddressId ?? 0);
            var billingCountry = await _countryService.GetCountryByIdAsync(billingAddress?.CountryId ?? 0);
            var billingStateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(billingAddress?.StateProvinceId ?? 0);
            var shippingAddress = await _addressService.GetAddressByIdAsync(customer.ShippingAddressId ?? 0);
            var shippingCountry = await _countryService.GetCountryByIdAsync(shippingAddress?.CountryId ?? 0);
            var shippingStateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(shippingAddress?.StateProvinceId ?? 0);

            var orderInformationAmountDetails = new Ptsv2paymentsOrderInformationAmountDetails(TotalAmount: orderTotal.ToString(),
                Currency: currency);
            var orderInformationBillTo = new Ptsv2paymentsOrderInformationBillTo(FirstName: billingAddress?.FirstName,
                LastName: billingAddress?.LastName,
                Address1: billingAddress?.Address1,
                Address2: billingAddress?.Address2,
                PostalCode: billingAddress?.ZipPostalCode,
                Locality: billingAddress?.City,
                AdministrativeArea: billingStateProvince?.Abbreviation,
                Country: billingCountry?.TwoLetterIsoCode,
                Email: billingAddress?.Email,
                PhoneNumber: billingAddress?.PhoneNumber);
            var orderInformationShipTo = new Ptsv2paymentsOrderInformationShipTo(FirstName: shippingAddress?.FirstName,
                LastName: shippingAddress?.LastName,
                Address1: shippingAddress?.Address1,
                Address2: shippingAddress?.Address2,
                PostalCode: shippingAddress?.ZipPostalCode,
                Locality: shippingAddress?.City,
                AdministrativeArea: shippingStateProvince?.Abbreviation,
                Country: shippingCountry?.TwoLetterIsoCode,
                PhoneNumber: shippingAddress?.PhoneNumber);
            var orderInformation = new Ptsv2paymentsOrderInformation(AmountDetails: orderInformationAmountDetails,
                BillTo: orderInformationBillTo,
                ShipTo: orderInformationShipTo);

            return orderInformation;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Check whether the plugin is configured
        /// </summary>
        /// <param name="settings">Plugin settings</param>
        /// <returns>Result</returns>
        public static bool IsConfigured(CyberSourceSettings settings)
        {
            return !string.IsNullOrEmpty(settings.MerchantId)
                && !string.IsNullOrEmpty(settings.KeyId)
                && !string.IsNullOrEmpty(settings.SecretKey);
        }

        #region Payments

        /// <summary>
        /// Authorize a payment request
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="isNewCard">Whether it's a new card</param>
        /// <param name="saveCardOnFile">Whether to save card on file for future use</param>
        /// <param name="customerTokenId">CyberSource customer token id</param>
        /// <param name="transientToken">Flex microform transient token</param>
        /// <param name="authenticationTransactionId">Authentication transaction id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the authorized response; error message if exists
        /// </returns>
        public async Task<(PtsV2PaymentsPost201Response Result, string Error)> AuthorizeAsync(Nop.Services.Payments.ProcessPaymentRequest processPaymentRequest,
            bool isNewCard = false, bool saveCardOnFile = false,
            int? customerTokenId = null, string transientToken = null, string authenticationTransactionId = null)
        {
            return await HandleFunctionAsync(async () =>
            {
                //pay with saved card
                if (!isNewCard)
                {
                    var authorization = await AuthorizeWithTokenAsync(processPaymentRequest: processPaymentRequest,
                        customerTokenId: customerTokenId ?? 0,
                        capture: _cyberSourceSettings.TransactionType == TransactionType.Sale,
                        authenticationTransactionId: authenticationTransactionId);

                    return authorization;
                }

                //pay with flex token
                if (_cyberSourceSettings.PaymentConnectionMethod == ConnectionMethodType.FlexMicroForm)
                {
                    var authorization = await AuthorizeWithTransientTokenAsync(processPaymentRequest: processPaymentRequest,
                        transientToken: transientToken,
                        capture: _cyberSourceSettings.TransactionType == TransactionType.Sale,
                        saveCardOnFile: saveCardOnFile,
                        authenticationTransactionId: authenticationTransactionId);

                    return authorization;
                }

                //pay with new card
                if (_cyberSourceSettings.PaymentConnectionMethod == ConnectionMethodType.RestApi)
                {
                    var authorization = await AuthorizeAsync(processPaymentRequest: processPaymentRequest,
                        capture: _cyberSourceSettings.TransactionType == TransactionType.Sale,
                        saveCardOnFile: saveCardOnFile);

                    return authorization;
                }

                return default;
            });
        }

        /// <summary>
        /// Capture an authorization
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the captured response; error message if exists
        /// </returns>
        public async Task<(PtsV2PaymentsCapturesPost201Response Result, string Error)> CaptureAsync(Nop.Services.Payments.CapturePaymentRequest capturePaymentRequest)
        {
            return await HandleFunctionAsync(async () =>
            {
                var currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
                if (string.IsNullOrEmpty(currency))
                    throw new NopException("Primary store currency not set");

                var clientReferenceInformation = new Ptsv2paymentsClientReferenceInformation(Code: capturePaymentRequest.Order?.OrderGuid.ToString(),
                    Partner: new Ptsv2paymentsClientReferenceInformationPartner { SolutionId = CyberSourceDefaults.SolutionId });
                var orderInformationAmountDetails = new Ptsv2paymentsidcapturesOrderInformationAmountDetails(TotalAmount: capturePaymentRequest.Order?.OrderTotal.ToString(),
                    Currency: currency);
                var orderInformation = new Ptsv2paymentsidcapturesOrderInformation(AmountDetails: orderInformationAmountDetails);
                var requestObj = new CapturePaymentRequest(ClientReferenceInformation: clientReferenceInformation,
                    OrderInformation: orderInformation);

                var apiInstance = new CaptureApi(GetConfiguration());
                var result = await apiInstance.CapturePaymentAsync(capturePaymentRequest: requestObj,
                    id: capturePaymentRequest.Order?.AuthorizationTransactionId);

                return result;
            });
        }

        /// <summary>
        /// Void a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Void payment request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the voided response; Error message if exists
        /// </returns>
        public async Task<(PtsV2PaymentsVoidsPost201Response Result, string Error)> VoidPaymentAsync(Nop.Services.Payments.VoidPaymentRequest voidPaymentRequest)
        {
            return await HandleFunctionAsync(async () =>
            {
                var clientReferenceInformation = new Ptsv2paymentsidreversalsClientReferenceInformation(Code: voidPaymentRequest.Order?.OrderGuid.ToString(),
                    Partner: new Ptsv2paymentsidreversalsClientReferenceInformationPartner { SolutionId = CyberSourceDefaults.SolutionId });
                var requestObj = new VoidPaymentRequest(ClientReferenceInformation: clientReferenceInformation);

                var apiInstance = new VoidApi(GetConfiguration());
                var result = await apiInstance.VoidPaymentAsync(voidPaymentRequest: requestObj,
                    id: voidPaymentRequest.Order?.AuthorizationTransactionId);

                return result;
            });
        }

        /// <summary>
        /// Void a capture
        /// </summary>
        /// <param name="voidPaymentRequest">Void payment request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the voided response; Error message if exists
        /// </returns>
        public async Task<(PtsV2PaymentsVoidsPost201Response Result, string Error)> VoidCaptureAsync(Nop.Services.Payments.VoidPaymentRequest voidPaymentRequest)
        {
            return await HandleFunctionAsync(async () =>
            {
                var clientReferenceInformation = new Ptsv2paymentsidreversalsClientReferenceInformation(Code: voidPaymentRequest.Order?.OrderGuid.ToString(),
                    Partner: new Ptsv2paymentsidreversalsClientReferenceInformationPartner { SolutionId = CyberSourceDefaults.SolutionId });
                var requestObj = new VoidCaptureRequest(ClientReferenceInformation: clientReferenceInformation);

                var apiInstance = new VoidApi(GetConfiguration());
                var result = await apiInstance.VoidCaptureAsync(voidCaptureRequest: requestObj,
                    id: voidPaymentRequest.Order?.CaptureTransactionId);

                return result;
            });
        }

        /// <summary>
        /// Refund a capture
        /// </summary>
        /// <param name="refundPaymentRequest">Refund payment request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the refund details; error message if exists
        /// </returns>
        public async Task<(PtsV2PaymentsRefundPost201Response Result, string Error)> RefundAsync(Nop.Services.Payments.RefundPaymentRequest refundPaymentRequest)
        {
            return await HandleFunctionAsync(async () =>
            {
                var currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
                if (string.IsNullOrEmpty(currency))
                    throw new NopException("Primary store currency not set");

                var clientReferenceInformation = new Ptsv2paymentsClientReferenceInformation(Code: refundPaymentRequest.Order?.OrderGuid.ToString(),
                    Partner: new Ptsv2paymentsClientReferenceInformationPartner { SolutionId = CyberSourceDefaults.SolutionId });
                var orderInformationAmountDetails = new Ptsv2paymentsidcapturesOrderInformationAmountDetails(TotalAmount: refundPaymentRequest.AmountToRefund.ToString(),
                    Currency: currency);
                var orderInformation = new Ptsv2paymentsidrefundsOrderInformation(AmountDetails: orderInformationAmountDetails);
                Ptsv2paymentsidrefundsPaymentInformation paymentInformation = null;
                if (refundPaymentRequest.Order.AllowStoringCreditCardNumber)
                {
                    var cardInformation = new Ptsv2paymentsidrefundsPaymentInformationCard(
                        Number: CreditCardHelper.RemoveSpecialCharacters(_encryptionService.DecryptText(refundPaymentRequest.Order?.CardNumber)),
                        ExpirationMonth: _encryptionService.DecryptText(refundPaymentRequest.Order?.CardExpirationMonth),
                        ExpirationYear: _encryptionService.DecryptText(refundPaymentRequest.Order?.CardExpirationYear));
                    paymentInformation = new Ptsv2paymentsidrefundsPaymentInformation(Card: cardInformation);
                }
                var requestObj = new RefundCaptureRequest(ClientReferenceInformation: clientReferenceInformation,
                    OrderInformation: orderInformation,
                    PaymentInformation: paymentInformation);

                var apiInstance = new RefundApi(GetConfiguration());
                var result = await apiInstance.RefundCaptureAsync(refundCaptureRequest: requestObj,
                    id: refundPaymentRequest.Order?.CaptureTransactionId ?? refundPaymentRequest.Order?.AuthorizationTransactionId);

                return result;
            });
        }

        /// <summary>
        /// Request a standalone credit
        /// </summary>
        /// <param name="refundPaymentRequest">Refund payment request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the credited details; error message if exists
        /// </returns>
        public async Task<(PtsV2CreditsPost201Response Result, string Error)> CreditAsync(Nop.Services.Payments.RefundPaymentRequest refundPaymentRequest)
        {
            return await HandleFunctionAsync(async () =>
            {
                var currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
                if (string.IsNullOrEmpty(currency))
                    throw new NopException("Primary store currency not set");

                var billingAddress = await _addressService.GetAddressByIdAsync(refundPaymentRequest.Order?.BillingAddressId ?? 0);
                var billingCountry = await _countryService.GetCountryByIdAsync(billingAddress?.CountryId ?? 0);
                var billingStateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(billingAddress?.StateProvinceId ?? 0);

                var clientReferenceInformation = new Ptsv2paymentsClientReferenceInformation(Code: refundPaymentRequest.Order?.OrderGuid.ToString(),
                    Partner: new Ptsv2paymentsClientReferenceInformationPartner { SolutionId = CyberSourceDefaults.SolutionId });
                Ptsv2paymentsidrefundsPaymentInformation paymentInformation = null;
                if (refundPaymentRequest.Order.AllowStoringCreditCardNumber)
                {
                    var cardInformation = new Ptsv2paymentsidrefundsPaymentInformationCard(
                        Number: CreditCardHelper.RemoveSpecialCharacters(_encryptionService.DecryptText(refundPaymentRequest.Order?.CardNumber)),
                        ExpirationMonth: _encryptionService.DecryptText(refundPaymentRequest.Order?.CardExpirationMonth),
                        ExpirationYear: _encryptionService.DecryptText(refundPaymentRequest.Order?.CardExpirationYear));
                    paymentInformation = new Ptsv2paymentsidrefundsPaymentInformation(Card: cardInformation);
                }
                var orderInformationBillTo = new Ptsv2paymentsidcapturesOrderInformationBillTo(FirstName: billingAddress?.FirstName,
                    LastName: billingAddress?.LastName,
                    Address1: billingAddress?.Address1,
                    Locality: billingAddress?.City,
                    AdministrativeArea: billingStateProvince?.Abbreviation,
                    PostalCode: billingAddress?.ZipPostalCode,
                    Country: billingCountry?.TwoLetterIsoCode,
                    Email: billingAddress?.Email,
                    PhoneNumber: billingAddress?.PhoneNumber);
                var refundInformationAmountDetails = new Ptsv2paymentsidcapturesOrderInformationAmountDetails(TotalAmount: refundPaymentRequest.AmountToRefund.ToString(),
                    Currency: currency);
                var orderInformation = new Ptsv2paymentsidrefundsOrderInformation(AmountDetails: refundInformationAmountDetails,
                    BillTo: orderInformationBillTo);
                var requestObj = new CreateCreditRequest(ClientReferenceInformation: clientReferenceInformation,
                    PaymentInformation: paymentInformation,
                    OrderInformation: orderInformation);

                var apiInstance = new CreditApi(GetConfiguration());
                var result = apiInstance.CreateCredit(requestObj);

                return result;
            });
        }

        #endregion

        #region Token management

        /// <summary>
        /// Create an instrument identifier
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the instrument identifier created response; Error message if exists
        /// </returns>
        public async Task<(Tmsv2customersEmbeddedDefaultPaymentInstrumentEmbeddedInstrumentIdentifier Result, string Error)> CreateInstrumentIdAsync(string cardNumber)
        {
            return await HandleFunctionAsync(async () =>
            {
                cardNumber = CreditCardHelper.RemoveSpecialCharacters(cardNumber);

                var card = new Tmsv2customersEmbeddedDefaultPaymentInstrumentEmbeddedInstrumentIdentifierCard(Number: cardNumber);
                var requestObj = new PostInstrumentIdentifierRequest(Card: card);

                var apiInstance = new InstrumentIdentifierApi(GetConfiguration());
                var result = await apiInstance.PostInstrumentIdentifierAsync(requestObj);

                return result;
            });
        }

        /// <summary>
        /// Create a payment instrument
        /// </summary>
        /// <param name="instrumentIdentifier">Card instrument identifier</param>
        /// <param name="cardExpirationMonth">Card expiration month</param>
        /// <param name="cardExpirationYear">Card expiration year</param>
        /// <param name="cardType">Card type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment instrument identifier created response; Error message if exists
        /// </returns>
        public async Task<(Tmsv2customersEmbeddedDefaultPaymentInstrument Result, string Error)> CreatePaymentInstrumentAsync(string instrumentIdentifier,
            string cardExpirationMonth, string cardExpirationYear, string cardType)
        {
            return await HandleFunctionAsync(async () =>
            {
                var card = new Tmsv2customersEmbeddedDefaultPaymentInstrumentCard(ExpirationMonth: cardExpirationMonth,
                    ExpirationYear: cardExpirationYear,
                    Type: cardType);
                var paymentInstrumentIdentifier = new Tmsv2customersEmbeddedDefaultPaymentInstrumentInstrumentIdentifier(Id: instrumentIdentifier);
                var requestObj = new PostPaymentInstrumentRequest(Card: card,
                    InstrumentIdentifier: paymentInstrumentIdentifier);

                var apiInstance = new PaymentInstrumentApi(GetConfiguration());
                var result = await apiInstance.PostPaymentInstrumentAsync(postPaymentInstrumentRequest: requestObj);

                return result;
            });
        }

        /// <summary>
        /// Update a payment instrument
        /// </summary>
        /// <param name="paymentInstrumentTokenId">Payment instrument token identifier</param>
        /// <param name="instrumentIdentifier">Card instrument identifier</param>
        /// <param name="cardExpirationMonth">Card expiration month</param>
        /// <param name="cardExpirationYear">Card expiration year</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment instrument identifier updated response; Error message if exists
        /// </returns>
        public async Task<(Tmsv2customersEmbeddedDefaultPaymentInstrument Result, string Error)> UpdatePaymentInstrumentAsync(string paymentInstrumentTokenId,
            string instrumentIdentifier, string cardExpirationMonth, string cardExpirationYear)
        {
            return await HandleFunctionAsync(async () =>
            {
                var card = new Tmsv2customersEmbeddedDefaultPaymentInstrumentCard(ExpirationMonth: cardExpirationMonth,
                    ExpirationYear: cardExpirationYear);
                var paymentInstrumentIdentifier = new Tmsv2customersEmbeddedDefaultPaymentInstrumentInstrumentIdentifier(Id: instrumentIdentifier);
                var requestObj = new PatchPaymentInstrumentRequest(Card: card,
                    InstrumentIdentifier: paymentInstrumentIdentifier);

                var apiInstance = new PaymentInstrumentApi(GetConfiguration());
                var result = await apiInstance.PatchPaymentInstrumentAsync(paymentInstrumentTokenId: paymentInstrumentTokenId,
                    patchPaymentInstrumentRequest: requestObj);

                return result;
            });
        }

        /// <summary>
        /// Delete a payment instrument
        /// </summary>
        /// <param name="paymentInstrumentTokenId">Payment instrument token identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment instrument identifier deleted response; Error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> DeletePaymentInstrumentAsync(string paymentInstrumentTokenId)
        {
            return await HandleFunctionAsync(async () =>
            {
                var apiInstance = new PaymentInstrumentApi(GetConfiguration());
                await apiInstance.DeletePaymentInstrumentAsync(paymentInstrumentTokenId);

                return true;
            });
        }

        /// <summary>
        /// Get an instrument by identifier
        /// </summary>
        /// <param name="instrumentId">Instrument identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the instrument identifier response; Error message if exists
        /// </returns>
        public async Task<(Tmsv2customersEmbeddedDefaultPaymentInstrumentEmbeddedInstrumentIdentifier Result, string Error)> GetInstrumentByIdAsync(string instrumentId)
        {
            return await HandleFunctionAsync(async () =>
            {
                var apiInstance = new InstrumentIdentifierApi(GetConfiguration());
                var result = await apiInstance.GetInstrumentIdentifierAsync(instrumentId);

                return result;
            });
        }

        #endregion

        #region Reporting

        /// <summary>
        /// Get conversion detail transactions report
        /// </summary>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the conversion detail transactions response; Error message if exists
        /// </returns>
        public async Task<(ReportingV3ConversionDetailsGet200Response Result, string Error)> GetConversionDetailTransactionsAsync(DateTime? startTime, DateTime? endTime)
        {
            return await HandleFunctionAsync(async () =>
            {
                var apiInstance = new ConversionDetailsApi(GetConfiguration());
                var result = await apiInstance.GetConversionDetailAsync(startTime, endTime);

                return result;
            }, logErrors: false);
        }

        #endregion

        #region Flex microform

        /// <summary>
        /// Generate flex microform public key
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the generated flex public key response; Error message if exists
        /// </returns>
        public async Task<(FlexV1KeysPost200Response Result, string Error)> GenerateFlexMicroformPublicKeyAsync()
        {
            return await HandleFunctionAsync(async () =>
            {
                if (_cyberSourceSettings.PaymentConnectionMethod != ConnectionMethodType.FlexMicroForm)
                    throw new NopException("Flex microform flow not enabled");

                var store = await _storeContext.GetCurrentStoreAsync();
                var requestObj = new GeneratePublicKeyRequest(EncryptionType: "RsaOaep",
                    TargetOrigin: store.Url.TrimEnd('/'));

                var apiInstance = new KeyGenerationApi(GetConfiguration());
                var result = await apiInstance.GeneratePublicKeyAsync(format: "JWT",
                    generatePublicKeyRequest: requestObj);

                return result;
            });
        }

        #endregion

        #region Payer authentication

        /// <summary>
        /// Setup payer authentication
        /// </summary>
        /// <param name="customerToken">Cybersource customer token</param>
        /// <param name="transientToken">Flex microform transient token</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setup payer authentication response; Error message if exists
        /// </returns>
        public async Task<(RiskV1AuthenticationSetupsPost201Response Result, string Error)> PayerAuthenticationSetupAsync(CyberSourceCustomerToken customerToken,
            string transientToken)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (_cyberSourceSettings.PaymentConnectionMethod != ConnectionMethodType.FlexMicroForm)
                    throw new NopException("Flex microform flow not enabled");

                if (!_cyberSourceSettings.PayerAuthenticationEnabled)
                    throw new NopException("Payer authentication not enabled");

                if (string.IsNullOrEmpty(customerToken?.SubscriptionId) && string.IsNullOrEmpty(transientToken))
                    throw new NopException("Invalid token");

                var clientReferenceInformation = new Riskv1decisionsClientReferenceInformation(Code: Guid.NewGuid().ToString(),
                    Partner: new Riskv1decisionsClientReferenceInformationPartner { SolutionId = CyberSourceDefaults.SolutionId });
                var requestObj = new PayerAuthSetupRequest(ClientReferenceInformation: clientReferenceInformation);
                if (customerToken is not null)
                {
                    var customerInformation = new Riskv1authenticationsetupsPaymentInformationCustomer(CustomerId: customerToken.SubscriptionId);
                    var paymentInformation = new Riskv1authenticationsetupsPaymentInformation(Customer: customerInformation);
                    requestObj.PaymentInformation = paymentInformation;
                }
                else if (!string.IsNullOrEmpty(transientToken))
                    requestObj.TokenInformation = new Riskv1authenticationsetupsTokenInformation { TransientToken = transientToken };

                var apiInstance = new PayerAuthenticationApi(GetConfiguration());
                var result = await apiInstance.PayerAuthSetupAsync(requestObj);

                return result;
            });
        }

        /// <summary>
        /// Check payer authentication enrollment
        /// </summary>
        /// <param name="referenceId">Reference ID that corresponds to the device fingerprinting data</param>
        /// <param name="customerToken">Cybersource customer token</param>
        /// <param name="transientToken">Flex microform transient token</param>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="returnUrl">URL to redirect the payer after the authentication is complete</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checked payer authentication enrollment response; Error message if exists
        /// </returns>
        public async Task<(RiskV1AuthenticationsPost201Response Result, string Error)> PayerAuthenticationEnrollmentAsync(string referenceId,
            CyberSourceCustomerToken customerToken, string transientToken, Nop.Services.Payments.ProcessPaymentRequest processPaymentRequest, string returnUrl)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (_cyberSourceSettings.PaymentConnectionMethod != ConnectionMethodType.FlexMicroForm)
                    throw new NopException("Flex microform flow not enabled");

                if (!_cyberSourceSettings.PayerAuthenticationEnabled)
                    throw new NopException("Payer authentication not enabled");

                if (string.IsNullOrEmpty(customerToken?.SubscriptionId) && string.IsNullOrEmpty(transientToken))
                    throw new NopException("Invalid token");

                var currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
                if (string.IsNullOrEmpty(currency))
                    throw new NopException("Primary store currency not set");

                var customer = await _customerService.GetCustomerByIdAsync(processPaymentRequest.CustomerId);
                var billingAddress = await _addressService.GetAddressByIdAsync(customer.BillingAddressId ?? 0);
                var billingCountry = await _countryService.GetCountryByIdAsync(billingAddress?.CountryId ?? 0);
                var billingStateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(billingAddress?.StateProvinceId ?? 0);

                var clientReferenceInformation = new Riskv1decisionsClientReferenceInformation(Code: processPaymentRequest.OrderGuid.ToString(),
                    Partner: new Riskv1decisionsClientReferenceInformationPartner { SolutionId = CyberSourceDefaults.SolutionId });
                var orderInformationAmountDetails = new Riskv1authenticationsOrderInformationAmountDetails(Currency: currency,
                    TotalAmount: processPaymentRequest.OrderTotal.ToString());
                var orderInformationBillTo = new Riskv1authenticationsOrderInformationBillTo(FirstName: billingAddress?.FirstName,
                    LastName: billingAddress?.LastName,
                    Address1: billingAddress?.Address1,
                    Address2: billingAddress?.Address2,
                    PostalCode: billingAddress?.ZipPostalCode,
                    Locality: billingAddress?.City,
                    AdministrativeArea: billingStateProvince?.Abbreviation,
                    Country: billingCountry?.TwoLetterIsoCode,
                    Email: billingAddress?.Email,
                    PhoneNumber: billingAddress?.PhoneNumber);
                var orderInformation = new Riskv1authenticationsOrderInformation(AmountDetails: orderInformationAmountDetails,
                    BillTo: orderInformationBillTo);
                var consumerAuthenticationInformation = new Riskv1decisionsConsumerAuthenticationInformation(ReferenceId: referenceId,
                    ReturnUrl: returnUrl);
                if (_cyberSourceSettings.PayerAuthenticationRequired)
                    consumerAuthenticationInformation.ChallengeCode = "04"; //challenge requested (mandate)
                var requestObj = new CheckPayerAuthEnrollmentRequest(OrderInformation: orderInformation,
                    ConsumerAuthenticationInformation: consumerAuthenticationInformation);
                if (customerToken is not null)
                {
                    var customerInformation = new Ptsv2paymentsPaymentInformationCustomer(CustomerId: customerToken.SubscriptionId);
                    var paymentInformation = new Riskv1authenticationsPaymentInformation(Customer: customerInformation);
                    requestObj.PaymentInformation = paymentInformation;
                }
                else if (!string.IsNullOrEmpty(transientToken))
                    requestObj.TokenInformation = new Riskv1authenticationsetupsTokenInformation { TransientToken = transientToken };

                var apiInstance = new PayerAuthenticationApi(GetConfiguration());
                var result = await apiInstance.CheckPayerAuthEnrollmentAsync(requestObj);

                return result;
            });
        }

        /// <summary>
        /// Validate payer authentication results
        /// </summary>
        /// <param name="authenticationTransactionId">Authentication transaction id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the validated authentication results response; Error message if exists
        /// </returns>
        public async Task<(RiskV1AuthenticationResultsPost201Response Result, string Error)> PayerAuthenticationValidateAsync(string authenticationTransactionId)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (_cyberSourceSettings.PaymentConnectionMethod != ConnectionMethodType.FlexMicroForm)
                    throw new NopException("Flex microform flow not enabled");

                if (!_cyberSourceSettings.PayerAuthenticationEnabled)
                    throw new NopException("Payer authentication not enabled");

                if (string.IsNullOrEmpty(authenticationTransactionId))
                    throw new NopException("Invalid authentication transaction id");

                var clientReferenceInformation = new Riskv1decisionsClientReferenceInformation(Code: Guid.NewGuid().ToString(),
                    Partner: new Riskv1decisionsClientReferenceInformationPartner { SolutionId = CyberSourceDefaults.SolutionId });
                var consumerAuthenticationInformation = new Riskv1authenticationresultsConsumerAuthenticationInformation(AuthenticationTransactionId: authenticationTransactionId);
                var requestObj = new ValidateRequest(ClientReferenceInformation: clientReferenceInformation,
                    ConsumerAuthenticationInformation: consumerAuthenticationInformation);

                var apiInstance = new PayerAuthenticationApi(GetConfiguration());
                var result = await apiInstance.ValidateAuthenticationResultsAsync(requestObj);

                return result;
            });
        }

        #endregion

        #endregion
    }
}