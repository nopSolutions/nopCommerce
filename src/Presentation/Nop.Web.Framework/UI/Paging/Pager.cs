//Contributor : MVCContrib

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.UI.Paging
{
    /// <summary>
    /// Renders a pager component from an IPageableModel datasource.
    /// </summary>
    public partial class Pager : IHtmlContent
    {
        /// <summary>
        /// Model
        /// </summary>
        protected readonly IPageableModel model;
        /// <summary>
        /// ViewContext
        /// </summary>
        protected readonly ViewContext viewContext;
	    /// <summary>
	    /// Page query string prameter name
	    /// </summary>
        protected string pageQueryName = "page";
	    /// <summary>
	    /// A value indicating whether to show Total summary
	    /// </summary>
        protected bool showTotalSummary;
        /// <summary>
        /// A value indicating whether to show pager items
        /// </summary>
        protected bool showPagerItems = true;
        /// <summary>
        /// A value indicating whether to show the first item
        /// </summary>
        protected bool showFirst = true;
        /// <summary>
        /// A value indicating whether to the previous item
        /// </summary>
        protected bool showPrevious = true;
        /// <summary>
        /// A value indicating whether to show the next item
        /// </summary>
        protected bool showNext = true;
        /// <summary>
        /// A value indicating whether to show the last item
        /// </summary>
        protected bool showLast = true;
        /// <summary>
        /// A value indicating whether to show individual page
        /// </summary>
        protected bool showIndividualPages = true;
        /// <summary>
        /// A value indicating whether to render empty query string parameters (without values)
        /// </summary>
        protected bool renderEmptyParameters = true;
        /// <summary>
        /// Number of individual page items to display
        /// </summary>
        protected int individualPagesDisplayedCount = 5;
        /// <summary>
        /// Boolean parameter names
        /// </summary>
        protected IList<string> booleanParameterNames;
		/// <summary>
		/// First page css class name
		/// </summary>
		protected string firstPageCssClass = "first-page";
		/// <summary>
		/// Previous page css class name
		/// </summary>
		protected string previousPageCssClass = "previous-page";
        /// <summary>
		/// Current page css class name
		/// </summary>
        protected string currentPageCssClass = "current-page";
		/// <summary>
		/// Individual page css class name
		/// </summary>
        protected string individualPageCssClass = "individual-page";
        /// <summary>
		/// Next page css class name
		/// </summary>
        protected string nextPageCssClass = "next-page";
        /// <summary>
		/// Last page css class name
		/// </summary>
        protected string lastPageCssClass = "last-page";
        /// <summary>
		/// Main ul css class name
		/// </summary>
        protected string mainUlCssClass = "";


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="context">ViewContext</param>
		public Pager(IPageableModel model, ViewContext context)
		{
            this.model = model;
            viewContext = context;
            booleanParameterNames = new List<string>();
		}

        /// <summary>
        /// ViewContext
        /// </summary>
		protected ViewContext ViewContext 
		{
			get { return viewContext; }
		}

        /// <summary>
        /// Set 
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager QueryParam(string value)
		{
            pageQueryName = value;
			return this;
        }
        /// <summary>
        /// Set a value indicating whether to show Total summary
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager ShowTotalSummary(bool value)
        {
            showTotalSummary = value;
            return this;
        }
        /// <summary>
        /// Set a value indicating whether to show pager items
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager ShowPagerItems(bool value)
        {
            showPagerItems = value;
            return this;
        }
        /// <summary>
        /// Set a value indicating whether to show the first item
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager ShowFirst(bool value)
        {
            showFirst = value;
            return this;
        }
        /// <summary>
        /// Set a value indicating whether to the previous item
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager ShowPrevious(bool value)
        {
            showPrevious = value;
            return this;
        }
        /// <summary>
        /// Set a  value indicating whether to show the next item
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager ShowNext(bool value)
        {
            showNext = value;
            return this;
        }
        /// <summary>
        /// Set a value indicating whether to show the last item
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager ShowLast(bool value)
        {
            showLast = value;
            return this;
        }
        /// <summary>
        /// Set number of individual page items to display
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager ShowIndividualPages(bool value)
        {
            showIndividualPages = value;
            return this;
        }
        /// <summary>
        /// Set a value indicating whether to render empty query string parameters (without values)
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager RenderEmptyParameters(bool value)
        {
            renderEmptyParameters = value;
            return this;
        }
        /// <summary>
        /// Set number of individual page items to display
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager IndividualPagesDisplayedCount(int value)
        {
            individualPagesDisplayedCount = value;
            return this;
        }
        /// <summary>
        /// little hack here due to ugly MVC implementation
        /// find more info here: http://www.mindstorminteractive.com/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
        /// </summary>
        /// <param name="paramName">Parameter name</param>
        /// <returns>Pager</returns>
        public Pager BooleanParameterName(string paramName)
        {
            booleanParameterNames.Add(paramName);
            return this;
        }
        /// <summary>
        /// Set first page pager css class name
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager FirstPageCssClass(string value) {
            firstPageCssClass = value;
            return this;
        }
        /// <summary>
        /// Set previous page pager css class name
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager PreviousPageCssClass(string value) {
            previousPageCssClass = value;
            return this;
        }
        /// <summary>
        /// Set current page pager css class name
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager CurrentPageCssClass(string value) {
            currentPageCssClass = value;
            return this;
        }
        /// <summary>
        /// Set individual page pager css class name
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager IndividualPageCssClass(string value) {
            individualPageCssClass = value;
            return this;
        }
        /// <summary>
        /// Set next page pager css class name
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager NextPageCssClass(string value) {
            nextPageCssClass = value;
            return this;
        }
        /// <summary>
        /// Set last page pager css class name
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager LastPageCssClass(string value) {
            lastPageCssClass = value;
            return this;
        }
        /// <summary>
        /// Set main ul css class name
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Pager</returns>
        public Pager MainUlCssClass(string value) {
            mainUlCssClass = value;
            return this;
        }

        /// <summary>
        /// Write control
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="encoder">Encoder</param>
	    public void WriteTo(TextWriter writer, HtmlEncoder encoder)
	    {
            var htmlString = GenerateHtmlString();
	        writer.Write(htmlString);
	    }
        /// <summary>
        /// Generate HTML control
        /// </summary>
        /// <returns>HTML control</returns>
	    public override string ToString()
	    {
	        return GenerateHtmlString();
	    }
        /// <summary>
        /// Generate HTML control
        /// </summary>
        /// <returns>HTML control</returns>
        public virtual string GenerateHtmlString()
		{
            if (model.TotalItems == 0) 
				return null;
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var links = new StringBuilder();
            if (showTotalSummary && (model.TotalPages > 0))
            {
                links.Append("<li class=\"total-summary\">");
                links.Append(string.Format(localizationService.GetResource("Pager.CurrentPage"), model.PageIndex + 1, model.TotalPages, model.TotalItems));
                links.Append("</li>");
            }
            if (showPagerItems && (model.TotalPages > 1))
            {
                if (showFirst)
                {
                    //first page
                    if ((model.PageIndex >= 3) && (model.TotalPages > individualPagesDisplayedCount))
                    {
                        links.Append(CreatePageLink(1, localizationService.GetResource("Pager.First"), firstPageCssClass));
                    }
                }
                if (showPrevious)
                {
                    //previous page
                    if (model.PageIndex > 0)
                    {
                        links.Append(CreatePageLink(model.PageIndex, localizationService.GetResource("Pager.Previous"), previousPageCssClass));
                    }
                }
                if (showIndividualPages)
                {
                    //individual pages
                    var firstIndividualPageIndex = GetFirstIndividualPageIndex();
                    var lastIndividualPageIndex = GetLastIndividualPageIndex();
                    for (var i = firstIndividualPageIndex; i <= lastIndividualPageIndex; i++)
                    {
                        if (model.PageIndex == i)
                        {
                            links.AppendFormat("<li class=\"" + currentPageCssClass + "\"><span>{0}</span></li>", (i + 1));
                        }
                        else
                        {
                            links.Append(CreatePageLink(i + 1, (i + 1).ToString(), individualPageCssClass));
                        }
                    }
                }
                if (showNext)
                {
                    //next page
                    if ((model.PageIndex + 1) < model.TotalPages)
                    {
                        links.Append(CreatePageLink(model.PageIndex + 2, localizationService.GetResource("Pager.Next"), nextPageCssClass));
                    }
                }
                if (showLast)
                {
                    //last page
                    if (((model.PageIndex + 3) < model.TotalPages) && (model.TotalPages > individualPagesDisplayedCount))
                    {
                        links.Append(CreatePageLink(model.TotalPages, localizationService.GetResource("Pager.Last"), lastPageCssClass));
                    }
                }
            }

            var result = links.ToString();
            if (!string.IsNullOrEmpty(result))
            {
                
                result = string.Format("<ul{0}>", string.IsNullOrEmpty(mainUlCssClass) ? "" : " class=\"" + mainUlCssClass + "\"") + result + "</ul>";
            }
            return result;
		}
        /// <summary>
        /// Is pager empty (only one page)?
        /// </summary>
        /// <returns>Result</returns>
	    public virtual bool IsEmpty()
	    {
            var html = GenerateHtmlString();
	        return string.IsNullOrEmpty(html);
	    }

        /// <summary>
        /// Get first individual page index
        /// </summary>
        /// <returns>Page index</returns>
        protected virtual int GetFirstIndividualPageIndex()
        {
            if ((model.TotalPages < individualPagesDisplayedCount) ||
                ((model.PageIndex - (individualPagesDisplayedCount / 2)) < 0))
            {
                return 0;
            }
            if ((model.PageIndex + (individualPagesDisplayedCount / 2)) >= model.TotalPages)
            {
                return (model.TotalPages - individualPagesDisplayedCount);
            }
            return (model.PageIndex - (individualPagesDisplayedCount / 2));
        }
        /// <summary>
        /// Get last individual page index
        /// </summary>
        /// <returns>Page index</returns>
        protected virtual int GetLastIndividualPageIndex()
        {
            var num = individualPagesDisplayedCount / 2;
            if ((individualPagesDisplayedCount % 2) == 0)
            {
                num--;
            }
            if ((model.TotalPages < individualPagesDisplayedCount) ||
                ((model.PageIndex + num) >= model.TotalPages))
            {
                return (model.TotalPages - 1);
            }
            if ((model.PageIndex - (individualPagesDisplayedCount / 2)) < 0)
            {
                return (individualPagesDisplayedCount - 1);
            }
            return (model.PageIndex + num);
        }
        /// <summary>
        /// Create page link
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="text">Text</param>
        /// <param name="cssClass">CSS class</param>
        /// <returns>Link</returns>
		protected virtual string CreatePageLink(int pageNumber, string text, string cssClass)
		{
            var liBuilder = new TagBuilder("li");
            if (!string.IsNullOrWhiteSpace(cssClass))
                liBuilder.AddCssClass(cssClass);

            var aBuilder = new TagBuilder("a");
            aBuilder.InnerHtml.AppendHtml(text);
            aBuilder.MergeAttribute("href", CreateDefaultUrl(pageNumber));

            liBuilder.InnerHtml.AppendHtml(aBuilder);
		    return liBuilder.RenderHtmlContent();
		}
        /// <summary>
        /// Create default URL
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <returns>URL</returns>
        protected virtual string CreateDefaultUrl(int pageNumber)
		{
            var routeValues = new RouteValueDictionary();

            var parametersWithEmptyValues = new List<string>();
			foreach (var key in viewContext.HttpContext.Request.Query.Keys.Where(key => key != null))
			{
			    //TODO test new implementation (QueryString, keys). And ensure no null exception is thrown when invoking ToString(). Is "StringValues.IsNullOrEmpty" required?
                var value = viewContext.HttpContext.Request.Query[key].ToString();
                if (renderEmptyParameters && string.IsNullOrEmpty(value))
			    {
                    //we store query string parameters with empty values separately
                    //we need to do it because they are not properly processed in the UrlHelper.GenerateUrl method (dropped for some reasons)
                    parametersWithEmptyValues.Add(key);
			    }
			    else
                {
                    if (booleanParameterNames.Contains(key, StringComparer.InvariantCultureIgnoreCase))
                    {
                        //little hack here due to ugly MVC implementation
                        //find more info here: http://www.mindstorminteractive.com/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
                        if (!string.IsNullOrEmpty(value) && value.Equals("true,false", StringComparison.InvariantCultureIgnoreCase))
                        {
                            value = "true";
                        }
                    }
                    routeValues[key] = value;
			    }
			}

            if (pageNumber > 1)
            {
                routeValues[pageQueryName] = pageNumber;
            }
            else
            {
                //SEO. we do not render pageindex query string parameter for the first page
                if (routeValues.ContainsKey(pageQueryName))
                {
                    routeValues.Remove(pageQueryName);
                }
            }

		    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
		    var url = webHelper.GetThisPageUrl(false);
		    foreach (var routeValue in routeValues)
		    {
		        url = webHelper.ModifyQueryString(url, routeValue.Key, routeValue.Value?.ToString());
		    }
            if (renderEmptyParameters && parametersWithEmptyValues.Any())
            {
                foreach (var key in parametersWithEmptyValues)
                {
                    url = webHelper.ModifyQueryString(url, key);
                }
            }
			return url;
		}

    }
}
