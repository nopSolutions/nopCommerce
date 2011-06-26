using System.ComponentModel;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Manual.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int TransactModeId { get; set; }

        [DisplayName("Additional fee")]
        public decimal AdditionalFee { get; set; }

        [DisplayNameAttribute("After checkout mark payment as")]
        public SelectList TransactModeValues { get; set; }
    }
}