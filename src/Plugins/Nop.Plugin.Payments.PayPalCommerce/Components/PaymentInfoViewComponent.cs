using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.PayPalCommerce.Models;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.PayPalCommerce.Components
{
    /// <summary>
    /// Represents the view component to display payment info in public store
    /// </summary>
    public class PaymentInfoViewComponent : NopViewComponent
    {
        #region Fields

        protected readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        protected readonly IPaymentService _paymentService;
        protected readonly OrderSettings _orderSettings;
        protected readonly PayPalCommerceSettings _settings;
        protected readonly ServiceManager _serviceManager;

        #endregion

        #region Ctor

        public PaymentInfoViewComponent(ILocalizationService localizationService,
            INotificationService notificationService,
            IPaymentService paymentService,
            OrderSettings orderSettings,
            PayPalCommerceSettings settings,
            ServiceManager serviceManager)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _paymentService = paymentService;
            _orderSettings = orderSettings;
            _settings = settings;
            _serviceManager = serviceManager;
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
            var model = new PaymentInfoModel();

            //prepare order GUID
            var paymentRequest = new ProcessPaymentRequest();
            await _paymentService.GenerateOrderGuidAsync(paymentRequest);

            //try to create an order
            var (order, error) = await _serviceManager.CreateOrderAsync(_settings, paymentRequest.OrderGuid);
            if (order != null)
            {
                model.OrderId = order.Id;
                model.OrderTotal = order.PurchaseUnits.FirstOrDefault().AmountWithBreakdown.Value;

                //save order details for future using
                var key = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.OrderId");
                paymentRequest.CustomValues.Add(key, order.Id);
            }
            else if (!string.IsNullOrEmpty(error))
            {
                model.Errors = error;
                if (_orderSettings.OnePageCheckoutEnabled)
                    ModelState.AddModelError(string.Empty, error);
                else
                    _notificationService.ErrorNotification(error);
            }

            await HttpContext.Session.SetAsync(PayPalCommerceDefaults.PaymentRequestSessionKey, paymentRequest);

            return View("~/Plugins/Payments.PayPalCommerce/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}
