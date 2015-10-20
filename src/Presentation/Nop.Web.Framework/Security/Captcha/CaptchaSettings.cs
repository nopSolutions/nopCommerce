using Nop.Core.Configuration;

namespace Nop.Web.Framework.Security.Captcha
{
    /// <summary>
    /// CAPTCHA settings
    /// </summary>
    public class CaptchaSettings : ISettings
    {
        /// <summary>
        /// Is CAPTCHA enabled?
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the login page
        /// </summary>
        public bool ShowOnLoginPage { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the registration page
        /// </summary>
        public bool ShowOnRegistrationPage { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the contacts page
        /// </summary>
        public bool ShowOnContactUsPage { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the wishlist page
        /// </summary>
        public bool ShowOnEmailWishlistToFriendPage { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the "email a friend" page
        /// </summary>
        public bool ShowOnEmailProductToFriendPage { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the "comment blog" page
        /// </summary>
        public bool ShowOnBlogCommentPage { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the "comment news" page
        /// </summary>
        public bool ShowOnNewsCommentPage { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the product reviews page
        /// </summary>
        public bool ShowOnProductReviewPage { get; set; }
        /// <summary>
        /// A value indicating whether CAPTCHA should be displayed on the "Apply for vendor account" page
        /// </summary>
        public bool ShowOnApplyVendorPage { get; set; }

        /// <summary>
        /// reCAPTCHA public key
        /// </summary>
        public string ReCaptchaPublicKey { get; set; }
        /// <summary>
        /// reCAPTCHA private key
        /// </summary>
        public string ReCaptchaPrivateKey { get; set; }
        /// <summary>
        /// reCAPTCHA theme
        /// </summary>
        public string ReCaptchaTheme { get; set; }
    }
}