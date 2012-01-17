using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Manual.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.Manual.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        public int TransactModeId { get; set; }
        [NopResourceDisplayName("Plugins.Payments.Manual.Fields.TransactMode")]
        public SelectList TransactModeValues { get; set; }
    }
}