using Nop.Services.Payments;
using System;

namespace NopBrasil.Plugin.Payments.PagSeguro.Services
{
    public interface IPagSeguroService
    {
        Uri CreatePayment(PostProcessPaymentRequest postProcessPaymentRequest);

        void CheckPayments();
    }
}
