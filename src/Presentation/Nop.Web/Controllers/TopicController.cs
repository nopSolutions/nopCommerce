using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class TopicController : BasePublicController
    {
        #region Fields

        protected IAclService AclService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected ITopicModelFactory TopicModelFactory { get; }
        protected ITopicService TopicService { get; }

        #endregion

        #region Ctor

        public TopicController(IAclService aclService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            ITopicModelFactory topicModelFactory,
            ITopicService topicService)
        {
            AclService = aclService;
            LocalizationService = localizationService;
            PermissionService = permissionService;
            StoreMappingService = storeMappingService;
            TopicModelFactory = topicModelFactory;
            TopicService = topicService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> TopicDetails(int topicId)
        {
            //allow administrators to preview any topic
            var hasAdminAccess = await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics);

            var model = await TopicModelFactory.PrepareTopicModelByIdAsync(topicId, hasAdminAccess);
            if (model == null)
                return InvokeHttp404();

            //display "edit" (manage) link
            if (hasAdminAccess)
                DisplayEditLink(Url.Action("Edit", "Topic", new { id = model.Id, area = AreaNames.Admin }));

            //template
            var templateViewPath = await TopicModelFactory.PrepareTemplateViewPathAsync(model.TopicTemplateId);
            return View(templateViewPath, model);
        }

        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> TopicDetailsPopup(string systemName)
        {
            var model = await TopicModelFactory.PrepareTopicModelBySystemNameAsync(systemName);
            if (model == null)
                return InvokeHttp404();

            ViewBag.IsPopup = true;

            //template
            var templateViewPath = await TopicModelFactory.PrepareTemplateViewPathAsync(model.TopicTemplateId);
            return PartialView(templateViewPath, model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Authenticate(int id, string password)
        {
            var authResult = false;
            var title = string.Empty;
            var body = string.Empty;
            var error = string.Empty;

            var topic = await TopicService.GetTopicByIdAsync(id);
            if (topic != null &&
                topic.Published &&
                //password protected?
                topic.IsPasswordProtected &&
                //store mapping
                await StoreMappingService.AuthorizeAsync(topic) &&
                //ACL (access control list)
                await AclService.AuthorizeAsync(topic))
            {
                if (topic.Password != null && topic.Password.Equals(password))
                {
                    authResult = true;
                    title = await LocalizationService.GetLocalizedAsync(topic, x => x.Title);
                    body = await LocalizationService.GetLocalizedAsync(topic, x => x.Body);
                }
                else
                {
                    error = await LocalizationService.GetResourceAsync("Topic.WrongPassword");
                }
            }

            return Json(new { Authenticated = authResult, Title = title, Body = body, Error = error });
        }

        #endregion
    }
}