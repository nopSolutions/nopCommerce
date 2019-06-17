using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Payments
{
    public interface IPaymentMethodAsync : IPaymentMethod
    {
        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest);
    }
}
