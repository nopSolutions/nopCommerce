using System.IO;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Stores;

namespace Nop.Web.Framework.Controllers
{
    public static class ContollerExtensions
    {
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this Controller controller)
        {
            return RenderPartialViewToString(controller, null, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <param name="viewName">View name</param>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this Controller controller, string viewName)
        {
            return RenderPartialViewToString(controller, viewName, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this Controller controller, object model)
        {
            return RenderPartialViewToString(controller, null, model);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public static string RenderPartialViewToString(this Controller controller, string viewName, object model)
        {
            //Original source code: http://craftycodeblog.com/2010/05/15/asp-net-mvc-render-partial-view-to-string/
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Get active store scope (for multi-store configuration mode)
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <param name="storeService">Store service</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Store ID; 0 if we are in a shared mode</returns>
        public static int GetActiveStoreScopeConfiguration(this Controller controller, IStoreService storeService, IWorkContext workContext)
        {
            //ensure that we have 2 (or more) stores
            if (storeService.GetAllStores().Count < 2)
                return 0;


            var storeId = workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
            var store = storeService.GetStoreById(storeId);
            return store != null ? store.Id : 0;
        }
    }
}
