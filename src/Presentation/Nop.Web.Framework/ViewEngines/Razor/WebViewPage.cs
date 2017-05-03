using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Themes;

namespace Nop.Web.Framework.ViewEngines.Razor
{
    /// <summary>
    /// Web view page
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    public abstract class WebViewPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
//TODO rename to RazorPage
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
                        var resFormat = _localizationService.GetResource(format);
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

        //TODO public override string Layout
        //{
        //    get
        //    {
        //        var layout = base.Layout;

        //        if (!string.IsNullOrEmpty(layout))
        //        {
        //            var filename = Path.GetFileNameWithoutExtension(layout);
        //            ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindView(ViewContext.Controller.ControllerContext, filename, "");

        //            if (viewResult.View != null && viewResult.View is RazorView)
        //            {
        //                layout = (viewResult.View as RazorView).ViewPath;
        //            }
        //        }

        //        return layout;
        //    }
        //    set
        //    {
        //        base.Layout = value;
        //    }
        //}

        /// <summary>
        /// Return a value indicating whether the working language and theme support RTL (right-to-left)
        /// </summary>
        /// <returns></returns>
        public bool ShouldUseRtlTheme()
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var supportRtl = workContext.WorkingLanguage.Rtl;
            if (supportRtl)
            {
                //ensure that the active theme also supports it
                var themeProvider = EngineContext.Current.Resolve<IThemeProvider>();
                var themeContext = EngineContext.Current.Resolve<IThemeContext>();
                supportRtl = themeProvider.GetThemeConfiguration(themeContext.WorkingThemeName).SupportRtl;
            }
            return supportRtl;
        }
    }

    /// <summary>
    /// Web view page
    /// </summary>
    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}