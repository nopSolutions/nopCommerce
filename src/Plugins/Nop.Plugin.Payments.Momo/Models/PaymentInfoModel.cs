using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Momo.Models;

public record PaymentInfoModel : BaseNopModel
{
    [NopResourceDisplayName("Payment.PhoneNumber"), Phone]
    public string PhoneNumber { get; set; }
    public bool PhoneNumberRequired { get; internal set; }
    public string PaymentNote { get; internal set; }
}
