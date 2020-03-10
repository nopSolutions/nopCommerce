using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Components
{
    /// <summary>
    /// Represents the view component to display logo and buttons
    /// </summary>
    [ViewComponent(Name = Defaults.BUTTONS_VIEW_COMPONENT_NAME)]
    public class LogoAndButtonsViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly PayPalSmartPaymentButtonsSettings _settings;

        #endregion

        #region Ctor

        public LogoAndButtonsViewComponent(IPaymentPluginManager paymentPluginManager,
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
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_paymentPluginManager.IsPluginActive(Defaults.SystemName, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id))
                return Content(string.Empty);

            if (!_settings.ButtonsWidgetZones.Contains(widgetZone))
                return Content(string.Empty);

            if (widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo) && additionalData is ProductDetailsModel.AddToCartModel model)
                return View("~/Plugins/Payments.PayPalSmartPaymentButtons/Views/Buttons.cshtml", (widgetZone, model.ProductId));

            if (widgetZone.Equals(PublicWidgetZones.OrderSummaryContentAfter) &&
                HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>() is RouteNameMetadata routeNameMetadata &&
                routeNameMetadata.RouteName.Equals(Defaults.ShoppingCartRouteName))
            {
                return View("~/Plugins/Payments.PayPalSmartPaymentButtons/Views/Buttons.cshtml", (widgetZone, 0));
            }

            if (widgetZone.Equals(PublicWidgetZones.HeaderLinksBefore) ||
                widgetZone.Equals(PublicWidgetZones.Footer))
            {
                return View("~/Plugins/Payments.PayPalSmartPaymentButtons/Views/Logo.cshtml", widgetZone);
            }

            return Content(string.Empty);
        }

        #endregion
    }
}