using System.IO;
using System.Web.Mvc;

using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Themes;

namespace Nop.Web.Framework.ViewEngines.Razor
{
    /// <summary>
    /// 定义使用Asp.Net Razor语法的视图所需的属性和方法。
    /// Web view page
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private ILocalizationService _localizationService;
        private Localizer _localizer;

        /// <summary>
        /// 获得一个本地化字符串资源。这是一个委托，支持的参数可以是资源名称，或是模板字符串。
        /// Get a localized resources
        /// </summary>
        public Localizer T
        {
            get
            {
                if (_localizer == null)
                {
                    //null localizer
                    //_localizer = (format, args) => new LocalizedString((args == null || args.Length == 0) ? format : string.Format(format, args));

                    //default localizer
                    _localizer = (format, args) =>
                                     {
                                         var resFormat = _localizationService.GetResource(format);
                                         if (string.IsNullOrEmpty(resFormat))
                                         {
                                             return new LocalizedString(format);
                                         }
                                         else
                                         {
                                             return new LocalizedString((args == null || args.Length == 0)
                                                                     ? resFormat
                                                                     : string.Format(resFormat, args));
                                         }                                         
                                     };
                }
                return _localizer;
            }
        }

        /// <summary>
        /// 初始化AjaxHelper，HtmlHelper和UrlHelper类。
        /// </summary>
        public override void InitHelpers()
        {
            base.InitHelpers();

            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            }
        }

        /// <summary>
        /// 获取或设置布局页的路径。
        /// </summary>
        public override string Layout
        {
            get
            {
                var layout = base.Layout;

                if (!string.IsNullOrEmpty(layout))
                {
                    var filename = Path.GetFileNameWithoutExtension(layout);
                    ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindView(ViewContext.Controller.ControllerContext, filename, "");

                    if (viewResult.View != null && viewResult.View is RazorView)
                    {
                        layout = (viewResult.View as RazorView).ViewPath;
                    }
                }

                return layout;
            }
            set
            {
                base.Layout = value;
            }
        }

        /// <summary>
        /// 返回一个值指示是否工作语言和主题支持RTL(从右到左)
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