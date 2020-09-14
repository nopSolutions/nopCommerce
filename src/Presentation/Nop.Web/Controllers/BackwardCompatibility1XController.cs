using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    public partial class BackwardCompatibility1XController : Controller
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

        public virtual async Task<IActionResult> GeneralRedirect()
        {
            // use Request.RawUrl, for instance to parse out what was invoked
            // this regex will extract anything between a "/" and a ".aspx"
            var regex = new Regex(@"(?<=/).+(?=\.aspx)", RegexOptions.Compiled);
            var rawUrl = await _webHelper.GetRawUrl(HttpContext.Request);
            var aspxfileName = regex.Match(rawUrl).Value.ToLowerInvariant();

            switch (aspxfileName)
            {
                //URL without rewriting
                case "product":
                    {
                        return await RedirectProduct(await _webHelper.QueryString<string>("productid"), false);
                    }
                case "category":
                    {
                        return await RedirectCategory(await _webHelper.QueryString<string>("categoryid"), false);
                    }
                case "manufacturer":
                    {
                        return await RedirectManufacturer(await _webHelper.QueryString<string>("manufacturerid"), false);
                    }
                case "producttag":
                    {
                        return await RedirectProductTag(await _webHelper.QueryString<string>("tagid"), false);
                    }
                case "news":
                    {
                        return await RedirectNewsItem(await _webHelper.QueryString<string>("newsid"), false);
                    }
                case "blog":
                    {
                        return await RedirectBlogPost(await _webHelper.QueryString<string>("blogpostid"), false);
                    }
                case "topic":
                    {
                        return await RedirectTopic(await _webHelper.QueryString<string>("topicid"), false);
                    }
                case "profile":
                    {
                        return await RedirectUserProfile(await _webHelper.QueryString<string>("UserId"));
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

        public virtual async Task<IActionResult> RedirectProduct(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var productId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var product = await _productService.GetProductById(productId);
            if (product == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Product", new { SeName = await _urlRecordService.GetSeName(product) });
        }

        public virtual async Task<IActionResult> RedirectCategory(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var categoryid = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var category = await _categoryService.GetCategoryById(categoryid);
            if (category == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Category", new { SeName = await _urlRecordService.GetSeName(category) });
        }

        public virtual async Task<IActionResult> RedirectManufacturer(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var manufacturerId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var manufacturer = await _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Manufacturer", new { SeName = await _urlRecordService.GetSeName(manufacturer) });
        }

        public virtual async Task<IActionResult> RedirectProductTag(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var tagId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var tag = await _productTagService.GetProductTagById(tagId);
            if (tag == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("ProductsByTag", new { productTagId = tag.Id });
        }

        public virtual async Task<IActionResult> RedirectNewsItem(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var newsId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var newsItem = await _newsService.GetNewsById(newsId);
            if (newsItem == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("NewsItem", new { newsItemId = newsItem.Id, SeName = await _urlRecordService.GetSeName(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        public virtual async Task<IActionResult> RedirectBlogPost(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var blogPostId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var blogPost = await _blogService.GetBlogPostById(blogPostId);
            if (blogPost == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("BlogPost", new { blogPostId = blogPost.Id, SeName = await _urlRecordService.GetSeName(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        public virtual async Task<IActionResult> RedirectTopic(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var topicId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var topic = await _topicService.GetTopicById(topicId);
            if (topic == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Topic", new { SeName = await _urlRecordService.GetSeName(topic) });
        }

        public virtual async Task<IActionResult> RedirectForumGroup(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumGroupId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var forumGroup = await _forumService.GetForumGroupById(forumGroupId);
            if (forumGroup == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("ForumGroupSlug", new { id = forumGroup.Id, slug = await _forumService.GetForumGroupSeName(forumGroup) });
        }

        public virtual async Task<IActionResult> RedirectForum(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var forum = await _forumService.GetForumById(forumId);
            if (forum == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("ForumSlug", new { id = forum.Id, slug = await _forumService.GetForumSeName(forum) });
        }

        public virtual async Task<IActionResult> RedirectForumTopic(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumTopicId = idIncludesSename ? Convert.ToInt32(id.Split(new[] { '-' })[0]) : Convert.ToInt32(id);
            var topic = await _forumService.GetTopicById(forumTopicId);
            if (topic == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("TopicSlug", new { id = topic.Id, slug = await _forumService.GetTopicSeName(topic) });
        }

        public virtual async Task<IActionResult> RedirectUserProfile(string id)
        {
            //we can't use dash in MVC
            var userId = Convert.ToInt32(id);
            var user = await _customerService.GetCustomerById(userId);
            if (user == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("CustomerProfile", new { id = user.Id });
        }

        #endregion
    }
}