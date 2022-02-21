using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.PaytrIframe.Paytr;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PaytrIframe.Components
{
    [ViewComponent(Name = "PaymentPaytrIframeWidget")]
    public class PaymentPaytrIframeWidgetViewComponent : NopViewComponent
    {
        #region Fields

        private readonly PaytrIframePaymentSettings _paytrIframePaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IProductService _productService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public PaymentPaytrIframeWidgetViewComponent(PaytrIframePaymentSettings paytrIframePaymentSettings, ILocalizationService localizationService, IStoreContext storeContext, IWorkContext workContext, ISettingService settingService, IProductService productService, IPriceCalculationService priceCalculationService, ICustomerService customerService)
        {
            _paytrIframePaymentSettings = paytrIframePaymentSettings;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
            _settingService = settingService;
            _productService = productService;
            _priceCalculationService = priceCalculationService;
            _customerService = customerService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            dynamic adData = additionalData;

            //load settings for a choosen stroe scope
            var paytrSettings = await _settingService.LoadSettingAsync<PaytrIframePaymentSettings>((await _storeContext.GetCurrentStoreAsync()).Id);
            var customer = await _customerService.GetCustomerByIdAsync((await _workContext.GetCurrentCustomerAsync()).Id);

            //get product
            var ProductId = (int)additionalData.GetType().GetProperty("Id").GetValue(additionalData, null);
            var getProduct = await _productService.GetProductByIdAsync(ProductId);
            bool showTable = true;
            var amount = Math.Round((decimal)adData.ProductPrice.PriceValue, 2);

            if (getProduct.CustomerEntersPrice || getProduct.CallForPrice || getProduct.IsRental || adData.ProductPrice.CurrencyCode != "TRY")
            {
                showTable = false;
            }

            var model = new InstallmentTable()
            {
                Title = await _localizationService.GetLocalizedSettingAsync(paytrSettings, x => x.InstallmentTableTitle, (await _workContext.GetWorkingLanguageAsync()).Id, (await _storeContext.GetCurrentStoreAsync()).Id),
                Token = _paytrIframePaymentSettings.InstallmentTableToken,
                MerchantId = _paytrIframePaymentSettings.MerchantId,
                MaxInstallment = _paytrIframePaymentSettings.InstallmentTableMax,
                AdvancedInstallment = _paytrIframePaymentSettings.InstallmentTableAdvanced,
                Amount = amount,
                ShowTable = showTable,
                TopDescription = await _localizationService.GetLocalizedSettingAsync(paytrSettings, x => x.InstallmentTableTopDesc, (await _workContext.GetWorkingLanguageAsync()).Id, (await _storeContext.GetCurrentStoreAsync()).Id),
                BottomDescription = await _localizationService.GetLocalizedSettingAsync(paytrSettings, x => x.InstallmentTableBottomDesc, (await _workContext.GetWorkingLanguageAsync()).Id, (await _storeContext.GetCurrentStoreAsync()).Id)
            };

            return View("~/Plugins/Payments.PaytrIframe/Views/Table.cshtml", model);
        }
    }
}
