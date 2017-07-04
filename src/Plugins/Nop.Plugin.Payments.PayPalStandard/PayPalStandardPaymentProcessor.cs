using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;

namespace Nop.Plugin.Payments.PayPalStandard
{
    /// <summary>
    /// PayPalStandard payment processor
    /// </summary>
    public class PayPalStandardPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Constants

        /// <summary>
        /// nopCommerce partner code
        /// </summary>
        private const string BN_CODE = "nopCommerce_SP";

        #endregion

        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly PayPalStandardPaymentSettings _paypalStandardPaymentSettings;

        #endregion

        #region Ctor

        public PayPalStandardPaymentProcessor(CurrencySettings currencySettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            ITaxService taxService,
            IWebHelper webHelper,
            PayPalStandardPaymentSettings paypalStandardPaymentSettings)
        {
            this._currencySettings = currencySettings;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._currencyService = currencyService;
            this._genericAttributeService = genericAttributeService;
            this._httpContextAccessor = httpContextAccessor;
            this._localizationService = localizationService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._settingService = settingService;
            this._taxService = taxService;
            this._webHelper = webHelper;
            this._paypalStandardPaymentSettings = paypalStandardPaymentSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetPaypalUrl()
        {
            return _paypalStandardPaymentSettings.UseSandbox ?
                "https://www.sandbox.paypal.com/us/cgi-bin/webscr" :
                "https://www.paypal.com/us/cgi-bin/webscr";
        }

        /// <summary>
        /// Gets IPN Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetIpnPaypalUrl()
        {
            return _paypalStandardPaymentSettings.UseSandbox ? 
                "https://ipnpb.sandbox.paypal.com/cgi-bin/webscr" :
                "https://ipnpb.paypal.com/cgi-bin/webscr";
        }

        /// <summary>
        /// Gets PDT details
        /// </summary>
        /// <param name="tx">TX</param>
        /// <param name="values">Values</param>
        /// <param name="response">Response</param>
        /// <returns>Result</returns>
        public bool GetPdtDetails(string tx, out Dictionary<string, string> values, out string response)
        {
            var req = (HttpWebRequest)WebRequest.Create(GetPaypalUrl());
            req.Method = WebRequestMethods.Http.Post;
            req.ContentType = MimeTypes.ApplicationXWwwFormUrlencoded;
            //now PayPal requires user-agent. otherwise, we can get 403 error
            req.UserAgent = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent];

            string formContent = string.Format("cmd=_notify-synch&at={0}&tx={1}", _paypalStandardPaymentSettings.PdtToken, tx);
            req.ContentLength = formContent.Length;
            
            using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
                sw.Write(formContent);

            using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
                response = WebUtility.UrlDecode(sr.ReadToEnd());

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            bool firstLine = true, success = false;
            foreach (string l in response.Split('\n'))
            {
                string line = l.Trim();
                if (firstLine)
                {
                    success = line.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase);
                    firstLine = false;
                }
                else
                {
                    int equalPox = line.IndexOf('=');
                    if (equalPox >= 0)
                        values.Add(line.Substring(0, equalPox), line.Substring(equalPox + 1));
                }
            }

            return success;
        }

        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <param name="values">Values</param>
        /// <returns>Result</returns>
        public bool VerifyIpn(string formString, out Dictionary<string, string> values)
        {
            var req = (HttpWebRequest)WebRequest.Create(GetIpnPaypalUrl());
            req.Method = WebRequestMethods.Http.Post;
            req.ContentType = MimeTypes.ApplicationXWwwFormUrlencoded;
            //now PayPal requires user-agent. otherwise, we can get 403 error
            req.UserAgent = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent];

            var formContent = string.Format("cmd=_notify-validate&{0}", formString);
            req.ContentLength = formContent.Length;
            
            using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
            {
                sw.Write(formContent);
            }

            string response;
            using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                response = WebUtility.UrlDecode(sr.ReadToEnd());
            }
            bool success = response.Trim().Equals("VERIFIED", StringComparison.OrdinalIgnoreCase);

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string l in formString.Split('&'))
            {
                string line = l.Trim();
                int equalPox = line.IndexOf('=');
                if (equalPox >= 0)
                    values.Add(line.Substring(0, equalPox), line.Substring(equalPox + 1));
            }

            return success;
        }

        /// <summary>
        /// Generate string (URL) for redirection
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        /// <param name="passProductNamesAndTotals">A value indicating whether to pass product names and totals</param>
        private string GenerationRedirectionUrl(PostProcessPaymentRequest postProcessPaymentRequest, bool passProductNamesAndTotals)
        {
            var builder = new StringBuilder();
            builder.Append(GetPaypalUrl());
            var cmd = passProductNamesAndTotals
                ? "_cart"
                : "_xclick";
            builder.AppendFormat("?cmd={0}&business={1}", cmd, WebUtility.UrlEncode(_paypalStandardPaymentSettings.BusinessEmail));
            if (passProductNamesAndTotals)
            {
                builder.AppendFormat("&upload=1");

                //get the items in the cart
                decimal cartTotal = decimal.Zero;
                var cartTotalRounded = decimal.Zero;
                var cartItems = postProcessPaymentRequest.Order.OrderItems;
                int x = 1;
                foreach (var item in cartItems)
                {
                    var unitPriceExclTax = item.UnitPriceExclTax;
                    var priceExclTax = item.PriceExclTax;
                    //round
                    var unitPriceExclTaxRounded = Math.Round(unitPriceExclTax, 2);
                    builder.AppendFormat("&item_name_" + x + "={0}", WebUtility.UrlEncode(item.Product.Name));
                    builder.AppendFormat("&amount_" + x + "={0}", unitPriceExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
                    builder.AppendFormat("&quantity_" + x + "={0}", item.Quantity);
                    x++;
                    cartTotal += priceExclTax;
                    cartTotalRounded += unitPriceExclTaxRounded * item.Quantity;
                }

                //the checkout attributes that have a dollar value and send them to Paypal as items to be paid for
                var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(postProcessPaymentRequest.Order.CheckoutAttributesXml);
                foreach (var val in attributeValues)
                {
                    var attPrice = _taxService.GetCheckoutAttributePrice(val, false, postProcessPaymentRequest.Order.Customer);
                    //round
                    var attPriceRounded = Math.Round(attPrice, 2);
                    if (attPrice > decimal.Zero) //if it has a price
                    {
                        var attribute = val.CheckoutAttribute;
                        if (attribute != null)
                        {
                            var attName = attribute.Name; //set the name
                            builder.AppendFormat("&item_name_" + x + "={0}", WebUtility.UrlEncode(attName)); //name
                            builder.AppendFormat("&amount_" + x + "={0}", attPriceRounded.ToString("0.00", CultureInfo.InvariantCulture)); //amount
                            builder.AppendFormat("&quantity_" + x + "={0}", 1); //quantity
                            x++;
                            cartTotal += attPrice;
                            cartTotalRounded += attPriceRounded;
                        }
                    }
                }

                //order totals

                //shipping
                var orderShippingExclTax = postProcessPaymentRequest.Order.OrderShippingExclTax;
                var orderShippingExclTaxRounded = Math.Round(orderShippingExclTax, 2);
                if (orderShippingExclTax > decimal.Zero)
                {
                    builder.AppendFormat("&item_name_" + x + "={0}", "Shipping fee");
                    builder.AppendFormat("&amount_" + x + "={0}", orderShippingExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
                    builder.AppendFormat("&quantity_" + x + "={0}", 1);
                    x++;
                    cartTotal += orderShippingExclTax;
                    cartTotalRounded += orderShippingExclTaxRounded;
                }

                //payment method additional fee
                var paymentMethodAdditionalFeeExclTax = postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax;
                var paymentMethodAdditionalFeeExclTaxRounded = Math.Round(paymentMethodAdditionalFeeExclTax, 2);
                if (paymentMethodAdditionalFeeExclTax > decimal.Zero)
                {
                    builder.AppendFormat("&item_name_" + x + "={0}", "Payment method fee");
                    builder.AppendFormat("&amount_" + x + "={0}", paymentMethodAdditionalFeeExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
                    builder.AppendFormat("&quantity_" + x + "={0}", 1);
                    x++;
                    cartTotal += paymentMethodAdditionalFeeExclTax;
                    cartTotalRounded += paymentMethodAdditionalFeeExclTaxRounded;
                }

                //tax
                var orderTax = postProcessPaymentRequest.Order.OrderTax;
                var orderTaxRounded = Math.Round(orderTax, 2);
                if (orderTax > decimal.Zero)
                {
                    //builder.AppendFormat("&tax_1={0}", orderTax.ToString("0.00", CultureInfo.InvariantCulture));

                    //add tax as item
                    builder.AppendFormat("&item_name_" + x + "={0}", WebUtility.UrlEncode("Sales Tax")); //name
                    builder.AppendFormat("&amount_" + x + "={0}", orderTaxRounded.ToString("0.00", CultureInfo.InvariantCulture)); //amount
                    builder.AppendFormat("&quantity_" + x + "={0}", 1); //quantity

                    cartTotal += orderTax;
                    cartTotalRounded += orderTaxRounded;
                    x++;
                }

                if (cartTotal > postProcessPaymentRequest.Order.OrderTotal)
                {
                    /* Take the difference between what the order total is and what it should be and use that as the "discount".
                     * The difference equals the amount of the gift card and/or reward points used. 
                     */
                    decimal discountTotal = cartTotal - postProcessPaymentRequest.Order.OrderTotal;
                    discountTotal = Math.Round(discountTotal, 2);
                    cartTotalRounded -= discountTotal;
                    //gift card or rewared point amount applied to cart in nopCommerce - shows in Paypal as "discount"
                    builder.AppendFormat("&discount_amount_cart={0}", discountTotal.ToString("0.00", CultureInfo.InvariantCulture));
                }

                //save order total that actually sent to PayPal (used for PDT order total validation)
                _genericAttributeService.SaveAttribute(postProcessPaymentRequest.Order, "OrderTotalSentToPayPal", cartTotalRounded);
            }
            else
            {
                //pass order total
                builder.AppendFormat("&item_name=Order Number {0}", postProcessPaymentRequest.Order.Id);
                var orderTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
                builder.AppendFormat("&amount={0}", orderTotal.ToString("0.00", CultureInfo.InvariantCulture));

                //save order total that actually sent to PayPal (used for PDT order total validation)
                _genericAttributeService.SaveAttribute(postProcessPaymentRequest.Order, "OrderTotalSentToPayPal", orderTotal);
            }

            builder.AppendFormat("&custom={0}", postProcessPaymentRequest.Order.OrderGuid);
            builder.AppendFormat("&charset={0}", "utf-8");
            builder.AppendFormat("&bn={0}", BN_CODE);
            builder.Append(string.Format("&no_note=1&currency_code={0}", WebUtility.UrlEncode(_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode)));
            builder.AppendFormat("&invoice={0}", postProcessPaymentRequest.Order.Id);
            builder.AppendFormat("&rm=2", new object[0]);
            if (postProcessPaymentRequest.Order.ShippingStatus != ShippingStatus.ShippingNotRequired)
                builder.AppendFormat("&no_shipping=2", new object[0]);
            else
                builder.AppendFormat("&no_shipping=1", new object[0]);

            string returnUrl = _webHelper.GetStoreLocation(false) + "Plugins/PaymentPayPalStandard/PDTHandler";
            string cancelReturnUrl = _webHelper.GetStoreLocation(false) + "Plugins/PaymentPayPalStandard/CancelOrder";
            builder.AppendFormat("&return={0}&cancel_return={1}", WebUtility.UrlEncode(returnUrl), WebUtility.UrlEncode(cancelReturnUrl));

            //Instant Payment Notification (server to server message)
            if (_paypalStandardPaymentSettings.EnableIpn)
            {
                string ipnUrl;
                if (String.IsNullOrWhiteSpace(_paypalStandardPaymentSettings.IpnUrl))
                    ipnUrl = _webHelper.GetStoreLocation(false) + "Plugins/PaymentPayPalStandard/IPNHandler";
                else
                    ipnUrl = _paypalStandardPaymentSettings.IpnUrl;
                builder.AppendFormat("&notify_url={0}", ipnUrl);
            }

            //address
            builder.AppendFormat("&address_override={0}", _paypalStandardPaymentSettings.AddressOverride ? "1" : "0");
            builder.AppendFormat("&first_name={0}", WebUtility.UrlEncode(postProcessPaymentRequest.Order.BillingAddress.FirstName));
            builder.AppendFormat("&last_name={0}", WebUtility.UrlEncode(postProcessPaymentRequest.Order.BillingAddress.LastName));
            builder.AppendFormat("&address1={0}", WebUtility.UrlEncode(postProcessPaymentRequest.Order.BillingAddress.Address1));
            builder.AppendFormat("&address2={0}", WebUtility.UrlEncode(postProcessPaymentRequest.Order.BillingAddress.Address2));
            builder.AppendFormat("&city={0}", WebUtility.UrlEncode(postProcessPaymentRequest.Order.BillingAddress.City));
            //if (!String.IsNullOrEmpty(postProcessPaymentRequest.Order.BillingAddress.PhoneNumber))
            //{
            //    //strip out all non-digit characters from phone number;
            //    string billingPhoneNumber = System.Text.RegularExpressions.Regex.Replace(postProcessPaymentRequest.Order.BillingAddress.PhoneNumber, @"\D", string.Empty);
            //    if (billingPhoneNumber.Length >= 10)
            //    {
            //        builder.AppendFormat("&night_phone_a={0}", WebUtility.UrlEncode(billingPhoneNumber.Substring(0, 3)));
            //        builder.AppendFormat("&night_phone_b={0}", WebUtility.UrlEncode(billingPhoneNumber.Substring(3, 3)));
            //        builder.AppendFormat("&night_phone_c={0}", WebUtility.UrlEncode(billingPhoneNumber.Substring(6, 4)));
            //    }
            //}
            if (postProcessPaymentRequest.Order.BillingAddress.StateProvince != null)
                builder.AppendFormat("&state={0}", WebUtility.UrlEncode(postProcessPaymentRequest.Order.BillingAddress.StateProvince.Abbreviation));
            else
                builder.AppendFormat("&state={0}", "");
            if (postProcessPaymentRequest.Order.BillingAddress.Country != null)
                builder.AppendFormat("&country={0}", WebUtility.UrlEncode(postProcessPaymentRequest.Order.BillingAddress.Country.TwoLetterIsoCode));
            else
                builder.AppendFormat("&country={0}", "");
            builder.AppendFormat("&zip={0}", WebUtility.UrlEncode(postProcessPaymentRequest.Order.BillingAddress.ZipPostalCode));
            builder.AppendFormat("&email={0}", WebUtility.UrlEncode(postProcessPaymentRequest.Order.BillingAddress.Email));

            return builder.ToString();
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
            return new ProcessPaymentResult();
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var urlToRedirect = GenerationRedirectionUrl(postProcessPaymentRequest, _paypalStandardPaymentSettings.PassProductNamesAndTotals);
            if (urlToRedirect == null)
                throw new Exception("PayPal URL cannot be generated");

            //ensure URL doesn't exceed 2K chars. Otherwise, customers can get "too long URL" exception
            if (urlToRedirect.Length > 2048)
                urlToRedirect = GenerationRedirectionUrl(postProcessPaymentRequest, false);

            _httpContextAccessor.HttpContext?.Response?.Redirect(urlToRedirect);
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
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                _paypalStandardPaymentSettings.AdditionalFee, _paypalStandardPaymentSettings.AdditionalFeePercentage);
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult { Errors = new[] { "Capture method not supported" } };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            return new RefundPaymentResult { Errors = new[] { "Refund method not supported" } };
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult { Errors = new[] { "Void method not supported" } };
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            
            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return false;

            return true;
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentPayPalStandard/Configure";
        }

        /// <summary>
        /// Gets a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <param name="viewComponentName">View component name</param>
        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "PaymentPayPalStandard";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new PayPalStandardPaymentSettings
            {
                UseSandbox = true,
                BusinessEmail = "test@test.com",
                PdtToken= "Your PDT token here...",
                PdtValidateOrderTotal = true,
                EnableIpn = true,
                AddressOverride = true,
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AddressOverride", "Address override");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AddressOverride.Hint", "For people who already have PayPal accounts and whom you already prompted for a shipping address before they choose to pay with PayPal, you can use the entered address instead of the address the person has stored with PayPal.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.BusinessEmail", "Business Email");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.BusinessEmail.Hint", "Specify your PayPal business email.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.EnableIpn", "Enable IPN (Instant Payment Notification)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.EnableIpn.Hint", "Check if IPN is enabled.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.EnableIpn.Hint2", "Leave blank to use the default IPN handler URL.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.IpnUrl", "IPN Handler");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.IpnUrl.Hint", "Specify IPN Handler.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PassProductNamesAndTotals", "Pass product names and order totals to PayPal");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PassProductNamesAndTotals.Hint", "Check if product names and order totals should be passed to PayPal.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PDTToken", "PDT Identity Token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PDTToken.Hint", "Specify PDT identity token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PDTValidateOrderTotal", "PDT. Validate order total");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PDTValidateOrderTotal.Hint", "Check if PDT handler should validate order totals.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.RedirectionTip", "You will be redirected to PayPal site to complete the order.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage", "Return to order details page");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage.Hint", "Enable if a customer should be redirected to the order details page when he clicks \"return to store\" link on PayPal site WITHOUT completing a payment");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.UseSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.Instructions", "<p><b>If you're using this gateway ensure that your primary store currency is supported by Paypal.</b><br /><br />To use PDT, you must activate PDT and Auto Return in your PayPal account profile. You must also acquire a PDT identity token, which is used in all PDT communication you send to PayPal. Follow these steps to configure your account for PDT:<br /><br />1. Log in to your PayPal account (click <a href=\"https://www.paypal.com/us/webapps/mpp/referral/paypal-business-account2?partner_id=9JJPJNNPQ7PZ8\" target=\"_blank\">here</a> to create your account).<br />2. Click the Profile subtab.<br />3. Click Website Payment Preferences in the Seller Preferences column.<br />4. Under Auto Return for Website Payments, click the On radio button.<br />5. For the Return URL, enter the URL on your site that will receive the transaction ID posted by PayPal after a customer payment (http://www.yourStore.com/Plugins/PaymentPayPalStandard/PDTHandler).<br />6. Under Payment Data Transfer, click the On radio button.<br />7. Click Save.<br />8. Click Website Payment Preferences in the Seller Preferences column.<br />9. Scroll down to the Payment Data Transfer section of the page to view your PDT identity token.<br /><br /><b>Two ways to be able to receive IPN messages (optional):</b><br /><br /><b>The first way is to check 'Enable IPN' below.</b> It will include in the request the url of you IPN handler<br /><br /><b>The second way is to confugure your paypal account to activate this service</b>; follow these steps:<br />1. Log in to your Premier or Business account.<br />2. Click the Profile subtab.<br />3. Click Instant Payment Notification in the Selling Preferences column.<br />4. Click the 'Edit IPN Settings' button to update your settings.<br />5. Select 'Receive IPN messages' (Enabled) and enter the URL of your IPN handler (http://www.yourStore.com/Plugins/PaymentPayPalStandard/IPNHandler).<br />6. Click Save, and you should get a message that you have successfully activated IPN.</p>");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.PaymentMethodDescription", "You will be redirected to PayPal site to complete the payment");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalStandard.RoundingWarning", "It looks like you have \"ShoppingCartSettings.RoundPricesDuringCalculation\" setting disabled. Keep in mind that this can lead to a discrepancy of the order total amount, as PayPal only rounds to two decimals.");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<PayPalStandardPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AddressOverride");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.AddressOverride.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.BusinessEmail");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.BusinessEmail.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.EnableIpn");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.EnableIpn.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.EnableIpn.Hint2");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.IpnUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.IpnUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PassProductNamesAndTotals");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PassProductNamesAndTotals.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PDTToken");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PDTToken.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PDTValidateOrderTotal");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.PDTValidateOrderTotal.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.RedirectionTip");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Fields.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.Instructions");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.PaymentMethodDescription");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalStandard.RoundingWarning");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get { return RecurringPaymentType.NotSupported; }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.Redirection; }
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
            get { return _localizationService.GetResource("Plugins.Payments.PayPalStandard.PaymentMethodDescription"); }
        }

        #endregion
    }
}
