
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Common;
using Nop.Web.Models.LiveChat;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;

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
