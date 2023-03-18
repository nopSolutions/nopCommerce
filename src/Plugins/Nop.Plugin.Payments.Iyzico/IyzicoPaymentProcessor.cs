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
using LinqToDB.Common;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Plugin.Payments.Iyzico.Validators;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

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

        private readonly IPaymentIyzicoService _paymentIyzicoService;
        private readonly IyzicoPaymentSettings _iyzicoPaymentSettings;


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
            ICategoryService categoryService, ILogger logger,IScheduleTaskService scheduleTaskService)
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

            //if (processPaymentRequest.CreditCardType=="AMEX")
            //{
            //     cart = await _shoppingCartService.GetShoppingCartAsync(customer: customer, ShoppingCartType.ShoppingCart, processPaymentRequest.StoreId);
                

            //   shoppingCartSubTotal = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, true);
            //    shoppingCartTotal = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, true);
            //    shoppingCartUnitPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartSubTotal.subTotalWithDiscount, currency);
            //    shoppingCartUnitPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartTotal.shoppingCartTotal.Value, currency); 
            //}

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
                        enableInstallment = true;
                        request.Locale = Locale.TR.ToString();
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

            //Checkout form için sonuç
            // CheckoutFormInitialize payment = CheckoutFormInitialize.Create(request, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));

              var payment = Payment.Create(request, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));
            

            var result = new ProcessPaymentResult
            {
                AllowStoringCreditCardNumber = _iyzicoPaymentSettings.IsCardStorage
            };

            if (payment.Status == "failure")
            {
                request.CallbackUrl =
                    $"{_webHelper.GetStoreLocation()}PaymentIyzicoPC/PaymentConfirm?orderGuid={processPaymentRequest.OrderGuid}";

                var payment3d = ThreedsInitialize.Create(request, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));

                if (payment3d.Status == "failure")
                {


                    result.AddError(payment.ErrorMessage);
                    result.NewPaymentStatus = PaymentStatus.Pending;
                }
                else
                {
                    //result.AvsResult= payment3d.HtmlContent;
                    result.CaptureTransactionId = payment.ConversationId;
                    result.AuthorizationTransactionId = payment.PaymentId;
                    result.CaptureTransactionResult = payment.Status;
                    result.NewPaymentStatus = PaymentStatus.Authorized;
                    _httpContextAccessor.HttpContext.Session.Remove("PaymentPage");
                    string paymentPage;
                    paymentPage = payment3d.HtmlContent;
                    _httpContextAccessor.HttpContext.Session.SetString("PaymentPage", paymentPage);
                }
            }
            else
            {
                result.CaptureTransactionId = payment.ConversationId;
                result.AuthorizationTransactionId=payment.PaymentId;
                result.CaptureTransactionResult = payment.Status;
                result.NewPaymentStatus= PaymentStatus.Pending;


                _httpContextAccessor.HttpContext.Response.Cookies.Append("CurrentShopCartTemp", JsonConvert.SerializeObject(cart));

                _httpContextAccessor.HttpContext.Session.Remove("PaymentPage");

                string paymentPage;

                //if (_iyzicoPaymentSettings.UseToPaymentPopup)
                //{
                //    paymentPage = payment.CheckoutFormContent;
                //}
                //else
                //{
                //    paymentPage = payment.PaymentPageUrl;
                //}
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

                return createItem(shoppingCartUnitPriceWithDiscount.Result * sci.Quantity,
                    product.Id.ToString(),
                    product.Name,
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
        public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            string paymentPage = _httpContextAccessor.HttpContext.Session.GetString("PaymentPage");

            if (string.IsNullOrEmpty(paymentPage) == false)
            {
                _httpContextAccessor.HttpContext.Session.Remove("PaymentPage");
                _httpContextAccessor.HttpContext.Response.Redirect(paymentPage);
               
            }
            else
            {
                _httpContextAccessor.HttpContext.Response.Redirect($"{_webHelper.GetStoreLocation()}PaymentIyzicoPC/PaymentConfirm?orderGuid={postProcessPaymentRequest.Order.OrderGuid}");
            }

            return Task.CompletedTask;
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
        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            return Task.FromResult(new RefundPaymentResult { Errors = new[] { "Refund method not supported" } });
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void method not supported" } });
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
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Payments.Iyzico.Instructions"] = "Iyzico sanal pos entegrasyonunuza ait ayarları düzenleyebilirsiniz.",
                ["Plugins.Payments.Iyzico.Fields.UseToPaymentPopup"] = "Ödeme Sayfası Popup Görünüm",
                ["Plugins.Payments.Iyzico.Fields.UseToPaymentPopup.Hint"] = "Ödeme sayfasının mevcut pencere içerisinde popup olarak açılmasını sağlar.",
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
                ["Plugins.Payments.Iyzico.Fields.PaymentSuccessUrl"] = "Ödeme Başarılı Url",
                ["Plugins.Payments.Iyzico.Fields.PaymentSuccessUrl.Hint"] = "Iyzico sanal pos ödemesi başarılı bir şekilde gerçekleştirildiğinde yönlendirilecek sayfa (url) bilgisinizi tanımlayabilirsiniz. Varsayılan: /checkout/completed/",
                ["Plugins.Payments.Iyzico.Fields.PaymentErrorUrl"] = "Ödeme Başarısız Url",
                ["Plugins.Payments.Iyzico.Fields.PaymentErrorUrl.Hint"] = "Iyzico sanal pos ödemesi başarısız olduğunda yönlendirilecek sayfa (url) bilgisinizi tanımlayabilirsiniz. Varsayılan: /onepagecheckout/",
                ["Plugins.Payments.Iyzico.Fields.InstallmentNumbers"] = "Taksit Seçenekleri",
                ["Plugins.Payments.Iyzico.Fields.InstallmentNumbers.Hint"] = "Iyzico panelinizde taksit seçenekleri aktif ise tarafınıza tanımlı taksit seçeneklerini kullanmak için işaretleyebilirsiniz.",
                ["Plugins.Payments.Iyzico.Fields.InstallmentNumber2"] = "2 Taksit",
                ["Plugins.Payments.Iyzico.Fields.InstallmentNumber3"] = "3 Taksit",
                ["Plugins.Payments.Iyzico.Fields.InstallmentNumber6"] = "6 Taksit",
                ["Plugins.Payments.Iyzico.Fields.InstallmentNumber9"] = "9 Taksit",
                ["Plugins.Payments.Iyzico.Fields.InstallmentNumber12"] = "12 Taksit",
                ["Plugins.Payments.Iyzico.Fields.RedirectionTip"] = "Siparişi tamamlamak için ödeme sistemine yönlendirileceksiniz.",
                ["Plugins.Payments.Iyzico.Fields.PopupTip"] = "Siparişi tamamlamak için ödeme penceresi açılacaktır."
             
            });

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
        public bool SupportPartiallyRefund => false;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => false;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => false;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType { get; set; } = PaymentMethodType.Standard;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => false;

        #endregion
    }
}