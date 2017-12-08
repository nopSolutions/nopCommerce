using Microsoft.AspNetCore.Mvc.Controllers;
using Nop.Core.Domain.Payments;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Tasks;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Framework.UI;

namespace Nop.Plugin.Payments.Square.Services
{
    /// <summary>
    /// Represents event consumer of the Square payment plugin
    /// </summary>
    public class EventConsumer : 
        IConsumer<PageRenderingEvent>,
        IConsumer<ModelReceived<BaseNopModel>>
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly PaymentSettings _paymentSettings;

        #endregion

        #region Ctor

        public EventConsumer(ILocalizationService localizationService,
            IPaymentService paymentService,
            IScheduleTaskService scheduleTaskService,
            PaymentSettings paymentSettings)
        {
            this._localizationService = localizationService;     
            this._paymentService = paymentService;
            this._scheduleTaskService = scheduleTaskService;
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
                eventMessage.Helper
                    .AddScriptParts(ResourceLocation.Footer, "https://js.squareup.com/v2/paymentform", excludeFromBundle: true);
            }
        }

        /// <summary>
        /// Handle model received event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(ModelReceived<BaseNopModel> eventMessage)
        {
            //whether received model is ScheduleTaskModel
            if (!(eventMessage?.Model is ScheduleTaskModel scheduleTaskModel))
                return;

            //whether renew access token task is changed
            var scheduleTask = _scheduleTaskService.GetTaskById(scheduleTaskModel.Id);
            if (!scheduleTask?.Type.Equals(SquarePaymentDefaults.RenewAccessTokenTask) ?? true)
                return;

            //check whether the plugin is installed
            if (!(_paymentService.LoadPaymentMethodBySystemName(SquarePaymentDefaults.SystemName)?.PluginDescriptor?.Installed ?? false))
                return;

            //check token renewal limit
            var accessTokenRenewalPeriod = scheduleTaskModel.Seconds / 60 / 60 / 24;
            if (accessTokenRenewalPeriod > SquarePaymentDefaults.AccessTokenRenewalPeriodMax)
            {
                var error = string.Format(_localizationService.GetResource("Plugins.Payments.Square.AccessTokenRenewalPeriod.Error"), 
                    SquarePaymentDefaults.AccessTokenRenewalPeriodMax, SquarePaymentDefaults.AccessTokenRenewalPeriodRecommended);
                eventMessage.ModelState.AddModelError(string.Empty, error);
            }
        }

        #endregion
    }
}