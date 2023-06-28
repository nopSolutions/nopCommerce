using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.CyberSource.Components;
using Nop.Plugin.Payments.CyberSource.Domain;
using Nop.Plugin.Payments.CyberSource.Models;
using Nop.Plugin.Payments.CyberSource.Services;
using Nop.Plugin.Payments.CyberSource.Services.Helpers;
using Nop.Plugin.Payments.CyberSource.Validators;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.CyberSource
{
    /// <summary>
    /// Represents payment method implementation
    /// </summary>
    public class CyberSourcePaymentMethod : BasePlugin, IPaymentMethod, IWidgetPlugin
    {
        #region Fields

        protected readonly CustomerTokenService _customerTokenService;
        protected readonly CyberSourceService _cyberSourceService;
        protected readonly CyberSourceSettings _cyberSourceSettings;
        protected readonly IActionContextAccessor _actionContextAccessor;
        protected readonly ICustomerService _customerService;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILocalizationService _localizationService;
        protected readonly IPaymentService _paymentService;
        protected readonly IScheduleTaskService _scheduleTaskService;
        protected readonly ISettingService _settingService;
        protected readonly IStoreContext _storeContext;
        protected readonly IUrlHelperFactory _urlHelperFactory;
        protected readonly IWorkContext _workContext;
        protected readonly PaymentSettings _paymentSettings;
        protected readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public CyberSourcePaymentMethod(CustomerTokenService customerTokenService,
            CyberSourceService cyberSourceService,
            CyberSourceSettings cyberSourceSettings,
            IActionContextAccessor actionContextAccessor,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IPaymentService paymentService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            IStoreContext storeContext,
            IUrlHelperFactory urlHelperFactory,
            IWorkContext workContext,
            PaymentSettings paymentSettings,
            WidgetSettings widgetSettings)
        {
            _customerTokenService = customerTokenService;
            _cyberSourceService = cyberSourceService;
            _cyberSourceSettings = cyberSourceSettings;
            _actionContextAccessor = actionContextAccessor;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _paymentService = paymentService;
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
            _storeContext = storeContext;
            _urlHelperFactory = urlHelperFactory;
            _workContext = workContext;
            _paymentSettings = paymentSettings;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the process payment result
        /// </returns>
        public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest is null)
                throw new ArgumentNullException(nameof(processPaymentRequest));

            var result = new ProcessPaymentResult();
            var customer = await _customerService.GetCustomerByIdAsync(processPaymentRequest.CustomerId);
            var storeId = processPaymentRequest.StoreId;
            processPaymentRequest.CreditCardNumber = CreditCardHelper.RemoveSpecialCharacters(processPaymentRequest.CreditCardNumber);

            var isNewCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, CyberSourceDefaults.PaymentWithNewCardAttributeName, storeId);
            var saveCardOnFile = await _genericAttributeService.GetAttributeAsync<bool>(customer, CyberSourceDefaults.SaveCardOnFileAttributeName, storeId);
            var transientToken = await _genericAttributeService.GetAttributeAsync<string>(customer, CyberSourceDefaults.TransientTokenAttributeName, storeId);
            var tokenId = await _genericAttributeService.GetAttributeAsync<int>(customer, CyberSourceDefaults.SelectedTokenIdAttributeName, storeId);
            var authenticationTransactionId = await _genericAttributeService.GetAttributeAsync<string>(customer, CyberSourceDefaults.AuthenticationTransactionIdAttributeName, storeId);

            var (authorization, error) = await _cyberSourceService.AuthorizeAsync(processPaymentRequest: processPaymentRequest,
                isNewCard: isNewCard,
                saveCardOnFile: _cyberSourceSettings.TokenizationEnabled && isNewCard && saveCardOnFile,
                customerTokenId: tokenId,
                transientToken: transientToken,
                authenticationTransactionId: authenticationTransactionId);
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(authorization?.Id))
            {
                result.AddError(await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Payment.AutorizationFailed"));
                if (!string.IsNullOrEmpty(error))
                    result.AddError(error);

                return result;
            }

            if (authorization.Status == CyberSourceDefaults.ResponseStatus.Declined)
            {
                if (authorization.ErrorInformation?.Reason != CyberSourceDefaults.ResponseErrorReason.AvsFailed &&
                    authorization.ErrorInformation?.Reason != CyberSourceDefaults.ResponseErrorReason.CvFailed &&
                    authorization.ErrorInformation?.Reason != CyberSourceDefaults.ResponseErrorReason.DecisionProfileReject &&
                    authorization.ErrorInformation?.Reason != CyberSourceDefaults.ResponseErrorReason.DecisionProfileReview)
                {
                    result.AddError(await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Payment.AutorizationFailed"));
                    if (!string.IsNullOrEmpty(authorization.ErrorInformation?.Message))
                        result.AddError(authorization.ErrorInformation.Message);

                    return result;
                }
            }

            result.AuthorizationTransactionId = authorization.Id;
            result.AuthorizationTransactionResult = authorization.Status;
            result.AvsResult = authorization.ProcessorInformation?.Avs?.Code;
            result.Cvv2Result = authorization.ProcessorInformation?.CardVerification?.ResultCode;
            result.AllowStoringCreditCardNumber = _cyberSourceSettings.PaymentConnectionMethod == ConnectionMethodType.RestApi;
            result.NewPaymentStatus = _cyberSourceSettings.TransactionType == TransactionType.Sale
                ? PaymentStatus.Paid
                : PaymentStatus.Authorized;
            if (_cyberSourceSettings.TransactionType == TransactionType.Sale)
                result.CaptureTransactionId = authorization.Id;

            //clear attributes
            await _genericAttributeService.SaveAttributeAsync<bool?>(customer, CyberSourceDefaults.PaymentWithNewCardAttributeName, null, storeId);
            await _genericAttributeService.SaveAttributeAsync<bool?>(customer, CyberSourceDefaults.SaveCardOnFileAttributeName, null, storeId);
            await _genericAttributeService.SaveAttributeAsync<string>(customer, CyberSourceDefaults.TransientTokenAttributeName, null, storeId);
            await _genericAttributeService.SaveAttributeAsync<int?>(customer, CyberSourceDefaults.SelectedTokenIdAttributeName, null, storeId);
            await _genericAttributeService.SaveAttributeAsync<string>(customer, CyberSourceDefaults.AuthenticationTransactionIdAttributeName, null, storeId);

            if (_cyberSourceSettings.TokenizationEnabled && isNewCard && saveCardOnFile)
            {
                var instrumentIdentifierId = authorization.TokenInformation?.InstrumentIdentifier?.Id;
                var tokens = await _customerTokenService.GetAllTokensAsync(processPaymentRequest.CustomerId, instrumentIdentifierId);
                var customerToken = tokens.FirstOrDefault();
                if (customerToken is null)
                {
                    var firstSixDigitOfCard = string.Empty;
                    var lastFourDigitOfCard = string.Empty;
                    if (!string.IsNullOrEmpty(processPaymentRequest.CreditCardNumber))
                    {
                        firstSixDigitOfCard = processPaymentRequest.CreditCardNumber.Length >= 6
                            ? processPaymentRequest.CreditCardNumber.Substring(0, 6)
                            : string.Empty;
                        lastFourDigitOfCard = processPaymentRequest.CreditCardNumber.Length >= 4
                            ? processPaymentRequest.CreditCardNumber[^4..]
                            : string.Empty;
                    }
                    else
                    {
                        var (paymentInstrument, _) = await _cyberSourceService.GetInstrumentByIdAsync(instrumentIdentifierId);
                        var cardNumber = paymentInstrument?.Card?.Number;
                        var splitedCardNumber = cardNumber?.Split("XXXXXX");
                        if (splitedCardNumber?.Length == 2)
                        {
                            firstSixDigitOfCard = splitedCardNumber[0];
                            lastFourDigitOfCard = splitedCardNumber[1];
                        }
                    }

                    await _customerTokenService.InsertAsync(new CyberSourceCustomerToken
                    {
                        CustomerId = processPaymentRequest.CustomerId,
                        CardExpirationMonth = processPaymentRequest.CreditCardExpireMonth.ToString().PadLeft(2, '0'),
                        CardExpirationYear = processPaymentRequest.CreditCardExpireYear.ToString(),
                        ThreeDigitCardType = CreditCardHelper.GetThreeDigitCardTypeByNumber(processPaymentRequest.CreditCardNumber),
                        FirstSixDigitOfCard = firstSixDigitOfCard,
                        InstrumentIdentifier = instrumentIdentifierId,
                        InstrumentIdentifierStatus = authorization.TokenInformation?.InstrumentIdentifier?.State,
                        IsInstrumentIdentifierNew = authorization.TokenInformation?.InstrumentidentifierNew ?? false,
                        LastFourDigitOfCard = lastFourDigitOfCard,
                        SubscriptionId = authorization.TokenInformation?.PaymentInstrument?.Id,
                        CyberSourceCustomerId = authorization.TokenInformation?.Customer.Id,
                        TransactionId = authorization.ProcessorInformation?.TransactionId
                    });
                }
                else
                {
                    var threeDigitCardType = CreditCardHelper.GetThreeDigitCardTypeByNumber(processPaymentRequest.CreditCardNumber);
                    customerToken.CardExpirationMonth = processPaymentRequest.CreditCardExpireMonth.ToString().PadLeft(2, '0') ?? customerToken.CardExpirationMonth;
                    customerToken.CardExpirationYear = processPaymentRequest.CreditCardExpireYear.ToString() ?? customerToken.CardExpirationYear;
                    customerToken.ThreeDigitCardType = !string.IsNullOrEmpty(threeDigitCardType) ? threeDigitCardType : customerToken.ThreeDigitCardType;
                    customerToken.InstrumentIdentifierStatus = authorization.TokenInformation?.InstrumentIdentifier?.State;
                    customerToken.SubscriptionId = authorization.TokenInformation?.PaymentInstrument?.Id ?? customerToken.SubscriptionId;
                    customerToken.CyberSourceCustomerId = authorization.TokenInformation?.Customer.Id ?? customerToken.CyberSourceCustomerId;
                    customerToken.TransactionId = authorization.ProcessorInformation?.TransactionId ?? customerToken.TransactionId;

                    await _customerTokenService.UpdateAsync(customerToken);
                }
            }

            OrderStatus? orderStatus = null;
            if (authorization.Status == CyberSourceDefaults.ResponseStatus.Declined ||
                authorization.Status == CyberSourceDefaults.ResponseStatus.AuthorizedRiskDeclined ||
                authorization.Status == CyberSourceDefaults.ResponseStatus.AuthorizedPendingReview)
            {
                if (_cyberSourceSettings.AvsActionType != AvsActionType.Ignore &&
                    authorization.ErrorInformation?.Reason == CyberSourceDefaults.ResponseErrorReason.AvsFailed)
                {
                    if (_cyberSourceSettings.AvsActionType == AvsActionType.Reject)
                        orderStatus = OrderStatus.Cancelled;
                    else if (_cyberSourceSettings.AvsActionType == AvsActionType.Review)
                        orderStatus = OrderStatus.Pending;
                }
                else if (_cyberSourceSettings.CvnActionType != CvnActionType.Ignore &&
                    authorization.ErrorInformation?.Reason == CyberSourceDefaults.ResponseErrorReason.CvFailed)
                {
                    if (_cyberSourceSettings.CvnActionType == CvnActionType.Reject)
                        orderStatus = OrderStatus.Cancelled;
                    else if (_cyberSourceSettings.CvnActionType == CvnActionType.Review)
                        orderStatus = OrderStatus.Pending;
                }
                else if (_cyberSourceSettings.DecisionManagerEnabled)
                {
                    if (authorization.ErrorInformation?.Reason == CyberSourceDefaults.ResponseErrorReason.DecisionProfileReject)
                        orderStatus = OrderStatus.Cancelled;
                    else if (authorization.ErrorInformation?.Reason == CyberSourceDefaults.ResponseErrorReason.DecisionProfileReview)
                        orderStatus = OrderStatus.Pending;
                }

                if (orderStatus.HasValue)
                {
                    var paymentStatus = result.NewPaymentStatus;
                    result.NewPaymentStatus = PaymentStatus.Pending;
                    var key = string.Format(CyberSourceDefaults.OrderStatusesSessionKey, processPaymentRequest.OrderGuid);
                    await _httpContextAccessor.HttpContext.Session.SetAsync(key, (orderStatus, paymentStatus));
                }
            }

            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the capture payment result
        /// </returns>
        public async Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            var customValues = _paymentService.DeserializeCustomValues(capturePaymentRequest.Order);
            var authorizationDate = customValues.TryGetValue(CyberSourceDefaults.AuthorizationDateCustomValue, out var value)
                && DateTime.TryParse(value.ToString(), out var authorizationDatetime)
                ? authorizationDatetime
                : (DateTime?)null;
            if (authorizationDate.HasValue && authorizationDate < DateTime.UtcNow.AddDays(-CyberSourceDefaults.NumberOfDaysAuthorizationAvailable))
            {
                var expired = await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Payment.AutorizationExpired");
                return new CapturePaymentResult { Errors = new[] { expired } };
            }

            var (capture, error) = await _cyberSourceService.CaptureAsync(capturePaymentRequest);
            if (!string.IsNullOrEmpty(error))
                return new CapturePaymentResult { Errors = new[] { error } };

            if (capture is null)
                return new CapturePaymentResult { Errors = new[] { "Capture failed" } };

            return new CapturePaymentResult
            {
                CaptureTransactionId = capture.Id,
                CaptureTransactionResult = capture.Status,
                NewPaymentStatus = PaymentStatus.Paid
            };
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public async Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            var (_, error) = !string.IsNullOrEmpty(voidPaymentRequest.Order?.CaptureTransactionId)
                ? await _cyberSourceService.VoidCaptureAsync(voidPaymentRequest)
                : await _cyberSourceService.VoidPaymentAsync(voidPaymentRequest);
            if (!string.IsNullOrEmpty(error))
                return new VoidPaymentResult { Errors = new[] { error } };

            return new VoidPaymentResult
            {
                NewPaymentStatus = PaymentStatus.Voided
            };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            var customValues = _paymentService.DeserializeCustomValues(refundPaymentRequest.Order);
            var captureDate = customValues.TryGetValue(CyberSourceDefaults.CaptureDateCustomValue, out var value)
                && DateTime.TryParse(value.ToString(), out var captureDatetime)
                ? captureDatetime
                : (DateTime?)null;

            string refundId;
            if (captureDate >= DateTime.UtcNow.AddDays(-_cyberSourceSettings.NumberOfDaysRefundAvailable))
            {
                var (refund, error) = await _cyberSourceService.RefundAsync(refundPaymentRequest);
                refundId = refund?.Id;
                if (!string.IsNullOrEmpty(error))
                    return new RefundPaymentResult { Errors = new[] { error } };
            }
            else
            {
                var (credit, error) = await _cyberSourceService.CreditAsync(refundPaymentRequest);
                refundId = credit?.Id;
                if (!string.IsNullOrEmpty(error))
                    return new RefundPaymentResult { Errors = new[] { error } };
            }

            if (string.IsNullOrEmpty(refundId))
            {
                var error = await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.Payment.RefundError");
                return new RefundPaymentResult { Errors = new[] { error } };
            }

            //request succeeded
            var refundIds = await _genericAttributeService
                .GetAttributeAsync<List<string>>(refundPaymentRequest.Order, CyberSourceDefaults.RefundIdAttributeName)
                ?? new List<string>();
            if (!refundIds.Contains(refundId))
                refundIds.Add(refundId);
            await _genericAttributeService.SaveAttributeAsync(refundPaymentRequest.Order, CyberSourceDefaults.RefundIdAttributeName, refundIds);

            return new RefundPaymentResult
            {
                NewPaymentStatus = refundPaymentRequest.IsPartialRefund ? PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded
            };
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the process payment result
        /// </returns>
        public async Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError(await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.RecurringPayment.NotSupported"));
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public async Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError(await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.RecurringPayment.NotSupported"));
            return result;
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - hide; false - display.
        /// </returns>
        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(!CyberSourceService.IsConfigured(_cyberSourceSettings));
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the additional handling fee
        /// </returns>
        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            return Task.FromResult(false);
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of validating errors
        /// </returns>
        public async Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            var warnings = new List<string>();

            var isNewCard = form.TryGetValue("NewCard", out var value) && bool.TryParse(value.FirstOrDefault(), out var newCard) && newCard;
            if (!isNewCard || _cyberSourceSettings.PaymentConnectionMethod == ConnectionMethodType.FlexMicroForm)
                return warnings;

            var validator = new PaymentInfoValidator(_localizationService, _cyberSourceSettings);
            var validationResult = await validator.ValidateAsync(new PaymentInfoModel
            {
                NewCard = isNewCard,
                CardNumber = form["CardNumber"],
                Cvv = form["Cvv"],
                ExpireMonth = form["ExpireMonth"],
                ExpireYear = form["ExpireYear"],
                SelectedTokenId = int.TryParse(form["SelectedTokenId"], out var selectedTokenId) ? selectedTokenId : 0,
                SaveCardOnFile = bool.TryParse(form["SaveCardOnFile"].FirstOrDefault(), out var saveCard) && saveCard
            });
            if (!validationResult.IsValid)
                warnings.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));

            return warnings;
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment info holder
        /// </returns>
        public async Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            var processPaymentRequest = new ProcessPaymentRequest();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            var isNewCard = form.TryGetValue("NewCard", out var value) && bool.TryParse(value.FirstOrDefault(), out var newCard) && newCard;
            await _genericAttributeService.SaveAttributeAsync(customer, CyberSourceDefaults.PaymentWithNewCardAttributeName, isNewCard, store.Id);

            var authenticationTransactionId = form["AuthenticationTransactionId"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authenticationTransactionId))
                await _genericAttributeService.SaveAttributeAsync(customer, CyberSourceDefaults.AuthenticationTransactionIdAttributeName, authenticationTransactionId, store.Id);

            if (_cyberSourceSettings.TokenizationEnabled && !isNewCard)
            {
                var selectedTokenId = int.TryParse(form["SelectedTokenId"], out var selectedToken) ? selectedToken : 0;
                await _genericAttributeService.SaveAttributeAsync(customer, CyberSourceDefaults.SelectedTokenIdAttributeName, selectedTokenId, store.Id);
            }
            else if (isNewCard)
            {
                processPaymentRequest.CreditCardNumber = form["CardNumber"];
                processPaymentRequest.CreditCardExpireMonth = int.TryParse(form["ExpireMonth"], out var expireMonth) ? expireMonth : 0;
                processPaymentRequest.CreditCardExpireYear = int.TryParse(form["ExpireYear"], out var expireYear) ? expireYear : 0;
                var saveCardOnFile = bool.TryParse(form["SaveCardOnFile"].FirstOrDefault(), out var saveCard) && saveCard;

                await _genericAttributeService.SaveAttributeAsync(customer, CyberSourceDefaults.SaveCardOnFileAttributeName, saveCardOnFile, store.Id);

                if (_cyberSourceSettings.PaymentConnectionMethod == ConnectionMethodType.FlexMicroForm)
                {
                    var transientToken = form["TransientToken"].FirstOrDefault();

                    if (!string.IsNullOrEmpty(transientToken))
                        await _genericAttributeService.SaveAttributeAsync(customer, CyberSourceDefaults.TransientTokenAttributeName, transientToken, store.Id);
                }
            }

            return processPaymentRequest;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(CyberSourceDefaults.ConfigurationRouteName);
        }

        /// <summary>
        /// Gets a type of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component type</returns>
        public Type GetPublicViewComponent()
        {
            return typeof(PaymentInfoViewComponent);
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.BodyStartHtmlTagAfter
            });
        }

        /// <summary>
        /// Gets a type of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component type</returns>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone is null)
                throw new ArgumentNullException(nameof(widgetZone));

            if (widgetZone.Equals(PublicWidgetZones.BodyStartHtmlTagAfter))
                return typeof(PayerAuthenticationViewComponent);

            return null;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await _settingService.SaveSettingAsync(new CyberSourceSettings
            {
                UseSandbox = true,
                TokenizationEnabled = true,
                PaymentConnectionMethod = ConnectionMethodType.FlexMicroForm,
                TransactionType = TransactionType.Sale,
                AvsActionType = AvsActionType.Ignore,
                CvnActionType = CvnActionType.Ignore,
                ConversionDetailReportingFrequency = CyberSourceDefaults.ConversionDetailReportingFrequency,
                NumberOfDaysRefundAvailable = CyberSourceDefaults.NumberOfDaysRefundAvailable,
                RequestTimeout = CyberSourceDefaults.RequestTimeout
            });

            if (!_paymentSettings.ActivePaymentMethodSystemNames.Contains(CyberSourceDefaults.SystemName))
            {
                _paymentSettings.ActivePaymentMethodSystemNames.Add(CyberSourceDefaults.SystemName);
                await _settingService.SaveSettingAsync(_paymentSettings);
            }

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(CyberSourceDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(CyberSourceDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            if (await _scheduleTaskService.GetTaskByTypeAsync(CyberSourceDefaults.OrderStatusUpdateTask) is null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
                {
                    Enabled = true,
                    LastEnabledUtc = DateTime.UtcNow,
                    Seconds = CyberSourceDefaults.ConversionDetailReportingFrequency * 60,
                    Name = CyberSourceDefaults.OrderStatusUpdateTaskName,
                    Type = CyberSourceDefaults.OrderStatusUpdateTask
                });
            }

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Enums.Nop.Plugin.Payments.CyberSource.Domain.AvsActionType.Ignore"] = "Ignore AVS results",
                ["Enums.Nop.Plugin.Payments.CyberSource.Domain.AvsActionType.Reject"] = "Cancel Order",
                ["Enums.Nop.Plugin.Payments.CyberSource.Domain.AvsActionType.Review"] = "Verification Review",
                ["Enums.Nop.Plugin.Payments.CyberSource.Domain.ConnectionMethodType.FlexMicroForm"] = "Flex Microform",
                ["Enums.Nop.Plugin.Payments.CyberSource.Domain.ConnectionMethodType.RestApi"] = "REST API",
                ["Enums.Nop.Plugin.Payments.CyberSource.Domain.CvnActionType.Ignore"] = "Ignore CVN results",
                ["Enums.Nop.Plugin.Payments.CyberSource.Domain.CvnActionType.Reject"] = "Cancel Order",
                ["Enums.Nop.Plugin.Payments.CyberSource.Domain.CvnActionType.Review"] = "Verification Review",
                ["Enums.Nop.Plugin.Payments.CyberSource.Domain.TransactionType.AuthorizeOnly"] = "Authorize only",
                ["Enums.Nop.Plugin.Payments.CyberSource.Domain.TransactionType.Sale"] = "Sale (Authorization and Capture)",

                ["Plugins.Payments.CyberSource.Fields.AvsActionType"] = "AVS action type",
                ["Plugins.Payments.CyberSource.Fields.AvsActionType.Hint"] = "Select how plugin should handle an AVS soft decline, either: Reject (Cancel Order), Review (Pending status).",
                ["Plugins.Payments.CyberSource.Fields.ConversionDetailReportingEnabled"] = "Enable On-Demand Conversion Detail Report",
                ["Plugins.Payments.CyberSource.Fields.ConversionDetailReportingEnabled.Hint"] = "Determine whether to enable the On-Demand Conversion Detail Report. This will allow using Decision Manager to manually review any orders in Review status in the Cybersource Case Management interface and automatically update the appropriate orders in the store.",
                ["Plugins.Payments.CyberSource.Fields.ConversionDetailReportingFrequency"] = "Report download frequency",
                ["Plugins.Payments.CyberSource.Fields.ConversionDetailReportingFrequency.Hint"] = "Set the frequency (in minutes) for downloading On-Demand Conversion Detail Report.",
                ["Plugins.Payments.CyberSource.Fields.ConversionDetailReportingFrequency.Invalid"] = "Frequency is invalid",
                ["Plugins.Payments.CyberSource.Fields.CvnActionType"] = "CVN action type",
                ["Plugins.Payments.CyberSource.Fields.CvnActionType.Hint"] = "Select how plugin should handle an CVN soft decline, either: Reject (Cancel Order), Review (Pending status).",
                ["Plugins.Payments.CyberSource.Fields.CvvRequired"] = "CVV required",
                ["Plugins.Payments.CyberSource.Fields.CvvRequired.Hint"] = "Determine whether CVV is required during payment.",
                ["Plugins.Payments.CyberSource.Fields.DecisionManagerEnabled"] = "Enable Decision Manager",
                ["Plugins.Payments.CyberSource.Fields.DecisionManagerEnabled.Hint"] = "Determine whether to screen transactions for fraud by Decision Manager. Order cannot be fulfilled until a manual decision has been made to ACCEPT it.",
                ["Plugins.Payments.CyberSource.Fields.KeyId"] = "Key ID",
                ["Plugins.Payments.CyberSource.Fields.KeyId.Hint"] = "Enter your CyberSource Key ID.",
                ["Plugins.Payments.CyberSource.Fields.KeyId.Required"] = "Key ID is required",
                ["Plugins.Payments.CyberSource.Fields.MerchantId"] = "Merchant ID",
                ["Plugins.Payments.CyberSource.Fields.MerchantId.Hint"] = "Enter your CyberSource Merchant ID.",
                ["Plugins.Payments.CyberSource.Fields.MerchantId.Required"] = "Merchant ID is required",
                ["Plugins.Payments.CyberSource.Fields.PayerAuthenticationEnabled"] = "Enable Payer Authentication",
                ["Plugins.Payments.CyberSource.Fields.PayerAuthenticationEnabled.Hint"] = "Determine whether Payer Authentication (3-D Secure) is enabled.",
                ["Plugins.Payments.CyberSource.Fields.PayerAuthenticationRequired"] = "Force Payer Authentication",
                ["Plugins.Payments.CyberSource.Fields.PayerAuthenticationRequired.Hint"] = "Determine whether Payer Authentication (3-D Secure) is required.",
                ["Plugins.Payments.CyberSource.Fields.PaymentConnectionMethod"] = "Payment connection method",
                ["Plugins.Payments.CyberSource.Fields.PaymentConnectionMethod.Hint"] = "Choose the payment connection method.",
                ["Plugins.Payments.CyberSource.Fields.SecretKey"] = "Secret Key",
                ["Plugins.Payments.CyberSource.Fields.SecretKey.Hint"] = "Enter your CyberSource shared Secret Key.",
                ["Plugins.Payments.CyberSource.Fields.SecretKey.Required"] = "Secret Key is required",
                ["Plugins.Payments.CyberSource.Fields.TokenizationEnabled"] = "Enable Tokenization",
                ["Plugins.Payments.CyberSource.Fields.TokenizationEnabled.Hint"] = "Determine whether to securely store and manage payment data within the Cybersource Token Management Service vault.",
                ["Plugins.Payments.CyberSource.Fields.TransactionType"] = "Transaction type",
                ["Plugins.Payments.CyberSource.Fields.TransactionType.Hint"] = "Choose the transaction type.",
                ["Plugins.Payments.CyberSource.Fields.UseSandbox"] = "Use sandbox",
                ["Plugins.Payments.CyberSource.Fields.UseSandbox.Hint"] = "Determine whether to use the sandbox environment for testing purposes.",

                ["Plugins.Payments.CyberSource.PayerAuthentication.Fail"] = "Payer Authentication failed, please try again",
                ["Plugins.Payments.CyberSource.PayerAuthentication.Title"] = "Please follow instructions from your bank",

                ["Plugins.Payments.CyberSource.Payment.AutorizationExpired"] = $"Capture cannot be done after {CyberSourceDefaults.NumberOfDaysAuthorizationAvailable} days of autorization. Please try to initiate a new authorization",
                ["Plugins.Payments.CyberSource.Payment.AutorizationFailed"] = "Autorization failed, please contact the manager.",
                ["Plugins.Payments.CyberSource.Payment.NewCard"] = "Pay with a new card",
                ["Plugins.Payments.CyberSource.Payment.PayNow"] = "Pay Now",
                ["Plugins.Payments.CyberSource.Payment.RefundError"] = "Refund failed, please try again",
                ["Plugins.Payments.CyberSource.Payment.SaveCardOnFile"] = "Save card on file",
                ["Plugins.Payments.CyberSource.Payment.SaveCardOnFile.Hint"] = "Save card on file for future use.",
                ["Plugins.Payments.CyberSource.Payment.SelectedTokenId.Invalid"] = "Select an existing token",

                ["Plugins.Payments.CyberSource.PaymentMethodDescription"] = "Pay by CyberSource",

                ["Plugins.Payments.CyberSource.PaymentTokens"] = "CyberSource payment tokens",
                ["Plugins.Payments.CyberSource.PaymentTokens.AddNew"] = "Add payment token",
                ["Plugins.Payments.CyberSource.PaymentTokens.Edit"] = "Edit payment token",
                ["Plugins.Payments.CyberSource.PaymentTokens.NoTokens"] = "No payment tokens",

                ["Plugins.Payments.CyberSource.RecurringPayment.NotSupported"] = "Recurring payment not supported",
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            if (_paymentSettings.ActivePaymentMethodSystemNames.Contains(CyberSourceDefaults.SystemName))
            {
                _paymentSettings.ActivePaymentMethodSystemNames.Remove(CyberSourceDefaults.SystemName);
                await _settingService.SaveSettingAsync(_paymentSettings);
            }

            if (_widgetSettings.ActiveWidgetSystemNames.Contains(CyberSourceDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(CyberSourceDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            await _settingService.DeleteSettingAsync<CyberSourceSettings>();

            var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(CyberSourceDefaults.OrderStatusUpdateTask);
            if (scheduleTask is not null)
                await _scheduleTaskService.DeleteTaskAsync(scheduleTask);

            await _localizationService.DeleteLocaleResourcesAsync("Enums.Nop.Plugin.Payments.CyberSource");
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.CyberSource");

            await base.UninstallAsync();
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.PaymentMethodDescription");
        }

        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => true;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => true;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => true;

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => true;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => false;

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => true;

        #endregion
    }
}
