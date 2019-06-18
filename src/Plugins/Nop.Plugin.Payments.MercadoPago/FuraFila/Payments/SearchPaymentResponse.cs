using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Models;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Payments
{
    public class SearchPaymentResponse
    {
        public Paging Paging { get; set; }

        public Payment[] Results { get; set; }
    }

    public class Paging
    {
        public int Total { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }
    }
}
