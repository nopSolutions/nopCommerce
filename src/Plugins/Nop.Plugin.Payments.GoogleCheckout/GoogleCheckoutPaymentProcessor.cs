using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Routing;
using System.Xml;
using System.Xml.Serialization;
using GCheckout.AutoGen;
using GCheckout.Checkout;
using GCheckout.Util;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.GoogleCheckout.Controllers;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Plugin.Payments.GoogleCheckout
{
    /// <summary>
    /// GoogleCheckout payment processor
    /// </summary>
    public class GoogleCheckoutPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ITaxService _taxService;
        private readonly IShippingService _shippingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly HttpContextBase _httpContext;

        #endregion

        #region Ctor

        public GoogleCheckoutPaymentProcessor(ISettingService settingService, 
            IWebHelper webHelper, ITaxService taxService,
            IShippingService shippingService, 
            IOrderTotalCalculationService orderTotalCalculationService,
            IProductAttributeFormatter productAttributeFormatter,
            IPriceCalculationService priceCalculationService, IWorkContext workContext,
            ICustomerService customerService, IGenericAttributeService genericAttributeService, 
            ICountryService countryService,
            IStateProvinceService stateProvinceService, IOrderProcessingService orderProcessingService,
            IOrderService orderService, ILogger logger, HttpContextBase httpContext)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._taxService = taxService;
            this._shippingService = shippingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._priceCalculationService = priceCalculationService;
            this._workContext = workContext;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._orderProcessingService = orderProcessingService;
            this._orderService = orderService;
            this._logger = logger;
            this._httpContext = httpContext;
        }

        #endregion

        #region Utilities

        private void LogMessage(string message)
        {
            try
            {
                if (!_settingService.GetSettingByKey<bool>("googlecheckoutpaymentsettings.logfileenabled"))
                    return;
                message = string.Format("{0}*******{1}{2}", DateTime.Now, Environment.NewLine, message);
                string logPath = _httpContext.Server.MapPath("~/App_Data/googlecheckout_log.txt");
                using (var fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine(message);
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
            }
        }
        private void ProcessNewOrderNotification(string xmlData)
        {
            try
            {
                var newOrderNotification = (NewOrderNotification)EncodeHelper.Deserialize(xmlData, typeof(NewOrderNotification));
                string googleOrderNumber = newOrderNotification.googleordernumber;

                XmlNode customerInfo = newOrderNotification.shoppingcart.merchantprivatedata.Any[0];
                int customerId = Convert.ToInt32(customerInfo.Attributes["CustomerID"].Value);
                //int customerLanguageId = Convert.ToInt32(customerInfo.Attributes["CustomerLanguageID"].Value);
                //int customerCurrencyId = Convert.ToInt32(customerInfo.Attributes["CustomerCurrencyID"].Value);
                var customer = _customerService.GetCustomerById(customerId);

                if (customer == null)
                {
                    LogMessage("Could not load a customer");
                    return;
                }

                var cart = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

                _workContext.CurrentCustomer = customer;

                if (cart.Count == 0)
                {
                    LogMessage("Cart is empty");
                    return;
                }

                //validate cart
                foreach (var sci in cart)
                {
                    bool ok = false;
                    foreach (Item item in newOrderNotification.shoppingcart.items)
                    {
                        if (!String.IsNullOrEmpty(item.merchantitemid))
                        {
                            if ((Convert.ToInt32(item.merchantitemid) == sci.Id) && (item.quantity == sci.Quantity))
                            {
                                ok = true;
                                break;
                            }
                        }
                    }

                    if (!ok)
                    {
                        LogMessage(string.Format("Shopping Cart item has been changed. {0}. {1}", sci.Id, sci.Quantity));
                        return;
                    }
                }


                string[] billingFullname = newOrderNotification.buyerbillingaddress.contactname.Trim().Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string billingFirstName = billingFullname[0];
                string billingLastName = string.Empty;
                if (billingFullname.Length > 1)
                    billingLastName = billingFullname[1];
                string billingEmail = newOrderNotification.buyerbillingaddress.email.Trim();
                string billingAddress1 = newOrderNotification.buyerbillingaddress.address1.Trim();
                string billingAddress2 = newOrderNotification.buyerbillingaddress.address2.Trim();
                string billingPhoneNumber = newOrderNotification.buyerbillingaddress.phone.Trim();
                string billingCity = newOrderNotification.buyerbillingaddress.city.Trim();
                int? billingStateProvinceId = null;
                var billingStateProvince = _stateProvinceService.GetStateProvinceByAbbreviation(newOrderNotification.buyerbillingaddress.region.Trim());
                if (billingStateProvince != null)
                    billingStateProvinceId = billingStateProvince.Id;
                string billingZipPostalCode = newOrderNotification.buyerbillingaddress.postalcode.Trim();
                int? billingCountryId = null;
                var billingCountry = _countryService.GetCountryByTwoLetterIsoCode(newOrderNotification.buyerbillingaddress.countrycode.Trim());
                if (billingCountry != null)
                    billingCountryId = billingCountry.Id;

                var billingAddress = customer.Addresses.ToList().FindAddress(
                    billingFirstName, billingLastName, billingPhoneNumber,
                    billingEmail, string.Empty, string.Empty, billingAddress1, billingAddress2, billingCity,
                    billingStateProvinceId, billingZipPostalCode, billingCountryId);

                if (billingAddress == null)
                {
                    billingAddress = new Core.Domain.Common.Address()
                    {
                        FirstName = billingFirstName,
                        LastName = billingLastName,
                        PhoneNumber = billingPhoneNumber,
                        Email = billingEmail,
                        Address1 = billingAddress1,
                        Address2 = billingAddress2,
                        City = billingCity,
                        StateProvinceId = billingStateProvinceId,
                        ZipPostalCode = billingZipPostalCode,
                        CountryId = billingCountryId,
                        CreatedOnUtc = DateTime.UtcNow,
                    };
                    customer.Addresses.Add(billingAddress);
                }
                //set default billing address
                customer.BillingAddress = billingAddress;
                _customerService.UpdateCustomer(customer);

                _genericAttributeService.SaveAttribute<ShippingOption>(customer, SystemCustomerAttributeNames.LastShippingOption, null);

                bool shoppingCartRequiresShipping = cart.RequiresShipping();
                if (shoppingCartRequiresShipping)
                {
                    string[] shippingFullname = newOrderNotification.buyershippingaddress.contactname.Trim().Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    string shippingFirstName = shippingFullname[0];
                    string shippingLastName = string.Empty;
                    if (shippingFullname.Length > 1)
                        shippingLastName = shippingFullname[1];
                    string shippingEmail = newOrderNotification.buyershippingaddress.email.Trim();
                    string shippingAddress1 = newOrderNotification.buyershippingaddress.address1.Trim();
                    string shippingAddress2 = newOrderNotification.buyershippingaddress.address2.Trim();
                    string shippingPhoneNumber = newOrderNotification.buyershippingaddress.phone.Trim();
                    string shippingCity = newOrderNotification.buyershippingaddress.city.Trim();
                    int? shippingStateProvinceId = null;
                    var shippingStateProvince = _stateProvinceService.GetStateProvinceByAbbreviation(newOrderNotification.buyershippingaddress.region.Trim());
                    if (shippingStateProvince != null)
                        shippingStateProvinceId = shippingStateProvince.Id;
                    int? shippingCountryId = null;
                    string shippingZipPostalCode = newOrderNotification.buyershippingaddress.postalcode.Trim();
                    var shippingCountry = _countryService.GetCountryByTwoLetterIsoCode(newOrderNotification.buyershippingaddress.countrycode.Trim());
                    if (shippingCountry != null)
                        shippingCountryId = shippingCountry.Id;

                    var shippingAddress = customer.Addresses.ToList().FindAddress(
                        shippingFirstName, shippingLastName, shippingPhoneNumber,
                        shippingEmail, string.Empty, string.Empty,
                        shippingAddress1, shippingAddress2, shippingCity,
                        shippingStateProvinceId, shippingZipPostalCode, shippingCountryId);
                    if (shippingAddress == null)
                    {
                        shippingAddress = new Core.Domain.Common.Address()
                        {
                            FirstName = shippingFirstName,
                            LastName = shippingLastName,
                            PhoneNumber = shippingPhoneNumber,
                            Email = shippingEmail,
                            Address1 = shippingAddress1,
                            Address2 = shippingAddress2,
                            City = shippingCity,
                            StateProvinceId = shippingStateProvinceId,
                            ZipPostalCode = shippingZipPostalCode,
                            CountryId = shippingCountryId,
                            CreatedOnUtc = DateTime.UtcNow,
                        };
                        customer.Addresses.Add(shippingAddress);
                    }
                    //set default shipping address
                    customer.ShippingAddress = shippingAddress;
                    _customerService.UpdateCustomer(customer);

                    if (newOrderNotification.orderadjustment != null &&
                        newOrderNotification.orderadjustment.shipping != null &&
                        newOrderNotification.orderadjustment.shipping.Item != null)
                    {
                        var shippingMethod = (FlatRateShippingAdjustment)newOrderNotification.orderadjustment.shipping.Item;
                        var shippingOption = new ShippingOption();
                        shippingOption.Name = shippingMethod.shippingname;
                        shippingOption.Rate = shippingMethod.shippingcost.Value;
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastShippingOption, shippingOption);
                    }
                }

                //customer.LastCalculatedTax = decimal.Zero;

                var paymentInfo = new ProcessPaymentRequest()
                {
                    PaymentMethodSystemName = "Payments.GoogleCheckout",
                    CustomerId = customer.Id,
                    GoogleOrderNumber = googleOrderNumber
                };
                //TODO set customer language and currency
                //paymentInfo.CustomerLanguage = IoC.Resolve<ILanguageService>().GetLanguageById(CustomerLanguageID);
                //paymentInfo.CustomerCurrency = IoC.Resolve<ICurrencyService>().GetCurrencyById(CustomerCurrencyID);
                var result = _orderProcessingService.PlaceOrder(paymentInfo);
                if (!result.Success)
                {
                    LogMessage("new-order-notification received. CreateOrder() error: Order Number " + googleOrderNumber + ". " + result);
                    return;
                }

                var order = result.PlacedOrder;
                if (order != null)
                {
                    LogMessage("new-order-notification received and saved: Order Number " + order.Id);
                }
            }
            catch (Exception exc)
            {
                LogMessage("processNewOrderNotification Exception: " + exc.Message + ": " + exc.StackTrace);
            }
        }
        private void ProcessOrderStateChangeNotification(string xmlData)
        {
            try
            {
                var changeOrder = (OrderStateChangeNotification)EncodeHelper.Deserialize(xmlData, typeof(OrderStateChangeNotification));

                FinancialOrderState orderState = changeOrder.newfinancialorderstate;
                Order order = GetMerchantOrderByGoogleOrderId(changeOrder.googleordernumber);
                if (order != null)
                {
                    string message = string.Format("Order status {0} from Google: Order Number {1}", orderState, changeOrder.googleordernumber);
                    LogMessage(message);
                    
                    //add a note
                    order.OrderNotes.Add(new OrderNote()
                    {
                        Note = message,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(order);

                    if (orderState == FinancialOrderState.CHARGING ||
                        orderState == FinancialOrderState.REVIEWING)
                    {
                    }

                    if (orderState == FinancialOrderState.CHARGEABLE)
                    {
                        _orderProcessingService.MarkAsAuthorized(order);
                    }
                    if (orderState == FinancialOrderState.CHARGED)
                    {
                        _orderProcessingService.MarkOrderAsPaid(order);
                    }
                    if (orderState == FinancialOrderState.CANCELLED || orderState == FinancialOrderState.CANCELLED_BY_GOOGLE)
                    {
                        _orderProcessingService.CancelOrder(order, true);
                    }
                }
            }
            catch (Exception exc)
            {
                LogMessage("processOrderStateChangeNotification Exception: " + exc.Message + ": " + exc.StackTrace);
            }
        }
        private void ProcessErrorNotification(string xmlData)
        {
            try
            {
                var errorResponse = (ErrorResponse)EncodeHelper.Deserialize(xmlData, typeof(ErrorResponse));

                var errorSb = new StringBuilder();
                errorSb.Append(string.Format("Error response message received: {0}", errorResponse.errormessage));
                foreach (string warning in errorResponse.warningmessages)
                    errorSb.Append("Warning: " + warning);
                string message = errorSb.ToString();
                LogMessage(message);
            }
            catch (Exception exc)
            {
                LogMessage("processErrorNotification Exception: " + exc.Message + ": " + exc.StackTrace);
            }
        }
        private void ProcessRiskInformationNotification(string xmlData)
        {
            var riskInformationNotification = (RiskInformationNotification)EncodeHelper.Deserialize(xmlData, typeof(RiskInformationNotification));

            var riskSb = new StringBuilder();
            riskSb.Append("Risk Information: ");
            riskSb.Append("googleordernumber: ");
            riskSb.Append(riskInformationNotification.googleordernumber);
            riskSb.Append(", avsresponse: ");
            riskSb.Append(riskInformationNotification.riskinformation.avsresponse);
            riskSb.Append(", ipaddress: ");
            riskSb.Append(riskInformationNotification.riskinformation.ipaddress);
            riskSb.Append(", partialccnumber: ");
            riskSb.Append(riskInformationNotification.riskinformation.partialccnumber);
            string message = riskSb.ToString();
            LogMessage(message);

            Order order = GetMerchantOrderByGoogleOrderId(riskInformationNotification.googleordernumber);
            if (order != null)
            {
                //add a note
                order.OrderNotes.Add(new OrderNote()
                {
                    Note = message,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
            }
        }
        private Order GetMerchantOrderByGoogleOrderId(string googleOrderId)
        {
            if (String.IsNullOrEmpty(googleOrderId))
                return null;

            return _orderService.GetOrderByAuthorizationTransactionIdAndPaymentMethod(googleOrderId, "Payments.GoogleCheckout");
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
            result.NewPaymentStatus = PaymentStatus.Pending;
            result.AuthorizationTransactionId = processPaymentRequest.GoogleOrderNumber;
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
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<Nop.Core.Domain.Orders.ShoppingCartItem> cart)
        {
            return decimal.Zero;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();

            string googleOrderNumber = capturePaymentRequest.Order.AuthorizationTransactionId;
            var chargeOrderRequest = new GCheckout.OrderProcessing.ChargeOrderRequest(googleOrderNumber);
            var chargeOrderResponse = chargeOrderRequest.Send();
            if (chargeOrderResponse.IsGood)
            {
                result.NewPaymentStatus = PaymentStatus.Paid;
                result.CaptureTransactionResult = chargeOrderResponse.ResponseXml;
            }
            else
            {
                result.AddError(chargeOrderResponse.ErrorMessage);
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
            result.AddError("Refund method not supported");
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
            result.AddError("Void method not supported");
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
            result.AddError("Recurring payment not supported");
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
            result.AddError("Recurring payment not supported");
            return result;
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

            //We always return false
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
            controllerName = "PaymentGoogleCheckout";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.GoogleCheckout.Controllers" }, { "area", null } };
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
            controllerName = "PaymentGoogleCheckout";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.GoogleCheckout.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentGoogleCheckoutController);
        }

        /// <summary>
        /// Post cart to google
        /// </summary>
        /// <param name="req">Pre-generated request</param>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Response</returns>
        public GCheckoutResponse PostCartToGoogle(CheckoutShoppingCartRequest req,
            IList<Core.Domain.Orders.ShoppingCartItem> cart)
        {
            //there's no need to round prices (Math.Round(,2)) because GCheckout library does it for us
            //items
            foreach (Core.Domain.Orders.ShoppingCartItem sci in cart)
            {
                var productVariant = sci.ProductVariant;
                if (productVariant != null)
                {
                    decimal taxRate = decimal.Zero;
                    string description = _productAttributeFormatter.FormatAttributes(productVariant, 
                        sci.AttributesXml, _workContext.CurrentCustomer,
                        ", ", false, true, true, true, false);
                    string fullName = "";
                    if (!String.IsNullOrEmpty(sci.ProductVariant.GetLocalized(x => x.Name)))
                        fullName = string.Format("{0} ({1})", sci.ProductVariant.Product.GetLocalized(x => x.Name), sci.ProductVariant.GetLocalized(x => x.Name));
                    else
                        fullName = sci.ProductVariant.Product.GetLocalized(x => x.Name);
                    decimal unitPrice = _taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetUnitPrice(sci, true), out taxRate);
                    req.AddItem(fullName, description, sci.Id.ToString(), unitPrice, sci.Quantity);
                }
            }

            if (cart.RequiresShipping())
            {
                //AddMerchantCalculatedShippingMethod
                //AddCarrierCalculatedShippingOption
                var shippingOptions = _shippingService.GetShippingOptions(cart, null);
                foreach (ShippingOption shippingOption in shippingOptions.ShippingOptions)
                {
                    //adjust rate
                    Discount appliedDiscount = null;
                    var shippingTotal = _orderTotalCalculationService.AdjustShippingRate(
                        shippingOption.Rate, cart, out appliedDiscount);
                    decimal shippingRateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                    req.AddFlatRateShippingMethod(shippingOption.Name, shippingRateBase);
                }
            }

            //add only US, GB states
            //CountryCollection countries = IoC.Resolve<ICountryService>().GetAllCountries();                
            //foreach (Country country in countries)
            //{
            //    foreach (StateProvince state in country.StateProvinces)
            //    {
            //        TaxByStateProvinceCollection taxByStateProvinceCollection = TaxByIoC.Resolve<IStateProvinceService>().GetAllByStateProvinceID(state.StateProvinceID);
            //        foreach (TaxByStateProvince taxByStateProvince in taxByStateProvinceCollection)
            //        {
            //            if (!String.IsNullOrEmpty(state.Abbreviation))
            //            {
            //                Req.AddStateTaxRule(state.Abbreviation, (double)taxByStateProvince.Percentage, false);
            //            }
            //        }
            //    }
            //}

            //if (subTotalDiscountBase > decimal.Zero)
            //{
            //    req.AddItem("Discount", string.Empty, string.Empty, (decimal)(-1.0) * subTotalDiscountBase, 1);
            //}

            //foreach (AppliedGiftCard agc in appliedGiftCards)
            //{
            //    req.AddItem(string.Format("Gift Card - {0}", agc.GiftCard.GiftCardCouponCode), string.Empty, string.Empty, (decimal)(-1.0) * agc.AmountCanBeUsed, 1);
            //}

            var customerInfoDoc = new XmlDocument();
            XmlElement customerInfo = customerInfoDoc.CreateElement("CustomerInfo");
            customerInfo.SetAttribute("CustomerID", _workContext.CurrentCustomer.Id.ToString());
            customerInfo.SetAttribute("CustomerLanguageID", _workContext.WorkingLanguage.Id.ToString());
            customerInfo.SetAttribute("CustomerCurrencyID", _workContext.WorkingCurrency.Id.ToString());
            req.AddMerchantPrivateDataNode(customerInfo);

            req.ContinueShoppingUrl = _webHelper.GetStoreLocation(false);
            req.EditCartUrl = _webHelper.GetStoreLocation(false) + "cart";

            GCheckoutResponse resp = req.Send();
            return resp;
        }

        /// <summary>
        /// Get notification acknowledgment text
        /// </summary>
        /// <returns>Notification ack</returns>
        public string GetNotificationAcknowledgmentText()
        {
            var gNotificationAcknowledgment = new NotificationAcknowledgment();

            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                var serResponse = new XmlSerializer(gNotificationAcknowledgment.GetType(), "http://checkout.google.com/schema/2");
                serResponse.Serialize(sw, gNotificationAcknowledgment);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Verifies message authentication
        /// </summary>
        /// <param name="authStr">Authenticatio string</param>
        /// <returns>Result</returns>
        public bool VerifyMessageAuthentication(string authStr)
        {
            bool result = false;
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

            if (!Convert.ToBoolean(config.AppSettings.Settings["GoogleAuthenticateCallback"].Value))
            {
                result = true;
            }
            else if (String.IsNullOrEmpty(authStr) || authStr.IndexOf("Basic", 0) != 0)
            {
                result = false;
            }
            else
            {
                byte[] decodedBytes = Convert.FromBase64String(authStr.Trim().Substring(6));
                string decodedAuthString = Encoding.ASCII.GetString(decodedBytes);

                string username = decodedAuthString.Split(':')[0];
                string password = decodedAuthString.Split(':')[1];

                string merchantID = config.AppSettings.Settings["GoogleMerchantID"].Value;
                string merchantKey = config.AppSettings.Settings["GoogleMerchantKey"].Value;

                result = (username == merchantID && password == merchantKey);
            }

            LogMessage(string.Format("callback authorization result = {0}", result));

            return result;
        }

        /// <summary>
        /// Process google callback request
        /// </summary>
        /// <param name="xmlData">xml data</param>
        public void ProcessCallBackRequest(string xmlData)
        {
            if (String.IsNullOrEmpty(xmlData))
                return;

            try
            {
                string commandName = EncodeHelper.GetTopElement(xmlData);
                LogMessage(string.Format("Google callback command: {0}", commandName));
                LogMessage(string.Format("Raw xml request: {0}", xmlData));

                switch (commandName)
                {
                    case "new-order-notification":
                        ProcessNewOrderNotification(xmlData);
                        break;
                    case "order-state-change-notification":
                        ProcessOrderStateChangeNotification(xmlData);
                        break;
                    case "risk-information-notification":
                        ProcessRiskInformationNotification(xmlData);
                        break;
                    case "error":
                        ProcessErrorNotification(xmlData);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                LogMessage(string.Format("An error occurred: {0}", exc));
            }
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.UseSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.GoogleVendorId", "Google Vendor ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.GoogleVendorId.Hint", "Specify Google Vendor ID.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.GoogleMerchantKey", "Google Merchant Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.GoogleMerchantKey.Hint", "Specify Google Merchant Key.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.AuthenticateCallback", "Authenticate callback");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.AuthenticateCallback.Hint", "Check to ensure that Google handler callback is authenticated.");

            base.Install();
        }
        
        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.GoogleVendorId");
            this.DeletePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.GoogleVendorId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.GoogleMerchantKey");
            this.DeletePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.GoogleMerchantKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.AuthenticateCallback");
            this.DeletePluginLocaleResource("Plugins.Payments.GoogleCheckout.Fields.AuthenticateCallback.Hint");

            base.Uninstall();
        }

        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Button;
            }
        }

        #endregion
    }
}
