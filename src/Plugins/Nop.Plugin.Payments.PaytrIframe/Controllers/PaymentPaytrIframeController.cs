using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.PaytrIframe.Models;
using Nop.Plugin.Payments.PaytrIframe.Paytr;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PaytrIframe.Controllers
{
    public class PaymentPaytrIframeController : BasePaymentController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IAddressService _addressService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICountryService _countryService;
        private readonly IProductService _productService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly PaymentSettings _paymentSettings;

        #endregion

        #region Ctor

        public PaymentPaytrIframeController(IWorkContext workContext, IStoreContext storeContext, ISettingService settingService, IWebHelper webHelper, ILocalizationService localizationService, ILanguageService languageService, ICategoryService categoryService, IOrderService orderService, IOrderProcessingService orderProcessingService, IAddressService addressService, IStateProvinceService stateProvinceService, ICountryService countryService, IProductService productService, ICurrencyService currencyService, IPriceFormatter priceFormatter, IPermissionService permissionService, IPaymentPluginManager paymentPluginManager, INotificationService notificationService, PaymentSettings paymentSettings)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _settingService = settingService;
            _webHelper = webHelper;
            _localizationService = localizationService;
            _languageService = languageService;
            _categoryService = categoryService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _addressService = addressService;
            _stateProvinceService = stateProvinceService;
            _countryService = countryService;
            _productService = productService;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _permissionService = permissionService;
            _paymentPluginManager = paymentPluginManager;
            _notificationService = notificationService;
            _paymentSettings = paymentSettings;
        }

        #endregion

        #region Utilities

        public async Task<List<LanguageOption>> LanguageOptions()
        {
            var languageOption = new List<LanguageOption>();

            if ((await _workContext.GetWorkingLanguageAsync()).LanguageCulture == "tr-TR")
            {
                languageOption.Add(new LanguageOption { Value = 0, Option = "Otomatik" });
                languageOption.Add(new LanguageOption { Value = 1, Option = "Türkçe" });
                languageOption.Add(new LanguageOption { Value = 2, Option = "İngilizce" });
            }
            else
            {
                languageOption.Add(new LanguageOption { Value = 0, Option = "Automatic" });
                languageOption.Add(new LanguageOption { Value = 1, Option = "Turkish" });
                languageOption.Add(new LanguageOption { Value = 2, Option = "English" });
            }

            return languageOption;
        }

        public async Task<List<InstallmentOption>> InstallmentOptions(bool categoryBased = false, bool installmentTable = false)
        {
            var installmentOptions = new List<InstallmentOption>();

            if ((await _workContext.GetWorkingLanguageAsync()).LanguageCulture == "tr-TR")
            {
                installmentOptions.Add(new InstallmentOption { Value = 0, Option = "Tüm Taksit Seçenekleri" });

                if (!installmentTable)
                {
                    installmentOptions.Add(new InstallmentOption { Value = 1, Option = "Tek Çekim" });
                }

                for (int i = 2; i <= 12; i++)
                {
                    installmentOptions.Add(new InstallmentOption { Value = i, Option = i + " Taksit'e Kadar" });
                }

                if (categoryBased)
                {
                    installmentOptions.Add(new InstallmentOption { Value = 13, Option = "KATEGORİ BAZLI" });
                }
            }
            else
            {
                installmentOptions.Add(new InstallmentOption { Value = 0, Option = "All Installment Options" });

                if (!installmentTable)
                {
                    installmentOptions.Add(new InstallmentOption { Value = 1, Option = "One Shot (No Installment)" });
                }

                for (int i = 2; i <= 12; i++)
                {
                    installmentOptions.Add(new InstallmentOption { Value = i, Option = "Up to " + i + " Installments" });
                }

                if (categoryBased)
                {
                    installmentOptions.Add(new InstallmentOption { Value = 13, Option = "CATEGORY BASED" });
                }
            }

            return installmentOptions;
        }

        public async Task<ListDictionary> InstallmentTableAdvanced()
        {
            var list = new ListDictionary();

            if ((await _workContext.GetWorkingLanguageAsync()).LanguageCulture == "tr-TR")
            {
                list.Add(0, "Avantajlı Taksitler");
                list.Add(1, "Tüm Taksitler");
            }
            else
            {
                list.Add(0, "Advantageous Installment");
                list.Add(1, "All Installments");
            }

            return list;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a choosen stroe scope
            int storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            PaytrIframePaymentSettings paytrSettings = await _settingService.LoadSettingAsync<PaytrIframePaymentSettings>(storeScope);

            //installment options
            ViewBag.InstallmentOptions = InstallmentOptions(true);

            //language options
            ViewBag.LanguageOptions = LanguageOptions();

            //widget
            ViewBag.InstallmentTableMaxOptions = InstallmentOptions(false, true);
            ViewBag.InstallmentTableAdvancedOptions = InstallmentTableAdvanced();

            var model = new ConfigurationModel()
            {
                PaymentInfo = paytrSettings.PaymentInfo,
                PaymentMethodDescription = paytrSettings.PaymentMethodDescription,
                MerchantId = paytrSettings.MerchantId,
                MerchantKey = paytrSettings.MerchantKey,
                MerchantSalt = paytrSettings.MerchantSalt,
                Language = paytrSettings.Language,
                Installment = paytrSettings.Installment,
                InstallmentOptions = paytrSettings.InstallmentOptions,
                //widget
                InstallmentTableTitle = paytrSettings.InstallmentTableTitle,
                InstallmentTableToken = paytrSettings.InstallmentTableToken,
                InstallmentTableMax = paytrSettings.InstallmentTableMax,
                InstallmentTableAdvanced = paytrSettings.InstallmentTableAdvanced,
                InstallmentTableTopDescription = paytrSettings.InstallmentTableTopDesc,
                InstallmentTableBottomDescription = paytrSettings.InstallmentTableBottomDesc,
                ActiveStoreScopeConfiguration = storeScope
            };

            //locales
            await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
            {
                locale.PaymentInfo = await _localizationService.GetLocalizedSettingAsync(paytrSettings, x => x.PaymentInfo, languageId, 0, false, false);
                locale.PaymentMethodDescription = await _localizationService.GetLocalizedSettingAsync(paytrSettings, x => x.PaymentMethodDescription, languageId, 0, false, false);
                locale.InstallmentTableTitle = await _localizationService.GetLocalizedSettingAsync(paytrSettings, x => x.InstallmentTableTitle, languageId, 0, false, false);
                locale.InstallmentTableTopDescription = await _localizationService.GetLocalizedSettingAsync(paytrSettings, x => x.InstallmentTableTopDesc, languageId, 0, false, false);
                locale.InstallmentTableBottomDescription = await _localizationService.GetLocalizedSettingAsync(paytrSettings, x => x.InstallmentTableBottomDesc, languageId, 0, false, false);
            });

            if (storeScope > 0)
            {
                model.PaymentInfo_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.PaymentInfo, storeScope);
                model.PaymentMethodDescription_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.PaymentMethodDescription, storeScope);
                model.MerchantId_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.MerchantId, storeScope);
                model.MerchantKey_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.MerchantKey, storeScope);
                model.MerchantSalt_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.MerchantSalt, storeScope);
                model.Language_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.Language, storeScope);
                model.Installment_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.Installment, storeScope);
                model.InstallmentOptions_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.InstallmentOptions, storeScope);
                //widget
                model.InstallmentTableTitle_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.InstallmentTableTitle, storeScope);
                model.InstallmentTableToken_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.InstallmentTableToken, storeScope);
                model.InstallmentTableMax_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.InstallmentTableMax, storeScope);
                model.InstallmentTableAdvanced_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.InstallmentTableAdvanced, storeScope);
                model.InstallmentTableTopDescription_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.InstallmentTableTopDesc, storeScope);
                model.InstallmentTableBottomDescription_OverrideForStore = await _settingService.SettingExistsAsync(paytrSettings, x => x.InstallmentTableBottomDesc, storeScope);
            }

            return View("~/Plugins/Payments.PaytrIframe/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        async public Task<IActionResult> Configure(ConfigurationModel model, IFormCollection collection)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a choosen stroe scope
            int storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            PaytrIframePaymentSettings paytrSettings = await _settingService.LoadSettingAsync<PaytrIframePaymentSettings>(storeScope);

            //save settings
            paytrSettings.PaymentInfo = model.PaymentInfo;
            paytrSettings.PaymentMethodDescription = model.PaymentMethodDescription;
            paytrSettings.MerchantId = model.MerchantId;
            paytrSettings.MerchantKey = model.MerchantKey;
            paytrSettings.MerchantSalt = model.MerchantSalt;
            paytrSettings.Language = model.Language;
            paytrSettings.Installment = model.Installment;
            //widget
            paytrSettings.InstallmentTableTitle = model.InstallmentTableTitle;
            paytrSettings.InstallmentTableToken = model.InstallmentTableToken;
            paytrSettings.InstallmentTableMax = model.InstallmentTableMax;
            paytrSettings.InstallmentTableAdvanced = model.InstallmentTableAdvanced;
            paytrSettings.InstallmentTableTopDesc = model.InstallmentTableTopDescription;
            paytrSettings.InstallmentTableBottomDesc = model.InstallmentTableBottomDescription;

            if (model.Installment == 13)
            {
                var list = new Dictionary<string, string>();
                foreach (string item in collection.Keys)
                {
                    if (item.IndexOf("CategoryInstallment") != -1)
                    {
                        int cIF = item.IndexOf("[") + 1;
                        int cIS = item.IndexOf("]");
                        string cI = item.Substring(cIF, cIS - cIF).ToString();
                        list.Add(cI, collection[item]);
                    }
                }
                paytrSettings.InstallmentOptions = JsonConvert.SerializeObject(list);
            }

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update 
             */
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.PaymentInfo, model.PaymentInfo_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.PaymentMethodDescription, model.PaymentMethodDescription_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.MerchantId, model.MerchantId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.MerchantKey, model.MerchantKey_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.MerchantSalt, model.MerchantSalt_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.Language, model.Language_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.Installment, model.Installment_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.InstallmentOptions, model.InstallmentOptions_OverrideForStore, storeScope, false);
            //widget
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.InstallmentTableTitle, model.InstallmentTableTitle_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.InstallmentTableToken, model.InstallmentTableToken_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.InstallmentTableMax, model.InstallmentTableMax_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.InstallmentTableAdvanced, model.InstallmentTableAdvanced_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.InstallmentTableTopDesc, model.InstallmentTableTopDescription_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(paytrSettings, x => x.InstallmentTableBottomDesc, model.InstallmentTableBottomDescription_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //localization. no multi-store support for localization yet.
            foreach (var localized in model.Locales)
            {
                await _localizationService.SaveLocalizedSettingAsync(paytrSettings, x => x.PaymentInfo,
                     localized.LanguageId,
                     localized.PaymentInfo);
                await _localizationService.SaveLocalizedSettingAsync(paytrSettings, x => x.PaymentMethodDescription,
                    localized.LanguageId,
                    localized.PaymentMethodDescription);
                await _localizationService.SaveLocalizedSettingAsync(paytrSettings, x => x.InstallmentTableTitle,
                    localized.LanguageId,
                    localized.InstallmentTableTitle);
                await _localizationService.SaveLocalizedSettingAsync(paytrSettings, x => x.InstallmentTableTopDesc,
                    localized.LanguageId,
                    localized.InstallmentTableTopDescription);
                await _localizationService.SaveLocalizedSettingAsync(paytrSettings, x => x.InstallmentTableBottomDesc,
                    localized.LanguageId,
                    localized.InstallmentTableBottomDescription);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        public async Task<IActionResult> PaytrPluginPayment(int? orderId)
        {
            if (orderId == null)
                return RedirectToAction("Index", "Home", new { area = "" });

            var order = await _orderService.GetOrderByIdAsync((int)orderId);

            if (order == null)
                return RedirectToAction("Index", "Home", new { area = "" });

            if ((await _workContext.GetCurrentCustomerAsync()).Id != order.CustomerId)
                return RedirectToAction("Index", "Home", new { area = "" });

            var model = new PaytrPayment();

            if (order.PaymentStatus == PaymentStatus.Paid || order.OrderStatus != OrderStatus.Pending)
            {
                model.status = false;
                model.message = await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.PaymentPage.ErrorAvailable", (await _workContext.GetWorkingLanguageAsync()).Id);
                return View("~/Plugins/Payments.PaytrIframe/Views/Payment.cshtml", model);
            }

            //load settings
            int storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            PaytrIframePaymentSettings paytrPaymentSettings = await _settingService.LoadSettingAsync<PaytrIframePaymentSettings>(storeScope);

            NameValueCollection data = new();

            //merchant
            data["merchant_id"] = paytrPaymentSettings.MerchantId;
            string merchantKey = paytrPaymentSettings.MerchantKey;
            string merchantSalt = paytrPaymentSettings.MerchantSalt;

            //customer
            Address billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
            data["email"] = billingAddress.Email;
            data["user_name"] = string.Format("{0} {1}", billingAddress.FirstName, billingAddress.LastName);
            data["user_address"] = billingAddress.Address1 + " " + billingAddress.Address2 + " " + billingAddress.ZipPostalCode + " " + billingAddress.City + " ";

            if (billingAddress.StateProvinceId != null)
                data["user_address"] += (await _stateProvinceService.GetStateProvinceByIdAsync((int)billingAddress.StateProvinceId)).Name + " ";

            if (billingAddress.CountryId != null)
                data["user_address"] += (await _countryService.GetCountryByIdAsync((int)billingAddress.CountryId)).Name;

            data["user_phone"] = billingAddress.PhoneNumber;
            data["user_ip"] = order.CustomerIp;

            //basket && installment
            List<object> basketList = new List<object>();
            IList<OrderItem> cartItems = await _orderService.GetOrderItemsAsync(order.Id);

            data["max_installment"] = "0";

            if (paytrPaymentSettings.Installment != 13)
            {
                foreach (OrderItem item in cartItems)
                {
                    Product product = await _productService.GetProductByIdAsync(item.ProductId);
                    decimal calcProductPriceExchange = _currencyService.ConvertCurrency(item.UnitPriceInclTax, order.CurrencyRate);
                    object[] b = new object[] { product.Name, Math.Round(calcProductPriceExchange, 2).ToString(), item.Quantity };
                    basketList.Add(b);
                }

                data["max_installment"] = Enumerable.Range(0, 12).Contains(paytrPaymentSettings.Installment) ? paytrPaymentSettings.Installment.ToString() : "0";
            }
            else
            {
                Dictionary<int, int> installmentOptions = JsonConvert.DeserializeObject<Dictionary<int, int>>(paytrPaymentSettings.InstallmentOptions);
                Dictionary<int, int> installment = new Dictionary<int, int>();

                foreach (OrderItem item in cartItems)
                {
                    Product product = await _productService.GetProductByIdAsync(item.ProductId);
                    decimal calcProductPriceExchange = _currencyService.ConvertCurrency(item.UnitPriceInclTax, order.CurrencyRate);
                    object[] b = new object[] { product.Name, Math.Round(calcProductPriceExchange, 2).ToString(), item.Quantity };
                    basketList.Add(b);
                    IList<ProductCategory> categories = await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId);

                    foreach (ProductCategory cat in categories)
                    {
                        if (installmentOptions.ContainsKey(cat.CategoryId))
                        {
                            installment[cat.CategoryId] = installmentOptions[cat.CategoryId];
                        }
                    }
                }

                KeyValuePair<int, int> minValueExceptZero = installment.OrderBy(kvp => kvp.Value).Where(kvp => kvp.Value != 0).FirstOrDefault();
                data["max_installment"] = minValueExceptZero.Value.ToString();
            }

            data["no_installment"] = data["max_installment"] == "1" ? "1" : "0";

            string basketJson = JsonConvert.SerializeObject(basketList);
            data["user_basket"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(basketJson));

            //order
            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            data["merchant_oid"] = unixTimestamp + "PAYTRNOP" + order.Id.ToString();
            decimal orderTotalExchange = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            string orderTotal = (Math.Round(orderTotalExchange, 2) * 100).ToString("#", CultureInfo.InvariantCulture);
            data["payment_amount"] = orderTotal.ToString();
            data["currency"] = order.CustomerCurrencyCode;

            data["merchant_ok_url"] = _webHelper.GetStoreLocation().Substring(0, _webHelper.GetStoreLocation().Length - 1) + Url.RouteUrl("CheckoutCompleted", new { orderId = order.Id });
            data["merchant_fail_url"] = _webHelper.GetStoreLocation() + "Plugins/PaymentPaytrIframe/CancelOrder";

            //language
            data["lang"] = "tr";
            if (paytrPaymentSettings.Language == 0)
            {
                if ((await _workContext.GetWorkingLanguageAsync()).LanguageCulture == "tr-TR")
                {
                    data["lang"] = "tr";
                }
                else
                {
                    data["lang"] = "en";
                }
            }
            else if (paytrPaymentSettings.Language == 2)
            {
                data["lang"] = "en";
            }

            //debug & test
            data["debug_mode"] = "1";
            data["test_mode"] = "0";

            //get paytr_token
            string hash = string.Concat(data["merchant_id"], data["user_ip"], data["merchant_oid"], data["email"], data["payment_amount"], data["user_basket"], data["no_installment"], data["max_installment"], data["currency"], data["test_mode"], merchantSalt);
            HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(merchantKey));
            byte[] byteHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));
            data["paytr_token"] = Convert.ToBase64String(byteHash);

            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");

            HttpContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");


            HttpResponseMessage result = await client.PostAsync("https://www.paytr.com/odeme/api/get-token", content);

            if (result.IsSuccessStatusCode==false)
            {
                model.status = false;
                model.message = $"PAYTR IFRAME failed. reason:{result.StatusCode} {result.ReasonPhrase}";
            }

            string resultData =await result.Content.ReadAsStringAsync();

                dynamic json = JValue.Parse(resultData);

                if (json.status == "success")
                {
                    model.status = true;
                    model.token = json.token;
                }
                else
                {
                    model.status = false;
                    model.message = "PAYTR IFRAME failed. reason: " + json.reason;
                }
            

            return View("~/Plugins/Payments.PaytrIframe/Views/Payment.cshtml", model);
        }

        [HttpPost]
        async public Task<IActionResult> Callback(IFormCollection form)
        {
            var processor = await _paymentPluginManager.LoadPluginBySystemNameAsync("Payments.PaytrIframe") as PaytrIframePaymentProcessor;

            if (processor == null || !_paymentPluginManager.IsPluginActive(processor))
                throw new NopException("PaytrIframe module cannot be loaded");

            var logList = new Dictionary<string, string>();
            foreach (string key in form.Keys)
            {
                logList.Add(key, form[key]);
            }

            //load settings for a choosen stroe scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paytrIframeSettings = await _settingService.LoadSettingAsync<PaytrIframePaymentSettings>(storeScope);

            //merchant
            string merchantKey = paytrIframeSettings.MerchantKey;
            string merchantSalt = paytrIframeSettings.MerchantSalt;

            string getHash = form["hash"];
            string getMerchantOID = form["merchant_oid"];
            string getStatus = form["status"];
            string getTotalAmount = form["total_amount"];

            string strConcat = string.Concat(getMerchantOID, merchantSalt, getStatus, getTotalAmount);
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchantKey));
            byte[] byteHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(strConcat));
            string genHash = Convert.ToBase64String(byteHash);

            if (getHash != genHash)
                return Content("PAYTR notification failed: bad hash");

            string getPaymentAmount = form["payment_amount"];
            string getCurrency = form["currency"] == "TL" ? "TRY" : form["currency"];
            string getInstallmentCount = form["installment_count"];

            var spOrderId = getMerchantOID.Split(new string[] { "PAYTRNOP" }, StringSplitOptions.None);
            var orderId = Convert.ToInt32(spOrderId[1]);

            //get order
            Order order = await _orderService.GetOrderByIdAsync(orderId);

            var orderNote = new StringBuilder();

            if (order.OrderStatus == OrderStatus.Pending && order.PaymentStatus == PaymentStatus.Pending)
            {
                var currentLangId = (await _workContext.GetWorkingLanguageAsync()).Id;

                if (getStatus.Equals("success"))
                {
                    // get currency
                    var currency = await _currencyService.GetCurrencyByCodeAsync(getCurrency);
                    var totalAmount = Math.Round(Convert.ToDecimal(getTotalAmount) / 100, 2);
                    var paymentAmount = Math.Round(Convert.ToDecimal(getPaymentAmount) / 100, 2);
                    var installmentDif = Math.Round((totalAmount - paymentAmount), 2);
                    var installmentCount = Convert.ToInt32(getInstallmentCount) > 1 ? getInstallmentCount : await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.OneShot", currentLangId);

                    orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleNotification", currentLangId) + " - " + await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleSuccess", currentLangId));
                    orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TotalPaid", currentLangId) + ": " + await _priceFormatter.FormatPriceAsync(totalAmount, true, currency.CurrencyCode, false, currentLangId));
                    orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.Paid", currentLangId) + ": " + await _priceFormatter.FormatPriceAsync(paymentAmount, true, currency.CurrencyCode, false, currentLangId));

                    if (Convert.ToInt32(getInstallmentCount) > 1)
                    {
                        orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.InstallmentDiff", currentLangId) + ": " + await _priceFormatter.FormatPriceAsync(installmentDif, true, currency.CurrencyCode, false, currentLangId));
                    }

                    orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.InstallmentCount", currentLangId) + ": " + installmentCount);
                    orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.PaytrOrderId", currentLangId) + ": " + getMerchantOID);

                    await _orderService.InsertOrderNoteAsync(new OrderNote
                    {
                        OrderId = order.Id,
                        Note = orderNote.ToString(),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    await _orderService.UpdateOrderAsync(order);

                    //mark order as paid
                    if (_orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        order.AuthorizationTransactionId = getMerchantOID;
                        await _orderService.UpdateOrderAsync(order);

                        await _orderProcessingService.MarkOrderAsPaidAsync(order);
                    }
                }
                else
                {
                    orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleNotification", currentLangId) + " - " + await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleFail", currentLangId));
                    orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.Error", currentLangId) + ": " + form["failed_reason_code"] + " - " + form["failed_reason_msg"]);
                    orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.PaytrOrderId", currentLangId) + ": " + getMerchantOID);

                    await _orderService.InsertOrderNoteAsync(new OrderNote
                    {
                        OrderId = order.Id,
                        Note = orderNote.ToString(),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    await _orderService.UpdateOrderAsync(order);

                    //mark order as canceled
                    if (_orderProcessingService.CanCancelOrder(order))
                    {
                        order.AuthorizationTransactionId = getMerchantOID;
                        await _orderService.UpdateOrderAsync(order);

                        await _orderProcessingService.CancelOrderAsync(order, false);
                    }
                }
            }

            return Content("OK");
        }

        public async Task<JsonResult> CategoryOptions()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var categoryInstallment = new CategoryInstallmentOption();
            var categories = new List<CategoryOption>();
            var getAllCategories = await _categoryService.GetAllCategoriesAsync();

            foreach (var item in getAllCategories)
            {
                var category = new CategoryOption
                {
                    Id = item.Id,
                    Name = item.Name
                };
                categories.Add(category);
            }

            categoryInstallment.CategoryOptions = categories;
            categoryInstallment.CategoryInstallment = (await _settingService.LoadSettingAsync<PaytrIframePaymentSettings>(storeScope)).InstallmentOptions;
            categoryInstallment.InstallmentOptions = await InstallmentOptions();

            return Json(categoryInstallment);
        }

        #endregion
    }
}