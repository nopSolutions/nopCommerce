using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Services.Blogs;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Blogs;

namespace Nop.Web.Controllers
{
    public class BlogController : BaseNopController
    {
		#region Fields

        private readonly IBlogService _blogService;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerContentService _customerContentService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWebHelper _webHelper;

        private readonly MediaSettings _mediaSettings;
        private readonly BlogSettings _blogSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        
        #endregion

		#region Constructors

        public BlogController(IBlogService blogService, 
            IWorkContext workContext, IPictureService pictureService, ILocalizationService localizationService,
            ICustomerContentService customerContentService, IDateTimeHelper dateTimeHelper,
            IWorkflowMessageService workflowMessageService, IWebHelper webHelper,
            MediaSettings mediaSettings, BlogSettings blogSettings,
            LocalizationSettings localizationSettings, CustomerSettings customerSettings,
            StoreInformationSettings storeInformationSettings)
        {
            this._blogService = blogService;
            this._workContext = workContext;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._customerContentService = customerContentService;
            this._dateTimeHelper = dateTimeHelper;
            this._workflowMessageService = workflowMessageService;
            this._webHelper = webHelper;

            this._mediaSettings = mediaSettings;
            this._blogSettings = blogSettings;
            this._localizationSettings = localizationSettings;
            this._customerSettings = customerSettings;
            this._storeInformationSettings = storeInformationSettings;
        }

		#endregion

        #region Utilities

        [NonAction]
        private void PrepareBlogPostModel(BlogPostModel model, BlogPost blogPost, bool prepareComments)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost");

            if (model == null)
                throw new ArgumentNullException("model");

            model.Id = blogPost.Id;
            model.SeName = blogPost.GetSeName();
            model.Title = blogPost.Title;
            model.Body = blogPost.Body;
            model.AllowComments = blogPost.AllowComments;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(blogPost.CreatedOnUtc, DateTimeKind.Utc);
            model.Tags = blogPost.ParseTags().ToList();
            model.NumberOfComments = blogPost.BlogComments.Count;
            if (prepareComments)
            {
                var blogComments = blogPost.BlogComments.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc);
                foreach (var bc in blogComments)
                {
                    var commentModel = new BlogCommentModel()
                    {
                        Id = bc.Id,
                        CustomerId = bc.CustomerId,
                        CustomerName = bc.Customer.FormatUserName(),
                        CommentText = bc.CommentText,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(bc.CreatedOnUtc, DateTimeKind.Utc),
                        AllowViewingProfiles = _customerSettings.AllowViewingProfiles && bc.Customer != null && !bc.Customer.IsGuest(),
                    };
                    if (_customerSettings.AllowCustomersToUploadAvatars)
                    {
                        var customer = bc.Customer;
                        string avatarUrl = _pictureService.GetPictureUrl(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId), _mediaSettings.AvatarPictureSize, false);
                        if (String.IsNullOrEmpty(avatarUrl) && _customerSettings.DefaultAvatarEnabled)
                            avatarUrl = _pictureService.GetDefaultPictureUrl(_mediaSettings.AvatarPictureSize, PictureType.Avatar);
                        commentModel.CustomerAvatarUrl = avatarUrl;
                    }
                    model.Comments.Add(commentModel);
                }
            }
        }
        
        #endregion

        #region Methods

        public ActionResult List(BlogPagingFilteringModel command)
        {
            if (!_blogSettings.Enabled)
                return RedirectToAction("Index", "Home");

            var model = new BlogPostListModel();
            model.PagingFilteringContext.Tag = command.Tag;
            model.PagingFilteringContext.Month = command.Month;
            model.WorkingLanguageId = _workContext.WorkingLanguage.Id;

            if (command.PageSize <= 0) command.PageSize = _blogSettings.PostsPageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            DateTime? dateFrom = command.GetFromMonth();
            DateTime? dateTo = command.GetToMonth();

            IList<BlogPost> blogPosts;
            if (String.IsNullOrEmpty(command.Tag))
            {
                blogPosts = _blogService.GetAllBlogPosts(_workContext.WorkingLanguage.Id,
                    dateFrom, dateTo, command.PageNumber - 1, command.PageSize);
                model.PagingFilteringContext.LoadPagedList(blogPosts as IPagedList<BlogPost>);
            }
            else
            {
                blogPosts = _blogService.GetAllBlogPostsByTag(_workContext.WorkingLanguage.Id, command.Tag);
            }

            model.BlogPosts = blogPosts
                .Select(x =>
                {
                    var blogPostModel = new BlogPostModel();
                    PrepareBlogPostModel(blogPostModel, x, false);
                    return blogPostModel;
                })
                .ToList();

            return View(model);
        }

        public ActionResult ListRss(int languageId)
        {
            var feed = new SyndicationFeed(
                                    string.Format("{0}: Blog", _storeInformationSettings.StoreName),
                                    "Blog",
                                    new Uri(_webHelper.GetStoreLocation(false)),
                                    "BlogRSS",
                                    DateTime.UtcNow);

            if (!_blogSettings.Enabled)
                return new RssActionResult() { Feed = feed };

            var items = new List<SyndicationItem>();
            var blogPosts = _blogService.GetAllBlogPosts(languageId,
                null, null, 0, int.MaxValue);
            foreach (var blogPost in blogPosts)
            {
                string topicUrl = Url.RouteUrl("BlogPost", new { blogPostId = blogPost.Id, SeName = blogPost.GetSeName() }, "http");
                items.Add(new SyndicationItem(blogPost.Title, blogPost.Body, new Uri(topicUrl), String.Format("Blog:{0}", blogPost.Id), blogPost.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult() { Feed = feed };
        }

        public ActionResult BlogPost(int blogPostId)
        {
            if (!_blogSettings.Enabled)
                return RedirectToAction("Index", "Home");

            var blogPost = _blogService.GetBlogPostById(blogPostId);
            if (blogPost == null)
                return RedirectToAction("Index", "Home");

            var model = new BlogPostModel();
            PrepareBlogPostModel(model, blogPost, true);

            return View(model);
        }

        [HttpPost, ActionName("BlogPost")]
        [FormValueRequired("add-comment")]
        public ActionResult BlogCommentAdd(int blogPostId, BlogPostModel model)
        {
            if (!_blogSettings.Enabled)
                return RedirectToAction("Index", "Home");

            var blogPost = _blogService.GetBlogPostById(blogPostId);
            if (blogPost == null || !blogPost.AllowComments)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                if (_workContext.CurrentCustomer.IsGuest() && !_blogSettings.AllowNotRegisteredUsersToLeaveComments)
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Blog.Comments.OnlyRegisteredUsersLeaveComments"));
                }
                else
                {
                    var comment = new BlogComment()
                    {
                        BlogPostId = blogPost.Id,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        IpAddress = _webHelper.GetCurrentIpAddress(),
                        CommentText = model.AddNewComment.CommentText,
                        IsApproved = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                    };
                    _customerContentService.InsertCustomerContent(comment);

                    //notify store owner
                    if (_blogSettings.NotifyAboutNewBlogComments)
                        _workflowMessageService.SendBlogCommentNotificationMessage(comment, _localizationSettings.DefaultAdminLanguageId);


                    PrepareBlogPostModel(model, blogPost, true);
                    model.AddNewComment.CommentText = null;

                    model.AddNewComment.Result = _localizationService.GetResource("Blog.Comments.SuccessfullyAdded");

                    return View(model);
                }
            }

            //If we got this far, something failed, redisplay form
            PrepareBlogPostModel(model, blogPost, true);
            return View(model);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult BlogTags()
        {
            if (!_blogSettings.Enabled)
                return Content("");

            var model = new BlogPostTagListModel();

            //get tags
            var tags = _blogService.GetAllBlogPostTags(_workContext.WorkingLanguage.Id)
                .OrderByDescending(x => x.BlogPostCount)
                .Take(_blogSettings.NumberOfTags)
                .ToList();
            //sorting
            tags = tags.OrderBy(x => x.Name).ToList();

            foreach (var tag in tags)
                model.Tags.Add(new BlogPostTagModel()
                    {
                        Name = tag.Name,
                        BlogPostCount = tag.BlogPostCount
                    });

            return PartialView(model);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult BlogMonths()
        {
            if (!_blogSettings.Enabled)
                return Content("");

            var model = new List<BlogPostYearModel>();

            var blogPosts = _blogService.GetAllBlogPosts(_workContext.WorkingLanguage.Id, null, null, 0, int.MaxValue);
            if (blogPosts.Count > 0)
            {
                var months = new SortedDictionary<DateTime, int>();

                var first = blogPosts[blogPosts.Count - 1].CreatedOnUtc;
                while (DateTime.SpecifyKind(first, DateTimeKind.Utc) <= DateTime.UtcNow.AddMonths(1))
                {
                    var list = blogPosts.GetPostsByDate(new DateTime(first.Year, first.Month, 1), new DateTime(first.Year, first.Month, 1).AddMonths(1).AddSeconds(-1));
                    if (list.Count > 0)
                    {
                        var date = new DateTime(first.Year, first.Month, 1);
                        months.Add(date, list.Count);
                    }

                    first = first.AddMonths(1);
                }


                int current = 0;
                foreach (var kvp in months)
                {
                    var date = kvp.Key;
                    var blogPostCount = kvp.Value;
                    if (current == 0)
                        current = date.Year;

                    if (date.Year > current || model.Count == 0)
                    {
                        var yearModel = new BlogPostYearModel()
                        {
                            Year = date.Year
                        };
                        model.Add(yearModel);
                    }

                    model.Last().Months.Add(new BlogPostMonthModel()
                        {
                            Month = date.Month,
                            BlogPostCount = blogPostCount
                        });

                    current = date.Year;
                }
            }
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult RssHeaderLink()
        {
            if (!_blogSettings.Enabled || !_blogSettings.ShowHeaderRssUrl)
                return Content("");

            string link = string.Format("<link href=\"{0}\" rel=\"alternate\" type=\"application/rss+xml\" title=\"{1}: Blog\" />",
                Url.RouteUrl("BlogRSS", new { languageId = _workContext.WorkingLanguage.Id }, "http"), _storeInformationSettings.StoreName);

            return Content(link);
        }
        #endregion
    }
}
