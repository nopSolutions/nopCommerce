using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Customer;

public record PhoneVerificationModel : BaseNopModel
{
    [NopResourceDisplayName("PhoneVerification.Fields.OtpCode")]
    public string OtpCode { get; set; }

    [DataType(DataType.PhoneNumber)]
    [NopResourceDisplayName("PhoneVerification.Fields.Phone")]
    public string Phone { get; set; }

    public string ReturnUrl { get; set; }

    public string Result { get; set; }

    public PhoneVerificationFlowEnum VerificationFlow { get; set; }
}
