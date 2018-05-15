using Nop.Services.Localization;

namespace Nop.Web.Framework.Security.Captcha
{
    /// <summary>
    /// Captcha extensions
    /// </summary>
    public static class CaptchaSettingsExtension
    {
        /// <summary>
        /// Get warning message if a selected Captcha version is not supported
        /// </summary>
        /// <param name="captchaSettings"></param>
        /// <param name="localizationService"></param>
        /// <returns></returns>
        public static string GetWrongCaptchaMessage(this CaptchaSettings captchaSettings,
            ILocalizationService localizationService)
        {
            return localizationService.GetResource("Common.WrongCaptchaMessage");           
        }
    }
}