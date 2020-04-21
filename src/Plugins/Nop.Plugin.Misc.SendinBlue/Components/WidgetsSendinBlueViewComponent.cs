using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.SendinBlue.Components
{
    /// <summary>
    /// Represents view component to embed tracking script on pages
    /// </summary>
    [ViewComponent(Name = SendinBlueDefaults.TRACKING_VIEW_COMPONENT_NAME)]
    public class WidgetsSendinBlueViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly SendinBlueSettings _sendinBlueSettings;

        #endregion

        #region Ctor

        public WidgetsSendinBlueViewComponent(ICustomerService customerService,
            IWorkContext workContext,
            SendinBlueSettings sendinBlueSettings)
        {
            _customerService = customerService;
            _workContext = workContext;
            _sendinBlueSettings = sendinBlueSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var trackingScript = string.Empty;

            //ensure Marketing Automation is enabled
            if (!_sendinBlueSettings.UseMarketingAutomation)
                return Content(trackingScript);

            //get customer email
            var customerEmail = !_customerService.IsGuest(_workContext.CurrentCustomer)
                ? _workContext.CurrentCustomer.Email?.Replace("'", "\\'")
                : string.Empty;

            //prepare tracking script
            trackingScript = $"{_sendinBlueSettings.TrackingScript}{Environment.NewLine}"
                .Replace(SendinBlueDefaults.TrackingScriptId, _sendinBlueSettings.MarketingAutomationKey)
                .Replace(SendinBlueDefaults.TrackingScriptCustomerEmail, customerEmail);

            return View("~/Plugins/Misc.SendinBlue/Views/PublicInfo.cshtml", trackingScript);
        }

        #endregion
    }
}