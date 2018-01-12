using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.PayPalDirect.Models;
using Nop.Plugin.Payments.PayPalDirect.Validators;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;
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
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentService _paymentService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly PayPalDirectPaymentSettings _paypalDirectPaymentSettings;

        #endregion

        #region Ctor

        public PayPalDirectPaymentProcessor(CurrencySettings currencySettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentService paymentService,
            IPriceCalculationService priceCalculationService,
            IProductAttributeParser productAttributeParser,
            ISettingService settingService, 
            IStoreContext storeContext,
            ITaxService taxService,
            IWebHelper webHelper,
            PayPalDirectPaymentSettings paypalDirectPaymentSettings)
        {
            this._currencySettings = currencySettings;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._paymentService = paymentService;
            this._priceCalculationService = priceCalculationService;
            this._productAttributeParser = productAttributeParser;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._taxService = taxService;
            this._webHelper = webHelper;
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

            return $"{startDate.ToString("s")}Z";
        }

        #region Items

        /// <summary>
        /// Get PayPal items
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="currencyCode">Currency code</param>
        /// <returns>List of PayPal items</returns>
        protected List<Item> GetItems(IList<ShoppingCartItem> shoppingCart, Customer customer, int storeId, string currencyCode)
        {
            var items = new List<Item>();

            if (!_paypalDirectPaymentSettings.PassPurchasedItems)
                return items;

            //create PayPal items from shopping cart items
            items.AddRange(CreateItems(shoppingCart));

            //create PayPal items from checkout attributes
            items.AddRange(CreateItemsForCheckoutAttributes(customer, storeId));

            //create PayPal item for payment method additional fee
            items.Add(CreateItemForPaymentAdditionalFee(shoppingCart, customer));

            //currently there are no ways to add discount for all order directly to amount details, so we add them as extra items 
            //create PayPal item for subtotal discount
            items.Add(CreateItemForSubtotalDiscount(shoppingCart));

            //create PayPal item for total discount
            items.Add(CreateItemForTotalDiscount(shoppingCart));

            items.RemoveAll(item => item == null);

            //add currency code for all items
            items.ForEach(item => item.currency = currencyCode);

            return items;
        }

        /// <summary>
        /// Create items from shopping cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>Collection of PayPal items</returns>
        protected IEnumerable<Item> CreateItems(IEnumerable<ShoppingCartItem> shoppingCart)
        {
            return shoppingCart.Select(shoppingCartItem =>
            {
                if (shoppingCartItem.Product == null)
                    return null;

                var item = new Item
                {

                    //name
                    name = shoppingCartItem.Product.Name
                };

                //SKU
                if (!string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
                {
                    var combination = _productAttributeParser.FindProductAttributeCombination(shoppingCartItem.Product, shoppingCartItem.AttributesXml);
                    item.sku = combination != null && !string.IsNullOrEmpty(combination.Sku) ? combination.Sku : shoppingCartItem.Product.Sku;
                }
                else
                    item.sku = shoppingCartItem.Product.Sku;

                //item price
                var unitPrice = _priceCalculationService.GetUnitPrice(shoppingCartItem);
                var price = _taxService.GetProductPrice(shoppingCartItem.Product, unitPrice, false, shoppingCartItem.Customer, out decimal _);
                item.price = price.ToString("N", new CultureInfo("en-US"));

                //quantity
                item.quantity = shoppingCartItem.Quantity.ToString();

                return item;
            });
        }

        /// <summary>
        /// Create items for checkout attributes
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Collection of PayPal items</returns>
        protected IEnumerable<Item> CreateItemsForCheckoutAttributes(Customer customer, int storeId)
        {
            var checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, storeId);
            if (string.IsNullOrEmpty(checkoutAttributesXml))
                return new List<Item>();

            //get attribute values
            var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);

            return attributeValues.Select(checkoutAttributeValue =>
            {
                if (checkoutAttributeValue.CheckoutAttribute == null)
                    return null;

                //get price
                var attributePrice = _taxService.GetCheckoutAttributePrice(checkoutAttributeValue, false, customer);

                //create item
                return new Item
                {
                    name = $"{checkoutAttributeValue.CheckoutAttribute.Name} ({checkoutAttributeValue.Name})",
                    price = attributePrice.ToString("N", new CultureInfo("en-US")),
                    quantity = "1"
                };
            });
        }

        /// <summary>
        /// Create item for payment method additional fee
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>PayPal item</returns>
        protected Item CreateItemForPaymentAdditionalFee(IList<ShoppingCartItem> shoppingCart, Customer customer)
        {
            //get price
            var paymentAdditionalFee = _paymentService.GetAdditionalHandlingFee(shoppingCart, PluginDescriptor.SystemName);
            var paymentPrice = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, false, customer);

            if (paymentPrice <= decimal.Zero)
                return null;

            //create item
            return new Item
            {
                name = $"Payment method ({PluginDescriptor.FriendlyName}) additional fee",
                price = paymentPrice.ToString("N", new CultureInfo("en-US")),
                quantity = "1"
            };
        }

        /// <summary>
        /// Create item for discount to order subtotal
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>PayPal item</returns>
        protected Item CreateItemForSubtotalDiscount(IList<ShoppingCartItem> shoppingCart)
        {
            //get subtotal discount amount
            _orderTotalCalculationService.GetShoppingCartSubTotal(shoppingCart, false, out decimal discountAmount, out List<DiscountForCaching> _, out decimal _, out decimal _);

            if (discountAmount <= decimal.Zero)
                return null;

            //create item with negative price
            return new Item
            {
                name = "Discount for the subtotal of order",
                price = (-discountAmount).ToString("N", new CultureInfo("en-US")),
                quantity = "1"
            };
        }

        /// <summary>
        /// Create item for discount to order total 
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>PayPal item</returns>
        protected Item CreateItemForTotalDiscount(IList<ShoppingCartItem> shoppingCart)
        {
            //get total discount amount
            var orderTotal = _orderTotalCalculationService.GetShoppingCartTotal(shoppingCart,
                out decimal discountAmount,
                out List<DiscountForCaching> _, out List<AppliedGiftCard> _, out int _, out decimal _);

            if (discountAmount <= decimal.Zero)
                return null;

            //create item with negative price
            return new Item
            {
                name = "Discount for the total of order",
                price = (-discountAmount).ToString("N", new CultureInfo("en-US")),
                quantity = "1"
            };
        }

        #endregion

        /// <summary>
        /// Get transaction amount details
        /// </summary>
        /// <param name="paymentRequest">Payment info required for an order processing</param>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="items">List of PayPal items</param>
        /// <returns>Amount details object</returns>
        protected Details GetAmountDetails(ProcessPaymentRequest paymentRequest, IList<ShoppingCartItem> shoppingCart, IList<Item> items)
        {
            //get shipping total
            var shipping = _orderTotalCalculationService.GetShoppingCartShippingTotal(shoppingCart, false);
            var shippingTotal = shipping ?? 0;

            //get tax total
            var taxTotal = _orderTotalCalculationService.GetTaxTotal(shoppingCart, out SortedDictionary<decimal, decimal> _);

            //get subtotal
            decimal subTotal;
            if (items != null && items.Any())
            {
                //items passed to PayPal, so calculate subtotal based on them
                subTotal = items.Sum(item => !decimal.TryParse(item.price, out decimal tmpPrice) || !int.TryParse(item.quantity, out int tmpQuantity) ? 0 : tmpPrice * tmpQuantity);
            }
            else
                subTotal = paymentRequest.OrderTotal - shippingTotal - taxTotal;

            //adjust order total to avoid PayPal payment error: "Transaction amount details (subtotal, tax, shipping) must add up to specified amount total"
            paymentRequest.OrderTotal = Math.Round(shippingTotal, 2) + Math.Round(subTotal, 2) + Math.Round(taxTotal, 2);

            //create amount details
            return new Details
            {
                shipping = shippingTotal.ToString("N", new CultureInfo("en-US")),
                subtotal = subTotal.ToString("N", new CultureInfo("en-US")),
                tax = taxTotal.ToString("N", new CultureInfo("en-US"))
            };
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
                
                //currency
                var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

                //get current shopping cart
                var shoppingCart = customer.ShoppingCartItems
                    .Where(shoppingCartItem => shoppingCartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(processPaymentRequest.StoreId).ToList();

                //items
                var items = GetItems(shoppingCart, customer, processPaymentRequest.StoreId, currency.CurrencyCode);

                //amount details
                var amountDetails = GetAmountDetails(processPaymentRequest, shoppingCart, items);

                //payment
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
                                details = amountDetails,
                                total = processPaymentRequest.OrderTotal.ToString("N", new CultureInfo("en-US")),
                                currency = currency != null ? currency.CurrencyCode : null
                            },

                            #endregion

                            item_list = new ItemList
                            {
                                items = items,

                                #region shipping address

                                shipping_address = customer.ShippingAddress == null ? null : new ShippingAddress
                                {
                                    country_code = customer.ShippingAddress.Country != null ? customer.ShippingAddress.Country.TwoLetterIsoCode : null,
                                    state = customer.ShippingAddress.StateProvince != null ? customer.ShippingAddress.StateProvince.Abbreviation : null,
                                    city = customer.ShippingAddress.City,
                                    line1 = customer.ShippingAddress.Address1,
                                    line2 = customer.ShippingAddress.Address2,
                                    phone = customer.ShippingAddress.PhoneNumber,
                                    postal_code = customer.ShippingAddress.ZipPostalCode,
                                    recipient_name =
                                        $"{customer.ShippingAddress.FirstName} {customer.ShippingAddress.LastName}"
                                }

                                #endregion
                            },

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
                            if (authorization.fmf_details != null && !string.IsNullOrEmpty(authorization.fmf_details.filter_id))
                            {
                                result.AuthorizationTransactionResult =
                                    $"Authorization is {authorization.fmf_details.filter_type}. Based on fraud filter: {authorization.fmf_details.name}. {authorization.fmf_details.description}";
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
                            if (sale.fmf_details != null && !string.IsNullOrEmpty(sale.fmf_details.filter_id))
                            {
                                result.CaptureTransactionResult =
                                    $"Sale is {sale.fmf_details.filter_type}. Based on fraud filter: {sale.fmf_details.name}. {sale.fmf_details.description}";
                                result.NewPaymentStatus = GetPaymentStatus(Sale.Get(apiContext, sale.id).state);
                            }
                            else
                            {
                                result.CaptureTransactionResult = sale.state;
                                result.NewPaymentStatus = GetPaymentStatus(sale.state);
                            }
                            result.CaptureTransactionId = sale.id;
                            result.AvsResult = sale.processor_response?.avs_code ?? string.Empty;
                            result.Cvv2Result = sale.processor_response?.cvv_code ?? string.Empty;
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
                        result.AddError($"PayPal error: {error.message} ({error.name})");
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError($"{x.field} {x.issue}"));
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
        /// <param name="cart">Shopping cart</param>
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
                        result.AddError($"PayPal error: {error.message} ({error.name})");
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError($"{x.field} {x.issue}"));
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
                        result.AddError($"PayPal error: {error.message} ({error.name})");
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError($"{x.field} {x.issue}"));
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
                        result.AddError($"PayPal error: {error.message} ({error.name})");
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError($"{x.field} {x.issue}"));
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
                var url = _webHelper.GetStoreLocation(_storeContext.CurrentStore.SslEnabled);
                var billingPlan = new Plan
                {
                    name = processPaymentRequest.OrderGuid.ToString(),
                    description = $"nopCommerce billing plan for the {processPaymentRequest.OrderGuid} order",
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
                    name = $"nopCommerce subscription for the {processPaymentRequest.OrderGuid} order",
                    //we set order GUID in the description, then use it in the webhook handler
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
                        result.AddError($"PayPal error: {error.message} ({error.name})");
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError($"{x.field} {x.issue}"));
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
                    note = $"Cancel subscription {cancelPaymentRequest.Order.OrderGuid}"
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
                        result.AddError($"PayPal error: {error.message} ({error.name})");
                        if (error.details != null)
                            error.details.ForEach(x => result.AddError($"{x.field} {x.issue}"));
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
                throw new ArgumentNullException(nameof(order));

            //it's not a redirection payment method. So we always return false
            return false;
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            var warnings = new List<string>();

            //validate
            var validator = new PaymentInfoValidator(_localizationService);
            var model = new PaymentInfoModel
            {
                CardNumber = form["CardNumber"],
                CardCode = form["CardCode"],
                ExpireMonth = form["ExpireMonth"],
                ExpireYear = form["ExpireYear"]
            };
            var validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
                warnings.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));

            return warnings;
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest
            {
                CreditCardType = form["CreditCardType"],
                CreditCardNumber = form["CardNumber"],
                CreditCardExpireMonth = int.Parse(form["ExpireMonth"]),
                CreditCardExpireYear = int.Parse(form["ExpireYear"]),
                CreditCardCvv2 = form["CardCode"]
            };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentPayPalDirect/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return "PaymentPayPalDirect";
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
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.PassPurchasedItems", "Pass purchased items");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.PassPurchasedItems.Hint", "Check to pass information about purchased items to PayPal.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode", "Transaction mode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode.Hint", "Choose transaction mode.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.WebhookId", "Webhook ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.WebhookId.Hint", "Specify webhook ID.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Instructions", "<p><b>If you're using this gateway ensure that your primary store currency is supported by PayPal.</b><br /><br />To configure plugin follow these steps:<br />1. Log into your Developer PayPal account (click <a href=\"https://www.paypal.com/us/webapps/mpp/referral/paypal-business-account2?partner_id=9JJPJNNPQ7PZ8\" target=\"_blank\">here</a> to create your account).<br />2. Click on My Apps & Credentials from the Dashboard.<br />3. Create new REST API app.<br />4. Copy your Client ID and Secret key below.<br />5. To be able to use recurring payments you need to set the webhook ID. You can get it manually in your PayPal account (enter the URL {0} below REST API application credentials), or automatically by pressing \"{1}\" button (not visible when running the site locally).<br /></p>");
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
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.PassPurchasedItems");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.PassPurchasedItems.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.WebhookId");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.WebhookId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Instructions");
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