using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.MercadoPago.Exceptions
{
    public class CreatePaymentPreferenceHttpException : Exception
    {
        public CreatePaymentPreferenceHttpException()
            : base("Error when trying to send message to mercado pago")
        {
        }
    }
}
