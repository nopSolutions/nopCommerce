using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.CashOnDelivery.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.CashOnDelivery.Components
{
    [ViewComponent(Name = CashOnDeliveryDefaults.PAYMENT_INFO_VIEW_COMPONENT_NAME)]
    public class PaymentCashOnDeliveryViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PaymentCashOnDeliveryViewComponent(ILocalizationService localizationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            var cashOnDeliveryPaymentSettings = await _settingService.LoadSettingAsync<CashOnDeliveryPaymentSettings>(currentStore.Id);

            var model = new PaymentInfoModel
            {
                DescriptionText = await _localizationService.GetLocalizedSettingAsync(cashOnDeliveryPaymentSettings, x => x.DescriptionText, currentLanguage.Id, 0)
            };

            return View("~/Plugins/Payments.CashOnDelivery/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}
