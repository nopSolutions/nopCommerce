using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models.PromotionFeeds;
using Nop.Services.PromotionFeed;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class PromotionFeedController : BaseNopController
	{
		#region Fields

        private readonly IPromotionFeedService _promotionFeedService;

		#endregion

		#region Constructors

        public PromotionFeedController(IPromotionFeedService promotionFeedService)
		{
            this._promotionFeedService = promotionFeedService;
		}

		#endregion 

        #region Methods

        public ActionResult List()
        {
            var feedsModel = new List<PromotionFeedModel>();
            var feeds = _promotionFeedService.LoadAllPromotionFeeds();
            foreach (var feed in feeds)
                feedsModel.Add(feed.ToModel());
            var gridModel = new GridModel<PromotionFeedModel>
            {
                Data = feedsModel,
                Total = feedsModel.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var feedsModel = new List<PromotionFeedModel>();
            var feeds = _promotionFeedService.LoadAllPromotionFeeds();
            foreach (var feed in feeds)
                feedsModel.Add(feed.ToModel());
            feedsModel = feedsModel.ForCommand(command).ToList();
            var gridModel = new GridModel<PromotionFeedModel>
            {
                Data = feedsModel,
                Total = feedsModel.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult ConfigureMethod(string systemName)
        {
            var feed = _promotionFeedService.LoadPromotionFeedBySystemName(systemName);
            if (feed == null) 
                throw new ArgumentException("No feed found with the specified system name", "systemName");

            var model = feed.ToModel();
            string actionName, controllerName;
            RouteValueDictionary routeValues;
            feed.GetConfigurationRoute(out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;
            return View(model);
        }

        #endregion
    }
}
