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

        private readonly MediaSettings _mediaSetting;
        private readonly BlogSettings _blogSetting;
        private readonly LocalizationSettings _localizationSettings;
        
        #endregion

		#region Constructors

        public BlogController(IBlogService blogService, 
            IWorkContext workContext, IPictureService pictureService, ILocalizationService localizationService,
            ICustomerContentService customerContentService, IDateTimeHelper dateTimeHelper,
            IWorkflowMessageService workflowMessageService,
            MediaSettings mediaSetting, BlogSettings blogSetting, 
            LocalizationSettings localizationSettings)
        {
            this._blogService = blogService;
            this._workContext = workContext;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._customerContentService = customerContentService;
            this._dateTimeHelper = dateTimeHelper;
            this._workflowMessageService = workflowMessageService;

            this._mediaSetting = mediaSetting;
            this._blogSetting = blogSetting;
            this._localizationSettings = localizationSettings;
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

        #endregion

        #region Methods

        public ActionResult List(BlogPagingFilteringModel command)
        {
            var model = new BlogPostListModel();
            model.PagingFilteringContext.Tag = command.Tag;
            model.PagingFilteringContext.Month = command.Month;

            if (command.PageSize <= 0) command.PageSize = _blogSetting.PostsPageSize;
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
                    var blogPostModel = new BlogPostModel()
                    {
                        Id = x.Id,
                        SeName = x.GetSeName(),
                        Title = x.Title,
                        Body = x.Body,
                        AllowComments = x.AllowComments,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                        Tags = x.ParsedTags.ToList(),
                        NumberOfComments= x.BlogComments.Count
                    };
                    
                    return blogPostModel;
                })
                .ToList();

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult BlogTags()
        {
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
