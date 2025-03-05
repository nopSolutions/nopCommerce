using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Nop.Services.Payments;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.MercadoPagoPlugin.Services
{
    public class MercadoPagoService
    {
        private readonly MercadoPagoPaymentSettings _settings;

        public MercadoPagoService(MercadoPagoPaymentSettings settings)
        {
            _settings = settings;
            MercadoPagoConfig.AccessToken = _settings.AccessToken;
        }

        //public async Task<Payment> CreatePaymentAsync(ProcessPaymentRequest processPaymentRequest)
        //{
        //    var paymentRequest = new PaymentCreateRequest
        //    {
        //        TransactionAmount = processPaymentRequest.OrderTotal,
        //        Token = processPaymentRequest.PaymentToken,
        //        Description = "Pago de pedido",
        //        Installments = 1,
        //        PaymentMethodId = "visa",
        //        Payer = new PaymentPayerRequest
        //        {
        //            Email = processPaymentRequest.PayerEmail
        //        }
        //    };

        //    var paymentClient = new PaymentClient();
        //    return await paymentClient.CreateAsync(paymentRequest);
        //}

        //public async Task VoidPaymentAsync(VoidPaymentRequest voidPaymentRequest)
        //{
        //    var paymentClient = new PaymentClient();
        //    await paymentClient.CancelAsync(voidPaymentRequest.PaymentId);
        //}

        //public async Task RefundPaymentAsync(RefundPaymentRequest refundPaymentRequest)
        //{
        //    var paymentClient = new PaymentClient();
        //    await paymentClient.RefundAsync(refundPaymentRequest.PaymentId);
        //}

        //public async Task CapturePaymentAsync(CapturePaymentRequest capturePaymentRequest)
        //{
        //    var paymentClient = new PaymentClient();
        //    await paymentClient.CaptureAsync(capturePaymentRequest.PaymentId);
        //}
    }
}
