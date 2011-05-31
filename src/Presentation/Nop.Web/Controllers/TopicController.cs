using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Controllers
{
    public class TopicController : BaseNopController
    {
		#region Fields

        private readonly ITopicService _topicService;
        private readonly IWorkContext _workContext;

        
        #endregion

		#region Constructors

        public TopicController(ITopicService topicService, 
            IWorkContext workContext)
        {
            this._topicService = topicService;
            this._workContext = workContext;
        }

		#endregion Constructors 
        
        #region Methods
        
        public ActionResult TopicDetails(string systemName)
        {
            var topic = _topicService.GetTopicBySystemName(systemName);
            if (topic == null)
                return RedirectToAction("Index", "Home");

            var model = topic.ToModel();
            return View("TopicDetails", model);
        }

        public ActionResult TopicDetailsPopup(string systemName)
        {
            ViewBag.IsPopup = true;
            var topic = _topicService.GetTopicBySystemName(systemName);
            if (topic == null)
                return RedirectToAction("Index", "Home");

            var model = topic.ToModel();
            return View("TopicDetails", model);
        }
        
        [ChildActionOnly]
        public ActionResult TopicBlock(string systemName)
        {
            var topic = _topicService.GetTopicBySystemName(systemName);
            if (topic == null)
                return Content("");

            var model = topic.ToModel();
            return PartialView(model);
        }

        #endregion
    }
}
