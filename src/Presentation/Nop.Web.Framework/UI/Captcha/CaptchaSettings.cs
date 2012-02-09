
using Nop.Core.Configuration;

namespace Nop.Web.Framework.UI.Captcha
{
    public class CaptchaSettings : ISettings
    {
        public bool Enabled { get; set; }
        public bool ShowOnLoginPage { get; set; }
        public bool ShowOnRegistrationPage { get; set; }
        public bool ShowOnContactUsPage { get; set; }
        public bool ShowOnEmailWishlistToFriendPage { get; set; }
        public bool ShowOnEmailProductToFriendPage { get; set; }
        public string ReCaptchaPublicKey { get; set; }
        public string ReCaptchaPrivateKey { get; set; }
    }
}