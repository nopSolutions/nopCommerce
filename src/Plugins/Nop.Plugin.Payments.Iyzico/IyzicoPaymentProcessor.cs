using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Iyzico.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using LinqToDB.Common;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Plugin.Payments.Iyzico.Validators;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using StackExchange.Profiling.Internal;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Payments.Iyzico
{
    /// <summary>
    /// Iyzico payment processor
    /// </summary>
    public class IyzicoPaymentProcessor : BasePlugin, IPaymentMethod
    {
        /// <summary>
        /// https://github.com/iyzico/iyzipay-dotnet
        /// https://github.com/andrejpk/Nop.Plugin.Payments.Stripe/blob/master/Nop.Plugin.Payments.Stripe/StripePaymentProcessor.cs
        /// 
        /// </summary>
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrencySettings _currencySettings;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerService _customerService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IAddressService _addressService;
        private readonly IProductService _productService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IWorkContext _workContext;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IOrderService _orderService;
        private readonly ILanguageService _languageService;

        private readonly IPaymentIyzicoService _paymentIyzicoService;
        private readonly IyzicoPaymentSettings _iyzicoPaymentSettings;
        private readonly ICountryService _countryService;



        #endregion

        #region Ctor

        public IyzicoPaymentProcessor(
            ILocalizationService localizationService,
            IPaymentService paymentService,
            IPaymentIyzicoService paymentIyzicoService,
            ISettingService settingService,
            IWebHelper webHelper,
            IHttpContextAccessor httpContextAccessor,
            IyzicoPaymentSettings iyzicoPaymentSettings,
            CurrencySettings currencySettings,
            IShoppingCartService shoppingCartService,
            ICustomerService customerService,
            IPriceCalculationService priceCalculationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IAddressService addressService,
            IProductService productService,
            ITaxService taxService,
            ICurrencyService currencyService,
            IWorkContext workContext,
            ICategoryService categoryService,
            ILogger logger,
            IScheduleTaskService scheduleTaskService,
            IOrderService orderService,
            ILanguageService languageService,
            ICountryService countryService
            
            )
        {
            _localizationService = localizationService;
            _paymentService = paymentService;
            _paymentIyzicoService = paymentIyzicoService;
            _settingService = settingService;
            _webHelper = webHelper;
            _httpContextAccessor = httpContextAccessor;
            _iyzicoPaymentSettings = iyzicoPaymentSettings;
            _currencySettings = currencySettings;
            _shoppingCartService = shoppingCartService;
            _customerService = customerService;
            _priceCalculationService = priceCalculationService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _addressService = addressService;
            _productService = productService;
            _taxService = taxService;
            _currencyService = currencyService;
            _workContext = workContext;
            _categoryService = categoryService;
            _logger = logger;
            _scheduleTaskService = scheduleTaskService;
            _orderService = orderService;
            _languageService = languageService;
            _countryService = countryService;
            

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
            var customer = await _customerService.GetCustomerByIdAsync(processPaymentRequest.CustomerId);
            if (customer == null || customer.Id == 0)
                throw new Exception("No Valid Customer Found!");

            var cart = await _shoppingCartService.GetShoppingCartAsync(customer: customer, ShoppingCartType.ShoppingCart, processPaymentRequest.StoreId);
            if (!cart.Any())
                throw new Exception("No Product Found in Your Cart!");

            var billingAddress = await _addressService.GetAddressByIdAsync(customer.BillingAddressId ?? 0);
            if (billingAddress == null)
                throw new NopException("Customer billing address not set!");

            var shippingAddress = await _addressService.GetAddressByIdAsync(customer.ShippingAddressId ?? customer.BillingAddressId ?? 0);
            if (shippingAddress == null)
                throw new NopException("Customer shipping address not set!");

            var currency = await _workContext.GetWorkingCurrencyAsync();

            var currenctLanguage = await _workContext.GetWorkingLanguageAsync();

            Country country = null;
            try
            {
                country= await _countryService.GetCountryByIdAsync(shippingAddress.CountryId.Value);
            }
            catch
            {
               
            }
             

            var shoppingCartSubTotal = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, true);
            var shoppingCartTotal = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, true);
            var shoppingCartUnitPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartSubTotal.subTotalWithDiscount, currency);
            var shoppingCartUnitPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartTotal.shoppingCartTotal.Value, currency);
            


            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = processPaymentRequest.CreditCardName.ToString();
            paymentCard.CardNumber = processPaymentRequest.CreditCardNumber.ToString();
            paymentCard.ExpireMonth = processPaymentRequest.CreditCardExpireMonth.ToString();
            paymentCard.ExpireYear = processPaymentRequest.CreditCardExpireYear.ToString();
            paymentCard.Cvc = processPaymentRequest.CreditCardCvv2;
            paymentCard.CardAlias = processPaymentRequest.CreditCardType;

         

            //Ödeme isteği başlıkları
            CreatePaymentRequest request = new()
            {
                Locale = Locale.EN.ToString(),
                ConversationId = processPaymentRequest.OrderGuid.ToString().Replace("-",string.Empty),
                Price = IyzicoHelper.ToDecimalStringInvariant(shoppingCartUnitPriceWithoutDiscount),
                PaidPrice = IyzicoHelper.ToDecimalStringInvariant(shoppingCartUnitPriceWithDiscount),
                Currency = currency.CurrencyCode,
                BasketId = processPaymentRequest.OrderGuid.ToString(),
                PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                Buyer = await _paymentIyzicoService.GetBuyer(processPaymentRequest.CustomerId),
                BasketItems = await GetBasketItems(customer, processPaymentRequest.StoreId),
                BillingAddress = await _paymentIyzicoService.GetAddress(billingAddress),
                ShippingAddress = await _paymentIyzicoService.GetAddress(shippingAddress),
                PaymentChannel = "WEB"
               
            };

            try
            {
           
            if (currenctLanguage.LanguageCulture=="tr")
            {
                if (country!=null)
                {
                    if (country.Name.Contains("Turkey") || country.Name.Contains("Türkiye"))
                    {
                        request.Locale = Locale.TR.ToString();
                    }
                }
            }

          
            }
            catch
            {
               
            }


            //Taksit seçeneği seçili ise Taksit özelliğini aktif et
            bool enableInstallment = false;
            if (_iyzicoPaymentSettings.InstallmentNumbers.IsNullOrEmpty())
            {
                enableInstallment = false;
            }
            else
            {
                if (currency.CurrencyCode=="TRY")
                {
                    if (processPaymentRequest.CreditCardType!="Amex")
                    {
                        if (country != null)
                        {
                            if (country.Name.Contains("Turkey") || country.Name.Contains("Türkiye"))
                            {
                                enableInstallment = true;
                                request.Locale = Locale.TR.ToString();
                            }
                        }
                    }
                  
                }
             
            }
            request.PaymentCard = paymentCard;


            if (enableInstallment)
            {
                request.Installment = (from l in _iyzicoPaymentSettings.InstallmentNumbers select l).Max();

            }

            if (string.IsNullOrEmpty(request.Buyer.Name))
            {
                request.Buyer.Name = shippingAddress.FirstName;
                request.Buyer.Surname = shippingAddress.LastName;
            }

            if (string.IsNullOrEmpty(request.Buyer.Email))
            {
                request.Buyer.Email = shippingAddress.Email;
            }




            var payment = Payment.Create(request, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));
            var result = new ProcessPaymentResult
            {
                AllowStoringCreditCardNumber = _iyzicoPaymentSettings.IsCardStorage
            };

            string paymentInfo = "";
            try
            {
                paymentInfo = payment.ToJson();
            }
            catch
            {

            }

           var errorCodeList = new List<string> { "10051", "10041", "10043", "10054", "10084", "10034", "10093", "10206", "10207", "10208", "10209", "10210", "10211", "10213", "10214", "10215", "10216", "10219", "10222","10223","10225","10226","10227","10229","10232" };

            if (payment.Status == "failure")
            {
                if (!errorCodeList.Contains(payment.ErrorCode))
                {
                    await _logger.ErrorAsync(payment.ErrorMessage + "/" + payment.ErrorCode + "/" + payment.ErrorGroup + "/" + paymentInfo);
                    request.CallbackUrl =
                        $"{_webHelper.GetStoreLocation()}PaymentIyzicoPC/PaymentConfirm?orderGuid={processPaymentRequest.OrderGuid}";
                    var payment3d = ThreedsInitialize.Create(request, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));

                    paymentInfo = "";
                    try
                    {
                        paymentInfo = payment3d.ToJson();
                    }
                    catch
                    {

                    }
                    if (payment3d.Status == "failure")
                    {
                        await _logger.ErrorAsync(payment3d.ErrorMessage + "/" + payment3d.ErrorCode + "/" + payment3d.ErrorGroup + "/" + paymentInfo);

                        result.AddError(payment3d.ErrorMessage);
                        result.NewPaymentStatus = PaymentStatus.Refunded;
                    }
                    else
                    {

                        await _logger.InsertLogAsync(LogLevel.Information, "iyzicoPaymentLog", payment3d.Status + "/" + paymentInfo);
                        RetrievePaymentRequest request1 = new()
                        {
                            PaymentConversationId = payment3d.ConversationId
                        };

                        var paymentRes = Payment.Retrieve(request1, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));


                        result.CaptureTransactionId = payment3d.ConversationId;
                        result.AuthorizationTransactionId = paymentRes.PaymentId;
                        result.CaptureTransactionResult = payment3d.Status;
                        result.NewPaymentStatus = PaymentStatus.Pending;

                        string paymentPage;
                        paymentPage = payment3d.HtmlContent;
                        _httpContextAccessor.HttpContext.Session.SetString("3dsPage", paymentPage);

                        result.AuthorizationTransactionResult = "3d";

                        //sepeti temp olarak yükle
                        try
                        {
                            _httpContextAccessor.HttpContext.Response.Cookies.Append("CurrentShopCartTemp", JsonConvert.SerializeObject(cart));
                        }
                        catch 
                        {
                           
                        }

                    }
                }
                else
                {
                    result.AddError(payment.ErrorMessage);
                }
            }
            else
            {
                result.CaptureTransactionId = payment.ConversationId;
                result.AuthorizationTransactionId = payment.PaymentId;
                result.CaptureTransactionResult = payment.Status;
                result.NewPaymentStatus = PaymentStatus.Authorized;
                result.AuthorizationTransactionResult = "standard";
              

                _logger.InsertLogAsync(LogLevel.Information, "iyzicoPaymentLog", payment.Status + "/" + paymentInfo);

                string paymentPage;

                paymentPage = $"{_webHelper.GetStoreLocation()}PaymentIyzicoPC/PaymentConfirm?orderGuid={processPaymentRequest.OrderGuid}";

                _httpContextAccessor.HttpContext.Session.SetString("PaymentPage", paymentPage);




            }


            return await Task.FromResult(result);
        }

        /// <summary>
        /// Get transaction line items
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>List of transaction items</returns>
        private async Task<List<BasketItem>> GetBasketItems(Core.Domain.Customers.Customer customer, int storeId)
        {
            var items = new List<BasketItem>();

            //get current shopping cart            
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId);

            //define function to create item
            BasketItem createItem(decimal price, string productId, string productName, string categoryName, BasketItemType itemType = BasketItemType.PHYSICAL)
            {
               
                return new BasketItem
                {
                    Id = productId,
                    Name = productName,
                    Category1 = categoryName,
                    ItemType = itemType.ToString(),
                    Price = IyzicoHelper.ToDecimalStringInvariant(price)
                };
            }

            items.AddRange(shoppingCart.Select(sci =>
            {
                var product = _productService.GetProductByIdAsync(sci.ProductId).Result;
                var price = IyzicoHelper.ToDecimalInvariant(_shoppingCartService.GetUnitPriceAsync(sci, true).Result.unitPrice);
                var shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPriceAsync(product, price, true, customer);
                var shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartUnitPriceWithDiscountBase.Result.price, _workContext.GetWorkingCurrencyAsync().Result);
                var productName = product.Name;

                if (!product.Sku.IsNullOrEmpty())
                    productName = productName + "(" + product.Sku + ")";
                
                return createItem(shoppingCartUnitPriceWithDiscount.Result * sci.Quantity,
                    product.Id.ToString(),
                    productName,
                    _categoryService.GetProductCategoriesByProductIdAsync(sci.ProductId).Result.Aggregate(",", (all, pc) =>
                    {
                        var res = _categoryService.GetCategoryByIdAsync(pc.CategoryId).Result.Name;
                        res = all == "," ? res : all + ", " + res;
                        return res;
                    }),
                    product.IsShipEnabled ? BasketItemType.PHYSICAL : BasketItemType.VIRTUAL);
            }));

            return items;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            string paymentPage = _httpContextAccessor.HttpContext.Session.GetString("PaymentPage");
            string thHtml = _httpContextAccessor.HttpContext.Session.GetString("3dsPage");
            
            if (string.IsNullOrEmpty(paymentPage) == false)
            {
                _httpContextAccessor.HttpContext.Session.Remove("PaymentPage");
                _httpContextAccessor.HttpContext.Response.Redirect(paymentPage);
                //_httpContextAccessor.HttpContext.Response.Redirect($"{_webHelper.GetStoreLocation()}PaymentIyzicoPC/PaymentConfirm?orderGuid={postProcessPaymentRequest.Order.OrderGuid}");
                await Task.CompletedTask;
            }

            if (string.IsNullOrEmpty(thHtml) == false)
            {
                //_httpContextAccessor.HttpContext.Session.Remove("3dsPage");
                //_httpContextAccessor.HttpContext.Response.Redirect($"{_webHelper.GetStoreLocation()}PaymentIyzicoPC/PaymentConfirm?orderGuid={postProcessPaymentRequest.Order.OrderGuid}");

                string url = $"{_webHelper.GetStoreLocation()}PaymentIyzicoPC/ThView";
                _httpContextAccessor.HttpContext.Response.Redirect(url);
                await Task.CompletedTask;
            }

            if (postProcessPaymentRequest.Order.PaymentStatus==PaymentStatus.Paid)
            {
                //return Task.CompletedTask;
                if ((DateTime.UtcNow - postProcessPaymentRequest.Order.CreatedOnUtc).TotalSeconds < 5)
                await Task.FromResult(false);
                
                    
                await Task.FromResult(true);
            }
            else
            {
                if ((DateTime.UtcNow - postProcessPaymentRequest.Order.CreatedOnUtc).TotalSeconds < 5)
                    await Task.FromResult(false);

                await Task.FromResult(true);
            }
         
            //return Task.FromResult(new ProcessPaymentResult() { Errors = new[] { "Capture method not supported" } });
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - hide; false - display.
        /// </returns>
        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return Task.FromResult(false);
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the capture payment result
        /// </returns>
        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
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
            var result = new RefundPaymentResult();
            if (!refundPaymentRequest.IsPartialRefund)
            {
                var request = new CreateRefundRequest();
                request.ConversationId = refundPaymentRequest.Order.Id.ToString();
                request.Locale = Locale.EN.ToString();
                request.PaymentTransactionId = refundPaymentRequest.Order.AuthorizationTransactionId;
                request.Price = IyzicoHelper.ToDecimalStringInvariant(refundPaymentRequest.AmountToRefund);
                request.Ip = refundPaymentRequest.Order.CustomerIp;
                request.Currency = refundPaymentRequest.Order.CustomerCurrencyCode;

                RetrievePaymentRequest request1 = new()
                {
                    PaymentConversationId = refundPaymentRequest.Order.CaptureTransactionId
                };
                try
                {

                    var paymentRes = Payment.Retrieve(request1, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));
                if (paymentRes.Status=="success")
                {
                    if (!paymentRes.PaymentItems.IsNullOrEmpty())
                    {
                        request.PaymentTransactionId = paymentRes.PaymentItems.First().PaymentTransactionId;
                    }
                }
                }
                catch
                {
                }
                var refund = Refund.Create(request, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));
                if (refund.Status == "success")
                {
                    result.NewPaymentStatus = PaymentStatus.Refunded;
                    try
                    {
                        string refundIdTxt = await _localizationService.GetResourceAsync("Plugins.Payments.Iyzico.Fields.refundIdTxt");
                        string transactionIdTxt = await _localizationService.GetResourceAsync("Plugins.Payments.Iyzico.Fields.transactionIdTxt");
                        string refundAmountTxt = await _localizationService.GetResourceAsync("Plugins.Payments.Iyzico.Fields.refundAmountTxt");

                        var resTxt = refundIdTxt + refund.PaymentId + Environment.NewLine +
                                     transactionIdTxt + refund.PaymentTransactionId + Environment.NewLine +
                                     refundAmountTxt + String.Format("{C2}", refund.Price);
                        List<string> PaymentInformation = new()
                        {
                            resTxt

                        };
                        //order note
                        await _orderService.InsertOrderNoteAsync(new OrderNote
                        {
                            OrderId = refundPaymentRequest.Order.Id,
                            Note = string.Join(" | ", PaymentInformation),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow

                        });

                    }
                    catch 
                    {
                       
                    }
                }
                else
                {
                    result.Errors.Add(refund.ErrorMessage);
                }
            }
            else
            {
                var request = new CreateAmountBasedRefundRequest();
                request.ConversationId = refundPaymentRequest.Order.Id.ToString();
                request.Locale = Locale.EN.ToString();
                request.PaymentId = refundPaymentRequest.Order.AuthorizationTransactionId;
                request.Price = IyzicoHelper.ToDecimalStringInvariant(refundPaymentRequest.AmountToRefund);
                request.Ip = refundPaymentRequest.Order.CustomerIp;
               

                var refund = Refund.CreateAmountBasedRefundRequest(request, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));
                if (refund.Status=="success")
                {
                    result.NewPaymentStatus = PaymentStatus.PartiallyRefunded;
                    try
                    {

                        string refundIdTxt = await _localizationService.GetResourceAsync("Plugins.Payments.Iyzico.Fields.refundIdTxt");
                        string transactionIdTxt = await _localizationService.GetResourceAsync("Plugins.Payments.Iyzico.Fields.transactionIdTxt");
                        string refundAmountTxt = await _localizationService.GetResourceAsync("Plugins.Payments.Iyzico.Fields.refundAmountTxt");

                   
                       
                        List<string> PaymentInformation =new List<string>();
                        PaymentInformation.Add(refundIdTxt + refund.PaymentId);
                        PaymentInformation.Add(transactionIdTxt + refund.PaymentTransactionId);
                        PaymentInformation.Add(refundAmountTxt + refund.Price);
                        //order note
                        await _orderService.InsertOrderNoteAsync(new OrderNote
                        {
                            OrderId = refundPaymentRequest.Order.Id,
                            Note = string.Join(" | ", PaymentInformation),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });




                       

                    }
                    catch
                    {

                    }
                }
                else
                {
                    result.Errors.Add(refund.ErrorMessage);
                }
            }

            return await Task.FromResult(result);
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
            var result = new VoidPaymentResult();
           
                var request = new CreateRefundRequest();
                request.ConversationId = voidPaymentRequest.Order.Id.ToString();
                request.Locale = Locale.EN.ToString();
                request.PaymentTransactionId = voidPaymentRequest.Order.AuthorizationTransactionId;
                request.Price = IyzicoHelper.ToDecimalStringInvariant(voidPaymentRequest.Order.OrderTotal);
                request.Ip = voidPaymentRequest.Order.CustomerIp;
                request.Currency = voidPaymentRequest.Order.CustomerCurrencyCode;

                RetrievePaymentRequest request1 = new()
                {
                    PaymentConversationId = voidPaymentRequest.Order.CaptureTransactionId
                };
                try
                {

                    var paymentRes = Payment.Retrieve(request1, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));
                    if (paymentRes.Status == "success")
                    {
                        if (!paymentRes.PaymentItems.IsNullOrEmpty())
                        {
                            request.PaymentTransactionId = paymentRes.PaymentItems.First().PaymentTransactionId;
                        }
                    }
                }
                catch
                {
                }
                    
                var refund = Refund.Create(request, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));
                if (refund.Status == "success")
                {
                    result.NewPaymentStatus = PaymentStatus.Refunded;
                    try
                    {
                        string refundIdTxt = await _localizationService.GetResourceAsync("Plugins.Payments.Iyzico.Fields.refundIdTxt");
                        string transactionIdTxt = await _localizationService.GetResourceAsync("Plugins.Payments.Iyzico.Fields.transactionIdTxt");
                        string refundAmountTxt = await _localizationService.GetResourceAsync("Plugins.Payments.Iyzico.Fields.refundAmountTxt");

                        var resTxt = refundIdTxt + refund.PaymentId + Environment.NewLine +
                                     transactionIdTxt + refund.PaymentTransactionId + Environment.NewLine +
                                     refundAmountTxt + String.Format("{C2}", refund.Price);
                        List<string> PaymentInformation = new()
                        {
                            resTxt

                        };

                        //order note
                        await _orderService.InsertOrderNoteAsync(new OrderNote
                        {
                            OrderId = voidPaymentRequest.Order.Id,
                            Note = string.Join(" | ", PaymentInformation),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });

                    }
                    catch
                    {

                    }
                }
                else
                {
                    result.Errors.Add(refund.ErrorMessage);
                }
            



            return await Task.FromResult(result);
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the process payment result
        /// </returns>
        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Process Recurring Payment not supported" } });
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the additional handling fee
        /// </returns>
        public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return await _orderTotalCalculationService.CalculatePaymentAdditionalFeeAsync(cart, 0, false);
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            //always success
            return Task.FromResult(new CancelRecurringPaymentResult());
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
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //it's not a redirection payment method. So we always return false
            return Task.FromResult(true);
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of validating errors
        /// </returns>
        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            var warnings = Task.FromResult<IList<string>>(new List<string>());

            //validate
            var validator = new PaymentInfoValidator(this._localizationService);
            var model = new PaymentInfoModel
            {
                CardholderName = form["CardholderName"],
                CardNumber = form["CardNumber"],
                CardCode = form["CardCode"],
                ExpireMonth = form["ExpireMonth"],
                ExpireYear = form["ExpireYear"]
            };
            var validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
            {
                var result = new List<string>();
                result.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));
                warnings = Task.FromResult<IList<string>>(result);
            }


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
        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return Task.FromResult(new ProcessPaymentRequest()
            {
                CreditCardType = form["CreditCardType"],
                CreditCardName = form["CardholderName"],
                CreditCardNumber = form["CardNumber"],
                CreditCardExpireMonth = int.Parse(form["ExpireMonth"]),
                CreditCardExpireYear = int.Parse(form["ExpireYear"]),
                CreditCardCvv2 = form["CardCode"]
            });
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentIyzico/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return "PaymentIyzico";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //locales
            var languages =await _languageService.GetAllLanguagesAsync();
            Language enLanguage=null;
            Language trLanguage = null;
            if (languages.Count>0)
            {
                foreach (var language in languages)
                {
                    if (language.UniqueSeoCode=="en")
                    {
                        enLanguage= language;
                    }
                    else if (language.UniqueSeoCode == "tr")
                    {
                        trLanguage = language;
                    }
                    
                }
            }

            
            
           
            

            if (enLanguage!=null)
            {
                await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
                {
                    ["Plugins.Payments.Iyzico.Instructions"] = "You can edit the settings of your Iyzico virtual pos integration.",
                    ["Plugins.Payments.Iyzico.PaymentMethodDescription"] = "Payment by Credit/Debit card",
                    ["Plugins.Payments.Iyzico.AccountInfo"] = "Define Your Iyzico Api Information",
                    ["Plugins.Payments.Iyzico.Fields.ApiKey"] = "Api Key",
                    ["Plugins.Payments.Iyzico.Fields.ApiKey.Hint"] = "Enter your Api Key information on your Iyzico control panel.",
                    ["Plugins.Payments.Iyzico.Fields.ApiSecret"] = "Api Secret",
                    ["Plugins.Payments.Iyzico.Fields.ApiSecret.Hint"] = "Enter your Api Secret information on your Iyzico control panel.",
                    ["Plugins.Payments.Iyzico.Fields.ApiUrl"] = "Api Url",
                    ["Plugins.Payments.Iyzico.Fields.ApiUrl.Hint"] = "Enter your Api Url information on your Iyzico control panel.",
                    ["Plugins.Payments.Iyzico.VirtualPosInfo"] = "Define Your Iyzico Payment Settings",
                    ["Plugins.Payments.Iyzico.Fields.IsCardStorage"] = "Store Card Information",
                    ["Plugins.Payments.Iyzico.Fields.IsCardStorage.Hint"] = "This option stores the first six digits and the last four digits of the credit card information transmitted by Iyzico in the database (not sent to any third party processors).",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumbers"] = "Installment Options",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumbers.Hint"] = "If the installment options are active on your Iyzico panel, you can mark them to use the installment options defined by you.",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber2"] = "2 Installments",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber3"] = "3 Installments",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber6"] = "6 Installments",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber9"] = "9 Installments",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber12"] = "12 Installments",
                    ["Plugins.Payments.Iyzico.Fields.RedirectionTip"] = "You will be directed to the payment system to complete the order.",
                    ["Plugins.Payments.Iyzico.Fields.PaymentFailed"] = "Payment Failed",
                    ["Plugins.Payments.Iyzico.Fields.PaymentErrors"] = "Payment Errors",
                    ["Plugins.Payments.Iyzico.Fields.refundIdTxt"] = "Iyzico Refund Id : ",
                    ["Plugins.Payments.Iyzico.Fields.transactionIdTxt"] = "Transaction Id : ",
                    ["Plugins.Payments.Iyzico.Fields.refundAmountTxt"] = "Refund Amount : ",
                    ["Plugins.Payments.Iyzico.Fields.PaymentFailed.Order"] = "Payment Failed. Order Number #", 
                    ["Plugins.Payments.Iyzico.Fields.Fraoud.Fail"] = "The payment was not accepted because the fraud risk of the transaction is high. Order #", 
                    ["Plugins.Payments.Iyzico.Fields.Fraoud.Review"] = "Since there is a Fraud risk related to the transaction, the payment has been taken under review. Order #",
                    ["Plugins.Payments.Iyzico.Fields.Order.NotFound"] = "Order Not Found! Order #"
                },enLanguage.Id);
            }

            if (trLanguage != null)
            {
                await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
                {
                    ["Plugins.Payments.Iyzico.Instructions"] = "Iyzico sanal pos entegrasyonunuza ait ayarları düzenleyebilirsiniz.",
                    ["Plugins.Payments.Iyzico.PaymentMethodDescription"] = "Kredi/Banka kartı ile ödeme",
                    ["Plugins.Payments.Iyzico.AccountInfo"] = "Iyzico Api Bilgilerinizi Tanımlayın",
                    ["Plugins.Payments.Iyzico.Fields.ApiKey"] = "Api Key",
                    ["Plugins.Payments.Iyzico.Fields.ApiKey.Hint"] = "Iyzico kontrol panelinizde yer alan Api Key bilginizi girin.",
                    ["Plugins.Payments.Iyzico.Fields.ApiSecret"] = "Api Secret",
                    ["Plugins.Payments.Iyzico.Fields.ApiSecret.Hint"] = "Iyzico kontrol panelinizde yer alan Api Secret bilginizi girin.",
                    ["Plugins.Payments.Iyzico.Fields.ApiUrl"] = "Api Url",
                    ["Plugins.Payments.Iyzico.Fields.ApiUrl.Hint"] = "Iyzico kontrol panelinizde yer alan Api Url bilginizi girin.",
                    ["Plugins.Payments.Iyzico.VirtualPosInfo"] = "Iyzico Ödeme Ayarlarınızı Tanımlayın",
                    ["Plugins.Payments.Iyzico.Fields.IsCardStorage"] = "Kart Bilgilerini Sakla",
                    ["Plugins.Payments.Iyzico.Fields.IsCardStorage.Hint"] = "Bu seçenek, Iyzico tarafından iletilen kredi kartı bilgilerinin ilk altı hanesini ve son dört hanesini veritabanında saklar (herhangi bir üçüncü taraf işlemciye gönderilmez).",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumbers"] = "Taksit Seçenekleri",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumbers.Hint"] = "Iyzico panelinizde taksit seçenekleri aktif ise tarafınıza tanımlı taksit seçeneklerini kullanmak için işaretleyebilirsiniz.",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber2"] = "2 Taksit",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber3"] = "3 Taksit",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber6"] = "6 Taksit",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber9"] = "9 Taksit",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber12"] = "12 Taksit",
                    ["Plugins.Payments.Iyzico.Fields.RedirectionTip"] = "Siparişi tamamlamak için ödeme sistemine yönlendirileceksiniz.",
                    ["Plugins.Payments.Iyzico.Fields.PaymentFailed"] = "Ödeme Başarısız",
                    ["Plugins.Payments.Iyzico.Fields.PaymentErrors"] = "Ödeme Hataları",
                    ["Plugins.Payments.Iyzico.Fields.refundIdTxt"] = "Iyzico Geri Ödeme Id : ",
                    ["Plugins.Payments.Iyzico.Fields.transactionIdTxt"] = "İşlem Id : ",
                    ["Plugins.Payments.Iyzico.Fields.refundAmountTxt"] = " Geri Ödeme Tutarı : ",
                    ["Plugins.Payments.Iyzico.Fields.PaymentFailed.Order"] = "Ödeme başarısız. Sipariş #",
                    ["Plugins.Payments.Iyzico.Fields.Fraoud.Fail"] = "İşleme ait Fraud riski yüksek olduğu için ödeme kabul edilmemiştir. Sipariş #",
                    ["Plugins.Payments.Iyzico.Fields.Fraoud.Review"] = "İşleme ait Fraud riski bulunduğu için ödeme incelemeye alınmıştır. Sipariş #",
                    ["Plugins.Payments.Iyzico.Fields.Order.NotFound"] = "Sipariş Bulunamadı! Sipariş #"

                }, trLanguage.Id);
            }

            if (trLanguage==null)
            {
              
                
                //Default Fields
                await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
                {
                    ["Plugins.Payments.Iyzico.Instructions"] = "Iyzico sanal pos entegrasyonunuza ait ayarları düzenleyebilirsiniz.",
                    ["Plugins.Payments.Iyzico.PaymentMethodDescription"] = "Kredi/Banka kartı ile ödeme",
                    ["Plugins.Payments.Iyzico.AccountInfo"] = "Iyzico Api Bilgilerinizi Tanımlayın",
                    ["Plugins.Payments.Iyzico.Fields.ApiKey"] = "Api Key",
                    ["Plugins.Payments.Iyzico.Fields.ApiKey.Hint"] = "Iyzico kontrol panelinizde yer alan Api Key bilginizi girin.",
                    ["Plugins.Payments.Iyzico.Fields.ApiSecret"] = "Api Secret",
                    ["Plugins.Payments.Iyzico.Fields.ApiSecret.Hint"] = "Iyzico kontrol panelinizde yer alan Api Secret bilginizi girin.",
                    ["Plugins.Payments.Iyzico.Fields.ApiUrl"] = "Api Url",
                    ["Plugins.Payments.Iyzico.Fields.ApiUrl.Hint"] = "Iyzico kontrol panelinizde yer alan Api Url bilginizi girin.",
                    ["Plugins.Payments.Iyzico.VirtualPosInfo"] = "Iyzico Ödeme Ayarlarınızı Tanımlayın",
                    ["Plugins.Payments.Iyzico.Fields.IsCardStorage"] = "Kart Bilgilerini Sakla",
                    ["Plugins.Payments.Iyzico.Fields.IsCardStorage.Hint"] = "Bu seçenek, Iyzico tarafından iletilen kredi kartı bilgilerinin ilk altı hanesini ve son dört hanesini veritabanında saklar (herhangi bir üçüncü taraf işlemciye gönderilmez).",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumbers"] = "Taksit Seçenekleri",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumbers.Hint"] = "Iyzico panelinizde taksit seçenekleri aktif ise tarafınıza tanımlı taksit seçeneklerini kullanmak için işaretleyebilirsiniz.",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber2"] = "2 Taksit",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber3"] = "3 Taksit",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber6"] = "6 Taksit",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber9"] = "9 Taksit",
                    ["Plugins.Payments.Iyzico.Fields.InstallmentNumber12"] = "12 Taksit",
                    ["Plugins.Payments.Iyzico.Fields.RedirectionTip"] = "Siparişi tamamlamak için ödeme sistemine yönlendirileceksiniz.",
                    ["Plugins.Payments.Iyzico.Fields.PaymentFailed"] = "Ödeme Başarısız",
                    ["Plugins.Payments.Iyzico.Fields.PaymentErrors"] = "Ödeme Hataları",
                    ["Plugins.Payments.Iyzico.Fields.refundIdTxt"] = "Iyzico Geri Ödeme Id : ",
                    ["Plugins.Payments.Iyzico.Fields.transactionIdTxt"] = "İşlem Id : ",
                    ["Plugins.Payments.Iyzico.Fields.refundAmountTxt"] = " Geri Ödeme Tutarı : ",
                    ["Plugins.Payments.Iyzico.Fields.PaymentFailed.Order"] = "Ödeme başarısız. Sipariş #",
                    ["Plugins.Payments.Iyzico.Fields.Fraoud.Fail"] = "İşleme ait Fraud riski yüksek olduğu için ödeme kabul edilmemiştir. Sipariş #",
                    ["Plugins.Payments.Iyzico.Fields.Fraoud.Review"] = "İşleme ait Fraud riski bulunduğu için ödeme incelemeye alınmıştır. Sipariş #",
                    ["Plugins.Payments.Iyzico.Fields.Order.NotFound"] = "Sipariş Bulunamadı! Sipariş #"
                });
            }
       

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<IyzicoPaymentSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.Iyzico");

            await base.UninstallAsync();
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        /// <remarks>
        /// return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
        /// for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
        /// </remarks>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugins.Payments.Iyzico.PaymentMethodDescription");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => false;

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => true;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => true;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => true;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType { get; set; } = PaymentMethodType.Redirection;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => false;

        #endregion
    }
}