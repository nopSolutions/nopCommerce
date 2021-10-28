using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Core.Rss;
using Nop.Services.Blogs;
using Nop.Services.Customers;
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
using Nop.Web.Models.Blogs;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class BlogController : BasePublicController
    {
        #region Fields

        protected BlogSettings BlogSettings { get; }
        protected CaptchaSettings CaptchaSettings { get; }
        protected IBlogModelFactory BlogModelFactory { get; }
        protected IBlogService BlogService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected LocalizationSettings LocalizationSettings { get; }

        #endregion

        #region Ctor

        public BlogController(BlogSettings blogSettings,
            CaptchaSettings captchaSettings,
            IBlogModelFactory blogModelFactory,
            IBlogService blogService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
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
            BlogSettings = blogSettings;
            CaptchaSettings = captchaSettings;
            BlogModelFactory = blogModelFactory;
            BlogService = blogService;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            EventPublisher = eventPublisher;
            LocalizationService = localizationService;
            PermissionService = permissionService;
            StoreContext = storeContext;
            StoreMappingService = storeMappingService;
            UrlRecordService = urlRecordService;
            WebHelper = webHelper;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            LocalizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List(BlogPagingFilteringModel command)
        {
            if (!BlogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var model = await BlogModelFactory.PrepareBlogPostListModelAsync(command);
            return View("List", model);
        }

        public virtual async Task<IActionResult> BlogByTag(BlogPagingFilteringModel command)
        {
            if (!BlogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var model = await BlogModelFactory.PrepareBlogPostListModelAsync(command);
            return View("List", model);
        }

        public virtual async Task<IActionResult> BlogByMonth(BlogPagingFilteringModel command)
        {
            if (!BlogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var model = await BlogModelFactory.PrepareBlogPostListModelAsync(command);
            return View("List", model);
        }

        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> ListRss(int languageId)
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var feed = new RssFeed(
                $"{await LocalizationService.GetLocalizedAsync(store, x => x.Name)}: Blog",
                "Blog",
                new Uri(WebHelper.GetStoreLocation()),
                DateTime.UtcNow);

            if (!BlogSettings.Enabled)
                return new RssActionResult(feed, WebHelper.GetThisPageUrl(false));

            var items = new List<RssItem>();
            var blogPosts = await BlogService.GetAllBlogPostsAsync(store.Id, languageId);
            foreach (var blogPost in blogPosts)
            {
                var blogPostUrl = Url.RouteUrl("BlogPost", new { SeName = await UrlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false) }, WebHelper.GetCurrentRequestProtocol());
                items.Add(new RssItem(blogPost.Title, blogPost.Body, new Uri(blogPostUrl),
                    $"urn:store:{store.Id}:blog:post:{blogPost.Id}", blogPost.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult(feed, WebHelper.GetThisPageUrl(false));
        }

        public virtual async Task<IActionResult> BlogPost(int blogPostId)
        {
            if (!BlogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var blogPost = await BlogService.GetBlogPostByIdAsync(blogPostId);
            if (blogPost == null)
                return InvokeHttp404();

            var notAvailable =
                //availability dates
                !BlogService.BlogPostIsAvailable(blogPost) ||
                //Store mapping
                !await StoreMappingService.AuthorizeAsync(blogPost);
            //Check whether the current user has a "Manage blog" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog);
            if (notAvailable && !hasAdminAccess)
                return InvokeHttp404();

            //display "edit" (manage) link
            if (hasAdminAccess)
                DisplayEditLink(Url.Action("BlogPostEdit", "Blog", new { id = blogPost.Id, area = AreaNames.Admin }));

            var model = new BlogPostModel();
            await BlogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);

            return View(model);
        }

        [HttpPost, ActionName("BlogPost")]        
        [FormValueRequired("add-comment")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> BlogCommentAdd(int blogPostId, BlogPostModel model, bool captchaValid)
        {
            if (!BlogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var blogPost = await BlogService.GetBlogPostByIdAsync(blogPostId);
            if (blogPost == null || !blogPost.AllowComments)
                return RedirectToRoute("Homepage");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await CustomerService.IsGuestAsync(customer) && !BlogSettings.AllowNotRegisteredUsersToLeaveComments)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Blog.Comments.OnlyRegisteredUsersLeaveComments"));
            }

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnBlogCommentPage && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var store = await StoreContext.GetCurrentStoreAsync();
                var comment = new BlogComment
                {
                    BlogPostId = blogPost.Id,
                    CustomerId = customer.Id,
                    CommentText = model.AddNewComment.CommentText,
                    IsApproved = !BlogSettings.BlogCommentsMustBeApproved,
                    StoreId = store.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                };

                await BlogService.InsertBlogCommentAsync(comment);

                //notify a store owner
                if (BlogSettings.NotifyAboutNewBlogComments)
                    await WorkflowMessageService.SendBlogCommentNotificationMessageAsync(comment, LocalizationSettings.DefaultAdminLanguageId);

                //activity log
                await CustomerActivityService.InsertActivityAsync("PublicStore.AddBlogComment",
                    await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.AddBlogComment"), comment);

                //raise event
                if (comment.IsApproved)
                    await EventPublisher.PublishAsync(new BlogCommentApprovedEvent(comment));

                //The text boxes should be cleared after a comment has been posted
                //That' why we reload the page
                TempData["nop.blog.addcomment.result"] = comment.IsApproved
                    ? await LocalizationService.GetResourceAsync("Blog.Comments.SuccessfullyAdded")
                    : await LocalizationService.GetResourceAsync("Blog.Comments.SeeAfterApproving");
                return RedirectToRoute("BlogPost", new { SeName = await UrlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false) });
            }

            //If we got this far, something failed, redisplay form
            await BlogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);
            
            return View(model);
        }

        #endregion
    }
}