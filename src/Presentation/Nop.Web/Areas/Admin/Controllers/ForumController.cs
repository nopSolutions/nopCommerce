using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Forums;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Forums;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ForumController : BaseAdminController
    {
        #region Fields

        protected IForumModelFactory ForumModelFactory { get; }
        protected IForumService ForumService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }

        #endregion

        #region Ctor

        public ForumController(IForumModelFactory forumModelFactory,
            IForumService forumService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService)
        {
            ForumModelFactory = forumModelFactory;
            ForumService = forumService;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
        }

        #endregion

        #region Methods

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            //prepare model
            var model = await ForumModelFactory.PrepareForumGroupSearchModelAsync(new ForumGroupSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ForumGroupList(ForumGroupSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ForumModelFactory.PrepareForumGroupListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ForumList(ForumSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return await AccessDeniedDataTablesJson();

            //try to get a forum group with the specified id
            var forumGroup = await ForumService.GetForumGroupByIdAsync(searchModel.ForumGroupId)
                ?? throw new ArgumentException("No forum group found with the specified id");

            //prepare model
            var model = await ForumModelFactory.PrepareForumListModelAsync(searchModel, forumGroup);

            return Json(model);
        }

        #endregion

        #region Create

        public virtual async Task<IActionResult> CreateForumGroup()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            //prepare model
            var model = await ForumModelFactory.PrepareForumGroupModelAsync(new ForumGroupModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateForumGroup(ForumGroupModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var forumGroup = model.ToEntity<ForumGroup>();
                forumGroup.CreatedOnUtc = DateTime.UtcNow;
                forumGroup.UpdatedOnUtc = DateTime.UtcNow;
                await ForumService.InsertForumGroupAsync(forumGroup);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Added"));

                return continueEditing ? RedirectToAction("EditForumGroup", new { forumGroup.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await ForumModelFactory.PrepareForumGroupModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> CreateForum()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            //prepare model
            var model = await ForumModelFactory.PrepareForumModelAsync(new ForumModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateForum(ForumModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var forum = model.ToEntity<Forum>();
                forum.CreatedOnUtc = DateTime.UtcNow;
                forum.UpdatedOnUtc = DateTime.UtcNow;
                await ForumService.InsertForumAsync(forum);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Added"));

                return continueEditing ? RedirectToAction("EditForum", new { forum.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await ForumModelFactory.PrepareForumModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Edit

        public virtual async Task<IActionResult> EditForumGroup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            //try to get a forum group with the specified id
            var forumGroup = await ForumService.GetForumGroupByIdAsync(id);
            if (forumGroup == null)
                return RedirectToAction("List");

            //prepare model
            var model = await ForumModelFactory.PrepareForumGroupModelAsync(null, forumGroup);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditForumGroup(ForumGroupModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            //try to get a forum group with the specified id
            var forumGroup = await ForumService.GetForumGroupByIdAsync(model.Id);
            if (forumGroup == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                forumGroup = model.ToEntity(forumGroup);
                forumGroup.UpdatedOnUtc = DateTime.UtcNow;
                await ForumService.UpdateForumGroupAsync(forumGroup);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Updated"));

                return continueEditing ? RedirectToAction("EditForumGroup", forumGroup.Id) : RedirectToAction("List");
            }

            //prepare model
            model = await ForumModelFactory.PrepareForumGroupModelAsync(model, forumGroup, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditForum(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            //try to get a forum with the specified id
            var forum = await ForumService.GetForumByIdAsync(id);
            if (forum == null)
                return RedirectToAction("List");

            //prepare model
            var model = await ForumModelFactory.PrepareForumModelAsync(null, forum);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditForum(ForumModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            //try to get a forum with the specified id
            var forum = await ForumService.GetForumByIdAsync(model.Id);
            if (forum == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                forum = model.ToEntity(forum);
                forum.UpdatedOnUtc = DateTime.UtcNow;
                await ForumService.UpdateForumAsync(forum);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Updated"));

                return continueEditing ? RedirectToAction("EditForum", forum.Id) : RedirectToAction("List");
            }

            //prepare model
            model = await ForumModelFactory.PrepareForumModelAsync(model, forum, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        public virtual async Task<IActionResult> DeleteForumGroup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            //try to get a forum group with the specified id
            var forumGroup = await ForumService.GetForumGroupByIdAsync(id);
            if (forumGroup == null)
                return RedirectToAction("List");

            await ForumService.DeleteForumGroupAsync(forumGroup);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteForum(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            //try to get a forum with the specified id
            var forum = await ForumService.GetForumByIdAsync(id);
            if (forum == null)
                return RedirectToAction("List");

            await ForumService.DeleteForumAsync(forum);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #endregion
    }
}