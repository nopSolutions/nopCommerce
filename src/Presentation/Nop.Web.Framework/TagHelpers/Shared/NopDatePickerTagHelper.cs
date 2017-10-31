using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Web.Framework.TagHelpers.Shared
{
    /// <summary>
    /// nop-date-picker tag helper
    /// </summary>
    [HtmlTargetElement("nop-date-picker", 
        Attributes = DayNameAttributeName + "," + MonthNameAttributeName + "," + YearNameAttributeName, 
        TagStructure = TagStructure.WithoutEndTag)]
    public class NopDatePickerTagHelper : TagHelper
    {
        private const string DayNameAttributeName = "asp-day-name";
        private const string MonthNameAttributeName = "asp-month-name";
        private const string YearNameAttributeName = "asp-year-name";

        private const string BeginYearAttributeName = "asp-begin-year";
        private const string EndYearAttributeName = "asp-end-year";

        private const string SelectedDayAttributeName = "asp-selected-day";
        private const string SelectedMonthAttributeName = "asp-selected-month";
        private const string SelectedYearAttributeName = "asp-selected-year";

        private const string LocalizeLabelsAttributeName = "asp-localize-labels";
        private const string WrapTagsAttributeName = "asp-wrap-tags";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// HtmlGenerator
        /// </summary>
        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// Day name
        /// </summary>
        [HtmlAttributeName(DayNameAttributeName)]
        public string DayName { get; set; }

        /// <summary>
        /// Month name
        /// </summary>
        [HtmlAttributeName(MonthNameAttributeName)]
        public string MonthName { get; set; }

        /// <summary>
        /// Year name
        /// </summary>
        [HtmlAttributeName(YearNameAttributeName)]
        public string YearName { get; set; }

        /// <summary>
        /// Begin year
        /// </summary>
        [HtmlAttributeName(BeginYearAttributeName)]
        public int? BeginYear { get; set; }

        /// <summary>
        /// End year
        /// </summary>
        [HtmlAttributeName(EndYearAttributeName)]
        public int? EndYear { get; set; }

        /// <summary>
        /// Selected day
        /// </summary>
        [HtmlAttributeName(SelectedDayAttributeName)]
        public int? SelectedDay { get; set; }

        /// <summary>
        /// Selected month
        /// </summary>
        [HtmlAttributeName(SelectedMonthAttributeName)]
        public int? SelectedMonth { get; set; }

        /// <summary>
        /// Selected year
        /// </summary>
        [HtmlAttributeName(SelectedYearAttributeName)]
        public int? SelectedYear { get; set; }

        /// <summary>
        /// Localize labels
        /// </summary>
        [HtmlAttributeName(LocalizeLabelsAttributeName)]
        public string LocalizeLabels { get; set; }

        /// <summary>
        /// Wrap tags
        /// </summary>
        [HtmlAttributeName(WrapTagsAttributeName)]
        public string WrapTags { get; set; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="generator">HTML generator</param>
        /// <param name="htmlHelper">HTML helper</param>
        public NopDatePickerTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
        {
            Generator = generator;
            _htmlHelper = htmlHelper;
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="output">Output</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "date-picker-wrapper");

            //build date selects
            var daysList = new TagBuilder("select");
            var monthsList = new TagBuilder("select");
            var yearsList = new TagBuilder("select");

            daysList.Attributes.Add("name", DayName);
            monthsList.Attributes.Add("name", MonthName);
            yearsList.Attributes.Add("name", YearName);

            var tagHelperAttributes = new List<string>
            {
                DayNameAttributeName,
                MonthNameAttributeName,
                YearNameAttributeName,
                BeginYearAttributeName,
                EndYearAttributeName,
                SelectedDayAttributeName,
                SelectedMonthAttributeName,
                SelectedYearAttributeName,
                LocalizeLabelsAttributeName,
                WrapTagsAttributeName
            };
            var customerAttributes = new Dictionary<string, object>();
            foreach (var attribute in context.AllAttributes)
            {
                if (!tagHelperAttributes.Contains(attribute.Name))
                    customerAttributes.Add(attribute.Name, attribute.Value);
            }
            var htmlAttributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(customerAttributes);
            daysList.MergeAttributes(htmlAttributesDictionary, true);
            monthsList.MergeAttributes(htmlAttributesDictionary, true);
            yearsList.MergeAttributes(htmlAttributesDictionary, true);

            var days = new StringBuilder();
            var months = new StringBuilder();
            var years = new StringBuilder();

            string dayLocale, monthLocale, yearLocale;
            if (bool.TryParse(LocalizeLabels, out bool localizeLabels) && localizeLabels)
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
            for (var i = 1; i <= 31; i++)
                days.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                    (SelectedDay.HasValue && SelectedDay.Value == i) ? " selected=\"selected\"" : null);

            months.AppendFormat("<option value='{0}'>{1}</option>", "0", monthLocale);
            for (var i = 1; i <= 12; i++)
            {
                months.AppendFormat("<option value='{0}'{1}>{2}</option>",
                    i,
                    (SelectedMonth.HasValue && SelectedMonth.Value == i) ? " selected=\"selected\"" : null,
                    CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(i));
            }
            
            years.AppendFormat("<option value='{0}'>{1}</option>", "0", yearLocale);

            if (BeginYear == null)
                BeginYear = DateTime.UtcNow.Year - 100;
            if (EndYear == null)
                EndYear = DateTime.UtcNow.Year;

            if (EndYear > BeginYear)
            {
                for (var i = BeginYear.Value; i <= EndYear.Value; i++)
                    years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                        (SelectedYear.HasValue && SelectedYear.Value == i) ? " selected=\"selected\"" : null);
            }
            else
            {
                for (var i = BeginYear.Value; i >= EndYear.Value; i--)
                    years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                        (SelectedYear.HasValue && SelectedYear.Value == i) ? " selected=\"selected\"" : null);
            }

            daysList.InnerHtml.AppendHtml(days.ToString());
            monthsList.InnerHtml.AppendHtml(months.ToString());
            yearsList.InnerHtml.AppendHtml(years.ToString());
            
            if (bool.TryParse(WrapTags, out bool wrapTags) && wrapTags)
            {
                var wrapDaysList = "<span class=\"days-list select-wrapper\">" + daysList + "</span>";
                var wrapMonthsList = "<span class=\"months-list select-wrapper\">" + monthsList + "</span>";
                var wrapYearsList = "<span class=\"years-list select-wrapper\">" + yearsList + "</span>";

                output.Content.AppendHtml(wrapDaysList);
                output.Content.AppendHtml(wrapMonthsList);
                output.Content.AppendHtml(wrapYearsList);
            }
            else
            {
                output.Content.AppendHtml(daysList);
                output.Content.AppendHtml(monthsList);
                output.Content.AppendHtml(yearsList);
            }
        }
    }
}