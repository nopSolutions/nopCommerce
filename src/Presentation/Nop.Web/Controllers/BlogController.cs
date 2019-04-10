using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Rss;
using Nop.Services.Blogs;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Blogs;

namespace Nop.Web.Controllers
{
    [HttpsRequirement(SslRequirement.No)]
    public partial class BlogController : BasePublicController
    {
        #region Fields

        private readonly BlogSettings _blogSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IBlogModelFactory _blogModelFactory;
        private readonly IBlogService _blogService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public BlogController(BlogSettings blogSettings,
            CaptchaSettings captchaSettings,
            IBlogModelFactory blogModelFactory,
            IBlogService blogService,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings)
        {
            _blogSettings = blogSettings;
            _captchaSettings = captchaSettings;
            _blogModelFactory = blogModelFactory;
            _blogService = blogService;
            _customerActivityService = customerActivityService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        public virtual IActionResult List(BlogPagingFilteringModel command)
        {
            if (!_blogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var model = _blogModelFactory.PrepareBlogPostListModel(command);
            return View("List", model);
        }

        public virtual IActionResult BlogByTag(BlogPagingFilteringModel command)
        {
            if (!_blogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var model = _blogModelFactory.PrepareBlogPostListModel(command);
            return View("List", model);
        }

        public virtual IActionResult BlogByMonth(BlogPagingFilteringModel command)
        {
            if (!_blogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var model = _blogModelFactory.PrepareBlogPostListModel(command);
            return View("List", model);
        }

        public virtual IActionResult ListRss(int languageId)
        {
            var feed = new RssFeed(
                $"{_localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name)}: Blog",
                "Blog",
                new Uri(_webHelper.GetStoreLocation()),
                DateTime.UtcNow);

            if (!_blogSettings.Enabled)
                return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

            var items = new List<RssItem>();
            var blogPosts = _blogService.GetAllBlogPosts(_storeContext.CurrentStore.Id, languageId);
            foreach (var blogPost in blogPosts)
            {
                var blogPostUrl = Url.RouteUrl("BlogPost", new { SeName = _urlRecordService.GetSeName(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false) }, _webHelper.CurrentRequestProtocol);
                items.Add(new RssItem(blogPost.Title, blogPost.Body, new Uri(blogPostUrl),
                    $"urn:store:{_storeContext.CurrentStore.Id}:blog:post:{blogPost.Id}", blogPost.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

        public virtual IActionResult BlogPost(int blogPostId)
        {
            if (!_blogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var blogPost = _blogService.GetBlogPostById(blogPostId);
            if (blogPost == null)
                return InvokeHttp404();

            var notAvailable =
                //availability dates
                !_blogService.BlogPostIsAvailable(blogPost) ||
                //Store mapping
                !_storeMappingService.Authorize(blogPost);
            //Check whether the current user has a "Manage blog" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageBlog);
            if (notAvailable && !hasAdminAccess)
                return InvokeHttp404();

            //display "edit" (manage) link
            if (hasAdminAccess)
                DisplayEditLink(Url.Action("BlogPostEdit", "Blog", new { id = blogPost.Id, area = AreaNames.Admin }));

            var model = new BlogPostModel();
            _blogModelFactory.PrepareBlogPostModel(model, blogPost, true);

            return View(model);
        }

        [HttpPost, ActionName("BlogPost")]
        [PublicAntiForgery]
        [FormValueRequired("add-comment")]
        [ValidateCaptcha]
        public virtual IActionResult BlogCommentAdd(int blogPostId, BlogPostModel model, bool captchaValid)
        {
            if (!_blogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var blogPost = _blogService.GetBlogPostById(blogPostId);
            if (blogPost == null || !blogPost.AllowComments)
                return RedirectToRoute("Homepage");

            if (_workContext.CurrentCustomer.IsGuest() && !_blogSettings.AllowNotRegisteredUsersToLeaveComments)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Blog.Comments.OnlyRegisteredUsersLeaveComments"));
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnBlogCommentPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var comment = new BlogComment
                {
                    BlogPostId = blogPost.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    CommentText = model.AddNewComment.CommentText,
                    IsApproved = !_blogSettings.BlogCommentsMustBeApproved,
                    StoreId = _storeContext.CurrentStore.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                blogPost.BlogComments.Add(comment);
                _blogService.UpdateBlogPost(blogPost);

                //notify a store owner
                if (_blogSettings.NotifyAboutNewBlogComments)
                    _workflowMessageService.SendBlogCommentNotificationMessage(comment, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddBlogComment",
                    _localizationService.GetResource("ActivityLog.PublicStore.AddBlogComment"), comment);

                //raise event
                if (comment.IsApproved)
                    _eventPublisher.Publish(new BlogCommentApprovedEvent(comment));

                //The text boxes should be cleared after a comment has been posted
                //That' why we reload the page
                TempData["nop.blog.addcomment.result"] = comment.IsApproved
                    ? _localizationService.GetResource("Blog.Comments.SuccessfullyAdded")
                    : _localizationService.GetResource("Blog.Comments.SeeAfterApproving");
                return RedirectToRoute("BlogPost", new { SeName = _urlRecordService.GetSeName(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false) });
            }

            //If we got this far, something failed, redisplay form
            _blogModelFactory.PrepareBlogPostModel(model, blogPost, true);
            return View(model);
        }

        #endregion
    }
}