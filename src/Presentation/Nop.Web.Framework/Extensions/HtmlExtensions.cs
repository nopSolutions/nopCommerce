using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Extensions
{
    /// <summary>
    /// HTML extensions
    /// </summary>
    public static class HtmlExtensions
    {
        #region Admin area extensions

        /// <summary>
        /// Generate editor for localizable entities
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TLocalizedModelLocal">Locale model type</typeparam>
        /// <param name="helper">HTML helper</param>
        /// <param name="name">ID of control</param>
        /// <param name="localizedTemplate">Template with localizable values</param>
        /// <param name="standardTemplate">Template for standard (default) values</param>
        /// <param name="ignoreIfSeveralStores">A value indicating whether to ignore localization if we have multiple stores</param>
        /// <param name="cssClass">CSS class for localizedTemplate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized editor
        /// </returns>
        public static async Task<IHtmlContent> LocalizedEditorAsync<TModel, TLocalizedModelLocal>(this IHtmlHelper<TModel> helper,
            string name,
            Func<int, HelperResult> localizedTemplate,
            Func<TModel, HelperResult> standardTemplate,
            bool ignoreIfSeveralStores = false,
            string cssClass = null)
            where TModel : ILocalizedModel<TLocalizedModelLocal>
            where TLocalizedModelLocal : ILocalizedLocaleModel
        {
            var localizationSupported = helper.ViewData.Model.Locales.Count > 1;
            if (ignoreIfSeveralStores)
            {
                var storeService = EngineContext.Current.Resolve<IStoreService>();

                if ((await storeService.GetAllStoresAsync()).Count >= 2)
                    localizationSupported = false;
            }

            if (!localizationSupported)
                return new HtmlString(await standardTemplate(helper.ViewData.Model).RenderHtmlContentAsync());

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            var urlHelper = EngineContext.Current.Resolve<IUrlHelperFactory>().GetUrlHelper(helper.ViewContext);

            var tabStrip = new StringBuilder();
            var cssClassWithSpace = !string.IsNullOrEmpty(cssClass) ? $" {cssClass}" : null;
            tabStrip.AppendLine($"<div id=\"{name}\" class=\"nav-tabs-custom nav-tabs-localized-fields{cssClassWithSpace}\">");

            //render input contains selected tab name
            var tabNameToSelect = GetSelectedTabName(helper, name);
            var selectedTabInput = new TagBuilder("input");
            selectedTabInput.Attributes.Add("type", "hidden");
            selectedTabInput.Attributes.Add("id", $"selected-tab-name-{name}");
            selectedTabInput.Attributes.Add("name", $"selected-tab-name-{name}");
            selectedTabInput.Attributes.Add("value", tabNameToSelect);
            tabStrip.AppendLine(await selectedTabInput.RenderHtmlContentAsync());

            tabStrip.AppendLine($"<div class=\"card card-primary card-outline card-outline-tabs\">");
            tabStrip.AppendLine($"<div class=\"card-header p-0 pt-1 border-bottom-0\">");

            tabStrip.AppendLine("<ul class=\"nav nav-tabs\" id=\"custom-content-above-tab\" role=\"tablist\" >");

            //default tab
            var standardTabName = $"{name}-standard-tab";
            var standardTabSelected = string.IsNullOrEmpty(tabNameToSelect) || standardTabName == tabNameToSelect;
            tabStrip.AppendLine(string.Format("<li class=\"nav-item\">"));
            if (standardTabSelected)
            {
                tabStrip.AppendLine($"<a class=\"nav-link active\" data-tab-name=\"{standardTabName}\" href=\"#{standardTabName}\" data-toggle=\"pill\" role=\"tab\" aria-selected=\"false\">{await localizationService.GetResourceAsync("Admin.Common.Standard")}</a>");
            }
            else
            {
                tabStrip.AppendLine($"<a class=\"nav-link\" data-tab-name=\"{standardTabName}\" href=\"#{standardTabName}\" data-toggle=\"pill\" role=\"tab\" aria-selected=\"false\">{await localizationService.GetResourceAsync("Admin.Common.Standard")}</a>");
            }
            tabStrip.AppendLine("</li>");

            foreach (var locale in helper.ViewData.Model.Locales)
            {
                //languages
                var language = await languageService.GetLanguageByIdAsync(locale.LanguageId);
                if (language == null)
                    throw new Exception("Language cannot be loaded");

                var localizedTabName = $"{name}-{language.Id}-tab";
                tabStrip.AppendLine(string.Format("<li class=\"nav-item\">"));
                var iconUrl = urlHelper.Content("~/images/flags/" + language.FlagImageFileName);
                var active = localizedTabName == tabNameToSelect ? "active" : null;
                tabStrip.AppendLine($"<a class=\"nav-link {active}\" data-tab-name=\"{localizedTabName}\" href=\"#{localizedTabName}\" data-toggle=\"pill\" role=\"tab\" aria-selected=\"false\"><img alt='' src='{iconUrl}'>{WebUtility.HtmlEncode(language.Name)}</a>");

                tabStrip.AppendLine("</li>");
            }
            tabStrip.AppendLine("</ul>");
            tabStrip.AppendLine("</div>");
            tabStrip.AppendLine("<div class=\"card-body\">");

            //default tab
            tabStrip.AppendLine("<div class=\"tab-content\" id=\"custom-content-above-tabContent\">");
            tabStrip.AppendLine(string.Format("<div class=\"tab-pane fade{0}\" id=\"{1}\" role=\"tabpanel\">", standardTabSelected ? " show active" : null, standardTabName));
            tabStrip.AppendLine(await standardTemplate(helper.ViewData.Model).RenderHtmlContentAsync());
            tabStrip.AppendLine("</div>");

            for (var i = 0; i < helper.ViewData.Model.Locales.Count; i++)
            {
                //languages
                var language = await languageService.GetLanguageByIdAsync(helper.ViewData.Model.Locales[i].LanguageId);
                if (language == null)
                    throw new Exception("Language cannot be loaded");

                var localizedTabName = $"{name}-{language.Id}-tab";
                tabStrip.AppendLine(string.Format("<div class=\"tab-pane fade{0}\" id=\"{1}\" role=\"tabpanel\">", localizedTabName == tabNameToSelect ? " show active" : null, localizedTabName));
                tabStrip.AppendLine(await localizedTemplate(i).RenderHtmlContentAsync());
                tabStrip.AppendLine("</div>");
            }
            tabStrip.AppendLine("</div>");
            tabStrip.AppendLine("</div>");
            tabStrip.AppendLine("</div>");
            tabStrip.AppendLine("</div>");

            //render tabs script
            var script = new TagBuilder("script");
            script.InnerHtml.AppendHtml(
                "$(document).ready(function () {" +
                    "bindBootstrapTabSelectEvent('" + name + "', 'selected-tab-name-" + name + "');" +
                "});");
            var scriptTag = await script.RenderHtmlContentAsync();
            tabStrip.AppendLine(scriptTag);

            return new HtmlString(tabStrip.ToString());
        }

        /// <summary>
        /// Gets a selected card name (used in admin area to store selected panel name)
        /// </summary>
        /// <param name="helper">HtmlHelper</param>
        /// <returns>Name</returns>
        public static string GetSelectedCardName(this IHtmlHelper helper)
        {
            //keep this method synchronized with
            //"SaveSelectedCardName" method of \Area\Admin\Controllers\BaseAdminController.cs
            var cardName = string.Empty;
            const string dataKey = "nop.selected-card-name";

            if (helper.ViewData.ContainsKey(dataKey))
                cardName = helper.ViewData[dataKey].ToString();

            if (helper.ViewContext.TempData.ContainsKey(dataKey))
                cardName = helper.ViewContext.TempData[dataKey].ToString();

            return cardName;
        }

        /// <summary>
        /// Gets a selected tab name (used in admin area to store selected tab name)
        /// </summary>
        /// <param name="helper">HtmlHelper</param>
        /// <param name="dataKeyPrefix">Key prefix. Pass null to ignore</param>
        /// <returns>Name</returns>
        public static string GetSelectedTabName(this IHtmlHelper helper, string dataKeyPrefix = null)
        {
            //keep this method synchronized with
            //"SaveSelectedTab" method of \Area\Admin\Controllers\BaseAdminController.cs
            var tabName = string.Empty;
            var dataKey = "nop.selected-tab-name";
            if (!string.IsNullOrEmpty(dataKeyPrefix))
                dataKey += $"-{dataKeyPrefix}";

            if (helper.ViewData.ContainsKey(dataKey))
                tabName = helper.ViewData[dataKey].ToString();

            if (helper.ViewContext.TempData.ContainsKey(dataKey))
                tabName = helper.ViewContext.TempData[dataKey].ToString();

            return tabName;
        }

        /// <summary>
        /// Add a tab to TabStrip
        /// </summary>
        /// <param name="eventMessage">AdminTabStripCreated</param>
        /// <param name="tabId">Tab Id</param>
        /// <param name="tabName">Tab name</param>
        /// <param name="url">url</param>
        /// <returns>Html content of new Tab</returns>
        public static IHtmlContent TabContentByURL(this AdminTabStripCreated eventMessage, string tabId, string tabName, string url)
        {
            return new HtmlString($@"
                <script>
                    $(document).ready(function() {{
                        $('<li><a data-tab-name='{tabId}' data-toggle='tab' href='#{tabId}'>{tabName}</a></li>').appendTo('#{eventMessage.TabStripName} .nav-tabs:first');
                        $.get('{url}', function(result) {{
                            $(`<div class='tab-pane' id='{tabId}'>` + result + `</div>`).appendTo('#{eventMessage.TabStripName} .tab-content:first');
                        }});
                    }});
                </script>");
        }

        /// <summary>
        /// Add a tab to TabStrip
        /// </summary>
        /// <param name="eventMessage">AdminTabStripCreated</param>
        /// <param name="tabId">Tab Id</param>
        /// <param name="tabName">Tab name</param>
        /// <param name="contentModel">Content model</param>
        /// <returns>Html content of new Tab</returns>
        public static IHtmlContent TabContentByModel(this AdminTabStripCreated eventMessage, string tabId, string tabName, string contentModel)
        {
            return new HtmlString($@"
                <script>
                    $(document).ready(function() {{
                        $(`<li><a data-tab-name='{tabId}' data-toggle='tab' href='#{tabId}'>{tabName}</a></li>`).appendTo('#{eventMessage.TabStripName} .nav-tabs:first');
                        $(`<div class='tab-pane' id='{tabId}'>{contentModel}</div>`).appendTo('#{eventMessage.TabStripName} .tab-content:first');
                    }});
                </script>");
        }

        #region Form fields

        /// <summary>
        /// Generate hint control
        /// </summary>
        /// <param name="helper">HTML helper</param>
        /// <param name="value">TexHint text</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public static async Task<IHtmlContent> HintAsync(this IHtmlHelper helper, string value)
        {
            //create tag builder
            var builder = new TagBuilder("div");
            builder.MergeAttribute("title", value);
            builder.MergeAttribute("class", "ico-help");
            builder.MergeAttribute("data-toggle", "tooltip");
            var icon = new StringBuilder();
            icon.Append("<i class='fas fa-question-circle'></i>");
            builder.InnerHtml.AppendHtml(icon.ToString());

            //render tag
            return new HtmlString(await builder.RenderHtmlContentAsync());
        }

        #endregion

        #endregion

        #region Common extensions

        /// <summary>
        /// Convert IHtmlContent to string
        /// </summary>
        /// <param name="htmlContent">HTML content</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public static async Task<string> RenderHtmlContentAsync(this IHtmlContent htmlContent)
        {
            await using var writer = new StringWriter();
            htmlContent.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        #endregion
    }
}