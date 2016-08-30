using Nop.Services.Localization;

namespace Nop.Web.Framework.Security.Captcha
{
    public static class CaptchaSettingsExtension
    {
        public static string GetWrongCaptchaMessage(this CaptchaSettings captchaSettings,
            ILocalizationService localizationService)
        {
            if (captchaSettings.ReCaptchaVersion == ReCaptchaVersion.Version1)
                return localizationService.GetResource("Common.WrongCaptcha");
            else if (captchaSettings.ReCaptchaVersion == ReCaptchaVersion.Version2)
                return localizationService.GetResource("Common.WrongCaptchaV2");
            return string.Empty;
        }
    }
}