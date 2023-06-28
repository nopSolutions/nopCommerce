using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.Brevo.Components
{
    /// <summary>
    /// Represents view component to embed tracking script on pages
    /// </summary>
    public class WidgetsBrevoViewComponent : NopViewComponent
    {
        #region Fields

        protected readonly BrevoSettings _brevoSettings;
        protected readonly ICustomerService _customerService;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetsBrevoViewComponent(BrevoSettings brevoSettings,
            ICustomerService customerService,
            IWorkContext workContext)
        {
            _brevoSettings = brevoSettings;
            _customerService = customerService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var trackingScript = string.Empty;

            //ensure Marketing Automation is enabled
            if (!_brevoSettings.UseMarketingAutomation)
                return Content(trackingScript);

            //get customer email
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerEmail = !await _customerService.IsGuestAsync(customer)
                ? customer.Email?.Replace("'", "\\'")
                : string.Empty;

            //prepare tracking script
            trackingScript = $"{_brevoSettings.TrackingScript}{Environment.NewLine}"
                .Replace(BrevoDefaults.TrackingScriptId, _brevoSettings.MarketingAutomationKey)
                .Replace(BrevoDefaults.TrackingScriptCustomerEmail, customerEmail);

            return View("~/Plugins/Misc.Brevo/Views/PublicInfo.cshtml", trackingScript);
        }

        #endregion
    }
}