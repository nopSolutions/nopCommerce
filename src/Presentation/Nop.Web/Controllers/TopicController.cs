using System.Web.Mvc;
using Nop.Core;
using Nop.Services.Topics;
using Nop.Web.Extensions;

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

        #endregion

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
