using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Services.Common;
using Nop.Web.Models.LiveChat;

namespace Nop.Web.Controllers
{
    public class LiveChatController : BaseNopController
    {
		#region Fields

        private readonly ILiveChatService _liveChatService;

        #endregion

		#region Constructors

        public LiveChatController(ILiveChatService liveChatService)
        {
            this._liveChatService = liveChatService;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult List()
        {
            //model
            var model = new List<LiveChatModel>();

            var providers = _liveChatService.LoadActiveLiveChatProviders();
            foreach (var lcp in providers)
            {
                var lcpModel = new LiveChatModel();
                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                lcp.GetPublicInfoRoute(out actionName, out controllerName, out routeValues);
                lcpModel.ActionName = actionName;
                lcpModel.ControllerName = controllerName;
                lcpModel.RouteValues = routeValues;

                model.Add(lcpModel);
            }

            return PartialView(model);
        }

        #endregion
    }
}
