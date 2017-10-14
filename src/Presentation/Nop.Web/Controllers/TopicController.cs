using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    public partial class TopicController : BasePublicController
    {
        #region Fields

        private readonly ITopicModelFactory _topicModelFactory;
        private readonly ITopicService _topicService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAclService _aclService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public TopicController(ITopicModelFactory topicModelFactory,
            ITopicService topicService,
            ILocalizationService localizationService,
            IStoreMappingService storeMappingService,
            IAclService aclService,
            IPermissionService permissionService)
        {
            this._topicModelFactory = topicModelFactory;
            this._topicService = topicService;
            this._localizationService = localizationService;
            this._storeMappingService = storeMappingService;
            this._aclService = aclService;
            this._permissionService = permissionService;
        }

        #endregion
        
        #region Methods
        
        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult TopicDetails(int topicId)
        {
            var model = _topicModelFactory.PrepareTopicModelById(topicId);
            if (model == null)
                return RedirectToRoute("HomePage");

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                DisplayEditLink(Url.Action("Edit", "Topic", new { id = model.Id, area = AreaNames.Admin }));

            //template
            var templateViewPath = _topicModelFactory.PrepareTemplateViewPath(model.TopicTemplateId);
            return View(templateViewPath, model);
        }

        public virtual IActionResult TopicDetailsPopup(string systemName)
        {
            var model = _topicModelFactory.PrepareTopicModelBySystemName(systemName);
            if (model == null)
                return RedirectToRoute("HomePage");

            ViewBag.IsPopup = true;

            //template
            var templateViewPath = _topicModelFactory.PrepareTemplateViewPath(model.TopicTemplateId);
            return PartialView(templateViewPath, model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult Authenticate(int id, string password)
        {
            var authResult = false;
            var title = string.Empty;
            var body = string.Empty;
            var error = string.Empty;

            var topic = _topicService.GetTopicById(id);
            if (topic != null &&
                topic.Published &&
                //password protected?
                topic.IsPasswordProtected &&
                //store mapping
                _storeMappingService.Authorize(topic) &&
                //ACL (access control list)
                _aclService.Authorize(topic))
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