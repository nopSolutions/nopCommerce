using System.ComponentModel;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.AuthorizeNet.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [DisplayName("Use Sandbox")]
        public bool UseSandbox { get; set; }

        public int TransactModeId { get; set; }
        [DisplayNameAttribute("Transaction mode")]
        public SelectList TransactModeValues { get; set; }

        [DisplayName("Transaction key")]
        public string TransactionKey { get; set; }

        [DisplayName("Login ID")]
        public string LoginId { get; set; }

        [DisplayName("Additional fee")]
        public decimal AdditionalFee { get; set; }
    }
}