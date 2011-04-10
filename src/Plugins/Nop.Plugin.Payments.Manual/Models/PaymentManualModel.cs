using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.Manual.Models
{
    public class PaymentManualModel
    {
        public int TransactModeId { get; set; }

        [DisplayNameAttribute("Additional fee")]
        public decimal AdditionalFee { get; set; }

        [DisplayNameAttribute("After checkout mark payment as")]
        public SelectList TransactModeValues { get; set; }
    }
}