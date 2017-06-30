using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Models.Cms;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Components
{
    public class LocalizedEditor : ViewComponent
    {
        private readonly IHtmlHelper _htmlHelper;

        public LocalizedEditor(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public async Task<IViewComponentResult> InvokeAsync(string name,
            Func<int, HelperResult> localizedTemplate,
            Func<int, HelperResult> standardTemplate,
            bool ignoreIfSeveralStores = false)
        {
            var viewContextAware
                = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //var localizationSupported = _htmlHelper.ViewData.Model.Locales.Count > 1;
            //if (ignoreIfSeveralStores)
            //{
            //    var storeService = EngineContext.Current.Resolve<IStoreService>();
            //    if (storeService.GetAllStores().Count >= 2)
            //    {
            //        localizationSupported = false;
            //    }
            //}
            //if (localizationSupported)
            //{
            //    var tabStrip = new StringBuilder();
            //    tabStrip.AppendLine(string.Format("<div id=\"{0}\" class=\"nav-tabs-custom nav-tabs-localized-fields\">", name));
            //    tabStrip.AppendLine("<ul class=\"nav nav-tabs\">");

            //    //default tab
            //    tabStrip.AppendLine("<li class=\"active\">");
            //    tabStrip.AppendLine(string.Format("<a data-tab-name=\"{0}-{1}-tab\" href=\"#{0}-{1}-tab\" data-toggle=\"tab\">{2}</a>",
            //        name,
            //        "standard",
            //        EngineContext.Current.Resolve<ILocalizationService>().GetResource("Admin.Common.Standard")));
            //    tabStrip.AppendLine("</li>");

            //    var languageService = EngineContext.Current.Resolve<ILanguageService>();
            //    foreach (var locale in _htmlHelper.ViewData.Model.Locales)
            //    {
            //        //languages
            //        var language = languageService.GetLanguageById(locale.LanguageId);
            //        if (language == null)
            //            throw new Exception("Language cannot be loaded");

            //        tabStrip.AppendLine("<li>");
            //        var urlHelper = new UrlHelper(_htmlHelper.ViewContext);
            //        var iconUrl = urlHelper.Content("~/images/flags/" + language.FlagImageFileName);
            //        tabStrip.AppendLine(string.Format("<a data-tab-name=\"{0}-{1}-tab\" href=\"#{0}-{1}-tab\" data-toggle=\"tab\"><img alt='' src='{2}'>{3}</a>",
            //            name,
            //            language.Id,
            //            iconUrl,
            //            WebUtility.HtmlEncode(language.Name)));

            //        tabStrip.AppendLine("</li>");
            //    }
            //    tabStrip.AppendLine("</ul>");

            //    //default tab
            //    tabStrip.AppendLine("<div class=\"tab-content\">");
            //    tabStrip.AppendLine(string.Format("<div class=\"tab-pane active\" id=\"{0}-{1}-tab\">", name, "standard"));
            //    tabStrip.AppendLine(standardTemplate(_htmlHelper.ViewData.Model).ToHtmlString());
            //    tabStrip.AppendLine("</div>");

            //    for (int i = 0; i < _htmlHelper.ViewData.Model.Locales.Count; i++)
            //    {
            //        //languages
            //        var language = languageService.GetLanguageById(_htmlHelper.ViewData.Model.Locales[i].LanguageId);

            //        tabStrip.AppendLine(string.Format("<div class=\"tab-pane\" id=\"{0}-{1}-tab\">",
            //            name,
            //            language.Id));
            //        tabStrip.AppendLine(localizedTemplate(i).ToHtmlString());
            //        tabStrip.AppendLine("</div>");
            //    }
            //    tabStrip.AppendLine("</div>");
            //    tabStrip.AppendLine("</div>");
            //    return new HtmlString(tabStrip.ToString());
            //}
            //else
            //{
            //    return standardTemplate(_htmlHelper.ViewData.Model);
            //}

            return Content("");
        }
    }
}