
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Admin.Controllers
{

    [Area("Admin")]
    [HttpsRequirement(SslRequirement.Yes)]
    [AdminAntiForgery]
    [ValidateIpAddress]
    [AuthorizeAdmin]
    [ValidateVendor]
    public abstract partial class BaseAdminController : BaseController
    {

        /// <summary>
        /// Save selected TAB name
        /// </summary>
        /// <param name="tabName">Tab name to save; empty to automatically detect it</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void SaveSelectedTabName(string tabName = "", bool persistForTheNextRequest = true)
        {
            //keep this method synchronized with
            //"GetSelectedTabName" method of \Nop.Web.Framework\HtmlExtensions.cs
            if (string.IsNullOrEmpty(tabName))
            {
                tabName = this.Request.Form["selected-tab-name"];
            }
            
            if (!string.IsNullOrEmpty(tabName))
            {
                const string dataKey = "nop.selected-tab-name";
                if (persistForTheNextRequest)
                {
                    TempData[dataKey] = tabName;
                }
                else
                {
                    ViewData[dataKey] = tabName;
                }
            }
        }
        
        #if NET451
        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.JsonResult"/> object that serializes the specified object to JavaScript Object Notation (JSON) format using the content type, content encoding, and the JSON request behavior.
        /// </summary>
        /// 
        /// <returns>
        /// The result object that serializes the specified object to JSON format.
        /// </returns>
        /// <param name="data">The JavaScript object graph to serialize.</param>
        /// <param name="contentType">The content type (MIME type).</param>
        /// <param name="contentEncoding">The content encoding.</param>
        /// <param name="behavior">The JSON request behavior</param>
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            //Json fix issue with dates in KendoUI grid
            //use json with IsoDateTimeConverter
            var result = EngineContext.Current.Resolve<AdminAreaSettings>().UseIsoDateTimeConverterInJson
                ? new ConverterJsonResult(new IsoDateTimeConverter()) : new JsonResult();

            result.Data = data;
            result.ContentType = contentType;
            result.ContentEncoding = contentEncoding;
            result.JsonRequestBehavior = behavior;

            //Json fix for admin area
            //sometime our entities have big text values returned (e.g. product desriptions)
            //of course, we can set and return them as "empty" (we already do it so). Furthermore, it's a perfoemance optimization
            //but it's better to avoid exceptions for other entities and allow maximum JSON length
            result.MaxJsonLength = int.MaxValue;

            return result;
            //return base.Json(data, contentType, contentEncoding, behavior);
        }
        
        #endif
    }
}