using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Services.Blogs;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class BlogController : BaseNopController
	{
		#region Fields

        private readonly IBlogService _blogService;
        private readonly ILanguageService _languageService;
        private readonly IDateTimeHelper _dateTimeHelper;

		#endregion Fields 

		#region Constructors

        public BlogController(IBlogService blogService, ILanguageService languageService,
            IDateTimeHelper dateTimeHelper)
        {
            this._blogService = blogService;
            this._languageService = languageService;
            this._dateTimeHelper = dateTimeHelper;
		}

		#endregion Constructors 

		#region BlogPosts

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
		{
			var blogPosts = _blogService.GetAllBlogPosts(0, null, null, 0, 10);
            var gridModel = new GridModel<BlogPostModel>
            {
                Data = blogPosts.Select(x =>
                {
                    var m = x.ToModel();
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    m.LanguageName = x.Language.Name;
                    m.Comments = x.BlogComments.Count;
                    return m;
                }),
                Total = blogPosts.TotalCount
            };
			return View(gridModel);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult List(GridCommand command)
        {
            var blogPosts = _blogService.GetAllBlogPosts(0, null, null, command.Page - 1, command.PageSize);
            var gridModel = new GridModel<BlogPostModel>
            {
                Data = blogPosts.Select(x =>
                {
                    var m = x.ToModel();
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    m.LanguageName = x.Language.Name;
                    m.Comments = x.BlogComments.Count;
                    return m;
                }),
                Total = blogPosts.TotalCount
            };
			return new JsonResult
			{
				Data = gridModel
			};
		}
        
        public ActionResult Create()
        {
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            var model = new BlogPostModel();
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(BlogPostModel model, bool continueEditing)
        {
            //decode body
            model.Body = HttpUtility.HtmlDecode(model.Body);

            if (ModelState.IsValid)
            {
                var blogPost = model.ToEntity();
                blogPost.CreatedOnUtc = DateTime.UtcNow;
                _blogService.InsertBlogPost(blogPost);
                return continueEditing ? RedirectToAction("Edit", new { id = blogPost.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            return View(model);
        }

		public ActionResult Edit(int id)
        {
            var blogPost = _blogService.GetBlogPostById(id);
            if (blogPost == null)
                throw new ArgumentException("No blog post found with the specified id", "id");

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            var model = blogPost.ToModel();
            return View(model);
		}
        
        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
		public ActionResult Edit(BlogPostModel model, bool continueEditing)
        {
            var blogPost = _blogService.GetBlogPostById(model.Id);
            if (blogPost == null)
                throw new ArgumentException("No blog post found with the specified id");

            //decode body
            model.Body = HttpUtility.HtmlDecode(model.Body);

            if (ModelState.IsValid)
            {
                blogPost = model.ToEntity(blogPost);
                _blogService.UpdateBlogPost(blogPost);
                return continueEditing ? RedirectToAction("Edit", new { id = blogPost.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            return View(model);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id)
        {
            var blogPost = _blogService.GetBlogPostById(id);
            if (blogPost == null)
                throw new ArgumentException("No blog post found with the specified id", "id");
            _blogService.DeleteBlogPost(blogPost);
			return RedirectToAction("List");
		}

		#endregion
    }
}
