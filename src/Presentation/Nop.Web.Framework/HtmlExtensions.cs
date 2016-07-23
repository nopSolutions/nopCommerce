using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Framework
{
    public static class HtmlExtensions
    {
        #region Admin area extensions

        public static HelperResult LocalizedEditor<T, TLocalizedModelLocal>(this HtmlHelper<T> helper,
            string name,
            Func<int, HelperResult> localizedTemplate,
            Func<T, HelperResult> standardTemplate,
            bool ignoreIfSeveralStores = false)
            where T : ILocalizedModel<TLocalizedModelLocal>
            where TLocalizedModelLocal : ILocalizedModelLocal
        {
            return new HelperResult(writer =>
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
                        var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
                        var iconUrl = urlHelper.Content("~/Content/images/flags/" + language.FlagImageFileName);
                        tabStrip.AppendLine(string.Format("<a data-tab-name=\"{0}-{1}-tab\" href=\"#{0}-{1}-tab\" data-toggle=\"tab\"><img alt='' src='{2}'>{3}</a>",
                                name, 
                                HttpUtility.HtmlEncode(language.Name).ToLower(),
                                iconUrl,
                                HttpUtility.HtmlEncode(language.Name)));

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
                            HttpUtility.HtmlEncode(language.Name).ToLower()));
                        tabStrip.AppendLine(localizedTemplate(i).ToHtmlString());
                        tabStrip.AppendLine("</div>");
                    }
                    tabStrip.AppendLine("</div>");
                    tabStrip.AppendLine("</div>");
                    writer.Write(new MvcHtmlString(tabStrip.ToString()));
                }
                else
                {
                    standardTemplate(helper.ViewData.Model).WriteTo(writer);
                }
            });
        }

        public static MvcHtmlString DeleteConfirmation<T>(this HtmlHelper<T> helper, string buttonsSelector) where T : BaseNopEntityModel
        {
            return DeleteConfirmation(helper, "", buttonsSelector);
        }

        public static MvcHtmlString DeleteConfirmation<T>(this HtmlHelper<T> helper, string actionName,
            string buttonsSelector) where T : BaseNopEntityModel
        {
            if (String.IsNullOrEmpty(actionName))
                actionName = "Delete";

            var modalId = MvcHtmlString.Create(helper.ViewData.ModelMetadata.ModelType.Name.ToLower() + "-delete-confirmation")
                .ToHtmlString();

            var deleteConfirmationModel = new DeleteConfirmationModel
            {
                Id = helper.ViewData.Model.Id,
                ControllerName = helper.ViewContext.RouteData.GetRequiredString("controller"),
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

            return MvcHtmlString.Create(window.ToString());
        }

        public static MvcHtmlString ActionConfirmation(this HtmlHelper helper, string buttonId, string actionName = "")
        {
            if (string.IsNullOrEmpty(actionName))
                actionName = helper.ViewContext.RouteData.GetRequiredString("action");

            var modalId = MvcHtmlString.Create(buttonId + "-action-confirmation").ToHtmlString();

            var actionConfirmationModel = new ActionConfirmationModel()
            {
                ControllerName = helper.ViewContext.RouteData.GetRequiredString("controller"),
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

            return MvcHtmlString.Create(window.ToString());
        }

        public static MvcHtmlString OverrideStoreCheckboxFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            Expression<Func<TModel, TValue>> forInputExpression,
            int activeStoreScopeConfiguration)
        {
            var dataInputIds = new List<string>();
            dataInputIds.Add(helper.FieldIdFor(forInputExpression));
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, null, dataInputIds.ToArray());
        }
        public static MvcHtmlString OverrideStoreCheckboxFor<TModel, TValue1, TValue2>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            Expression<Func<TModel, TValue1>> forInputExpression1,
            Expression<Func<TModel, TValue2>> forInputExpression2,
            int activeStoreScopeConfiguration)
        {
            var dataInputIds = new List<string>();
            dataInputIds.Add(helper.FieldIdFor(forInputExpression1));
            dataInputIds.Add(helper.FieldIdFor(forInputExpression2));
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, null, dataInputIds.ToArray());
        }
        public static MvcHtmlString OverrideStoreCheckboxFor<TModel, TValue1, TValue2, TValue3>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            Expression<Func<TModel, TValue1>> forInputExpression1,
            Expression<Func<TModel, TValue2>> forInputExpression2,
            Expression<Func<TModel, TValue3>> forInputExpression3,
            int activeStoreScopeConfiguration)
        {
            var dataInputIds = new List<string>();
            dataInputIds.Add(helper.FieldIdFor(forInputExpression1));
            dataInputIds.Add(helper.FieldIdFor(forInputExpression2));
            dataInputIds.Add(helper.FieldIdFor(forInputExpression3));
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, null, dataInputIds.ToArray());
        }
        public static MvcHtmlString OverrideStoreCheckboxFor<TModel>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            string parentContainer,
            int activeStoreScopeConfiguration)
        {
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, parentContainer);
        }
        private static MvcHtmlString OverrideStoreCheckboxFor<TModel>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            int activeStoreScopeConfiguration,
            string parentContainer = null,
            params string[] datainputIds)
        {
            if (String.IsNullOrEmpty(parentContainer) && datainputIds == null)
                throw new ArgumentException("Specify at least one selector");

            var result = new StringBuilder();
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
                result.Append(helper.CheckBoxFor(expression, new Dictionary<string, object>
                {
                    { "class", cssClass },
                    { "onclick", onClick },
                    { "data-for-input-selector", dataInputSelector },
                }));
            }
            return MvcHtmlString.Create(result.ToString());
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
        public static MvcHtmlString RenderBootstrapTabContent(this HtmlHelper helper, string currentTabName,
            HelperResult content, bool isDefaultTab = false, string tabNameToSelect = "")
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

            if (string.IsNullOrEmpty(tabNameToSelect))
                tabNameToSelect = helper.GetSelectedTabName();

            if (string.IsNullOrEmpty(tabNameToSelect) && isDefaultTab)
                tabNameToSelect = currentTabName;

            var tag = new TagBuilder("div")
            {
                InnerHtml = content.ToHtmlString(),
                Attributes =
                {
                    new KeyValuePair<string, string>("class", string.Format("tab-pane{0}", tabNameToSelect == currentTabName ? " active" : "")),
                    new KeyValuePair<string, string>("id", string.Format("{0}", currentTabName))
                }
            };

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
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
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString RenderBootstrapTabHeader(this HtmlHelper helper, string currentTabName,
            LocalizedString title, bool isDefaultTab = false, string tabNameToSelect = "", string customCssClass = "")
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

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
                },
                InnerHtml = title.Text
            };
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
                InnerHtml = a.ToString(TagRenderMode.Normal)
            };

            return MvcHtmlString.Create(li.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Gets a selected tab name (used in admin area to store selected tab name)
        /// </summary>
        /// <returns>Name</returns>
        public static string GetSelectedTabName(this HtmlHelper helper)
        {
            //keep this method synchornized with
            //"SaveSelectedTab" method of \Administration\Controllers\BaseAdminController.cs
            var tabName = string.Empty;
            const string dataKey = "nop.selected-tab-name";

            if (helper.ViewData.ContainsKey(dataKey))
                tabName = helper.ViewData[dataKey].ToString();

            if (helper.ViewContext.Controller.TempData.ContainsKey(dataKey))
                tabName = helper.ViewContext.Controller.TempData[dataKey].ToString();

            return tabName;
        }

        #region Form fields

        public static MvcHtmlString Hint(this HtmlHelper helper, string value)
        {
            //create tag builder
            var builder = new TagBuilder("div");
            builder.MergeAttribute("title", value);
            builder.MergeAttribute("class", "ico-help");
            var icon = new StringBuilder();
            icon.Append("<i class='fa fa-question-circle'></i>");
            builder.InnerHtml = icon.ToString();
            //render tag
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString NopLabelFor<TModel, TValue>(this HtmlHelper<TModel> helper,
                Expression<Func<TModel, TValue>> expression, bool displayHint = true)
        {
            var result = new StringBuilder();
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var hintResource = string.Empty;
            object value;

            result.Append(helper.LabelFor(expression, new { title = hintResource, @class = "control-label" }));

            if (metadata.AdditionalValues.TryGetValue("NopResourceDisplayName", out value))
            {
                var resourceDisplayName = value as NopResourceDisplayName;
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
            laberWrapper.InnerHtml = result.ToString();

            return MvcHtmlString.Create(laberWrapper.ToString());
        }

        public static MvcHtmlString NopEditorFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, bool? renderFormControlClass = null)
        {
            var result = new StringBuilder();

            object htmlAttributes = null;
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if ((!renderFormControlClass.HasValue && metadata.ModelType.Name.Equals("String")) ||
                (renderFormControlClass.HasValue && renderFormControlClass.Value))
                htmlAttributes = new { @class = "form-control" };

            result.Append(helper.EditorFor(expression, new { htmlAttributes }));

            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString NopDropDownList<TModel>(this HtmlHelper<TModel> helper, string name,
            IEnumerable<SelectListItem> itemList, object htmlAttributes = null, bool renderFormControlClass = true)
        {
            var result = new StringBuilder();

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (renderFormControlClass)
                attrs = AddFormControlClassToHtmlAttributes(attrs);

            result.Append(helper.DropDownList(name, itemList, attrs));

            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString NopDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> itemList,
            object htmlAttributes = null, bool renderFormControlClass = true)
        {
            var result = new StringBuilder();

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (renderFormControlClass)
                attrs = AddFormControlClassToHtmlAttributes(attrs);

            result.Append(helper.DropDownListFor(expression, itemList, attrs));

            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString NopTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null,
            bool renderFormControlClass = true, int rows = 4, int columns = 20)
        {
            var result = new StringBuilder();

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (renderFormControlClass)
                attrs = AddFormControlClassToHtmlAttributes(attrs);

            result.Append(helper.TextAreaFor(expression, rows, columns, attrs));

            return MvcHtmlString.Create(result.ToString());
        }


        public static MvcHtmlString NopDisplayFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var result = new TagBuilder("div");
            result.Attributes.Add("class", "form-text-row");
            result.InnerHtml = helper.DisplayFor(expression).ToString();

            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString NopDisplay<TModel>(this HtmlHelper<TModel> helper, string expression)
        {
            var result = new TagBuilder("div");
            result.Attributes.Add("class", "form-text-row");
            result.InnerHtml = expression;

            return MvcHtmlString.Create(result.ToString());
        }

        public static RouteValueDictionary AddFormControlClassToHtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes["class"] == null || string.IsNullOrEmpty(htmlAttributes["class"].ToString()))
                htmlAttributes["class"] = "form-control";
            else
                if (!htmlAttributes["class"].ToString().Contains("form-control"))
                htmlAttributes["class"] += " form-control";

            return htmlAttributes as RouteValueDictionary;
        }

        #endregion

        #endregion

        #region Common extensions

        public static MvcHtmlString RequiredHint(this HtmlHelper helper, string additionalText = null)
        {
            // Create tag builder
            var builder = new TagBuilder("span");
            builder.AddCssClass("required");
            var innerText = "*";
            //add additional text if specified
            if (!String.IsNullOrEmpty(additionalText))
                innerText += " " + additionalText;
            builder.SetInnerText(innerText);
            // Render tag
            return MvcHtmlString.Create(builder.ToString());
        }

        public static string FieldNameFor<T, TResult>(this HtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            return html.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }
        public static string FieldIdFor<T, TResult>(this HtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            var id = html.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
            // because "[" and "]" aren't replaced with "_" in GetFullHtmlFieldId
            return id.Replace('[', '_').Replace(']', '_');
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
        public static MvcHtmlString DatePickerDropDowns(this HtmlHelper html,
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

            daysList.InnerHtml = days.ToString();
            monthsList.InnerHtml = months.ToString();
            yearsList.InnerHtml = years.ToString();

            if (wrapTags) 
            {
                string wrapDaysList = "<span class=\"days-list select-wrapper\">" + daysList + "</span>";
                string wrapMonthsList = "<span class=\"months-list select-wrapper\">" + monthsList + "</span>";
                string wrapYearsList = "<span class=\"years-list select-wrapper\">" + yearsList + "</span>";

                return MvcHtmlString.Create(string.Concat(wrapDaysList, wrapMonthsList, wrapYearsList));
            }
            else
            {
                return MvcHtmlString.Create(string.Concat(daysList, monthsList, yearsList));
            }

        }

        public static MvcHtmlString Widget(this HtmlHelper helper, string widgetZone, object additionalData = null, string area = null)
        {
            return helper.Action("WidgetsByZone", "Widget", new { widgetZone = widgetZone, additionalData = additionalData, area = area });
        }

        /// <summary>
        /// Renders the standard label with a specified suffix added to label text
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        /// <typeparam name="TValue">Value</typeparam>
        /// <param name="html">HTML helper</param>
        /// <param name="expression">Expression</param>
        /// <param name="htmlAttributes">HTML attributes</param>
        /// <param name="suffix">Suffix</param>
        /// <returns>Label</returns>
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes, string suffix)
        {
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string resolvedLabelText = metadata.DisplayName ?? (metadata.PropertyName ?? htmlFieldName.Split(new[] { '.' }).Last());
            if (string.IsNullOrEmpty(resolvedLabelText))
            {
                return MvcHtmlString.Empty;
            }
            var tag = new TagBuilder("label");
            tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName)));
            if (!String.IsNullOrEmpty(suffix))
            {
                resolvedLabelText = String.Concat(resolvedLabelText, suffix);
            }
            tag.SetInnerText(resolvedLabelText);

            var dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            tag.MergeAttributes(dictionary, true);

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        #endregion
    }
}