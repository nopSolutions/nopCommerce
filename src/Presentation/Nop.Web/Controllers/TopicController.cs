using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Localization;
using Nop.Services.Topics;
using Nop.Web.Extensions;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Topics;

namespace Nop.Web.Controllers
{
    public partial class TopicController : BaseNopController
    {
        #region Fields

        private readonly ITopicService _topicService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public TopicController(ITopicService topicService,
            ILocalizationService localizationService,
            IWorkContext workContext, ICacheManager cacheManager)
        {
            this._topicService = topicService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected TopicModel PrepareTopicModel(string systemName)
        {
            var topic = _topicService.GetTopicBySystemName(systemName);
            if (topic == null)
                return null;

            var model = topic.ToModel();
            if (model.IsPasswordProtected)
            {
                model.Title = string.Empty;
                model.Body = string.Empty;
            }
            return model;
        }

        #endregion

        #region Methods

        public ActionResult TopicDetails(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return RedirectToRoute("HomePage");
            return View("TopicDetails", cacheModel);
        }

        public ActionResult TopicDetailsPopup(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return RedirectToRoute("HomePage");

            ViewBag.IsPopup = true;
            return View("TopicDetails", cacheModel);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult TopicBlock(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return Content("");

            return PartialView(cacheModel);
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
