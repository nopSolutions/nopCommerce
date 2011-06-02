using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Admin.Models.Forums;
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

        public ForumController(IForumService forumService)
        {
            _forumService = forumService;
        }

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var forumGroupsModel = _forumService.GetAllForumGroups()
                .Select(x => x.ToModel())
                .ToList();
            return View(forumGroupsModel);
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
            if (ModelState.IsValid)
            {
                model.CreatedOnUtc = DateTime.UtcNow;
                model.UpdatedOnUtc = DateTime.UtcNow;
                var forumGroup = model.ToEntity();
                _forumService.InsertForumGroup(forumGroup);
                return continueEditing ? RedirectToAction("EditForumGroup", new { forumGroup.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
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
            if (ModelState.IsValid)
            {
                model.CreatedOnUtc = DateTime.UtcNow;
                model.UpdatedOnUtc = DateTime.UtcNow;
                var forum = model.ToEntity();
                _forumService.InsertForum(forum);
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
            var forumGroup = _forumService.GetForumGroupById(id);
            if (forumGroup == null) 
                throw new ArgumentException("No Forum Group found with the specified id", "id");
            var model = forumGroup.ToModel();
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult EditForumGroup(ForumGroupModel model, bool continueEditing)
        {
            var forumGroup = _forumService.GetForumGroupById(model.Id);

            if (ModelState.IsValid)
            {
                model.UpdatedOnUtc = DateTime.UtcNow;
                forumGroup = model.ToEntity(forumGroup);
                _forumService.UpdateForumGroup(forumGroup);
                return continueEditing ? RedirectToAction("EditForumGroup", forumGroup.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult EditForum(int id)
        {
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
            var forum = _forumService.GetForumById(model.Id);
            if (ModelState.IsValid)
            {
                model.UpdatedOnUtc = DateTime.UtcNow;
                forum = model.ToEntity(forum);
                _forumService.UpdateForum(forum);

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
