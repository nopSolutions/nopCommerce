using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Forums;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Services.Security;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class ForumController : BaseNopController
    {
        private readonly IForumService _forumService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        public ForumController(IForumService forumService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._forumService = forumService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forumGroupsModel = _forumService.GetAllForumGroups()
                .Select(fg =>
                {
                    var forumGroupModel = fg.ToModel();
                    forumGroupModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(fg.CreatedOnUtc, DateTimeKind.Utc);
                    foreach (var f in fg.Forums.OrderBy(x=>x.DisplayOrder))
                    {
                        var forumModel = f.ToModel();
                        forumModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(f.CreatedOnUtc, DateTimeKind.Utc);
                        forumGroupModel.ForumModels.Add(forumModel);
                    }
                    return forumGroupModel;
                })
                .ToList();
            return View(forumGroupsModel);
        }

        #endregion

        #region Create

        public ActionResult CreateForumGroup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            return View(new ForumGroupModel { DisplayOrder = 1 });
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult CreateForumGroup(ForumGroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var forumGroup = model.ToEntity();
                forumGroup.CreatedOnUtc = DateTime.UtcNow;
                forumGroup.UpdatedOnUtc = DateTime.UtcNow;
                _forumService.InsertForumGroup(forumGroup);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.ForumGroup.Added"));
                return continueEditing ? RedirectToAction("EditForumGroup", new { forumGroup.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult CreateForum()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var model = new ForumModel();
            model.ForumGroups = _forumService.GetAllForumGroups().ToList();
            model.DisplayOrder = 1;
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult CreateForum(ForumModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var forum = model.ToEntity();
                forum.CreatedOnUtc = DateTime.UtcNow;
                forum.UpdatedOnUtc = DateTime.UtcNow;
                _forumService.InsertForum(forum);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.Forum.Added"));
                return continueEditing ? RedirectToAction("EditForum", new { forum.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.ForumGroups = _forumService.GetAllForumGroups().ToList();
            return View(model);
        }

        #endregion

        #region Edit

        public ActionResult EditForumGroup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forumGroup = _forumService.GetForumGroupById(id);
            if (forumGroup == null) 
                throw new ArgumentException("No Forum Group found with the specified id", "id");
            var model = forumGroup.ToModel();
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult EditForumGroup(ForumGroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forumGroup = _forumService.GetForumGroupById(model.Id);

            if (ModelState.IsValid)
            {
                forumGroup = model.ToEntity(forumGroup);
                forumGroup.UpdatedOnUtc = DateTime.UtcNow;
                _forumService.UpdateForumGroup(forumGroup);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.ForumGroup.Updated"));
                return continueEditing ? RedirectToAction("EditForumGroup", forumGroup.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult EditForum(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forum = _forumService.GetForumById(id);
            if (forum == null) 
                throw new ArgumentException("No Forum found with the specified id", "id");
            var model = forum.ToModel();
            model.ForumGroups = _forumService.GetAllForumGroups().ToList();
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult EditForum(ForumModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forum = _forumService.GetForumById(model.Id);
            if (ModelState.IsValid)
            {
                forum = model.ToEntity(forum);
                forum.UpdatedOnUtc = DateTime.UtcNow;
                _forumService.UpdateForum(forum);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.Forum.Updated"));
                return continueEditing ? RedirectToAction("EditForum", forum.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.ForumGroups = _forumService.GetAllForumGroups().ToList();
            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        public ActionResult DeleteForumGroup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forumGroup = _forumService.GetForumGroupById(id);
            _forumService.DeleteForumGroup(forumGroup);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.ForumGroup.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteForum(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageForums))
                return AccessDeniedView();

            var forum = _forumService.GetForumById(id);
            _forumService.DeleteForum(forum);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Forums.Forum.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }
}
