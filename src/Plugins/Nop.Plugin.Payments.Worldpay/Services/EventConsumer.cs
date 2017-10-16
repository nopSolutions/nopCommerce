using Nop.Services.Events;
using Nop.Web.Framework.UI;
using Nop.Web.Framework.Events;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Nop.Plugin.Payments.Worldpay.Services
{
    /// <summary>
    /// Represents event consumer of the Worldpay payment plugin
    /// </summary>
    public class EventConsumer : IConsumer<PageRenderingEvent>
    {
        #region Methods

        /// <summary>
        /// Handle page rendering event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(PageRenderingEvent eventMessage)
        {
            if (eventMessage?.Helper?.ViewContext?.ActionDescriptor == null)
                return;

            //add js sсript to one page checkout
            if (eventMessage.Helper.ViewContext.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
                actionDescriptor.ControllerName == "Checkout" && actionDescriptor.ActionName == "OnePageCheckout")
            {
                eventMessage.Helper.AddScriptParts(ResourceLocation.Footer, "https://gwapi.demo.securenet.com/v1/PayOS.js");
            }
        }

        #endregion
    }
}