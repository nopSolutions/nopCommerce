using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PaytrIframe.Paytr
{
    public class PaytrPayment
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string token { get; set; }
    }
}
