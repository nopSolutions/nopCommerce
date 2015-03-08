
using Nop.Core.Configuration;

namespace Nop.Web.Framework.Security.Captcha
{
    public class CaptchaSettings : ISettings
    {
        public bool Enabled { get; set; }
        public bool ShowOnLoginPage { get; set; }
        public bool ShowOnRegistrationPage { get; set; }
        public bool ShowOnContactUsPage { get; set; }
        public bool ShowOnEmailWishlistToFriendPage { get; set; }
        public bool ShowOnEmailProductToFriendPage { get; set; }
        public bool ShowOnBlogCommentPage { get; set; }
        public bool ShowOnNewsCommentPage { get; set; }
        public bool ShowOnProductReviewPage { get; set; }
        public string ReCaptchaPublicKey { get; set; }
        public string ReCaptchaPrivateKey { get; set; }
        public string ReCaptchaTheme { get; set; }
    }
}