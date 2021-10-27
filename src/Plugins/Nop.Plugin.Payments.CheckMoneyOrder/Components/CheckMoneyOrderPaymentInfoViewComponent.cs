using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.CheckMoneyOrder.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.CheckMoneyOrder.Components
{
    [ViewComponent(Name = "CheckMoneyOrder")]
    public class CheckMoneyOrderViewComponent : NopViewComponent
    {
        private readonly CheckMoneyOrderPaymentSettings _checkMoneyOrderPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public CheckMoneyOrderViewComponent(CheckMoneyOrderPaymentSettings checkMoneyOrderPaymentSettings,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _checkMoneyOrderPaymentSettings = checkMoneyOrderPaymentSettings;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            var model = new PaymentInfoModel
            {
                DescriptionText = await _localizationService.GetLocalizedSettingAsync(_checkMoneyOrderPaymentSettings,
                    x => x.DescriptionText, (await _workContext.GetWorkingLanguageAsync()).Id, store.Id)
            };

            return View("~/Plugins/Payments.CheckMoneyOrder/Views/PaymentInfo.cshtml", model);
        }
    }
}