using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Momo.Models;

public record PaymentInfoModel : BaseNopModel
{
    [NopResourceDisplayName("Payment.PhoneNumber")]
    public string PhoneNumber { get; set; }
}
