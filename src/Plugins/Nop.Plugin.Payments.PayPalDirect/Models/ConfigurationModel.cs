using System.ComponentModel;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.PayPalDirect.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [DisplayName("Use Sandbox")]
        public bool UseSandbox { get; set; }

        public int TransactModeId { get; set; }
        [DisplayNameAttribute("Transaction mode")]
        public SelectList TransactModeValues { get; set; }

        [DisplayName("API Account Name")]
        public string ApiAccountName { get; set; }

        [DisplayName("API Account Password")]
        public string ApiAccountPassword { get; set; }

        [DisplayName("Signature")]
        public string Signature { get; set; }

        [DisplayName("Additional fee")]
        public decimal AdditionalFee { get; set; }
    }
}