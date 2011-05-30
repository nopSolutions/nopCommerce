using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Services;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models;
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

		#endregion Constructors 

        #region Utilities

        [NonAction]
        protected int GetFontSize(double weight, double mean, double stdDev)
        {
            double factor = (weight - mean);

            if (factor != 0 && stdDev != 0) factor /= stdDev;

            return (factor > 2) ? 150 :
                (factor > 1) ? 120 :
                (factor > 0.5) ? 100 :
                (factor > -0.5) ? 90 :
                (factor > -1) ? 85 :
                (factor > -2) ? 80 :
                75;
        }

        [NonAction]
        protected double Mean(IEnumerable<double> values)
        {
            double sum = 0;
            int count = 0;

            foreach (double d in values)
            {
                sum += d;
                count++;
            }

            return sum / count;
        }

        [NonAction]
        protected double StdDev(IEnumerable<double> values, out double mean)
        {
            mean = Mean(values);
            double sumOfDiffSquares = 0;
            int count = 0;

            foreach (double d in values)
            {
                double diff = (d - mean);
                sumOfDiffSquares += diff * diff;
                count++;
            }

            return Math.Sqrt(sumOfDiffSquares / count);
        }

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
            model.Tags = blogPost.ParsedTags.ToList();
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
                        CustomerName = "TODO customername/email/username here",
                        CommentText = bc.CommentText,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(bc.CreatedOnUtc, DateTimeKind.Utc),
                        AllowViewingProfiles = _customerSettings.AllowViewingProfiles,
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
            if (!_blogSettings.Enabled)
                return Content("");

            var blogPosts = _blogService.GetAllBlogPosts(languageId,
                null, null, 0, int.MaxValue);

            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8
            };
            using (var writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("rss");
                writer.WriteAttributeString("version", "2.0");
                writer.WriteStartElement("channel");
                writer.WriteElementString("title", string.Format("{0}: Blog", _storeInformationSettings.StoreName));
                writer.WriteElementString("link", _webHelper.GetStoreLocation(false));
                writer.WriteElementString("description", "Information about products");
                writer.WriteElementString("copyright", string.Format("Copyright {0} by {1}", DateTime.Now.Year, _storeInformationSettings.StoreName));

                foreach (var blogPost in blogPosts)
                {
                    writer.WriteStartElement("item");

                    writer.WriteStartElement("title");
                    writer.WriteCData(blogPost.Title);
                    writer.WriteEndElement(); // title
                    writer.WriteStartElement("author");
                    writer.WriteCData(_storeInformationSettings.StoreName);
                    writer.WriteEndElement(); // author
                    writer.WriteStartElement("description");
                    writer.WriteCData(blogPost.Body);
                    writer.WriteEndElement(); // description
                    writer.WriteStartElement("link");
                    //TODO add a method for getting blog URL (e.g. SEOHelper.GetBlogUrl)
                    var productUrl = string.Format("{0}blog/{1}/{2}", _webHelper.GetStoreLocation(false), blogPost.Id, blogPost.GetSeName());
                    writer.WriteCData(productUrl);
                    writer.WriteEndElement(); // link
                    writer.WriteStartElement("pubDate");
                    writer.WriteCData(string.Format("{0:R}", _dateTimeHelper.ConvertToUserTime(blogPost.CreatedOnUtc, DateTimeKind.Utc)));
                    writer.WriteEndElement(); // pubDate


                    writer.WriteEndElement(); // item
                }

                writer.WriteEndElement(); // channel
                writer.WriteEndElement(); // rss
                writer.WriteEndDocument();
            }

            return this.Content(sb.ToString(), "text/xml");
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
        public ActionResult BlogTags()
        {
            if (!_blogSettings.Enabled)
                return Content("");

            var model = new List<BlogPostTagModel>();

            //get all tags
            var blogPostTags = _blogService.GetAllBlogPostTags(_workContext.WorkingLanguage.Id).ToList();
            //sorting
            blogPostTags.Sort(new BlogTagComparer());

            int maxItems = 15;
            for (int i = 0; i < blogPostTags.Count; i++)
            {
                BlogPostTag blogTag = blogPostTags[i];
                if (i < maxItems)
                {
                    model.Add(new BlogPostTagModel()
                        {
                            Name = blogTag.Name,
                            BlogPostCount = blogTag.BlogPostCount
                        });
                }
            }

            //font sizes (move appropriate font size calculations to the view)
            double mean = 0;
            var itemWeights = new List<double>();
            foreach (var blogTag in model)
                itemWeights.Add(blogTag.BlogPostCount);
            double stdDev = StdDev(itemWeights, out mean);
            foreach (var blogPost in model)
            {
                blogPost.FontSizePercent = GetFontSize(blogPost.BlogPostCount, mean, stdDev);
            }

            return PartialView(model);
        }

        [ChildActionOnly]
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

        #endregion

        #region Nested classes

        protected class BlogTagComparer : IComparer<BlogPostTag>
        {
            public int Compare(BlogPostTag x, BlogPostTag y)
            {
                if (y == null || String.IsNullOrEmpty(y.Name))
                    return -1;
                if (x == null || String.IsNullOrEmpty(x.Name))
                    return 1;
                return x.Name.CompareTo(y.Name);
            }
        }
        #endregion
    }
}
