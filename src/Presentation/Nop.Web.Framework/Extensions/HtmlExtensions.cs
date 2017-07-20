using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Framework.Extensions
{
    public static class HtmlExtensions
    {
        #region Admin area extensions
        
        public static IHtmlContent LocalizedEditor<T, TLocalizedModelLocal>(this IHtmlHelper<T> helper,
            string name,
            Func<int, HelperResult> localizedTemplate,
            Func<T, HelperResult> standardTemplate,
            bool ignoreIfSeveralStores = false)
            where T : ILocalizedModel<TLocalizedModelLocal>
            where TLocalizedModelLocal : ILocalizedModelLocal
        {
            var localizationSupported = helper.ViewData.Model.Locales.Count > 1;
            if (ignoreIfSeveralStores)
            {
                var storeService = EngineContext.Current.Resolve<IStoreService>();
                if (storeService.GetAllStores().Count >= 2)
                {
                    localizationSupported = false;
                }
            }
            if (localizationSupported)
            {
                var tabStrip = new StringBuilder();
                tabStrip.AppendLine(string.Format("<div id=\"{0}\" class=\"nav-tabs-custom nav-tabs-localized-fields\">", name));
                tabStrip.AppendLine("<ul class=\"nav nav-tabs\">");

                //default tab
                tabStrip.AppendLine("<li class=\"active\">");
                tabStrip.AppendLine(string.Format("<a data-tab-name=\"{0}-{1}-tab\" href=\"#{0}-{1}-tab\" data-toggle=\"tab\">{2}</a>",
                        name, 
                        "standard",
                        EngineContext.Current.Resolve<ILocalizationService>().GetResource("Admin.Common.Standard")));
                tabStrip.AppendLine("</li>");

                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                foreach (var locale in helper.ViewData.Model.Locales)
                {
                    //languages
                    var language = languageService.GetLanguageById(locale.LanguageId);
                    if (language == null)
                        throw new Exception("Language cannot be loaded");

                    tabStrip.AppendLine("<li>");
                    var urlHelper = new UrlHelper(helper.ViewContext);
                    var iconUrl = urlHelper.Content("~/images/flags/" + language.FlagImageFileName);
                    tabStrip.AppendLine(string.Format("<a data-tab-name=\"{0}-{1}-tab\" href=\"#{0}-{1}-tab\" data-toggle=\"tab\"><img alt='' src='{2}'>{3}</a>",
                            name, 
                            language.Id,
                            iconUrl,
                            WebUtility.HtmlEncode(language.Name)));

                    tabStrip.AppendLine("</li>");
                }
                tabStrip.AppendLine("</ul>");
                    
                //default tab
                tabStrip.AppendLine("<div class=\"tab-content\">");
                tabStrip.AppendLine(string.Format("<div class=\"tab-pane active\" id=\"{0}-{1}-tab\">", name, "standard"));
                tabStrip.AppendLine(standardTemplate(helper.ViewData.Model).ToHtmlString());
                tabStrip.AppendLine("</div>");

                for (int i = 0; i < helper.ViewData.Model.Locales.Count; i++)
                {
                    //languages
                    var language = languageService.GetLanguageById(helper.ViewData.Model.Locales[i].LanguageId);

                    tabStrip.AppendLine(string.Format("<div class=\"tab-pane\" id=\"{0}-{1}-tab\">",
                        name,
                        language.Id));
                    tabStrip.AppendLine(localizedTemplate(i).ToHtmlString());
                    tabStrip.AppendLine("</div>");
                }
                tabStrip.AppendLine("</div>");
                tabStrip.AppendLine("</div>");
                return new HtmlString(tabStrip.ToString());
            }
            else
            {
                return standardTemplate(helper.ViewData.Model);
            }
        }

        public static IHtmlContent DeleteConfirmation<T>(this IHtmlHelper<T> helper, string buttonsSelector) where T : BaseNopEntityModel
        {
            return DeleteConfirmation(helper, "", buttonsSelector);
        }

        public static IHtmlContent DeleteConfirmation<T>(this IHtmlHelper<T> helper, string actionName,
            string buttonsSelector) where T : BaseNopEntityModel
        {
            if (String.IsNullOrEmpty(actionName))
                actionName = "Delete";

            var modalId = new HtmlString(helper.ViewData.ModelMetadata.ModelType.Name.ToLower() + "-delete-confirmation").ToHtmlString();

            var deleteConfirmationModel = new DeleteConfirmationModel
            {
                Id = helper.ViewData.Model.Id,
                ControllerName = helper.ViewContext.RouteData.Values["controller"].ToString(),
                ActionName = actionName,
                WindowId = modalId
            };

            var window = new StringBuilder();
            window.AppendLine(string.Format("<div id='{0}' class=\"modal fade\"  tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"{0}-title\">", modalId));
            window.AppendLine(helper.Partial("Delete", deleteConfirmationModel).ToHtmlString());
            window.AppendLine("</div>");

            window.AppendLine("<script>");
            window.AppendLine("$(document).ready(function() {");
            window.AppendLine(string.Format("$('#{0}').attr(\"data-toggle\", \"modal\").attr(\"data-target\", \"#{1}\")", buttonsSelector, modalId));
            window.AppendLine("});");
            window.AppendLine("</script>");
            return new HtmlString(window.ToString());
        }

        public static IHtmlContent ActionConfirmation(this IHtmlHelper helper, string buttonId, string actionName = "")
        {
            if (string.IsNullOrEmpty(actionName))
                actionName = helper.ViewContext.RouteData.Values["action"].ToString();

            var modalId = new HtmlString(buttonId + "-action-confirmation").ToHtmlString();

            var actionConfirmationModel = new ActionConfirmationModel()
            {
                ControllerName = helper.ViewContext.RouteData.Values["controller"].ToString(),
                ActionName = actionName,
                WindowId = modalId
            };

            var window = new StringBuilder();
            window.AppendLine(string.Format("<div id='{0}' class=\"modal fade\"  tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"{0}-title\">", modalId));
            window.AppendLine(helper.Partial("Confirm", actionConfirmationModel).ToHtmlString());
            window.AppendLine("</div>");

            window.AppendLine("<script>");
            window.AppendLine("$(document).ready(function() {");
            window.AppendLine(string.Format("$('#{0}').attr(\"data-toggle\", \"modal\").attr(\"data-target\", \"#{1}\");", buttonId, modalId));
            window.AppendLine(string.Format("$('#{0}-submit-button').attr(\"name\", $(\"#{1}\").attr(\"name\"));", modalId, buttonId));
            window.AppendLine(string.Format("$(\"#{0}\").attr(\"name\", \"\")", buttonId));
            window.AppendLine(string.Format("if($(\"#{0}\").attr(\"type\") == \"submit\")$(\"#{0}\").attr(\"type\", \"button\")", buttonId));
            window.AppendLine("});");
            window.AppendLine("</script>");

            return new HtmlString(window.ToString());
        }

        public static IHtmlContent OverrideStoreCheckboxFor<TModel, TValue>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            Expression<Func<TModel, TValue>> forInputExpression,
            int activeStoreScopeConfiguration)
        {
            var dataInputIds = new List<string>();
            dataInputIds.Add(helper.IdFor(forInputExpression));
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, null, dataInputIds.ToArray());
        }
        public static IHtmlContent OverrideStoreCheckboxFor<TModel, TValue1, TValue2>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            Expression<Func<TModel, TValue1>> forInputExpression1,
            Expression<Func<TModel, TValue2>> forInputExpression2,
            int activeStoreScopeConfiguration)
        {
            var dataInputIds = new List<string>();
            dataInputIds.Add(helper.IdFor(forInputExpression1));
            dataInputIds.Add(helper.IdFor(forInputExpression2));
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, null, dataInputIds.ToArray());
        }
        public static IHtmlContent OverrideStoreCheckboxFor<TModel, TValue1, TValue2, TValue3>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            Expression<Func<TModel, TValue1>> forInputExpression1,
            Expression<Func<TModel, TValue2>> forInputExpression2,
            Expression<Func<TModel, TValue3>> forInputExpression3,
            int activeStoreScopeConfiguration)
        {
            var dataInputIds = new List<string>();
            dataInputIds.Add(helper.IdFor(forInputExpression1));
            dataInputIds.Add(helper.IdFor(forInputExpression2));
            dataInputIds.Add(helper.IdFor(forInputExpression3));
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, null, dataInputIds.ToArray());
        }
        public static IHtmlContent OverrideStoreCheckboxFor<TModel>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            string parentContainer,
            int activeStoreScopeConfiguration)
        {
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, parentContainer);
        }
        private static IHtmlContent OverrideStoreCheckboxFor<TModel>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            int activeStoreScopeConfiguration,
            string parentContainer = null,
            params string[] datainputIds)
        {
            if (String.IsNullOrEmpty(parentContainer) && datainputIds == null)
                throw new ArgumentException("Specify at least one selector");

            IHtmlContent result = null;
            if (activeStoreScopeConfiguration > 0)
            {
                //render only when a certain store is chosen
                const string cssClass = "multi-store-override-option";
                string dataInputSelector = "";
                if (!String.IsNullOrEmpty(parentContainer))
                {
                    dataInputSelector = "#" + parentContainer + " input, #" + parentContainer + " textarea, #" + parentContainer + " select";
                }
                if (datainputIds != null && datainputIds.Length > 0)
                {
                    dataInputSelector = "#" + String.Join(", #", datainputIds);
                }
                var onClick = string.Format("checkOverriddenStoreValue(this, '{0}')", dataInputSelector);
                result = helper.CheckBoxFor(expression, new Dictionary<string, object>
                {
                    { "class", cssClass },
                    { "onclick", onClick },
                    { "data-for-input-selector", dataInputSelector },
                });
            }

            return result;
        }

        
        /// <summary>
        /// Render CSS styles of selected index 
        /// </summary>
        /// <param name="helper">HTML helper</param>
        /// <param name="currentTabName">Current tab name (where appropriate CSS style should be rendred)</param>
        /// <param name="content">Tab content</param>
        /// <param name="isDefaultTab">Indicates that the tab is default</param>
        /// <param name="tabNameToSelect">Tab name to select</param>
        /// <returns>MvcHtmlString</returns>
        public static IHtmlContent RenderBootstrapTabContent(this IHtmlHelper helper, string currentTabName,
            IHtmlContent content, bool isDefaultTab = false, string tabNameToSelect = "")
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            if (string.IsNullOrEmpty(tabNameToSelect))
                tabNameToSelect = helper.GetSelectedTabName();

            if (string.IsNullOrEmpty(tabNameToSelect) && isDefaultTab)
                tabNameToSelect = currentTabName;

            var tag = new TagBuilder("div")
            {
                Attributes =
                {
                    new KeyValuePair<string, string>("class", string.Format("tab-pane{0}", tabNameToSelect == currentTabName ? " active" : "")),
                    new KeyValuePair<string, string>("id", string.Format("{0}", currentTabName))
                }
            };
            tag.InnerHtml.AppendHtml(content.ToHtmlString());

            return new HtmlString(tag.ToHtmlString());
        }

        /// <summary>
        /// Render CSS styles of selected index 
        /// </summary>
        /// <param name="helper">HTML helper</param>
        /// <param name="currentTabName">Current tab name (where appropriate CSS style should be rendred)</param>
        /// <param name="title">Tab title</param>
        /// <param name="isDefaultTab">Indicates that the tab is default</param>
        /// <param name="tabNameToSelect">Tab name to select</param>
        /// <param name="customCssClass">Tab name to select</param>
        /// <returns>Result</returns>
        public static IHtmlContent RenderBootstrapTabHeader(this IHtmlHelper helper, string currentTabName,
            LocalizedString title, bool isDefaultTab = false, string tabNameToSelect = "", string customCssClass = "")
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            if (string.IsNullOrEmpty(tabNameToSelect))
                tabNameToSelect = helper.GetSelectedTabName();

            if (string.IsNullOrEmpty(tabNameToSelect) && isDefaultTab)
                tabNameToSelect = currentTabName;

            var a = new TagBuilder("a")
            {
                Attributes =
                {
                    new KeyValuePair<string, string>("data-tab-name", currentTabName),
                    new KeyValuePair<string, string>("href", string.Format("#{0}", currentTabName)),
                    new KeyValuePair<string, string>("data-toggle", "tab"),
                }
            };
            a.InnerHtml.AppendHtml(title.Text);

            var liClassValue = "";
            if (tabNameToSelect == currentTabName)
            {
                liClassValue = "active";
            }
            if (!String.IsNullOrEmpty(customCssClass))
            {
                if (!String.IsNullOrEmpty(liClassValue))
                    liClassValue += " ";
                liClassValue += customCssClass;
            }

            var li = new TagBuilder("li")
            {
                Attributes =
                {
                    new KeyValuePair<string, string>("class", liClassValue),
                },
            };
            li.InnerHtml.AppendHtml(a.ToHtmlString());

            return new HtmlString(li.ToHtmlString());
        }
        
        /// <summary>
        /// Gets a selected tab name (used in admin area to store selected tab name)
        /// </summary>
        /// <returns>Name</returns>
        public static string GetSelectedTabName(this IHtmlHelper helper)
        {
            //keep this method synchornized with
            //"SaveSelectedTab" method of \Administration\Controllers\BaseAdminController.cs
            var tabName = string.Empty;
            const string dataKey = "nop.selected-tab-name";

            if (helper.ViewData.ContainsKey(dataKey))
                tabName = helper.ViewData[dataKey].ToString();

            //TODO test in asp.net core
            if (helper.ViewContext.TempData.ContainsKey(dataKey))
                tabName = helper.ViewContext.TempData[dataKey].ToString();

            return tabName;
        }
        
        #region Form fields

        public static IHtmlContent Hint(this IHtmlHelper helper, string value)
        {
            //create tag builder
            var builder = new TagBuilder("div");
            builder.MergeAttribute("title", value);
            builder.MergeAttribute("class", "ico-help");
            var icon = new StringBuilder();
            icon.Append("<i class='fa fa-question-circle'></i>");
            builder.InnerHtml.AppendHtml(icon.ToString());
            //render tag
            return new HtmlString(builder.ToHtmlString());
        }

        public static IHtmlContent NopLabelFor<TModel, TValue>(this IHtmlHelper<TModel> helper,
                Expression<Func<TModel, TValue>> expression, bool displayHint = true)
        {
            var result = new StringBuilder();
            var hintResource = string.Empty;
            object value;

            result.Append(helper.LabelFor(expression, new { title = hintResource, @class = "control-label" }).ToHtmlString());

            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, helper.ViewData, helper.MetadataProvider);
            if (metadata.Metadata.AdditionalValues.TryGetValue("NopResourceDisplayNameAttribute", out value))
            {
                var resourceDisplayName = value as NopResourceDisplayNameAttribute;
                if (resourceDisplayName != null && displayHint)
                {
                    var langId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                    hintResource = EngineContext.Current.Resolve<ILocalizationService>()
                        .GetResource(resourceDisplayName.ResourceKey + ".Hint",  langId, returnEmptyIfNotFound: true, logIfNotFound: false);
                    if (!String.IsNullOrEmpty(hintResource))
                    {
                        result.Append(helper.Hint(hintResource).ToHtmlString());
                    }
                }
            }

            var laberWrapper = new TagBuilder("div");
            laberWrapper.Attributes.Add("class", "label-wrapper");
            laberWrapper.InnerHtml.AppendHtml(result.ToString());

            return new HtmlString(laberWrapper.ToHtmlString());
        }

        public static IHtmlContent NopEditorFor<TModel, TValue>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, string postfix = "",
            bool? renderFormControlClass = null, bool required = false)
        {
            object htmlAttributes = null;
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, helper.ViewData, helper.MetadataProvider);
            if ((!renderFormControlClass.HasValue && metadata.ModelType.Name.Equals("String")) ||
                (renderFormControlClass.HasValue && renderFormControlClass.Value))
                htmlAttributes = new {@class = "form-control"};

            string result = "";
            var editorHtml = helper.EditorFor(expression, new {htmlAttributes, postfix}).ToHtmlString();
            if (required)
                result = string.Format("<div class=\"input-group input-group-required\">{0}<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>", editorHtml);
            else
                result = editorHtml;

            return new HtmlString(result);
        }

        public static IHtmlContent NopDropDownList<TModel>(this IHtmlHelper<TModel> helper, string name,
            IEnumerable<SelectListItem> itemList, object htmlAttributes = null, 
            bool renderFormControlClass = true, bool required = false)
        {
            var result = new StringBuilder();

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (renderFormControlClass)
                attrs = AddFormControlClassToHtmlAttributes(attrs);

            if (required)
                result.AppendFormat("<div class=\"input-group input-group-required\">{0}<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>",
                    helper.DropDownList(name, itemList, attrs).ToHtmlString());
            else
                result.Append(helper.DropDownList(name, itemList, attrs).ToHtmlString());

            return new HtmlString(result.ToString());
        }

        public static IHtmlContent NopDropDownListFor<TModel, TValue>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> itemList,
            object htmlAttributes = null, bool renderFormControlClass = true, bool required = false)
        {
            var result = new StringBuilder();

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (renderFormControlClass)
                attrs = AddFormControlClassToHtmlAttributes(attrs);

            if (required)
                result.AppendFormat("<div class=\"input-group input-group-required\">{0}<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>",
                    helper.DropDownListFor(expression, itemList, attrs).ToHtmlString());
            else
                result.Append(helper.DropDownListFor(expression, itemList, attrs).ToHtmlString());

            return new HtmlString(result.ToString());
        }

        public static IHtmlContent NopTextAreaFor<TModel, TValue>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null,
            bool renderFormControlClass = true, int rows = 4, int columns = 20, bool required = false)
        {
            var result = new StringBuilder();

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (renderFormControlClass)
                attrs = AddFormControlClassToHtmlAttributes(attrs);

            if (required)
                result.AppendFormat("<div class=\"input-group input-group-required\">{0}<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>",
                    helper.TextAreaFor(expression, rows, columns, attrs).ToHtmlString());
            else
                result.Append(helper.TextAreaFor(expression, rows, columns, attrs).ToHtmlString());

            return new HtmlString(result.ToString());
        }
        
        public static IDictionary<string, object> AddFormControlClassToHtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            //TODO test new implementation
            if (!htmlAttributes.ContainsKey("class"))
                htmlAttributes.Add("class", null);
            if (htmlAttributes["class"] == null || string.IsNullOrEmpty(htmlAttributes["class"].ToString()))
                htmlAttributes["class"] = "form-control";
            else
                if (!htmlAttributes["class"].ToString().Contains("form-control"))
                htmlAttributes["class"] += " form-control";

            return htmlAttributes;
        }
        
        #endregion
        
        #endregion
        
        #region Common extensions

        public static string RenderHtmlContent(this IHtmlContent htmlContent)
        {
            using (var writer = new StringWriter())
            {
                htmlContent.WriteTo(writer, HtmlEncoder.Default);
                var htmlOutput = writer.ToString();
                return htmlOutput;
            }
        }

        public static string ToHtmlString(this IHtmlContent tag)
        {
            using (var writer = new StringWriter())
            {
                tag.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Creates a days, months, years drop down list using an HTML select control. 
        /// The parameters represent the value of the "name" attribute on the select control.
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="dayName">"Name" attribute of the day drop down list.</param>
        /// <param name="monthName">"Name" attribute of the month drop down list.</param>
        /// <param name="yearName">"Name" attribute of the year drop down list.</param>
        /// <param name="beginYear">Begin year</param>
        /// <param name="endYear">End year</param>
        /// <param name="selectedDay">Selected day</param>
        /// <param name="selectedMonth">Selected month</param>
        /// <param name="selectedYear">Selected year</param>
        /// <param name="localizeLabels">Localize labels</param>
        /// <param name="htmlAttributes">HTML attributes</param>
		/// <param name="wrapTags">Wrap HTML select controls with span tags for styling/layout</param>
        /// <returns></returns>
        public static IHtmlContent DatePickerDropDowns(this IHtmlHelper html,
            string dayName, string monthName, string yearName,
            int? beginYear = null, int? endYear = null,
            int? selectedDay = null, int? selectedMonth = null, int? selectedYear = null,
            bool localizeLabels = true, object htmlAttributes = null, bool wrapTags = false)
        {
            var daysList = new TagBuilder("select");
            var monthsList = new TagBuilder("select");
            var yearsList = new TagBuilder("select");

            daysList.Attributes.Add("name", dayName);
            monthsList.Attributes.Add("name", monthName);
            yearsList.Attributes.Add("name", yearName);
            
            var htmlAttributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            daysList.MergeAttributes(htmlAttributesDictionary, true);
            monthsList.MergeAttributes(htmlAttributesDictionary, true);
            yearsList.MergeAttributes(htmlAttributesDictionary, true);

            var days = new StringBuilder();
            var months = new StringBuilder();
            var years = new StringBuilder();

            string dayLocale, monthLocale, yearLocale;
            if (localizeLabels)
            {
                var locService = EngineContext.Current.Resolve<ILocalizationService>();
                dayLocale = locService.GetResource("Common.Day");
                monthLocale = locService.GetResource("Common.Month");
                yearLocale = locService.GetResource("Common.Year");
            }
            else
            {
                dayLocale = "Day";
                monthLocale = "Month";
                yearLocale = "Year";
            }

            days.AppendFormat("<option value='{0}'>{1}</option>", "0", dayLocale);
            for (int i = 1; i <= 31; i++)
                days.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                    (selectedDay.HasValue && selectedDay.Value == i) ? " selected=\"selected\"" : null);


            months.AppendFormat("<option value='{0}'>{1}</option>", "0", monthLocale);
            for (int i = 1; i <= 12; i++)
            {
                months.AppendFormat("<option value='{0}'{1}>{2}</option>",
                                    i,
                                    (selectedMonth.HasValue && selectedMonth.Value == i) ? " selected=\"selected\"" : null,
                                    CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(i));
            }


            years.AppendFormat("<option value='{0}'>{1}</option>", "0", yearLocale);

            if (beginYear == null)
                beginYear = DateTime.UtcNow.Year - 100;
            if (endYear == null)
                endYear = DateTime.UtcNow.Year;

            if (endYear > beginYear)
            {
                for (int i = beginYear.Value; i <= endYear.Value; i++)
                    years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                        (selectedYear.HasValue && selectedYear.Value == i) ? " selected=\"selected\"" : null);
            }
            else
            {
                for (int i = beginYear.Value; i >= endYear.Value; i--)
                    years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                        (selectedYear.HasValue && selectedYear.Value == i) ? " selected=\"selected\"" : null);
            }

            daysList.InnerHtml.AppendHtml(days.ToString());
            monthsList.InnerHtml.AppendHtml(months.ToString());
            yearsList.InnerHtml.AppendHtml(years.ToString());

            if (wrapTags) 
            {
                string wrapDaysList = "<span class=\"days-list select-wrapper\">" + daysList.RenderHtmlContent() + "</span>";
                string wrapMonthsList = "<span class=\"months-list select-wrapper\">" + monthsList.RenderHtmlContent() + "</span>";
                string wrapYearsList = "<span class=\"years-list select-wrapper\">" + yearsList.RenderHtmlContent() + "</span>";

                return new HtmlString(string.Concat(wrapDaysList, wrapMonthsList, wrapYearsList));
            }
            else
            {
                return new HtmlString(string.Concat(daysList.RenderHtmlContent(), monthsList.RenderHtmlContent(), yearsList.RenderHtmlContent()));
            }

        }

        #endregion
    }
}