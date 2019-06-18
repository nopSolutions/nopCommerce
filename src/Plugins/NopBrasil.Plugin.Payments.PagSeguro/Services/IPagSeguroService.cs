using Nop.Services.Payments;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NopBrasil.Plugin.Payments.PagSeguro.Services
{
    public interface IPagSeguroService
    {
        [Obsolete("Use async version instead")]
        Uri CreatePayment(PostProcessPaymentRequest postProcessPaymentRequest);

        Task<Uri> CreatePaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest, CancellationToken cancellationToken = default);

        System.Threading.Tasks.Task CheckPayments(CancellationToken cancellationToken = default);
    }
}
