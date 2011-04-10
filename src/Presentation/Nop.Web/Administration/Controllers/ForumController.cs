using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core.Domain.Forums;
using Nop.Services.Configuration;
using Nop.Services.Forums;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class ForumController : BaseNopController
    {
        private readonly IForumService _forumService;
        private ForumSettings _forumSettings;
        private readonly ISettingService _settingService;

        public ForumController(IForumService forumService,
            ForumSettings forumSettings, ISettingService settingService)
        {
            _forumService = forumService;
            _forumSettings = forumSettings;
            _settingService = settingService;
        }

        #region Methods
        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult List()
        {
            var forumGroupsModel = _forumService.GetAllForumGroups()
                .Select(x => x.ToModel())
                .ToList();
            return View(forumGroupsModel);
        }
        #endregion

        #region Settings
        public ActionResult Settings()
        {
            var model = _forumSettings.ToModel();
            model.ForumEditorValues = _forumSettings.ForumEditor.ToSelectList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Settings(ForumSettingsModel model)
        {
            _forumSettings = model.ToEntity(_forumSettings);
            _settingService.SaveSetting(_forumSettings);
            return RedirectToAction("Settings");
        }
        #endregion

        #region Create
        public ActionResult CreateForumGroup()
        {
            return View(new ForumGroupModel { DisplayOrder = 1 });
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult CreateForumGroup(ForumGroupModel model, bool continueEditing)
        {
            model.CreatedOnUtc = DateTime.UtcNow;
            model.UpdatedOnUtc = DateTime.UtcNow;
            var forumGroup = model.ToEntity();
            _forumService.InsertForumGroup(forumGroup);
            return continueEditing ? RedirectToAction("EditForumGroup", new { forumGroup.Id }) : RedirectToAction("List");
        }

        public ActionResult CreateForum()
        {
            var model = new ForumModel();
            model.ForumGroups = _forumService.GetAllForumGroups().ToList();
            model.DisplayOrder = 1;
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult CreateForum(ForumModel model, bool continueEditing)
        {
            model.CreatedOnUtc = DateTime.UtcNow;
            model.UpdatedOnUtc = DateTime.UtcNow;
            var forum = model.ToEntity();
            _forumService.InsertForum(forum);
            if (continueEditing)
            {
                model.ForumGroups = _forumService.GetAllForumGroups().ToList();
                return RedirectToAction("EditForum", new { forum.Id });
            }
            return RedirectToAction("List");
        }
        #endregion

        #region Edit
        public ActionResult EditForumGroup(int id)
        {
            var forumGroup = _forumService.GetForumGroupById(id);
            if (forumGroup == null) throw new ArgumentException("No Forum Group found with the specified id", "id");
            var model = forumGroup.ToModel();
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult EditForumGroup(ForumGroupModel model, bool continueEditing)
        {
            model.UpdatedOnUtc = DateTime.UtcNow;
            var forumGroup = _forumService.GetForumGroupById(model.Id);
            forumGroup = model.ToEntity(forumGroup);
            _forumService.UpdateForumGroup(forumGroup);
            return continueEditing ? RedirectToAction("EditForumGroup", forumGroup.Id) : RedirectToAction("List");
        }

        public ActionResult EditForum(int id)
        {
            var forum = _forumService.GetForumById(id);
            if (forum == null) throw new ArgumentException("No Forum found with the specified id", "id");
            var model = forum.ToModel();
            model.ForumGroups = _forumService.GetAllForumGroups().ToList();
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult EditForum(ForumModel model, bool continueEditing)
        {
            model.UpdatedOnUtc = DateTime.UtcNow;
            var forum = _forumService.GetForumById(model.Id);
            forum = model.ToEntity(forum);
            _forumService.UpdateForum(forum);

            if (continueEditing)
            {
                model.ForumGroups = _forumService.GetAllForumGroups().ToList();
                return RedirectToAction("EditForum", forum.Id);
            }
            return RedirectToAction("List");
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult DeleteForumGroup(int id)
        {
            var forumGroup = _forumService.GetForumGroupById(id);
            _forumService.DeleteForumGroup(forumGroup.Id);
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteForum(int id)
        {
            var forum = _forumService.GetForumById(id);
            _forumService.DeleteForum(forum.Id);
            return RedirectToAction("List");
        }
        #endregion
    }
}
