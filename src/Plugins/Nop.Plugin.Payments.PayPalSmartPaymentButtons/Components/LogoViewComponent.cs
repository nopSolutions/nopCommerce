using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Components
{
    /// <summary>
    /// Represents the view component to display logo
    /// </summary>
    [ViewComponent(Name = Defaults.LOGO_VIEW_COMPONENT_NAME)]
    public class LogoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly PayPalSmartPaymentButtonsSettings _settings;

        #endregion

        #region Ctor

        public LogoViewComponent(IPaymentPluginManager paymentPluginManager,
            IStoreContext storeContext,
            IWorkContext workContext,
            PayPalSmartPaymentButtonsSettings settings)
        {
            _paymentPluginManager = paymentPluginManager;
            _storeContext = storeContext;
            _workContext = workContext;
            _settings = settings;
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
            if (!await _paymentPluginManager.IsPluginActiveAsync(Defaults.SystemName, await _workContext.GetCurrentCustomerAsync(), (await _storeContext.GetCurrentStoreAsync()).Id))
                return Content(string.Empty);

            var script = widgetZone.Equals(PublicWidgetZones.HeaderLinksBefore) && _settings.DisplayLogoInHeaderLinks
                ? _settings.LogoInHeaderLinks
                : (widgetZone.Equals(PublicWidgetZones.Footer) && _settings.DisplayLogoInFooter
                ? _settings.LogoInFooter
                : null);

            return new HtmlContentViewComponentResult(new HtmlString(script ?? string.Empty));
        }

        #endregion
    }
}