using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.PayPalDirect.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using PayPal.Api;

namespace Nop.Plugin.Payments.PayPalDirect
{
    /// <summary>
    /// PayPalDirect payment processor
    /// </summary>
    public class PayPalDirectPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly PayPalDirectPaymentSettings _paypalDirectPaymentSettings;
        
        #endregion

        #region Ctor

        public PayPalDirectPaymentProcessor(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService, 
            IStoreContext storeContext,
            PayPalDirectPaymentSettings paypalDirectPaymentSettings)
        {
            this._currencySettings = currencySettings;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._paypalDirectPaymentSettings = paypalDirectPaymentSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a payment status
        /// </summary>
        /// <param name="state">PayPal state</param>
        /// <returns>Payment status</returns>
        protected PaymentStatus GetPaymentStatus(string state)
        {
            state = state ?? string.Empty;
            var result = PaymentStatus.Pending;

            switch (state.ToLowerInvariant())
            {
                case "pending":
                    result = PaymentStatus.Pending;
                    break;
                case "authorized":
                    result = PaymentStatus.Authorized;
                    break;
                case "captured":
                case "completed":
                    result = PaymentStatus.Paid;
                    break;
                case "expired":
                case "voided":
                    result = PaymentStatus.Voided;
                    break;
                case "refunded":
                    result = PaymentStatus.Refunded;
                    break;
                case "partially_refunded":
                    result = PaymentStatus.PartiallyRefunded;
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Get start date of recurring payments
        /// </summary>
        /// <param name="period">Cycle period</param>
        /// <param name="length">Cycle length</param>
        /// <returns>Start date in ISO8601 format</returns>
        protected string GetStartDate(RecurringProductCyclePeriod period, int length)
        {
            //PayPal expects date in PDT timezone (UTC -7)
            var startDate = DateTime.UtcNow.AddHours(-7);
            switch (period)
            {
                case RecurringProductCyclePeriod.Days:
                    startDate = startDate.AddDays(length);
                    break;
                case RecurringProductCyclePeriod.Weeks:
                    startDate = startDate.AddDays(length * 7);
                    break;
                case RecurringProductCyclePeriod.Months:
                    startDate = startDate.AddMonths(length);
                    break;
                case RecurringProductCyclePeriod.Years:
                    startDate = startDate.AddYears(length);
                    break;
            }

            return string.Format("{0}Z", startDate.ToString("s"));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();

            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (customer == null)
                throw new Exception("Customer cannot be loaded");

            try
            {
                var apiContext = PaypalHelper.GetApiContext(_paypalDirectPaymentSettings);
                var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
                var payment = new Payment()
                {
                    #region payer

                    payer = new Payer()
                    {
                        payment_method = "credit_card",

                        #region credit card info

                        funding_instruments = new List<FundingInstrument>
                        {
                            new FundingInstrument
                            {
                                credit_card = new CreditCard
                                {
                                    type = processPaymentRequest.CreditCardType.ToLowerInvariant(),
                                    number = processPaymentRequest.CreditCardNumber,
                                    cvv2 = processPaymentRequest.CreditCardCvv2,
                                    expire_month = processPaymentRequest.CreditCardExpireMonth,
                                    expire_year = processPaymentRequest.CreditCardExpireYear
                                }
                            }
                        },

                        #endregion

                        #region payer info

                        payer_info = new PayerInfo
                        {
                            #region billing address

                            billing_address = customer.BillingAddress == null ? null : new Address
                            {
                                country_code = customer.BillingAddress.Country != null ? customer.BillingAddress.Country.TwoLetterIsoCode : null,
                                state = customer.BillingAddress.StateProvince != null ? customer.BillingAddress.StateProvince.Abbreviation : null,
                                city = customer.BillingAddress.City,
                                line1 = customer.BillingAddress.Address1,
                                line2 = customer.BillingAddress.Address2,
                                phone = customer.BillingAddress.PhoneNumber,
                                postal_code = customer.BillingAddress.ZipPostalCode
                            },

                            #endregion

                            email = customer.BillingAddress != null ? customer.BillingAddress.Email : null,
                            first_name = customer.BillingAddress != null ? customer.BillingAddress.FirstName : null,
                            last_name = customer.BillingAddress != null ? customer.BillingAddress.LastName : null
                        }

                        #endregion
                    },

                    #endregion

                    #region transaction

                    transactions = new List<Transaction>()
                    {
                        new Transaction
                        {
                            #region amount

                            amount = new Amount
                            {
                                total = processPaymentRequest.OrderTotal.ToString("N", new CultureInfo("en-US")),
                                currency = currency != null ? currency.CurrencyCode : null
                            },

                            #endregion

                            #region shipping address

                            item_list = customer.ShippingAddress == null ? null : new ItemList
                            {
                                shipping_address = new ShippingAddress
                                {
                                    country_code = customer.ShippingAddress.Country != null ? customer.ShippingAddress.Country.TwoLetterIsoCode : null,
                                    state = customer.ShippingAddress.StateProvince != null ? customer.ShippingAddress.StateProvince.Abbreviation : null,
                                    city = customer.ShippingAddress.City,
                                    line1 = customer.ShippingAddress.Address1,
                                    line2 = customer.ShippingAddress.Address2,
                                    phone = customer.ShippingAddress.PhoneNumber,
                                    postal_code = customer.ShippingAddress.ZipPostalCode,
                                    recipient_name = string.Format("{0} {1}", customer.ShippingAddress.FirstName, customer.ShippingAddress.LastName)
                                }
                            },

                            #endregion

                            invoice_number = processPaymentRequest.OrderGuid != Guid.Empty ? processPaymentRequest.OrderGuid.ToString() : null
                        }
                    },

                    #endregion

                    intent = _paypalDirectPaymentSettings.TransactMode == TransactMode.Authorize ? "authorize" : "sale",
                }.Create(apiContext);

                if (payment.transactions[0].related_resources.Any() && payment.transactions[0].related_resources[0] != null)
                    if (_paypalDirectPaymentSettings.TransactMode == TransactMode.Authorize)
                    {
                        var authorization = payment.transactions[0].related_resources[0].authorization;
                        if (authorization != null)
                        {
                            if (authorization.fmf_details != null)
                            {
                                result.AuthorizationTransactionResult = string.Format("Authorization is {0}. Based on fraud filter: {1}. {2}",
                                    authorization.fmf_details.filter_type, authorization.fmf_details.name, authorization.fmf_details.description);
                                result.NewPaymentStatus = GetPaymentStatus(Authorization.Get(apiContext, authorization.id).state);
                            }
                            else
                            {
                                result.AuthorizationTransactionResult = authorization.state;
                                result.NewPaymentStatus = GetPaymentStatus(authorization.state);
                            }
                            result.AuthorizationTransactionId = authorization.id;
                        }
                    }
                    else
                    {
                        var sale = payment.transactions[0].related_resources[0].sale;
                        if (sale != null)
                        {
                            if (sale.fmf_details != null)
                            {
                                result.CaptureTransactionResult = string.Format("Sale is {0}. Based on fraud filter: {1}. {2}",
                                    sale.fmf_details.filter_type, sale.fmf_details.name, sale.fmf_details.description);
                                result.NewPaymentStatus = GetPaymentStatus(Sale.Get(apiContext, sale.id).state);
                            }
                            else
                            {
                                result.CaptureTransactionResult = sale.state;
                                result.NewPaymentStatus = GetPaymentStatus(sale.state);
                            }
                            result.CaptureTransactionId = sale.id;
                            result.AvsResult = sale.processor_response != null ? sale.processor_response.avs_code : string.Empty;

                        }
                    }
                else
                    result.AddError("PayPal error");
            }
            catch (PayPal.PayPalException exc)
            {
                if (exc is PayPal.ConnectionException)
                {
                    var error = JsonFormatter.ConvertFromJson<Error>((exc as PayPal.ConnectionException).Response);
                    if (error != null)
                    {
                        result.AddError(string.Format("PayPal error: {0} ({1})", error.message, error.name));
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError(string.Format("{0} {1}", x.field, x.issue)));
                    }
                }

                //if there are not the specific errors add exception message
                if (result.Success)
                    result.AddError(exc.InnerException != null ? exc.InnerException.Message : exc.Message);
            }
            
            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                _paypalDirectPaymentSettings.AdditionalFee, _paypalDirectPaymentSettings.AdditionalFeePercentage);

            return result;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();

            try
            {
                var apiContext = PaypalHelper.GetApiContext(_paypalDirectPaymentSettings);
                var authorization = Authorization.Get(apiContext, capturePaymentRequest.Order.AuthorizationTransactionId);
                var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
                var capture = new Capture
                {
                    amount = new Amount
                    {
                        total = capturePaymentRequest.Order.OrderTotal.ToString("N", new CultureInfo("en-US")),
                        currency = currency != null ? currency.CurrencyCode : null
                    },
                    is_final_capture = true
                };
                capture = authorization.Capture(apiContext, capture);

                result.CaptureTransactionId = capture.id;
                result.CaptureTransactionResult = capture.state;
                result.NewPaymentStatus = GetPaymentStatus(capture.state);
            }
            catch (PayPal.PayPalException exc)
            {
                if (exc is PayPal.ConnectionException)
                {
                    var error = JsonFormatter.ConvertFromJson<Error>((exc as PayPal.ConnectionException).Response);
                    if (error != null)
                    {
                        result.AddError(string.Format("PayPal error: {0} ({1})", error.message, error.name));
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError(string.Format("{0} {1}", x.field, x.issue)));
                    }
                }

                //if there are not the specific errors add exception message
                if (result.Success)
                    result.AddError(exc.InnerException != null ? exc.InnerException.Message : exc.Message);
            }

            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();

            try
            {
                var apiContext = PaypalHelper.GetApiContext(_paypalDirectPaymentSettings);
                var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
                var refundRequest = new RefundRequest
                {
                    amount = new Amount
                    {
                        total = refundPaymentRequest.AmountToRefund.ToString("N", new CultureInfo("en-US")),
                        currency = currency != null ? currency.CurrencyCode : null
                    }
                };
                PayPal.Api.Capture.Refund(apiContext, refundPaymentRequest.Order.CaptureTransactionId, refundRequest);
                var capture = PayPal.Api.Capture.Get(apiContext, refundPaymentRequest.Order.CaptureTransactionId);
                result.NewPaymentStatus = GetPaymentStatus(capture.state);
            }
            catch (PayPal.PayPalException exc)
            {
                if (exc is PayPal.ConnectionException)
                {
                    var error = JsonFormatter.ConvertFromJson<Error>((exc as PayPal.ConnectionException).Response);
                    if (error != null)
                    {
                        result.AddError(string.Format("PayPal error: {0} ({1})", error.message, error.name));
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError(string.Format("{0} {1}", x.field, x.issue)));
                    }
                }

                //if there are not the specific errors add exception message
                if (result.Success)
                    result.AddError(exc.InnerException != null ? exc.InnerException.Message : exc.Message);
            }

            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();

            try
            {
                var apiContext = PaypalHelper.GetApiContext(_paypalDirectPaymentSettings);
                var authorization = Authorization.Get(apiContext, voidPaymentRequest.Order.AuthorizationTransactionId);
                authorization = authorization.Void(apiContext);

                result.NewPaymentStatus = GetPaymentStatus(authorization.state);
            }
            catch (PayPal.PayPalException exc)
            {
                if (exc is PayPal.ConnectionException)
                {
                    var error = JsonFormatter.ConvertFromJson<Error>((exc as PayPal.ConnectionException).Response);
                    if (error != null)
                    {
                        result.AddError(string.Format("PayPal error: {0} ({1})", error.message, error.name));
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError(string.Format("{0} {1}", x.field, x.issue)));
                    }
                }

                //if there are not the specific errors add exception message
                if (result.Success)
                    result.AddError(exc.InnerException != null ? exc.InnerException.Message : exc.Message);
            }

            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();

            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (customer == null)
                throw new Exception("Customer cannot be loaded");

            try
            {
                var apiContext = PaypalHelper.GetApiContext(_paypalDirectPaymentSettings);
                var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

                //check that webhook exists
                if (string.IsNullOrEmpty(_paypalDirectPaymentSettings.WebhookId))
                {
                    result.AddError("Recurring payments are not available until you create a webhook");
                    return result;
                }

                Webhook.Get(apiContext, _paypalDirectPaymentSettings.WebhookId);

                //create the plan
                var url = _storeContext.CurrentStore.SslEnabled ? _storeContext.CurrentStore.SecureUrl : _storeContext.CurrentStore.Url;
                var billingPlan = new Plan
                {
                    name = processPaymentRequest.OrderGuid.ToString(),
                    description = string.Format("nopCommerce billing plan for the {0} order", processPaymentRequest.OrderGuid),
                    type = "fixed",
                    merchant_preferences = new MerchantPreferences
                    {
                        return_url = url,
                        cancel_url = url,
                        auto_bill_amount = "YES",
                        //setting setup fee as the first payment (workaround for the processing first payment immediately)
                        setup_fee = new PayPal.Api.Currency
                        {
                            currency = currency != null ? currency.CurrencyCode : null,
                            value = processPaymentRequest.OrderTotal.ToString("N", new CultureInfo("en-US"))
                        }
                    },
                    payment_definitions = new List<PaymentDefinition>
                    {
                        new PaymentDefinition
                        {
                             name = "nopCommerce payment for the billing plan",
                             type = "REGULAR",
                             frequency_interval = processPaymentRequest.RecurringCycleLength.ToString(),
                             frequency = processPaymentRequest.RecurringCyclePeriod.ToString().TrimEnd('s'),
                             cycles = (processPaymentRequest.RecurringTotalCycles - 1).ToString(),
                             amount = new PayPal.Api.Currency
                             {
                                 currency = currency != null ? currency.CurrencyCode : null,
                                 value = processPaymentRequest.OrderTotal.ToString("N", new CultureInfo("en-US"))
                             }
                        }
                    }
                }.Create(apiContext);

                //activate the plan
                var patchRequest = new PatchRequest()
                {
                    new Patch()
                    {
                        op = "replace",
                        path = "/",
                        value = new Plan
                        {
                            state = "ACTIVE"
                        }
                    }
                };
                billingPlan.Update(apiContext, patchRequest);

                //create subscription
                var subscription = new Agreement
                {
                    name = string.Format("nopCommerce subscription for the {0} order", processPaymentRequest.OrderGuid),
                    //we set order guid in the description, then use it in the webhook handler
                    description = processPaymentRequest.OrderGuid.ToString(),
                    //setting start date as the next date of recurring payments as the setup fee was the first payment
                    start_date = GetStartDate(processPaymentRequest.RecurringCyclePeriod, processPaymentRequest.RecurringCycleLength),
                    
                    #region payer

                    payer = new Payer()
                    {
                        payment_method = "credit_card",
                        
                        #region credit card info

                        funding_instruments = new List<FundingInstrument>
                        {
                            new FundingInstrument
                            {
                                credit_card = new CreditCard
                                {
                                    type = processPaymentRequest.CreditCardType.ToLowerInvariant(),
                                    number = processPaymentRequest.CreditCardNumber,
                                    cvv2 = processPaymentRequest.CreditCardCvv2,
                                    expire_month = processPaymentRequest.CreditCardExpireMonth,
                                    expire_year = processPaymentRequest.CreditCardExpireYear
                                }
                            }
                        },

                        #endregion

                        #region payer info

                        payer_info = new PayerInfo
                        {
                            #region billing address

                            billing_address = customer.BillingAddress == null ? null : new Address
                            {
                                country_code = customer.BillingAddress.Country != null ? customer.BillingAddress.Country.TwoLetterIsoCode : null,
                                state = customer.BillingAddress.StateProvince != null ? customer.BillingAddress.StateProvince.Abbreviation : null,
                                city = customer.BillingAddress.City,
                                line1 = customer.BillingAddress.Address1,
                                line2 = customer.BillingAddress.Address2,
                                phone = customer.BillingAddress.PhoneNumber,
                                postal_code = customer.BillingAddress.ZipPostalCode
                            },

                            #endregion

                            email = customer.BillingAddress.Email,
                            first_name = customer.BillingAddress.FirstName,
                            last_name = customer.BillingAddress.LastName
                        }

                        #endregion
                    },

                    #endregion

                    #region shipping address

                    shipping_address = customer.ShippingAddress == null ? null : new ShippingAddress
                    {
                        country_code = customer.ShippingAddress.Country != null ? customer.ShippingAddress.Country.TwoLetterIsoCode : null,
                        state = customer.ShippingAddress.StateProvince != null ? customer.ShippingAddress.StateProvince.Abbreviation : null,
                        city = customer.ShippingAddress.City,
                        line1 = customer.ShippingAddress.Address1,
                        line2 = customer.ShippingAddress.Address2,
                        phone = customer.ShippingAddress.PhoneNumber,
                        postal_code = customer.ShippingAddress.ZipPostalCode
                    },

                    #endregion

                    plan = new Plan
                    {
                        id = billingPlan.id
                    }
                }.Create(apiContext);

                //if first payment failed, try again
                if (string.IsNullOrEmpty(subscription.agreement_details.last_payment_date))
                    subscription.BillBalance(apiContext, new AgreementStateDescriptor { amount = subscription.agreement_details.outstanding_balance });

                result.SubscriptionTransactionId = subscription.id;
            }
            catch (PayPal.PayPalException exc)
            {
                if (exc is PayPal.ConnectionException)
                {
                    var error = JsonFormatter.ConvertFromJson<Error>((exc as PayPal.ConnectionException).Response);
                    if (error != null)
                    {
                        result.AddError(string.Format("PayPal error: {0} ({1})", error.message, error.name));
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError(string.Format("{0} {1}", x.field, x.issue)));
                    }
                }

                //if there are not the specific errors add exception message
                if (result.Success)
                    result.AddError(exc.InnerException != null ? exc.InnerException.Message : exc.Message);
            }

            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();

            try
            {
                var apiContext = PaypalHelper.GetApiContext(_paypalDirectPaymentSettings);
                var subscription = Agreement.Get(apiContext, cancelPaymentRequest.Order.SubscriptionTransactionId);
                var reason = new AgreementStateDescriptor
                {
                    note = string.Format("Cancel subscription {0}", cancelPaymentRequest.Order.OrderGuid)
                };
                subscription.Cancel(apiContext, reason);
            }
            catch (PayPal.PayPalException exc)
            {
                if (exc is PayPal.ConnectionException)
                {
                    var error = JsonFormatter.ConvertFromJson<Error>((exc as PayPal.ConnectionException).Response);
                    if (error != null)
                    {
                        result.AddError(string.Format("PayPal error: {0} ({1})", error.message, error.name));
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError(string.Format("{0} {1}", x.field, x.issue)));
                    }
                }

                //if there are not the specific errors add exception message
                if (result.Success)
                    result.AddError(exc.InnerException != null ? exc.InnerException.Message : exc.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Core.Domain.Orders.Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //it's not a redirection payment method. So we always return false
            return false;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentPayPalDirect";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.PayPalDirect.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentPayPalDirect";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.PayPalDirect.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Get type of controller
        /// </summary>
        /// <returns>Type</returns>
        public Type GetControllerType()
        {
            return typeof(PaymentPayPalDirectController);
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new PayPalDirectPaymentSettings
            {
                TransactMode = TransactMode.Authorize,
                UseSandbox = true,
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ClientId", "Client ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ClientId.Hint", "Specify client ID.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ClientSecret", "Client secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ClientSecret.Hint", "Specify secret key.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode", "Transaction mode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode.Hint", "Choose transaction mode.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.WebhookId", "Webhook ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.WebhookId.Hint", "Specify webhook ID.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.PaymentMethodDescription", "Pay by credit / debit card");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.WebhookCreate", "Get webhook ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.WebhookError", "Webhook was not created (see details in the log)");

            base.Install();
        }
        
        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //delete webhook
            var settings = _settingService.LoadSetting<PayPalDirectPaymentSettings>();
            if (!string.IsNullOrEmpty(settings.WebhookId))
            {
                try
                {
                    var apiContext = PaypalHelper.GetApiContext(settings);
                    Webhook.Delete(apiContext, settings.WebhookId);
                }
                catch (PayPal.PayPalException) { }
            }

            //settings
            _settingService.DeleteSetting<PayPalDirectPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ClientId");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ClientId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ClientSecret.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.WebhookId");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.WebhookId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.PaymentMethodDescription");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.WebhookCreate");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.WebhookError");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get { return RecurringPaymentType.Automatic; }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.Standard; }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription
        {
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
            get { return _localizationService.GetResource("Plugins.Payments.PayPalDirect.PaymentMethodDescription"); }
        }

        #endregion
    }
}