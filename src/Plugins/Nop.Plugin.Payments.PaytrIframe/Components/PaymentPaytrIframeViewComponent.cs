using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.PaytrIframe.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PaytrIframe.Components
{
    [ViewComponent(Name = "PaymentPaytrIframe")]
    public class PaymentPaytrIframeViewComponent : NopViewComponent
    {
        #region Fields

        private readonly PaytrIframePaymentSettings _paytrIframePaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PaymentPaytrIframeViewComponent(PaytrIframePaymentSettings paytrIframePaymentSettings, ILocalizationService localizationService, IStoreContext storeContext, IWorkContext workContext)
        {
            _paytrIframePaymentSettings = paytrIframePaymentSettings;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new PaymentInfoModel
            {
                PaymentInfo = await _localizationService.GetLocalizedSettingAsync(_paytrIframePaymentSettings, x => x.PaymentInfo, (await
        _workContext.GetWorkingLanguageAsync()).Id, (await _storeContext.GetCurrentStoreAsync()).Id)
            };

            return View("~/Plugins/Payments.PaytrIframe/Views/PaymentInfo.cshtml", model);
        }
    }
}
