using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PaytrIframe.Models
{
    public record PaymentInfoModel : BaseNopModel
    {
        public string PaymentInfo { get; set; }
    }
}
