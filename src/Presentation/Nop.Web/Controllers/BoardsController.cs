using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Security;
using Nop.Core.Rss;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Boards;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class BoardsController : BasePublicController
    {
        #region Fields

        protected CaptchaSettings CaptchaSettings { get; }
        protected ForumSettings ForumSettings { get; }
        protected ICustomerService CustomerService { get; }
        protected IForumModelFactory ForumModelFactory { get; }
        protected IForumService ForumService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IStoreContext StoreContext { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public BoardsController(CaptchaSettings captchaSettings,
            ForumSettings forumSettings,
            ICustomerService customerService,
            IForumModelFactory forumModelFactory,
            IForumService forumService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            CaptchaSettings = captchaSettings;
            ForumSettings = forumSettings;
            CustomerService = customerService;
            ForumModelFactory = forumModelFactory;
            ForumService = forumService;
            LocalizationService = localizationService;
            StoreContext = storeContext;
            WebHelper = webHelper;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Index()
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = await ForumModelFactory.PrepareBoardsIndexModelAsync();
            
            return View(model);
        }

        public virtual async Task<IActionResult> ActiveDiscussions(int forumId = 0, int pageNumber = 1)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = await ForumModelFactory.PrepareActiveDiscussionsModelAsync(forumId, pageNumber);
            
            return View(model);
        }

        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> ActiveDiscussionsRss(int forumId = 0)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            if (!ForumSettings.ActiveDiscussionsFeedEnabled)
                return RedirectToRoute("Boards");

            var topics = await ForumService.GetActiveTopicsAsync(forumId, 0, ForumSettings.ActiveDiscussionsFeedCount);
            var url = Url.RouteUrl("ActiveDiscussionsRSS", null, WebHelper.GetCurrentRequestProtocol());

            var feedTitle = await LocalizationService.GetResourceAsync("Forum.ActiveDiscussionsFeedTitle");
            var feedDescription = await LocalizationService.GetResourceAsync("Forum.ActiveDiscussionsFeedDescription");

            var store = await StoreContext.GetCurrentStoreAsync();
            var feed = new RssFeed(
                string.Format(feedTitle, await LocalizationService.GetLocalizedAsync(store, x => x.Name)),
                feedDescription,
                new Uri(url),
                DateTime.UtcNow);

            var items = new List<RssItem>();

            var viewsText = await LocalizationService.GetResourceAsync("Forum.Views");
            var repliesText = await LocalizationService.GetResourceAsync("Forum.Replies");

            foreach (var topic in topics)
            {
                var topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = await ForumService.GetTopicSeNameAsync(topic) }, WebHelper.GetCurrentRequestProtocol());
                var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";

                items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl),
                    $"urn:store:{store.Id}:activeDiscussions:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
            }
            feed.Items = items;

            return new RssActionResult(feed, WebHelper.GetThisPageUrl(false));
        }

        public virtual async Task<IActionResult> ForumGroup(int id)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumGroup = await ForumService.GetForumGroupByIdAsync(id);
            if (forumGroup == null)
                return RedirectToRoute("Boards");

            var model = await ForumModelFactory.PrepareForumGroupModelAsync(forumGroup);
            
            return View(model);
        }

        public virtual async Task<IActionResult> Forum(int id, int pageNumber = 1)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = await ForumService.GetForumByIdAsync(id);
            if (forum == null)
                return RedirectToRoute("Boards");

            var model = await ForumModelFactory.PrepareForumPageModelAsync(forum, pageNumber);
            
            return View(model);
        }

        public virtual async Task<IActionResult> ForumRss(int id)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            if (!ForumSettings.ForumFeedsEnabled)
                return RedirectToRoute("Boards");

            var topicLimit = ForumSettings.ForumFeedCount;
            var forum = await ForumService.GetForumByIdAsync(id);

            if (forum != null)
            {
                //Order by newest topic posts & limit the number of topics to return
                var topics = await ForumService.GetAllTopicsAsync(forum.Id, 0, string.Empty,
                     ForumSearchType.All, 0, 0, topicLimit);

                var url = Url.RouteUrl("ForumRSS", new { id = forum.Id }, WebHelper.GetCurrentRequestProtocol());

                var feedTitle = await LocalizationService.GetResourceAsync("Forum.ForumFeedTitle");
                var feedDescription = await LocalizationService.GetResourceAsync("Forum.ForumFeedDescription");

                var store = await StoreContext.GetCurrentStoreAsync();
                var feed = new RssFeed(
                    string.Format(feedTitle, await LocalizationService.GetLocalizedAsync(store, x => x.Name), forum.Name),
                    feedDescription,
                    new Uri(url),
                    DateTime.UtcNow);

                var items = new List<RssItem>();

                var viewsText = await LocalizationService.GetResourceAsync("Forum.Views");
                var repliesText = await LocalizationService.GetResourceAsync("Forum.Replies");

                foreach (var topic in topics)
                {
                    var topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = await ForumService.GetTopicSeNameAsync(topic) }, WebHelper.GetCurrentRequestProtocol());
                    var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";

                    items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl), $"urn:store:{store.Id}:forum:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
                }

                feed.Items = items;

                return new RssActionResult(feed, WebHelper.GetThisPageUrl(false));
            }

            return new RssActionResult(new RssFeed(new Uri(WebHelper.GetStoreLocation())), WebHelper.GetThisPageUrl(false));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> ForumWatch(int id)
        {
            var watchTopic = await LocalizationService.GetResourceAsync("Forum.WatchForum");
            var unwatchTopic = await LocalizationService.GetResourceAsync("Forum.UnwatchForum");
            var returnText = watchTopic;

            var forum = await ForumService.GetForumByIdAsync(id);
            if (forum == null)
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await ForumService.IsCustomerAllowedToSubscribeAsync(customer))
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            var forumSubscription = (await ForumService.GetAllSubscriptionsAsync(customer.Id,
                forum.Id, 0, 0, 1)).FirstOrDefault();

            bool subscribed;
            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription
                {
                    SubscriptionGuid = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    ForumId = forum.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await ForumService.InsertSubscriptionAsync(forumSubscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                await ForumService.DeleteSubscriptionAsync(forumSubscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        public virtual async Task<IActionResult> Topic(int id, int pageNumber = 1)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await ForumService.GetTopicByIdAsync(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var model = await ForumModelFactory.PrepareForumTopicPageModelAsync(forumTopic, pageNumber);
            //if no posts loaded, redirect to the first page
            if (!model.ForumPostModels.Any() && pageNumber > 1)
                return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await ForumService.GetTopicSeNameAsync(forumTopic) });

            //update view count
            forumTopic.Views += 1;
            await ForumService.UpdateTopicAsync(forumTopic);

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> TopicWatch(int id)
        {
            var watchTopic = await LocalizationService.GetResourceAsync("Forum.WatchTopic");
            var unwatchTopic = await LocalizationService.GetResourceAsync("Forum.UnwatchTopic");
            var returnText = watchTopic;

            var forumTopic = await ForumService.GetTopicByIdAsync(id);
            if (forumTopic == null)
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await ForumService.IsCustomerAllowedToSubscribeAsync(customer))
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            var forumSubscription = (await ForumService.GetAllSubscriptionsAsync(customer.Id,
                0, forumTopic.Id, 0, 1)).FirstOrDefault();

            bool subscribed;
            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription
                {
                    SubscriptionGuid = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    TopicId = forumTopic.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await ForumService.InsertSubscriptionAsync(forumSubscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                await ForumService.DeleteSubscriptionAsync(forumSubscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        public virtual async Task<IActionResult> TopicMove(int id)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await ForumService.GetTopicByIdAsync(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var model = await ForumModelFactory.PrepareTopicMoveAsync(forumTopic);
            
            return View(model);
        }

        [HttpPost]        
        public virtual async Task<IActionResult> TopicMove(TopicMoveModel model)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await ForumService.GetTopicByIdAsync(model.Id);

            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var newForumId = model.ForumSelected;
            var forum = await ForumService.GetForumByIdAsync(newForumId);

            if (forum != null && forumTopic.ForumId != newForumId)
                await ForumService.MoveTopicAsync(forumTopic.Id, newForumId);

            return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await ForumService.GetTopicSeNameAsync(forumTopic) });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TopicDelete(int id)
        {
            if (!ForumSettings.ForumsEnabled)
                return Json(new
                {
                    redirect = Url.RouteUrl("Homepage"),
                });

            var forumTopic = await ForumService.GetTopicByIdAsync(id);
            if (forumTopic != null)
            {
                if (!await ForumService.IsCustomerAllowedToDeleteTopicAsync(await WorkContext.GetCurrentCustomerAsync(), forumTopic))
                    return Challenge();

                var forum = await ForumService.GetForumByIdAsync(forumTopic.ForumId);

                await ForumService.DeleteTopicAsync(forumTopic);

                if (forum != null)
                    return Json(new
                    {
                        redirect = Url.RouteUrl("ForumSlug", new { id = forum.Id, slug = await ForumService.GetForumSeNameAsync(forum) }),
                    });
            }

            return Json(new
            {
                redirect = Url.RouteUrl("Boards"),
            });
        }

        public virtual async Task<IActionResult> TopicCreate(int id)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = await ForumService.GetForumByIdAsync(id);
            if (forum == null)
                return RedirectToRoute("Boards");

            if (await ForumService.IsCustomerAllowedToCreateTopicAsync(await WorkContext.GetCurrentCustomerAsync(), forum) == false)
                return Challenge();

            var model = new EditForumTopicModel();
            await ForumModelFactory.PrepareTopicCreateModelAsync(forum, model);
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> TopicCreate(EditForumTopicModel model, bool captchaValid)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = await ForumService.GetForumByIdAsync(model.ForumId);
            if (forum == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = await WorkContext.GetCurrentCustomerAsync();
                    if (!await ForumService.IsCustomerAllowedToCreateTopicAsync(customer, forum))
                    {
                        return Challenge();
                    }

                    var subject = model.Subject;
                    var maxSubjectLength = ForumSettings.TopicSubjectMaxLength;
                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject[0..maxSubjectLength];
                    }

                    var text = model.Text;
                    var maxPostLength = ForumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text[0..maxPostLength];

                    var topicType = ForumTopicType.Normal;
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var nowUtc = DateTime.UtcNow;

                    if (await ForumService.IsCustomerAllowedToSetTopicPriorityAsync(customer))
                        topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

                    //forum topic
                    var forumTopic = new ForumTopic
                    {
                        ForumId = forum.Id,
                        CustomerId = customer.Id,
                        TopicTypeId = (int)topicType,
                        Subject = subject,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await ForumService.InsertTopicAsync(forumTopic, true);

                    //forum post
                    var forumPost = new ForumPost
                    {
                        TopicId = forumTopic.Id,
                        CustomerId = customer.Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await ForumService.InsertPostAsync(forumPost, false);

                    //update forum topic
                    forumTopic.NumPosts = 1;
                    forumTopic.LastPostId = forumPost.Id;
                    forumTopic.LastPostCustomerId = forumPost.CustomerId;
                    forumTopic.LastPostTime = forumPost.CreatedOnUtc;
                    forumTopic.UpdatedOnUtc = nowUtc;
                    await ForumService.UpdateTopicAsync(forumTopic);

                    //subscription                
                    if (await ForumService.IsCustomerAllowedToSubscribeAsync(customer))
                    {
                        if (model.Subscribed)
                        {
                            var forumSubscription = new ForumSubscription
                            {
                                SubscriptionGuid = Guid.NewGuid(),
                                CustomerId = customer.Id,
                                TopicId = forumTopic.Id,
                                CreatedOnUtc = nowUtc
                            };

                            await ForumService.InsertSubscriptionAsync(forumSubscription);
                        }
                    }

                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await ForumService.GetTopicSeNameAsync(forumTopic) });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            await ForumModelFactory.PrepareTopicCreateModelAsync(forum, model);
            
            return View(model);
        }

        public virtual async Task<IActionResult> TopicEdit(int id)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await ForumService.GetTopicByIdAsync(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            if (!await ForumService.IsCustomerAllowedToEditTopicAsync(await WorkContext.GetCurrentCustomerAsync(), forumTopic))
                return Challenge();

            var model = new EditForumTopicModel();
            await ForumModelFactory.PrepareTopicEditModelAsync(forumTopic, model, false);
            
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> TopicEdit(EditForumTopicModel model, bool captchaValid)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await ForumService.GetTopicByIdAsync(model.Id);

            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var forum = await ForumService.GetForumByIdAsync(forumTopic.ForumId);
            if (forum == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = await WorkContext.GetCurrentCustomerAsync();
                    if (!await ForumService.IsCustomerAllowedToEditTopicAsync(customer, forumTopic))
                        return Challenge();

                    var subject = model.Subject;
                    var maxSubjectLength = ForumSettings.TopicSubjectMaxLength;
                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject[0..maxSubjectLength];
                    }

                    var text = model.Text;
                    var maxPostLength = ForumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text[0..maxPostLength];

                    var topicType = ForumTopicType.Normal;
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var nowUtc = DateTime.UtcNow;

                    if (await ForumService.IsCustomerAllowedToSetTopicPriorityAsync(customer))
                        topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

                    //forum topic
                    forumTopic.TopicTypeId = (int)topicType;
                    forumTopic.Subject = subject;
                    forumTopic.UpdatedOnUtc = nowUtc;
                    await ForumService.UpdateTopicAsync(forumTopic);

                    //forum post                
                    var firstPost = await ForumService.GetFirstPostAsync(forumTopic);
                    if (firstPost != null)
                    {
                        firstPost.Text = text;
                        firstPost.UpdatedOnUtc = nowUtc;
                        await ForumService.UpdatePostAsync(firstPost);
                    }
                    else
                    {
                        //error (not possible)
                        firstPost = new ForumPost
                        {
                            TopicId = forumTopic.Id,
                            CustomerId = forumTopic.CustomerId,
                            Text = text,
                            IPAddress = ipAddress,
                            UpdatedOnUtc = nowUtc
                        };

                        await ForumService.InsertPostAsync(firstPost, false);
                    }

                    //subscription
                    if (await ForumService.IsCustomerAllowedToSubscribeAsync(customer))
                    {
                        var forumSubscription = (await ForumService.GetAllSubscriptionsAsync(customer.Id,
                            0, forumTopic.Id, 0, 1)).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    CustomerId = customer.Id,
                                    TopicId = forumTopic.Id,
                                    CreatedOnUtc = nowUtc
                                };

                                await ForumService.InsertSubscriptionAsync(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                await ForumService.DeleteSubscriptionAsync(forumSubscription);
                            }
                        }
                    }

                    // redirect to the topic page with the topic slug
                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await ForumService.GetTopicSeNameAsync(forumTopic) });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            await ForumModelFactory.PrepareTopicEditModelAsync(forumTopic, model, true);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostDelete(int id)
        {
            if (!ForumSettings.ForumsEnabled)
                return Json(new
                {
                    redirect = Url.RouteUrl("Homepage"),
                });

            var forumPost = await ForumService.GetPostByIdAsync(id);

            if (forumPost == null)
                return Json(new {redirect = Url.RouteUrl("Boards")});

            if (!await ForumService.IsCustomerAllowedToDeletePostAsync(await WorkContext.GetCurrentCustomerAsync(), forumPost))
                return Challenge();

            var forumTopic = await ForumService.GetTopicByIdAsync(forumPost.TopicId);
            var forumId = forumTopic.ForumId;
            var forum = await ForumService.GetForumByIdAsync(forumId);
            var forumSlug = await ForumService.GetForumSeNameAsync(forum);

            await ForumService.DeletePostAsync(forumPost);

            //get topic one more time because it can be deleted (first or only post deleted)
            forumTopic = await ForumService.GetTopicByIdAsync(forumPost.TopicId);
            if (forumTopic == null)
                return Json(new
                {
                    redirect = Url.RouteUrl("ForumSlug", new { id = forumId, slug = forumSlug }),
                });

            return Json(new
            {
                redirect = Url.RouteUrl("TopicSlug", new { id = forumTopic.Id, slug = await ForumService.GetTopicSeNameAsync(forumTopic) }),
            });

        }

        public virtual async Task<IActionResult> PostCreate(int id, int? quote)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await ForumService.GetTopicByIdAsync(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            if (!await ForumService.IsCustomerAllowedToCreatePostAsync(await WorkContext.GetCurrentCustomerAsync(), forumTopic))
                return Challenge();

            var model = await ForumModelFactory.PreparePostCreateModelAsync(forumTopic, quote, false);
            
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> PostCreate(EditForumPostModel model, bool captchaValid)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await ForumService.GetTopicByIdAsync(model.ForumTopicId);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = await WorkContext.GetCurrentCustomerAsync();
                    if (!await ForumService.IsCustomerAllowedToCreatePostAsync(customer, forumTopic))
                        return Challenge();

                    var text = model.Text;
                    var maxPostLength = ForumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text[0..maxPostLength];

                    var ipAddress = WebHelper.GetCurrentIpAddress();

                    var nowUtc = DateTime.UtcNow;

                    var forumPost = new ForumPost
                    {
                        TopicId = forumTopic.Id,
                        CustomerId = customer.Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await ForumService.InsertPostAsync(forumPost, true);

                    //subscription
                    if (await ForumService.IsCustomerAllowedToSubscribeAsync(customer))
                    {
                        var forumSubscription = (await ForumService.GetAllSubscriptionsAsync(customer.Id,
                            0, forumPost.TopicId, 0, 1)).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    CustomerId = customer.Id,
                                    TopicId = forumPost.TopicId,
                                    CreatedOnUtc = nowUtc
                                };

                                await ForumService.InsertSubscriptionAsync(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                await ForumService.DeleteSubscriptionAsync(forumSubscription);
                            }
                        }
                    }

                    var pageSize = ForumSettings.PostsPageSize > 0 ? ForumSettings.PostsPageSize : 10;

                    var pageIndex = await ForumService.CalculateTopicPageIndexAsync(forumPost.TopicId, pageSize, forumPost.Id) + 1;
                    string url;
                    if (pageIndex > 1)
                        url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.TopicId, slug = await ForumService.GetTopicSeNameAsync(forumTopic), pageNumber = pageIndex });
                    else
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = await ForumService.GetTopicSeNameAsync(forumTopic) });
                    return LocalRedirect($"{url}#{forumPost.Id}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            model = await ForumModelFactory.PreparePostCreateModelAsync(forumTopic, 0, true);

            return View(model);
        }

        public virtual async Task<IActionResult> PostEdit(int id)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumPost = await ForumService.GetPostByIdAsync(id);
            if (forumPost == null)
                return RedirectToRoute("Boards");

            if (!await ForumService.IsCustomerAllowedToEditPostAsync(await WorkContext.GetCurrentCustomerAsync(), forumPost))
                return Challenge();

            var model = await ForumModelFactory.PreparePostEditModelAsync(forumPost, false);
            
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> PostEdit(EditForumPostModel model, bool captchaValid)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumPost = await ForumService.GetPostByIdAsync(model.Id);
            if (forumPost == null)
                return RedirectToRoute("Boards");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await ForumService.IsCustomerAllowedToEditPostAsync(customer, forumPost))
                return Challenge();

            var forumTopic = await ForumService.GetTopicByIdAsync(forumPost.TopicId);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var forum = await ForumService.GetForumByIdAsync(forumTopic.ForumId);
            if (forum == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var nowUtc = DateTime.UtcNow;

                    var text = model.Text;
                    var maxPostLength = ForumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text[0..maxPostLength];
                    }

                    forumPost.UpdatedOnUtc = nowUtc;
                    forumPost.Text = text;
                    await ForumService.UpdatePostAsync(forumPost);

                    //subscription
                    if (await ForumService.IsCustomerAllowedToSubscribeAsync(customer))
                    {
                        var forumSubscription = (await ForumService.GetAllSubscriptionsAsync(customer.Id,
                            0, forumPost.TopicId, 0, 1)).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    CustomerId = customer.Id,
                                    TopicId = forumPost.TopicId,
                                    CreatedOnUtc = nowUtc
                                };
                                await ForumService.InsertSubscriptionAsync(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                await ForumService.DeleteSubscriptionAsync(forumSubscription);
                            }
                        }
                    }

                    var pageSize = ForumSettings.PostsPageSize > 0 ? ForumSettings.PostsPageSize : 10;
                    var pageIndex = (await ForumService.CalculateTopicPageIndexAsync(forumPost.TopicId, pageSize, forumPost.Id) + 1);
                    string url;
                    if (pageIndex > 1)
                    {
                        url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.TopicId, slug = await ForumService.GetTopicSeNameAsync(forumTopic), pageNumber = pageIndex });
                    }
                    else
                    {
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = await ForumService.GetTopicSeNameAsync(forumTopic) });
                    }
                    return LocalRedirect($"{url}#{forumPost.Id}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            model = await ForumModelFactory.PreparePostEditModelAsync(forumPost, true);

            return View(model);
        }

        public virtual async Task<IActionResult> Search(string searchterms, bool? advs, string forumId,
            string within, string limitDays, int pageNumber = 1)
        {
            if (!ForumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = await ForumModelFactory.PrepareSearchModelAsync(searchterms, advs, forumId, within, limitDays, pageNumber);
            
            return View(model);
        }

        public virtual async Task<IActionResult> CustomerForumSubscriptions(int? pageNumber)
        {
            if (!ForumSettings.AllowCustomersToManageSubscriptions)
                return RedirectToRoute("CustomerInfo");

            var model = await ForumModelFactory.PrepareCustomerForumSubscriptionsModelAsync(pageNumber);
            
            return View(model);
        }

        [HttpPost, ActionName("CustomerForumSubscriptions")]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> CustomerForumSubscriptionsPOST(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("fs", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("fs", "").Trim();
                    if (int.TryParse(id, out var forumSubscriptionId))
                    {
                        var forumSubscription = await ForumService.GetSubscriptionByIdAsync(forumSubscriptionId);
                        var customer = await WorkContext.GetCurrentCustomerAsync();

                        if (forumSubscription != null && forumSubscription.CustomerId == customer.Id)
                        {
                            await ForumService.DeleteSubscriptionAsync(forumSubscription);
                        }
                    }
                }
            }

            return RedirectToRoute("CustomerForumSubscriptions");
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostVote(int postId, bool isUp)
        {
            if (!ForumSettings.AllowPostVoting)
                return new NullJsonResult();

            var forumPost = await ForumService.GetPostByIdAsync(postId);
            if (forumPost == null)
                return new NullJsonResult();

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Json(new
                {
                    Error = await LocalizationService.GetResourceAsync("Forum.Votes.Login"),
                    VoteCount = forumPost.VoteCount
                });

            if (customer.Id == forumPost.CustomerId)
                return Json(new
                {
                    Error = await LocalizationService.GetResourceAsync("Forum.Votes.OwnPost"),
                    VoteCount = forumPost.VoteCount
                });

            var forumPostVote = await ForumService.GetPostVoteAsync(postId, customer);
            if (forumPostVote != null)
            {
                if ((forumPostVote.IsUp && isUp) || (!forumPostVote.IsUp && !isUp))
                    return Json(new
                    {
                        Error = await LocalizationService.GetResourceAsync("Forum.Votes.AlreadyVoted"),
                        VoteCount = forumPost.VoteCount
                    });

                await ForumService.DeletePostVoteAsync(forumPostVote);
                return Json(new { VoteCount = forumPost.VoteCount });
            }

            if (await ForumService.GetNumberOfPostVotesAsync(customer, DateTime.UtcNow.AddDays(-1)) >= ForumSettings.MaxVotesPerDay)
                return Json(new
                {
                    Error = string.Format(await LocalizationService.GetResourceAsync("Forum.Votes.MaxVotesReached"), ForumSettings.MaxVotesPerDay),
                    VoteCount = forumPost.VoteCount
                });

            await ForumService.InsertPostVoteAsync(new ForumPostVote
            {
                CustomerId = customer.Id,
                ForumPostId = postId,
                IsUp = isUp,
                CreatedOnUtc = DateTime.UtcNow
            });

            return Json(new { VoteCount = forumPost.VoteCount, IsUp = isUp });
        }

        #endregion
    }
}