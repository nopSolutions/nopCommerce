using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
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

        private readonly IWorkContext _workContext;
        private readonly SendinBlueSettings _sendinBlueSettings;

        #endregion

        #region Ctor

        public WidgetsSendinBlueViewComponent(IWorkContext workContext,
            SendinBlueSettings sendinBlueSettings)
        {
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
            var customerEmail = !_workContext.CurrentCustomer.IsGuest()
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