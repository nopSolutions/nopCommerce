using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Localization;
using Nop.Services.Topics;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Topics;

namespace Nop.Web.Controllers
{
    public partial class TopicController : BaseNopController
    {
        #region Fields

        private readonly ITopicService _topicService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public TopicController(ITopicService topicService,
            ILocalizationService localizationService,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ICacheManager cacheManager)
        {
            this._topicService = topicService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected TopicModel PrepareTopicModel(string systemName)
        {
            //load by store
            var topic = _topicService.GetTopicBySystemName(systemName, _storeContext.CurrentStore.Id);
            if (topic == null)
                return null;

            var model = new TopicModel()
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Title),
                Body = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Body),
                MetaKeywords = topic.GetLocalized(x => x.MetaKeywords),
                MetaDescription = topic.GetLocalized(x => x.MetaDescription),
                MetaTitle = topic.GetLocalized(x => x.MetaTitle),
            };
            return model;
        }

        #endregion

        #region Methods

        public ActionResult TopicDetails(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return RedirectToRoute("HomePage");
            return View("TopicDetails", cacheModel);
        }

        public ActionResult TopicDetailsPopup(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return RedirectToRoute("HomePage");

            ViewBag.IsPopup = true;
            return View("TopicDetails", cacheModel);
        }

        [ChildActionOnly]
        public ActionResult TopicBlock(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
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
