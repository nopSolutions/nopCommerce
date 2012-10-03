#region Using...

using System;
using System.IO;
using System.Web.Mvc;
using System.Web.WebPages;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Themes;

#endregion

namespace Nop.Web.Framework.ViewEngines.Razor
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {

        private ILocalizationService _localizationService;
        private Localizer _localizer;
        private IWorkContext _workContext;

        /// <summary>
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
                                         return
                                             new LocalizedString((args == null || args.Length == 0)
                                                                     ? resFormat
                                                                     : string.Format(resFormat, args));
                                     };
                }
                return _localizer;
            }
        }

        public IWorkContext WorkContext
        {
            get
            {
                return _workContext;
            }
        }
        
        public override void InitHelpers()
        {
            base.InitHelpers();

            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                _workContext = EngineContext.Current.Resolve<IWorkContext>();
            }
        }

        public HelperResult RenderWrappedSection(string name, object wrapperHtmlAttributes)
        {
            Action<TextWriter> action = delegate(TextWriter tw)
                                {
                                    var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(wrapperHtmlAttributes);
                                    var tagBuilder = new TagBuilder("div");
                                    tagBuilder.MergeAttributes(htmlAttributes);

                                    var section = RenderSection(name, false);
                                    if (section != null)
                                    {
                                        tw.Write(tagBuilder.ToString(TagRenderMode.StartTag));
                                        section.WriteTo(tw);
                                        tw.Write(tagBuilder.ToString(TagRenderMode.EndTag));
                                    }
                                };
            return new HelperResult(action);
        }

        public HelperResult RenderSection(string sectionName, Func<object, HelperResult> defaultContent)
        {
            return IsSectionDefined(sectionName) ? RenderSection(sectionName) : defaultContent(new object());
        }

        public override string Layout
        {
            get
            {
                var layout = base.Layout;

                if (!string.IsNullOrEmpty(layout))
                {
                    var filename = System.IO.Path.GetFileNameWithoutExtension(layout);
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
        /// Return a value indicating whether the working language and theme support RTL (right-to-left)
        /// </summary>
        /// <returns></returns>
        public bool ShouldUseRtlTheme()
        {
            var supportRtl = _workContext.WorkingLanguage.Rtl;
            if (supportRtl)
            {
                //ensure that the active theme also supports it
                var themeProvider = EngineContext.Current.Resolve<IThemeProvider>();
                var themeContext = EngineContext.Current.Resolve<IThemeContext>();
                supportRtl = themeProvider.GetThemeConfiguration(themeContext.WorkingDesktopTheme).SupportRtl;
            }
            return supportRtl;
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}