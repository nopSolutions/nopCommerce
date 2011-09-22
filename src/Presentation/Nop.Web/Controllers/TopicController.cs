using System.Web.Mvc;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Topics;
using Nop.Web.Extensions;

namespace Nop.Web.Controllers
{
    public class TopicController : BaseNopController
    {
        #region Fields

        private readonly ITopicService _topicService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public TopicController(ITopicService topicService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            this._topicService = topicService;
            this._workContext = workContext;
            this._localizationService = localizationService;
        }

        #endregion

        #region Methods

        public ActionResult TopicDetails(string systemName)
        {
            var topic = _topicService.GetTopicBySystemName(systemName);
            if (topic == null)
                return RedirectToAction("Index", "Home");

            var model = topic.ToModel();
            if (model.IsPasswordProtected)
            {
                model.Title = string.Empty;
                model.Body = string.Empty;
            }

            return View("TopicDetails", model);
        }

        public ActionResult TopicDetailsPopup(string systemName)
        {
            ViewBag.IsPopup = true;
            var topic = _topicService.GetTopicBySystemName(systemName);
            if (topic == null)
                return RedirectToAction("Index", "Home");

            var model = topic.ToModel();
            if (model.IsPasswordProtected)
            {
                model.Title = string.Empty;
                model.Body = string.Empty;
            }
            return View("TopicDetails", model);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult TopicBlock(string systemName)
        {
            var topic = _topicService.GetTopicBySystemName(systemName);
            if (topic == null)
                return Content("");

            var model = topic.ToModel();
            if (model.IsPasswordProtected)
            {
                model.Title = string.Empty;
                model.Body = string.Empty;
            }
            return PartialView(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Authenticate(int id, string password)
        {
            var authResult = false;
            var title = string.Empty;
            var body = string.Empty;
            var error = string.Empty;

            var topic = _topicService.GetTopicById(id);

            if (topic != null)
            {
                if (topic.Password != null && topic.Password.Equals(password))
                {
                    authResult = true;
                    title = topic.GetLocalized(x => x.Title);
                    body = topic.GetLocalized(x => x.Body);
                }
                else
                {
                    error = _localizationService.GetResource("Topic.WrongPassword");
                }
            }
            return Json(new { Authenticated = authResult, Title = title, Body = body, Error = error });
        }

        #endregion
    }
}
