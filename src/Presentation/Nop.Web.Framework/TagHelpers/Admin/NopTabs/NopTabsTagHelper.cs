using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin.NopTabs
{
    [HtmlTargetElement("nop-tabs", Attributes = IdAttributeName)]
    public class NopTabsTagHelper : TagHelper
    {
        private const string IdAttributeName = "id";
        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopTabsTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

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

            var tabsClass = "nav-tabs-custom";
            //merge classes
            var classValue = output.Attributes.ContainsName("class")
                ? $"{output.Attributes["class"].Value} {tabsClass}"
                : tabsClass;
            output.Attributes.SetAttribute("class", classValue);

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //render input contains selected tab name
            var selectedTabInput = new TagBuilder("input");
            selectedTabInput.Attributes.Add("type", "hidden");
            selectedTabInput.Attributes.Add("id", "selected-tab-name");
            selectedTabInput.Attributes.Add("name", "selected-tab-name");
            selectedTabInput.Attributes.Add("value", _htmlHelper.GetSelectedTabName());
            output.PreContent.SetHtmlContent(selectedTabInput.RenderHtmlContent());

            //render tabs script
            if (output.Attributes.ContainsName("id"))
            {
                var script = new TagBuilder("script");
                script.InnerHtml.AppendHtml("$(document).ready(function () {bindBootstrapTabSelectEvent('" + output.Attributes["id"].Value + "');});");
                output.PostContent.SetHtmlContent(script.RenderHtmlContent());
            }

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}