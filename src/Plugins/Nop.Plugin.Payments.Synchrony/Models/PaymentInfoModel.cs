using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Synchrony.Models
{
    public record PaymentInfoModel : BaseNopModel
    {
        public string TermNumber { get; set; }
        public string Description { get; set; }
    }
}
