using Nop.Services.Payments;
using System;
using System.Threading.Tasks;

namespace NopBrasil.Plugin.Payments.PagSeguro.Services
{
    public interface IPagSeguroService
    {
        [Obsolete("Use async version instead")]
        Uri CreatePayment(PostProcessPaymentRequest postProcessPaymentRequest);

        Task<Uri> CreatePaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest);

        void CheckPayments();
    }
}
