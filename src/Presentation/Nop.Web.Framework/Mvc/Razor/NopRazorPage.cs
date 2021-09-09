using System.Globalization;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Themes;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Themes;

namespace Nop.Web.Framework.Mvc.Razor
{
    /// <summary>
    /// Web view page
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    public abstract class NopRazorPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        private ILocalizationService _localizationService;
        private Localizer _localizer;

        /// <summary>
        /// Get a localized resources
        /// </summary>
        public Localizer T
        {
            get
            {
                if (_localizationService == null)
                    _localizationService = EngineContext.Current.Resolve<ILocalizationService>();

                if (_localizer == null)
                {
                    _localizer = (format, args) =>
                    {
                        var resFormat = _localizationService.GetResourceAsync(format).Result;
                        if (string.IsNullOrEmpty(resFormat))
                        {
                            return new LocalizedString(format);
                        }
                        return new LocalizedString((args == null || args.Length == 0)
                            ? resFormat
                            : string.Format(resFormat, args));
                    };
                }
                return _localizer;
            }
        }

        /// <summary>
        /// Return a value indicating whether the working language and theme support RTL (right-to-left)
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        public async Task<bool> ShouldUseRtlThemeAsync()
        {
            if (!CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
                return false;

            //ensure that the active theme also supports it
            var themeProvider = EngineContext.Current.Resolve<IThemeProvider>();
            var themeContext = EngineContext.Current.Resolve<IThemeContext>();

            return (await themeProvider.GetThemeBySystemNameAsync(await themeContext.GetWorkingThemeNameAsync()))?.SupportRtl ?? false;
        }
    }

    /// <summary>
    /// Web view page
    /// </summary>
    public abstract class NopRazorPage : NopRazorPage<dynamic>
    {
    }
}