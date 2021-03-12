using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Http;
using Nop.Services.Common;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class EuCookieLawViewComponent : NopViewComponent
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly StoreInformationSettings _storeInformationSettings;

        public EuCookieLawViewComponent(IGenericAttributeService genericAttributeService,
            IStoreContext storeContext,
            IWorkContext workContext,
            StoreInformationSettings storeInformationSettings)
        {
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _workContext = workContext;
            _storeInformationSettings = storeInformationSettings;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Content("");

            //ignore search engines because some pages could be indexed with the EU cookie as description
            if ((await _workContext.GetCurrentCustomerAsync()).IsSearchEngineAccount())
                return Content("");

            if (await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.EuCookieLawAcceptedAttribute, (await _storeContext.GetCurrentStoreAsync()).Id))
                //already accepted
                return Content("");

            //ignore notification?
            //right now it's used during logout so popup window is not displayed twice
            if (TempData[$"{NopCookieDefaults.Prefix}{NopCookieDefaults.IgnoreEuCookieLawWarning}"] != null && Convert.ToBoolean(TempData[$"{NopCookieDefaults.Prefix}{NopCookieDefaults.IgnoreEuCookieLawWarning}"]))
                return Content("");

            return View();
        }
    }
}