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
        /// <remarks>
        /// Optional. The color theme of the widget.
        /// Default for version 1: white
        /// Default for version 2: light
        /// </remarks>
        public string ReCaptchaTheme { get; set; }
        /// <summary>
        /// reCAPTCHA version
        /// </summary>
        public int ReCaptchaVersion { get; set; }
        /// <summary>
        /// reCAPTCHA language
        /// </summary>
        /// <remarks>
        /// See all: https://developers.google.com/recaptcha/docs/language
        /// Optional. Forces the widget to render in a specific language. Auto-detects the user's language if unspecified.
        /// </remarks>
        public string ReCaptchaLanguage { get; set; }
        /// <summary>
        /// reCAPTCHA type. 
        /// </summary>
        /// <remarks>
        /// Audio or image
        /// Optional. The type of CAPTCHA to serve. 
        /// Default: image
        /// </remarks>
        public string ReCaptchaType { get; set; }
        /// <summary>
        /// reCAPTCHA size.
        /// </summary>
        /// <remarks>
        /// Compact or normal
        /// Optional. The size of the widget.
        /// </remarks>
        public string ReCaptchaSize { get; set; }
    }
}