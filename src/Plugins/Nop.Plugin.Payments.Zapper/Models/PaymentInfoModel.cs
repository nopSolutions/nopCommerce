using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Zapper.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public string Selector { get; set; }
        public string MerchantId { get; set; }
        public string SiteId { get; set; }
        public decimal Amount { get; set; }
        public string MerchantReference { get; set; }
    }
}
