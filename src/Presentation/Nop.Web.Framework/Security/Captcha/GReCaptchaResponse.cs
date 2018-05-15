using System.Collections.Generic;

namespace Nop.Web.Framework.Security.Captcha
{
    /// <summary>
    /// Google reCAPTCHA response
    /// </summary>
    public class GReCaptchaResponse
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public GReCaptchaResponse()
        {
            ErrorCodes = new List<string>();
        }

        /// <summary>
        /// Is valid
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// Error codes
        /// </summary>
        public List<string> ErrorCodes { get; set; }
    }
}