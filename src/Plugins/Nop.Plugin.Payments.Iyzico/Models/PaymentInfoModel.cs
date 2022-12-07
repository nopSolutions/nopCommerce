using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Iyzico.Models
{
    public record PaymentInfoModel : BaseNopModel
    {
        public bool UseToPaymentPopup { get; set; }
    }
}