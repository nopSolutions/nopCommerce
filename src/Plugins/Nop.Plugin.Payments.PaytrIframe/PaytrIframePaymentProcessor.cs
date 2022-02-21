using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PaytrIframe
{
    public class PaytrIframePaymentProcessor : BasePlugin, IPaymentMethod, IWidgetPlugin
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderService _orderService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly IPaymentService _paymentService;
        private readonly PaytrIframePaymentSettings _paytrIframePaymentSettings;

        #endregion

        #region Ctor

        public PaytrIframePaymentProcessor(ICurrencyService currencyService, IPriceFormatter priceFormater, IOrderService orderService, ILocalizationService localizationService, ISettingService settingService, IStoreContext storeContext, IWorkContext workContext, IHttpContextAccessor httpContextAccessor, IWebHelper webHelper, IPaymentService paymentService, PaytrIframePaymentSettings paytrIframePaymentSettings)
        {
            _currencyService = currencyService;
            _priceFormatter = priceFormater;
            _orderService = orderService;
            _localizationService = localizationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
            _httpContextAccessor = httpContextAccessor;
            _webHelper = webHelper;
            _paymentService = paymentService;
            _paytrIframePaymentSettings = paytrIframePaymentSettings;
        }

        #endregion

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => true;

        public bool SupportRefund => true;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => false;

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            var paytrIframeSettings = await _settingService.LoadSettingAsync<PaytrIframePaymentSettings>((await _storeContext.GetCurrentStoreAsync()).Id);
            return await _localizationService.GetLocalizedSettingAsync(_paytrIframePaymentSettings, x => x.PaymentMethodDescription, (await _workContext.GetWorkingLanguageAsync()).Id, (await _storeContext.GetCurrentStoreAsync()).Id);
        }

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
        }

        public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return await Task.FromResult(0);
            //return await _paymentService.CalculateAdditionalFeeAsync(cart, 0, false);
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return Task.FromResult(new ProcessPaymentRequest());
        }

        public string GetPublicViewComponentName()
        {
            return "PaymentPaytrIframe";
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var storeUrl = (await _storeContext.GetCurrentStoreAsync()).Url;

            string url = _webHelper.GetStoreLocation() + "plugin/payment/" + postProcessPaymentRequest.Order.Id.ToString();

            _httpContextAccessor.HttpContext.Response.Redirect(url);
        }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult());
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();

            //load settings
            var paytrIframeSettings = await _settingService.LoadSettingAsync<PaytrIframePaymentSettings>((await _storeContext.GetCurrentStoreAsync()).Id);

            string merchantId = paytrIframeSettings.MerchantId;
            string merchantKey = paytrIframeSettings.MerchantKey;
            string merchantSalt = paytrIframeSettings.MerchantSalt;

            //get order
            var order = refundPaymentRequest.Order;
            var merchantOID = order.AuthorizationTransactionId;
            decimal returnAmount = refundPaymentRequest.AmountToRefund;
            decimal calcReturnAmountExchange = _currencyService.ConvertCurrency(returnAmount, order.CurrencyRate);
            var finalReturnAmount = Math.Round(calcReturnAmountExchange, 2).ToString().Replace(",", ".");

            NameValueCollection data = new NameValueCollection();
            data["merchant_id"] = merchantId;
            data["merchant_oid"] = merchantOID;
            data["return_amount"] = finalReturnAmount.ToString();

            string strHash = string.Concat(merchantId, merchantOID, finalReturnAmount, merchantSalt);
            HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(merchantKey));
            byte[] byteHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(strHash));
            data["paytr_token"] = Convert.ToBase64String(byteHash);

            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");

            HttpContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var returningResult = await client.PostAsync("https://www.paytr.com/odeme/iade", content);


            if (returningResult.IsSuccessStatusCode == false)
            {
                result.AddError($"PAYTR Refund Error: {returningResult.StatusCode} - {returningResult.RequestMessage}");
                return result;
            }

            var returningData = await returningResult.Content.ReadAsStringAsync();

            dynamic json = JValue.Parse(returningData);

            if (json.status == "success")
            {
                var orderNote = new StringBuilder();
                var currentLangId = (await _workContext.GetWorkingLanguageAsync()).Id;

                orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleNotification", currentLangId) + " - " + await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleRefunded", currentLangId));
                orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.Refund", currentLangId) + ": " + json.return_amount);
                orderNote.AppendLine(await _localizationService.GetResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.PaytrOrderId", currentLangId) + ": " + json.merchant_oid);

                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = orderNote.ToString(),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                return new RefundPaymentResult
                {
                    NewPaymentStatus = refundPaymentRequest.IsPartialRefund ? PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded
                };
            }
            else
            {
                result.AddError(string.Format("PAYTR Refund Error: {0} - {1}", json.err_no, json.err_msg));
                return result;
            }
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void method not supported" } });
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentPaytrIframe/Configure";
        }

        public override async Task InstallAsync()
        {
            //default settings
            await _settingService.SaveSettingAsync(new PaytrIframePaymentSettings
            {
                PaymentInfo = "Ödemeyi tamamlamak için bir sonraki sayfaya yönlendirileceksiniz.",
                PaymentMethodDescription = "Bu ödeme yöntemini seçtiğinizde Tüm Kredi Kartlarına taksit imkanı bulunmaktadır.",
                Language = 0,
                Installment = 0,
                InstallmentTableTitle = "Taksit Seçenekleri",
                InstallmentTableMax = 0,
                InstallmentTableAdvanced = 0
            });

            #region LocaleFieldsEN
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.CallbackInstruction", "<p>You must add the following callback url <strong>" + _webHelper.GetStoreLocation() + "Plugins/PaymentPaytrIframe/Callback</strong> to your <a href=\"https://www.paytr.com/magaza/ayarlar\" target=\"_blank\">Callback URL Settings</a>.</p>");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.StoreInformation", "Store information can be found at <a href=\"https://www.paytr.com/magaza/bilgi\" target=\"_blank\">this address</a>.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Tab.iFrame", "iFrame");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Tab.Table", "Installment Table");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.PaymentInfo", "Payment Info");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.PaymentInfo.Hint", "Enter the text to appear in the payment info field.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.PaymentMethodDescription", "Description");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.PaymentMethodDescription.Hint", "Enter the text that will appear under the payment method.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.MerchantId", "Merchant ID");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.MerchantKey", "Merchant Key");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.MerchantSalt", "Merchant Salt");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.Language", "Language");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.Installment", "Installment Options");
            #endregion

            #region LocaleOrderNotesEN
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleNotification", "PAYTR NOTIFICATION");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleSuccess", "Payment Accepted");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleFail", "Payment Failed");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleRefunded", "Payment Refunded");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TotalPaid", "Total Paid");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.Paid", "Paid");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.Refund", "Refunded Amount");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.InstallmentDiff", "Installment Difference");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.InstallmentCount", "Installment Count");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.PaytrOrderId", "PayTR Order ID");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.OneShot", "One Shot");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.Error", "Error");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.PaymentPage.ErrorAvailable", "This page has expired. Please, create a new order.");
            #endregion

            #region WidgetEN
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.InstallmentTokenInformation", "Token can be found at <a href=\"https://www.paytr.com/magaza/pft-ayar\" target =\"_blank\">this address</a>.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTitle", "Title");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTitle.Hint", "Tab title appearing on the product detail page.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableToken", "Token");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableMax", "Max Installment");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableMax.Hint", "You can choose the maximum number of installments you want to show.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableAdvanced", "Advantageous Installment");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTopDesc", "Top Description");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTopDesc.Hint", "Add content above of the installment table.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableBottomDesc", "Bottom Description");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableBottomDesc.Hint", "Add content under the installment table.");
            #endregion

            #region LocaleFieldTR
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.CallbackInstruction", "<strong>" + _webHelper.GetStoreLocation() + "Plugins/PaymentPaytrIframe/Callback</strong> adresini <a href=\"https://www.paytr.com/magaza/ayarlar\" target=\"_blank\">Bildirim URL</a> ayarı içerisine ekleyin.", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.StoreInformation", "Mağaza API bilgilerini <a href=\"https://www.paytr.com/magaza/bilgi\" target=\"_blank\">bu adreste</a> bulabilirsiniz.", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Tab.Table", "Taksit Tablosu", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.PaymentInfo", "Bilgi", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.PaymentInfo.Hint", "Ödeme yöntemi bilgi bölümünde görünecek metni girin.", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.PaymentMethodDescription", "Açıklama", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.PaymentMethodDescription.Hint", "Ödeme yöntemi açıklama alanında görünecek metni girin.", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.MerchantId", "Mağaza No", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.MerchantKey", "Mağaza Parola", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.MerchantSalt", "Mağaza Gizli Anahtar", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.Language", "Dil", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.Installment", "Taksit Seçeneği", "tr-TR");
            #endregion

            #region LocaleOrderNotesTR
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleNotification", "PAYTR BİLDİRİMİ", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleSuccess", "Ödeme Kabul Edildi", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleFail", "Ödeme Başarısız", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TitleRefunded", "Ödeme İade Edildi", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.TotalPaid", "Toplam Ödenen", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.Paid", "Ödenen", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.Refund", "İade Tutarı", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.InstallmentDiff", "Vade Farkı", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.InstallmentCount", "Taksit Sayısı", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.PaytrOrderId", "PayTR Sipariş Numarası", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.OneShot", "Tek Çekim", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.OrderNote.Error", "Hata", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.PaymentPage.ErrorAvailable", "Bu ödeme sayfasının süresi doldu. Lütfen yeni bir sipariş oluşturun.", "tr-TR");
            #endregion

            #region WidgetTR
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.InstallmentTokenInformation", "Token bilgisini <a href=\"https://www.paytr.com/magaza/pft-ayar\" target =\"_blank\">bu adreste</a> bulabilirsiniz.", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTitle", "Başlık", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTitle.Hint", "Ürün detay sayfasında görünen sekme başlığı.", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableToken", "Token", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableMax", "Maksimum Taksit Sayısı", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableMax.Hint", "Göstermek istediğiniz maksimum taksit sayısını seçebilirsiniz.", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableAdvanced", "Avantajlı Taksit", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTopDesc", "Üst Açıklama", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTopDesc.Hint", "Taksit tablosunun üstüne açıklama ekleyin.", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableBottomDesc", "Alt Açıklama", "tr-TR");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.PaytrIframe.Fields.InstallmentTableBottomDesc.Hint", "Taksit tablosunun altına açıklama ekleyin.", "tr-TR");
            #endregion

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<PaytrIframePaymentSettings>();

            //translate
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.PaytrIframe");

            await base.UninstallAsync();
        }

        //widget
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.ProductDetailsBeforeCollateral });
        }

        //widget
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "PaymentPaytrIframeWidget";
        }

        //widget
        public bool HideInWidgetList => false;
    }
}
