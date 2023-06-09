using System.Net;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.Controllers
{
    /// <summary>
    /// Base controller
    /// </summary>
    [HttpsRequirement]
    [PublishModelEvents]
    [SignOutFromExternalAuthentication]
    [ValidatePassword]
    [SaveIpAddress]
    [SaveLastActivity]
    [SaveLastVisitedPage]
    [ForceMultiFactorAuthentication]
    public abstract partial class BaseController : Controller
    {
        #region Rendering

        /// <summary>
        /// Render component to string
        /// </summary>
        /// <param name="componentType">Component type</param>
        /// <param name="arguments">Arguments</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<string> RenderViewComponentToStringAsync(Type componentType, object arguments = null)
        {
            var helper = new DefaultViewComponentHelper(
                EngineContext.Current.Resolve<IViewComponentDescriptorCollectionProvider>(),
                HtmlEncoder.Default,
                EngineContext.Current.Resolve<IViewComponentSelector>(),
                EngineContext.Current.Resolve<IViewComponentInvokerFactory>(),
                EngineContext.Current.Resolve<IViewBufferScope>());

            using var writer = new StringWriter();
            var context = new ViewContext(ControllerContext, NullView.Instance, ViewData, TempData, writer, new HtmlHelperOptions());
            helper.Contextualize(context);
            var result = await helper.InvokeAsync(componentType, arguments);
            result.WriteTo(writer, HtmlEncoder.Default);
            await writer.FlushAsync();
            return writer.ToString();
        }

        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            //get Razor view engine
            var razorViewEngine = EngineContext.Current.Resolve<IRazorViewEngine>();

            //create action context
            var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor, ModelState);

            //set view name as action name in case if not passed
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.ActionDescriptor.ActionName;

            //set model
            ViewData.Model = model;

            //try to get a view by the name
            var viewResult = razorViewEngine.FindView(actionContext, viewName, false);
            if (viewResult.View == null)
            {
                //or try to get a view by the path
                viewResult = razorViewEngine.GetView(null, viewName, false);
                if (viewResult.View == null)
                    throw new ArgumentNullException($"{viewName} view was not found");
            }
            await using var stringWriter = new StringWriter();
            var viewContext = new ViewContext(actionContext, viewResult.View, ViewData, TempData, stringWriter, new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewContext);
            return stringWriter.GetStringBuilder().ToString();
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Error's JSON data
        /// </summary>
        /// <param name="error">Error text</param>
        /// <returns>Error's JSON data</returns>
        protected JsonResult ErrorJson(string error)
        {
            return Json(new
            {
                error
            });
        }

        /// <summary>
        /// Error's JSON data
        /// </summary>
        /// <param name="errors">Error messages</param>
        /// <returns>Error's JSON data</returns>
        protected JsonResult ErrorJson(object errors)
        {
            return Json(new
            {
                error = errors
            });
        }
        /// <summary>
        /// Display "Edit" (manage) link (in public store)
        /// </summary>
        /// <param name="editPageUrl">Edit page URL</param>
        protected virtual void DisplayEditLink(string editPageUrl)
        {
            var nopHtmlHelper = EngineContext.Current.Resolve<INopHtmlHelper>();

            nopHtmlHelper.AddEditPageUrl(editPageUrl);
        }

        #endregion

        #region Localization

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        protected virtual async Task AddLocalesAsync<TLocalizedModelLocal>(ILanguageService languageService,
            IList<TLocalizedModelLocal> locales) where TLocalizedModelLocal : ILocalizedLocaleModel
        {
            await AddLocalesAsync(languageService, locales, null);
        }

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <param name="configure">Configure action</param>
        protected virtual async Task AddLocalesAsync<TLocalizedModelLocal>(ILanguageService languageService,
            IList<TLocalizedModelLocal> locales, Action<TLocalizedModelLocal, int> configure) where TLocalizedModelLocal : ILocalizedLocaleModel
        {
            foreach (var language in await languageService.GetAllLanguagesAsync(true))
            {
                var locale = Activator.CreateInstance<TLocalizedModelLocal>();
                locale.LanguageId = language.Id;

                if (configure != null)
                    configure.Invoke(locale, locale.LanguageId);

                locales.Add(locale);
            }
        }


        #endregion

        #region Security

        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected virtual IActionResult AccessDeniedView()
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

            //return Challenge();
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = webHelper.GetRawUrl(Request) });
        }

        /// <summary>
        /// Access denied JSON data for DataTables
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the access denied JSON data
        /// </returns>
        protected virtual async Task<JsonResult> AccessDeniedDataTablesJson()
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            return ErrorJson(await localizationService.GetResourceAsync("Admin.AccessDenied.Description"));
        }

        #endregion

        #region Cards and tabs

        /// <summary>
        /// Save selected card name
        /// </summary>
        /// <param name="cardName">Card name to save</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request. Pass null to ignore</param>
        public virtual void SaveSelectedCardName(string cardName, bool persistForTheNextRequest = true)
        {
            //keep this method synchronized with
            //"GetSelectedCardName" method of \Nop.Web.Framework\Extensions\HtmlExtensions.cs
            if (string.IsNullOrEmpty(cardName))
                throw new ArgumentNullException(nameof(cardName));

            const string dataKey = "nop.selected-card-name";
            if (persistForTheNextRequest)
            {
                TempData[dataKey] = cardName;
            }
            else
            {
                ViewData[dataKey] = cardName;
            }
        }

        /// <summary>
        /// Save selected tab name
        /// </summary>
        /// <param name="tabName">Tab name to save; empty to automatically detect it</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request. Pass null to ignore</param>
        public virtual void SaveSelectedTabName(string tabName = "", bool persistForTheNextRequest = true)
        {
            //default root tab
            SaveSelectedTabName(tabName, "selected-tab-name", null, persistForTheNextRequest);
            //child tabs (usually used for localization)
            //Form is available for POST only
            if (!Request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.InvariantCultureIgnoreCase))
                return;

            foreach (var key in Request.Form.Keys)
                if (key.StartsWith("selected-tab-name-", StringComparison.InvariantCultureIgnoreCase))
                    SaveSelectedTabName(null, key, key["selected-tab-name-".Length..], persistForTheNextRequest);
        }

        /// <summary>
        /// Save selected tab name
        /// </summary>
        /// <param name="tabName">Tab name to save; empty to automatically detect it</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request. Pass null to ignore</param>
        /// <param name="formKey">Form key where selected tab name is stored</param>
        /// <param name="dataKeyPrefix">A prefix for child tab to process</param>
        protected virtual void SaveSelectedTabName(string tabName, string formKey, string dataKeyPrefix, bool persistForTheNextRequest)
        {
            //keep this method synchronized with
            //"GetSelectedTabName" method of \Nop.Web.Framework\Extensions\HtmlExtensions.cs
            if (string.IsNullOrEmpty(tabName))
            {
                tabName = Request.Form[formKey];
            }

            if (string.IsNullOrEmpty(tabName))
                return;

            var dataKey = "nop.selected-tab-name";
            if (!string.IsNullOrEmpty(dataKeyPrefix))
                dataKey += $"-{dataKeyPrefix}";

            if (persistForTheNextRequest)
            {
                TempData[dataKey] = tabName;
            }
            else
            {
                ViewData[dataKey] = tabName;
            }
        }

        #endregion
    }
}