using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.MercadoPago.FuraFila;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.MercadoPago
{
    public class MercadoPagoPaymentProcessor : BasePlugin, IPaymentMethod, IPaymentMethodAsync
    {
        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => false;

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription
        {
            get
            {
                var settings = EngineContext.Current.Resolve<MercadoPagoPaymentSettings>();
                return settings.PaymentMethodDescription;
            }
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return false;

            return true;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult { Errors = new[] { "Capture method not supported" } };
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart) => 0m;

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form) => new ProcessPaymentRequest();

        public string GetPublicViewComponentName() => "PaymentMercadoPago";

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart) => false;

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            try
            {
                var svc = EngineContext.Current.Resolve<IMPPaymentService>();
                var settings = EngineContext.Current.Resolve<MercadoPagoPaymentSettings>();
                var redirectUri = svc.CreatePaymentRequest(postProcessPaymentRequest, settings, CancellationToken.None).GetAwaiter().GetResult();

                var httpContext = EngineContext.Current.Resolve<IHttpContextAccessor>();
                httpContext.HttpContext.Response.Redirect(redirectUri.AbsoluteUri);
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(ex.Message, ex);
            }
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var svc = EngineContext.Current.Resolve<IMPPaymentService>();
                var settings = EngineContext.Current.Resolve<MercadoPagoPaymentSettings>();
                var redirectUri = await svc.CreatePaymentRequest(postProcessPaymentRequest, settings, cancellationToken);

                var httpContext = EngineContext.Current.Resolve<IHttpContextAccessor>();
                httpContext.HttpContext.Response.Redirect(redirectUri.AbsoluteUri);
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(ex.Message, ex);
            }
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var processPaymentResult = new ProcessPaymentResult()
            {
                NewPaymentStatus = PaymentStatus.Pending
            };
            return processPaymentResult;
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest) => new ProcessPaymentResult();

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest) => new RefundPaymentResult();

        public IList<string> ValidatePaymentForm(IFormCollection form) => new List<string>();

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest) => new VoidPaymentResult();
    }
}
