using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Newsletter;

public partial record NewsLetterBoxModel : BaseNopModel
{
    public bool AllowToUnsubscribe { get; set; }
    public bool DisplayCaptcha { get; set; }
    [DataType(DataType.EmailAddress)]
    public string NewsLetterEmail { get; set; }
    public bool IsReCaptchaV3 { get; set; }
    public string ReCaptchaPublicKey { get; set; }
}