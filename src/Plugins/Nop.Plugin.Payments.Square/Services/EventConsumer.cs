using System.Linq;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Tasks;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.UI;

namespace Nop.Plugin.Payments.Square.Services
{
    /// <summary>
    /// Represents event consumer of the Square payment plugin
    /// </summary>
    public class EventConsumer :
        IConsumer<PageRenderingEvent>,
        IConsumer<ModelReceivedEvent<BaseNopModel>>
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly IScheduleTaskService _scheduleTaskService;

        #endregion

        #region Ctor

        public EventConsumer(ILocalizationService localizationService,
            IPaymentService paymentService,
            IScheduleTaskService scheduleTaskService)
        {
            _localizationService = localizationService;
            _paymentService = paymentService;
            _scheduleTaskService = scheduleTaskService;
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

            //check whether the plugin is active
            var squarePaymentMethod = _paymentService.LoadPaymentMethodBySystemName(SquarePaymentDefaults.SystemName);
            if (!_paymentService.IsPaymentMethodActive(squarePaymentMethod))
                return;

            //add js script to one page checkout
            if (eventMessage.GetRouteNames().Any(r => r.Equals("CheckoutOnePage")))
            {
                eventMessage.Helper.AddScriptParts(ResourceLocation.Footer, SquarePaymentDefaults.PaymentFormScriptPath, excludeFromBundle: true);
            }
        }

        /// <summary>
        /// Handle model received event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(ModelReceivedEvent<BaseNopModel> eventMessage)
        {
            //whether received model is ScheduleTaskModel
            if (!(eventMessage?.Model is ScheduleTaskModel scheduleTaskModel))
                return;

            //whether renew access token task is changed
            var scheduleTask = _scheduleTaskService.GetTaskById(scheduleTaskModel.Id);
            if (!scheduleTask?.Type.Equals(SquarePaymentDefaults.RenewAccessTokenTask) ?? true)
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