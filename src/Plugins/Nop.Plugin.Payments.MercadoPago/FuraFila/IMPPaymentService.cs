using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila
{
    public interface IMPPaymentService
    {
        Task<Uri> CreatePaymentRequest(PostProcessPaymentRequest request, MercadoPagoPaymentSettings settings, CancellationToken cancellationToken = default);

        Task CheckPayments(CancellationToken cancellationToken = default);
    }
}
