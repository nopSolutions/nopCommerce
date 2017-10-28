using Nop.Services.Events;
using Nop.Web.Framework.UI;
using Nop.Web.Framework.Events;
using Microsoft.AspNetCore.Mvc.Controllers;
using Nop.Core.Domain.Payments;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.Square.Services
{
    /// <summary>
    /// Represents event consumer of the Square payment plugin
    /// </summary>
    public class EventConsumer : IConsumer<PageRenderingEvent>
    {
        #region Fields

        private IPaymentService _paymentService;
        private PaymentSettings _paymentSettings;

        #endregion

        #region Ctor

        public EventConsumer(IPaymentService paymentService,
            PaymentSettings paymentSettings)
        {
            this._paymentService = paymentService;
            this._paymentSettings = paymentSettings;
        }


        #endregion

        #region Methods

        /// <summary>
        /// Handle page rendering event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(PageRenderingEvent eventMessage)
        {
            if (eventMessage?.Helper?.ViewContext?.ActionDescriptor == null)
                return;

            //check whether the plugin is installed and is active
            var squarePaymentMethod = _paymentService.LoadPaymentMethodBySystemName(SquarePaymentDefaults.SystemName);
            if (!(squarePaymentMethod?.PluginDescriptor?.Installed ?? false) || !squarePaymentMethod.IsPaymentMethodActive(_paymentSettings))
                return;

            //add js sсript to one page checkout
            if (eventMessage.Helper.ViewContext.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
                actionDescriptor.ControllerName == "Checkout" && actionDescriptor.ActionName == "OnePageCheckout")
            {
                eventMessage.Helper.AddScriptParts(ResourceLocation.Footer, "https://js.squareup.com/v2/paymentform");
            }
        }

        #endregion
    }
}