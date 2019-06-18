using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class BaseResponse
    {
        public string Message { get; set; }

        public string Error { get; set; }

        public string Status { get; set; }
    }
}
