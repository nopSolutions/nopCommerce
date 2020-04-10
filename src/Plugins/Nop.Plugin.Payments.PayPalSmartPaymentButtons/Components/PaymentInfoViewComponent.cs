using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Payments;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Models;
using Nop.Plugin.Payments.PayPalSmartPaymentButtons.Services;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Components
{
    /// <summary>
    /// Represents the view component to display payment info in public store
    /// </summary>
    [ViewComponent(Name = Defaults.PAYMENT_INFO_VIEW_COMPONENT_NAME)]
    public class PaymentInfoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly PaymentSettings _paymentSettings;
        private readonly ServiceManager _serviceManager;

        #endregion

        #region Ctor

        public PaymentInfoViewComponent(ILocalizationService localizationService,
            PaymentSettings paymentSettings,
            ServiceManager serviceManager)
        {
            _localizationService = localizationService;
            _paymentSettings = paymentSettings;
            _serviceManager = serviceManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Generate an order GUID
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        protected virtual void GenerateOrderGuid(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest == null)
                return;

            //we should use the same GUID for multiple payment attempts
            //this way a payment gateway can prevent security issues such as credit card brute-force attacks
            //in order to avoid any possible limitations by payment gateway we reset GUID periodically
            var previousPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>(Defaults.PaymentRequestSessionKey);
            if (_paymentSettings.RegenerateOrderGuidInterval > 0 && previousPaymentRequest?.OrderGuidGeneratedOnUtc != null)
            {
                var interval = DateTime.UtcNow - previousPaymentRequest.OrderGuidGeneratedOnUtc.Value;
                if (interval.TotalSeconds < _paymentSettings.RegenerateOrderGuidInterval)
                {
                    processPaymentRequest.OrderGuid = previousPaymentRequest.OrderGuid;
                    processPaymentRequest.OrderGuidGeneratedOnUtc = previousPaymentRequest.OrderGuidGeneratedOnUtc;
                }
            }

            if (processPaymentRequest.OrderGuid == Guid.Empty)
            {
                processPaymentRequest.OrderGuid = Guid.NewGuid();
                processPaymentRequest.OrderGuidGeneratedOnUtc = DateTime.UtcNow;
            }
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
            var model = new PaymentInfoModel();

            //prepare order GUID
            var paymentRequest = new ProcessPaymentRequest();
            GenerateOrderGuid(paymentRequest);

            //try to create an order
            var (order, error) = _serviceManager.CreateOrder(paymentRequest.OrderGuid);
            if (order != null)
            {
                model.OrderId = order.Id;

                //save order details for future using
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.PayPalSmartPaymentButtons.OrderId"), order.Id);
            }
            else if (!string.IsNullOrEmpty(error))
                model.Errors = error;

            HttpContext.Session.Set(Defaults.PaymentRequestSessionKey, paymentRequest);

            return View("~/Plugins/Payments.PayPalSmartPaymentButtons/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}