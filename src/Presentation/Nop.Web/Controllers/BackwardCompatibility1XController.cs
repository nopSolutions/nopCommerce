using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Services.Topics;

namespace Nop.Web.Controllers
{
    public partial class BackwardCompatibility1XController : BasePublicController
    {
        #region Fields

        private readonly IBlogService _blogService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerService _customerService;
        private readonly IForumService _forumService;
        private readonly IManufacturerService _manufacturerService;
        private readonly INewsService _newsService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly ITopicService _topicService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public BackwardCompatibility1XController(IBlogService blogService,
            ICategoryService categoryService,
            ICustomerService customerService,
            IForumService forumService,
            IManufacturerService manufacturerService,
            INewsService newsService,
            IProductService productService,
            IProductTagService productTagService,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper)
        {
            _blogService = blogService;
            _categoryService = categoryService;
            _customerService = customerService;
            _forumService = forumService;
            _manufacturerService = manufacturerService;
            _newsService = newsService;
            _productService = productService;
            _productTagService = productTagService;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        public virtual IActionResult GeneralRedirect()
        {
            // use Request.RawUrl, for instance to parse out what was invoked
            // this regex will extract anything between a "/" and a ".aspx"
            var regex = new Regex(@"(?<=/).+(?=\.aspx)", RegexOptions.Compiled);
            var rawUrl = _webHelper.GetRawUrl(HttpContext.Request);
            var aspxfileName = regex.Match(rawUrl).Value.ToLowerInvariant();

            switch (aspxfileName)
            {
                //URL without rewriting
                case "product":
                    {
                        return RedirectProduct(_webHelper.QueryString<string>("productid"), false);
                    }
                case "category":
                    {
                        return RedirectCategory(_webHelper.QueryString<string>("categoryid"), false);
                    }
                case "manufacturer":
                    {
                        return RedirectManufacturer(_webHelper.QueryString<string>("manufacturerid"), false);
                    }
                case "producttag":
                    {
                        return RedirectProductTag(_webHelper.QueryString<string>("tagid"), false);
                    }
                case "news":
                    {
                        return RedirectNewsItem(_webHelper.QueryString<string>("newsid"), false);
                    }
                case "blog":
                    {
                        return RedirectBlogPost(_webHelper.QueryString<string>("blogpostid"), false);
                    }
                case "topic":
                    {
                        return RedirectTopic(_webHelper.QueryString<string>("topicid"), false);
                    }
                case "profile":
                    {
                        return RedirectUserProfile(_webHelper.QueryString<string>("UserId"));
                    }
                case "compareproducts":
                    {
                        return RedirectToRoutePermanent("CompareProducts");
                    }
                case "contactus":
                    {
                        return RedirectToRoutePermanent("ContactUs");
                    }
                case "passwordrecovery":
                    {
                        return RedirectToRoutePermanent("PasswordRecovery");
                    }
                case "login":
                    {
                        return RedirectToRoutePermanent("Login");
                    }
                case "register":
                    {
                        return RedirectToRoutePermanent("Register");
                    }
                case "newsarchive":
                    {
                        return RedirectToRoutePermanent("NewsArchive");
                    }
                case "search":
                    {
                        return RedirectToRoutePermanent("ProductSearch");
                    }
                case "sitemap":
                    {
                        return RedirectToRoutePermanent("Sitemap");
                    }
                case "recentlyaddedproducts":
                    {
                        return RedirectToRoutePermanent("NewProducts");
                    }
                case "shoppingcart":
                    {
                        return RedirectToRoutePermanent("ShoppingCart");
                    }
                case "wishlist":
                    {
                        return RedirectToRoutePermanent("Wishlist");
                    }
                case "CheckGiftCardBalance":
                    {
                        return RedirectToRoutePermanent("CheckGiftCardBalance");
                    }
                default:
                    break;
            }

            //no permanent redirect in this case
            return RedirectToRoute("Homepage");
        }

        public virtual IActionResult RedirectProduct(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var productId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Product", new { SeName = _urlRecordService.GetSeName(product) });
        }

        public virtual IActionResult RedirectCategory(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var categoryid = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var category = _categoryService.GetCategoryById(categoryid);
            if (category == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Category", new { SeName = _urlRecordService.GetSeName(category) });
        }

        public virtual IActionResult RedirectManufacturer(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var manufacturerId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Manufacturer", new { SeName = _urlRecordService.GetSeName(manufacturer) });
        }

        public virtual IActionResult RedirectProductTag(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var tagId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var tag = _productTagService.GetProductTagById(tagId);
            if (tag == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("ProductsByTag", new { productTagId = tag.Id });
        }

        public virtual IActionResult RedirectNewsItem(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var newsId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var newsItem = _newsService.GetNewsById(newsId);
            if (newsItem == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("NewsItem", new { newsItemId = newsItem.Id, SeName = _urlRecordService.GetSeName(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        public virtual IActionResult RedirectBlogPost(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var blogPostId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var blogPost = _blogService.GetBlogPostById(blogPostId);
            if (blogPost == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("BlogPost", new { blogPostId = blogPost.Id, SeName = _urlRecordService.GetSeName(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        public virtual IActionResult RedirectTopic(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var topicid = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var topic = _topicService.GetTopicById(topicid);
            if (topic == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Topic", new { SeName = _urlRecordService.GetSeName(topic) });
        }

        public virtual IActionResult RedirectForumGroup(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumGroupId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var forumGroup = _forumService.GetForumGroupById(forumGroupId);
            if (forumGroup == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("ForumGroupSlug", new { id = forumGroup.Id, slug = _forumService.GetForumGroupSeName(forumGroup) });
        }

        public virtual IActionResult RedirectForum(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var forum = _forumService.GetForumById(forumId);
            if (forum == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("ForumSlug", new { id = forum.Id, slug = _forumService.GetForumSeName(forum) });
        }

        public virtual IActionResult RedirectForumTopic(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumTopicId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var topic = _forumService.GetTopicById(forumTopicId);
            if (topic == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("TopicSlug", new { id = topic.Id, slug = _forumService.GetTopicSeName(topic) });
        }

        public virtual IActionResult RedirectUserProfile(string id)
        {
            //we can't use dash in MVC
            var userId = Convert.ToInt32(id);
            var user = _customerService.GetCustomerById(userId);
            if (user == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("CustomerProfile", new { id = user.Id });
        }

        #endregion
    }
}