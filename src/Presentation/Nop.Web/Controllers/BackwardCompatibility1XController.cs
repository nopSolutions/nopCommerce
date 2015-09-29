using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
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

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductTagService _productTagService;
        private readonly INewsService _newsService;
        private readonly IBlogService _blogService;
        private readonly ITopicService _topicService;
        private readonly IForumService _forumService;
        private readonly ICustomerService _customerService;
        #endregion

		#region Constructors

        public BackwardCompatibility1XController(IProductService productService,
            ICategoryService categoryService, IManufacturerService manufacturerService,
            IProductTagService productTagService, INewsService newsService,
            IBlogService blogService, ITopicService topicService,
            IForumService forumService, ICustomerService customerService)
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productTagService = productTagService;
            this._newsService = newsService;
            this._blogService = blogService;
            this._topicService = topicService;
            this._forumService = forumService;
            this._customerService = customerService;
        }

		#endregion
        
        #region Methods

        public ActionResult GeneralRedirect()
        {
            
            // use Request.RawUrl, for instance to parse out what was invoked
            // this regex will extract anything between a "/" and a ".aspx"
            var regex = new Regex(@"(?<=/).+(?=\.aspx)", RegexOptions.Compiled);
            var aspxfileName = regex.Match(Request.RawUrl).Value.ToLowerInvariant();


            switch (aspxfileName)
            {
                //URL without rewriting
                case "product":
                    {
                        return RedirectProduct(Request.QueryString["productid"], false);
                    }
                case "category":
                    {
                        return RedirectCategory(Request.QueryString["categoryid"], false);
                    }
                case "manufacturer":
                    {
                        return RedirectManufacturer(Request.QueryString["manufacturerid"], false);
                    }
                case "producttag":
                    {
                        return RedirectProductTag(Request.QueryString["tagid"], false);
                    }
                case "news":
                    {
                        return RedirectNewsItem(Request.QueryString["newsid"], false);
                    }
                case "blog":
                    {
                        return RedirectBlogPost(Request.QueryString["blogpostid"], false);
                    }
                case "topic":
                    {
                        return RedirectTopic(Request.QueryString["topicid"], false);
                    }
                case "profile":
                    {
                        return RedirectUserProfile(Request.QueryString["UserId"]);
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
                default:
                    break;
            }

            //no permanent redirect in this case
            return RedirectToRoute("HomePage");
        }

        public ActionResult RedirectProduct(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var productId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Product", new { SeName = product.GetSeName() });
        }

        public ActionResult RedirectCategory(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var categoryid = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var category = _categoryService.GetCategoryById(categoryid);
            if (category == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Category", new { SeName = category.GetSeName() });
        }

        public ActionResult RedirectManufacturer(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var manufacturerId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Manufacturer", new { SeName = manufacturer.GetSeName() });
        }

        public ActionResult RedirectProductTag(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var tagId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var tag = _productTagService.GetProductTagById(tagId);
            if (tag == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("ProductsByTag", new { productTagId = tag.Id });
        }

        public ActionResult RedirectNewsItem(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var newsId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var newsItem = _newsService.GetNewsById(newsId);
            if (newsItem == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("NewsItem", new { newsItemId = newsItem.Id, SeName = newsItem.GetSeName(newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        public ActionResult RedirectBlogPost(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var blogPostId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var blogPost = _blogService.GetBlogPostById(blogPostId);
            if (blogPost == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("BlogPost", new { blogPostId = blogPost.Id, SeName = blogPost.GetSeName(blogPost.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        public ActionResult RedirectTopic(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var topicid = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var topic = _topicService.GetTopicById(topicid);
            if (topic == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Topic", new { SeName = topic.GetSeName() });
        }

        public ActionResult RedirectForumGroup(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumGroupId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var forumGroup = _forumService.GetForumGroupById(forumGroupId);
            if (forumGroup == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("ForumGroupSlug", new { id = forumGroup.Id, slug = forumGroup.GetSeName() });
        }

        public ActionResult RedirectForum(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var forum = _forumService.GetForumById(forumId);
            if (forum == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("ForumSlug", new { id = forum.Id, slug = forum.GetSeName() });
        }

        public ActionResult RedirectForumTopic(string id, bool idIncludesSename = true)
        {
            //we can't use dash in MVC
            var forumTopicId = idIncludesSename ? Convert.ToInt32(id.Split(new [] { '-' })[0]) : Convert.ToInt32(id);
            var topic = _forumService.GetTopicById(forumTopicId);
            if (topic == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("TopicSlug", new { id = topic.Id, slug = topic.GetSeName() });
        }

        public ActionResult RedirectUserProfile(string id)
        {
            //we can't use dash in MVC
            var userId = Convert.ToInt32(id);
            var user = _customerService.GetCustomerById(userId);
            if (user == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("CustomerProfile", new { id = user.Id});
        }
        
        #endregion
    }
}
