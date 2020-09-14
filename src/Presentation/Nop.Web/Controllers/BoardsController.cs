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

        private readonly CaptchaSettings _captchaSettings;
        private readonly ForumSettings _forumSettings;
        private readonly ICustomerService _customerService;
        private readonly IForumModelFactory _forumModelFactory;
        private readonly IForumService _forumService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

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
            _captchaSettings = captchaSettings;
            _forumSettings = forumSettings;
            _customerService = customerService;
            _forumModelFactory = forumModelFactory;
            _forumService = forumService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Index()
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = await _forumModelFactory.PrepareBoardsIndexModel();
            
            return View(model);
        }

        public virtual async Task<IActionResult> ActiveDiscussions(int forumId = 0, int pageNumber = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = await _forumModelFactory.PrepareActiveDiscussionsModel(forumId, pageNumber);
            
            return View(model);
        }

        public virtual async Task<IActionResult> ActiveDiscussionsRss(int forumId = 0)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            if (!_forumSettings.ActiveDiscussionsFeedEnabled)
                return RedirectToRoute("Boards");

            var topics = await _forumService.GetActiveTopics(forumId, 0, _forumSettings.ActiveDiscussionsFeedCount);
            var url = Url.RouteUrl("ActiveDiscussionsRSS", null, _webHelper.CurrentRequestProtocol);

            var feedTitle = await _localizationService.GetResource("Forum.ActiveDiscussionsFeedTitle");
            var feedDescription = await _localizationService.GetResource("Forum.ActiveDiscussionsFeedDescription");

            var feed = new RssFeed(
                string.Format(feedTitle, await _localizationService.GetLocalized(await _storeContext.GetCurrentStore(), x => x.Name)),
                feedDescription,
                new Uri(url),
                DateTime.UtcNow);

            var items = new List<RssItem>();

            var viewsText = await _localizationService.GetResource("Forum.Views");
            var repliesText = await _localizationService.GetResource("Forum.Replies");

            foreach (var topic in topics)
            {
                var topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = await _forumService.GetTopicSeName(topic) }, _webHelper.CurrentRequestProtocol);
                var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";

                items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl),
                    $"urn:store:{(await _storeContext.GetCurrentStore()).Id}:activeDiscussions:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
            }
            feed.Items = items;

            return new RssActionResult(feed, await _webHelper.GetThisPageUrl(false));
        }

        public virtual async Task<IActionResult> ForumGroup(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumGroup = await _forumService.GetForumGroupById(id);
            if (forumGroup == null)
                return RedirectToRoute("Boards");

            var model = await _forumModelFactory.PrepareForumGroupModel(forumGroup);
            
            return View(model);
        }

        public virtual async Task<IActionResult> Forum(int id, int pageNumber = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = await _forumService.GetForumById(id);
            if (forum == null)
                return RedirectToRoute("Boards");

            var model = await _forumModelFactory.PrepareForumPageModel(forum, pageNumber);
            
            return View(model);
        }

        public virtual async Task<IActionResult> ForumRss(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            if (!_forumSettings.ForumFeedsEnabled)
                return RedirectToRoute("Boards");

            var topicLimit = _forumSettings.ForumFeedCount;
            var forum = await _forumService.GetForumById(id);

            if (forum != null)
            {
                //Order by newest topic posts & limit the number of topics to return
                var topics = await _forumService.GetAllTopics(forum.Id, 0, string.Empty,
                     ForumSearchType.All, 0, 0, topicLimit);

                var url = Url.RouteUrl("ForumRSS", new { id = forum.Id }, _webHelper.CurrentRequestProtocol);

                var feedTitle = await _localizationService.GetResource("Forum.ForumFeedTitle");
                var feedDescription = await _localizationService.GetResource("Forum.ForumFeedDescription");

                var feed = new RssFeed(
                    string.Format(feedTitle, await _localizationService.GetLocalized(await _storeContext.GetCurrentStore(), x => x.Name), forum.Name),
                    feedDescription,
                    new Uri(url),
                    DateTime.UtcNow);

                var items = new List<RssItem>();

                var viewsText = await _localizationService.GetResource("Forum.Views");
                var repliesText = await _localizationService.GetResource("Forum.Replies");

                foreach (var topic in topics)
                {
                    var topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = await _forumService.GetTopicSeName(topic) }, _webHelper.CurrentRequestProtocol);
                    var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";

                    items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl), $"urn:store:{(await _storeContext.GetCurrentStore()).Id}:forum:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
                }

                feed.Items = items;

                return new RssActionResult(feed, await _webHelper.GetThisPageUrl(false));
            }

            return new RssActionResult(new RssFeed(new Uri(await _webHelper.GetStoreLocation())), await _webHelper.GetThisPageUrl(false));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> ForumWatch(int id)
        {
            var watchTopic = await _localizationService.GetResource("Forum.WatchForum");
            var unwatchTopic = await _localizationService.GetResource("Forum.UnwatchForum");
            var returnText = watchTopic;

            var forum = await _forumService.GetForumById(id);
            if (forum == null)
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            if (!await _forumService.IsCustomerAllowedToSubscribe(await _workContext.GetCurrentCustomer()))
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            var forumSubscription = (await _forumService.GetAllSubscriptions((await _workContext.GetCurrentCustomer()).Id,
                forum.Id, 0, 0, 1)).FirstOrDefault();

            bool subscribed;
            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription
                {
                    SubscriptionGuid = Guid.NewGuid(),
                    CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                    ForumId = forum.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _forumService.InsertSubscription(forumSubscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                await _forumService.DeleteSubscription(forumSubscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        public virtual async Task<IActionResult> Topic(int id, int pageNumber = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicById(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var model = await _forumModelFactory.PrepareForumTopicPageModel(forumTopic, pageNumber);
            //if no posts loaded, redirect to the first page
            if (!model.ForumPostModels.Any() && pageNumber > 1)
                return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeName(forumTopic) });

            //update view count
            forumTopic.Views += 1;
            await _forumService.UpdateTopic(forumTopic);

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> TopicWatch(int id)
        {
            var watchTopic = await _localizationService.GetResource("Forum.WatchTopic");
            var unwatchTopic = await _localizationService.GetResource("Forum.UnwatchTopic");
            var returnText = watchTopic;

            var forumTopic = await _forumService.GetTopicById(id);
            if (forumTopic == null)
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            if (!await _forumService.IsCustomerAllowedToSubscribe(await _workContext.GetCurrentCustomer()))
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            var forumSubscription = (await _forumService.GetAllSubscriptions((await _workContext.GetCurrentCustomer()).Id,
                0, forumTopic.Id, 0, 1)).FirstOrDefault();

            bool subscribed;
            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription
                {
                    SubscriptionGuid = Guid.NewGuid(),
                    CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                    TopicId = forumTopic.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await _forumService.InsertSubscription(forumSubscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                await _forumService.DeleteSubscription(forumSubscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        public virtual async Task<IActionResult> TopicMove(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicById(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var model = await _forumModelFactory.PrepareTopicMove(forumTopic);
            
            return View(model);
        }

        [HttpPost]        
        public virtual async Task<IActionResult> TopicMove(TopicMoveModel model)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicById(model.Id);

            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var newForumId = model.ForumSelected;
            var forum = await _forumService.GetForumById(newForumId);

            if (forum != null && forumTopic.ForumId != newForumId)
                await _forumService.MoveTopic(forumTopic.Id, newForumId);

            return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeName(forumTopic) });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TopicDelete(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return Json(new
                {
                    redirect = Url.RouteUrl("Homepage"),
                });

            var forumTopic = await _forumService.GetTopicById(id);
            if (forumTopic != null)
            {
                if (!await _forumService.IsCustomerAllowedToDeleteTopic(await _workContext.GetCurrentCustomer(), forumTopic))
                    return Challenge();

                var forum = await _forumService.GetForumById(forumTopic.ForumId);

                await _forumService.DeleteTopic(forumTopic);

                if (forum != null)
                    return Json(new
                    {
                        redirect = Url.RouteUrl("ForumSlug", new { id = forum.Id, slug = await _forumService.GetForumSeName(forum) }),
                    });
            }

            return Json(new
            {
                redirect = Url.RouteUrl("Boards"),
            });
        }

        public virtual async Task<IActionResult> TopicCreate(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = await _forumService.GetForumById(id);
            if (forum == null)
                return RedirectToRoute("Boards");

            if (await _forumService.IsCustomerAllowedToCreateTopic(await _workContext.GetCurrentCustomer(), forum) == false)
                return Challenge();

            var model = new EditForumTopicModel();
            await _forumModelFactory.PrepareTopicCreateModel(forum, model);
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> TopicCreate(EditForumTopicModel model, bool captchaValid)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = await _forumService.GetForumById(model.ForumId);
            if (forum == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _forumService.IsCustomerAllowedToCreateTopic(await _workContext.GetCurrentCustomer(), forum))
                    {
                        return Challenge();
                    }

                    var subject = model.Subject;
                    var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject.Substring(0, maxSubjectLength);
                    }

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text.Substring(0, maxPostLength);

                    var topicType = ForumTopicType.Normal;
                    var ipAddress = await _webHelper.GetCurrentIpAddress();
                    var nowUtc = DateTime.UtcNow;

                    if (await _forumService.IsCustomerAllowedToSetTopicPriority(await _workContext.GetCurrentCustomer()))
                        topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

                    //forum topic
                    var forumTopic = new ForumTopic
                    {
                        ForumId = forum.Id,
                        CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                        TopicTypeId = (int)topicType,
                        Subject = subject,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await _forumService.InsertTopic(forumTopic, true);

                    //forum post
                    var forumPost = new ForumPost
                    {
                        TopicId = forumTopic.Id,
                        CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await _forumService.InsertPost(forumPost, false);

                    //update forum topic
                    forumTopic.NumPosts = 1;
                    forumTopic.LastPostId = forumPost.Id;
                    forumTopic.LastPostCustomerId = forumPost.CustomerId;
                    forumTopic.LastPostTime = forumPost.CreatedOnUtc;
                    forumTopic.UpdatedOnUtc = nowUtc;
                    await _forumService.UpdateTopic(forumTopic);

                    //subscription                
                    if (await _forumService.IsCustomerAllowedToSubscribe(await _workContext.GetCurrentCustomer()))
                    {
                        if (model.Subscribed)
                        {
                            var forumSubscription = new ForumSubscription
                            {
                                SubscriptionGuid = Guid.NewGuid(),
                                CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                                TopicId = forumTopic.Id,
                                CreatedOnUtc = nowUtc
                            };

                            await _forumService.InsertSubscription(forumSubscription);
                        }
                    }

                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeName(forumTopic) });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            await _forumModelFactory.PrepareTopicCreateModel(forum, model);
            
            return View(model);
        }

        public virtual async Task<IActionResult> TopicEdit(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicById(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            if (!await _forumService.IsCustomerAllowedToEditTopic(await _workContext.GetCurrentCustomer(), forumTopic))
                return Challenge();

            var model = new EditForumTopicModel();
            await _forumModelFactory.PrepareTopicEditModel(forumTopic, model, false);
            
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> TopicEdit(EditForumTopicModel model, bool captchaValid)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicById(model.Id);

            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var forum = await _forumService.GetForumById(forumTopic.ForumId);
            if (forum == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _forumService.IsCustomerAllowedToEditTopic(await _workContext.GetCurrentCustomer(), forumTopic))
                        return Challenge();

                    var subject = model.Subject;
                    var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject.Substring(0, maxSubjectLength);
                    }

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text.Substring(0, maxPostLength);

                    var topicType = ForumTopicType.Normal;
                    var ipAddress = await _webHelper.GetCurrentIpAddress();
                    var nowUtc = DateTime.UtcNow;

                    if (await _forumService.IsCustomerAllowedToSetTopicPriority(await _workContext.GetCurrentCustomer()))
                        topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

                    //forum topic
                    forumTopic.TopicTypeId = (int)topicType;
                    forumTopic.Subject = subject;
                    forumTopic.UpdatedOnUtc = nowUtc;
                    await _forumService.UpdateTopic(forumTopic);

                    //forum post                
                    var firstPost = await _forumService.GetFirstPost(forumTopic);
                    if (firstPost != null)
                    {
                        firstPost.Text = text;
                        firstPost.UpdatedOnUtc = nowUtc;
                        await _forumService.UpdatePost(firstPost);
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

                        await _forumService.InsertPost(firstPost, false);
                    }

                    //subscription
                    if (await _forumService.IsCustomerAllowedToSubscribe(await _workContext.GetCurrentCustomer()))
                    {
                        var forumSubscription = (await _forumService.GetAllSubscriptions((await _workContext.GetCurrentCustomer()).Id,
                            0, forumTopic.Id, 0, 1)).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                                    TopicId = forumTopic.Id,
                                    CreatedOnUtc = nowUtc
                                };

                                await _forumService.InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                await _forumService.DeleteSubscription(forumSubscription);
                            }
                        }
                    }

                    // redirect to the topic page with the topic slug
                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeName(forumTopic) });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            await _forumModelFactory.PrepareTopicEditModel(forumTopic, model, true);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostDelete(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return Json(new
                {
                    redirect = Url.RouteUrl("Homepage"),
                });

            var forumPost = await _forumService.GetPostById(id);

            if (forumPost == null)
                return Json(new {redirect = Url.RouteUrl("Boards")});

            if (!await _forumService.IsCustomerAllowedToDeletePost(await _workContext.GetCurrentCustomer(), forumPost))
                return Challenge();

            var forumTopic = await _forumService.GetTopicById(forumPost.TopicId);
            var forumId = forumTopic.ForumId;
            var forum = await _forumService.GetForumById(forumId);
            var forumSlug = await _forumService.GetForumSeName(forum);

            await _forumService.DeletePost(forumPost);

            //get topic one more time because it can be deleted (first or only post deleted)
            forumTopic = await _forumService.GetTopicById(forumPost.TopicId);
            if (forumTopic == null)
                return Json(new
                {
                    redirect = Url.RouteUrl("ForumSlug", new { id = forumId, slug = forumSlug }),
                });

            return Json(new
            {
                redirect = Url.RouteUrl("TopicSlug", new { id = forumTopic.Id, slug = await _forumService.GetTopicSeName(forumTopic) }),
            });

        }

        public virtual async Task<IActionResult> PostCreate(int id, int? quote)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicById(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            if (!await _forumService.IsCustomerAllowedToCreatePost(await _workContext.GetCurrentCustomer(), forumTopic))
                return Challenge();

            var model = await _forumModelFactory.PreparePostCreateModel(forumTopic, quote, false);
            
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> PostCreate(EditForumPostModel model, bool captchaValid)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = await _forumService.GetTopicById(model.ForumTopicId);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _forumService.IsCustomerAllowedToCreatePost(await _workContext.GetCurrentCustomer(), forumTopic))
                        return Challenge();

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text.Substring(0, maxPostLength);

                    var ipAddress = await _webHelper.GetCurrentIpAddress();

                    var nowUtc = DateTime.UtcNow;

                    var forumPost = new ForumPost
                    {
                        TopicId = forumTopic.Id,
                        CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await _forumService.InsertPost(forumPost, true);

                    //subscription
                    if (await _forumService.IsCustomerAllowedToSubscribe(await _workContext.GetCurrentCustomer()))
                    {
                        var forumSubscription = (await _forumService.GetAllSubscriptions((await _workContext.GetCurrentCustomer()).Id,
                            0, forumPost.TopicId, 0, 1)).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                                    TopicId = forumPost.TopicId,
                                    CreatedOnUtc = nowUtc
                                };

                                await _forumService.InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                await _forumService.DeleteSubscription(forumSubscription);
                            }
                        }
                    }

                    var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;

                    var pageIndex = await _forumService.CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.Id) + 1;
                    string url;
                    if (pageIndex > 1)
                        url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeName(forumTopic), pageNumber = pageIndex });
                    else
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeName(forumTopic) });
                    return LocalRedirect($"{url}#{forumPost.Id}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            model = await _forumModelFactory.PreparePostCreateModel(forumTopic, 0, true);

            return View(model);
        }

        public virtual async Task<IActionResult> PostEdit(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumPost = await _forumService.GetPostById(id);
            if (forumPost == null)
                return RedirectToRoute("Boards");

            if (!await _forumService.IsCustomerAllowedToEditPost(await _workContext.GetCurrentCustomer(), forumPost))
                return Challenge();

            var model = await _forumModelFactory.PreparePostEditModel(forumPost, false);
            
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> PostEdit(EditForumPostModel model, bool captchaValid)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumPost = await _forumService.GetPostById(model.Id);
            if (forumPost == null)
                return RedirectToRoute("Boards");

            if (!await _forumService.IsCustomerAllowedToEditPost(await _workContext.GetCurrentCustomer(), forumPost))
                return Challenge();

            var forumTopic = await _forumService.GetTopicById(forumPost.TopicId);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var forum = await _forumService.GetForumById(forumTopic.ForumId);
            if (forum == null)
                return RedirectToRoute("Boards");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnForum && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var nowUtc = DateTime.UtcNow;

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text.Substring(0, maxPostLength);
                    }

                    forumPost.UpdatedOnUtc = nowUtc;
                    forumPost.Text = text;
                    await _forumService.UpdatePost(forumPost);

                    //subscription
                    if (await _forumService.IsCustomerAllowedToSubscribe(await _workContext.GetCurrentCustomer()))
                    {
                        var forumSubscription = (await _forumService.GetAllSubscriptions((await _workContext.GetCurrentCustomer()).Id,
                            0, forumPost.TopicId, 0, 1)).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                                    TopicId = forumPost.TopicId,
                                    CreatedOnUtc = nowUtc
                                };
                                await _forumService.InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                await _forumService.DeleteSubscription(forumSubscription);
                            }
                        }
                    }

                    var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;
                    var pageIndex = (await _forumService.CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.Id) + 1);
                    string url;
                    if (pageIndex > 1)
                    {
                        url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeName(forumTopic), pageNumber = pageIndex });
                    }
                    else
                    {
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeName(forumTopic) });
                    }
                    return LocalRedirect($"{url}#{forumPost.Id}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            model = await _forumModelFactory.PreparePostEditModel(forumPost, true);

            return View(model);
        }

        public virtual async Task<IActionResult> Search(string searchterms, bool? adv, string forumId,
            string within, string limitDays, int pageNumber = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = await _forumModelFactory.PrepareSearchModel(searchterms, adv, forumId, within, limitDays, pageNumber);
            
            return View(model);
        }

        public virtual async Task<IActionResult> CustomerForumSubscriptions(int? pageNumber)
        {
            if (!_forumSettings.AllowCustomersToManageSubscriptions)
                return RedirectToRoute("CustomerInfo");

            var model = await _forumModelFactory.PrepareCustomerForumSubscriptionsModel(pageNumber);
            
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
                        var forumSubscription = await _forumService.GetSubscriptionById(forumSubscriptionId);
                        if (forumSubscription != null && forumSubscription.CustomerId == (await _workContext.GetCurrentCustomer()).Id)
                        {
                            await _forumService.DeleteSubscription(forumSubscription);
                        }
                    }
                }
            }

            return RedirectToRoute("CustomerForumSubscriptions");
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostVote(int postId, bool isUp)
        {
            if (!_forumSettings.AllowPostVoting)
                return new NullJsonResult();

            var forumPost = await _forumService.GetPostById(postId);
            if (forumPost == null)
                return new NullJsonResult();

            if (!await _customerService.IsRegistered(await _workContext.GetCurrentCustomer()))
                return Json(new
                {
                    Error = await _localizationService.GetResource("Forum.Votes.Login"),
                    VoteCount = forumPost.VoteCount
                });

            if ((await _workContext.GetCurrentCustomer()).Id == forumPost.CustomerId)
                return Json(new
                {
                    Error = await _localizationService.GetResource("Forum.Votes.OwnPost"),
                    VoteCount = forumPost.VoteCount
                });

            var forumPostVote = await _forumService.GetPostVote(postId, await _workContext.GetCurrentCustomer());
            if (forumPostVote != null)
            {
                if ((forumPostVote.IsUp && isUp) || (!forumPostVote.IsUp && !isUp))
                    return Json(new
                    {
                        Error = await _localizationService.GetResource("Forum.Votes.AlreadyVoted"),
                        VoteCount = forumPost.VoteCount
                    });

                await _forumService.DeletePostVote(forumPostVote);
                return Json(new { VoteCount = forumPost.VoteCount });
            }

            if (await _forumService.GetNumberOfPostVotes(await _workContext.GetCurrentCustomer(), DateTime.UtcNow.AddDays(-1)) >= _forumSettings.MaxVotesPerDay)
                return Json(new
                {
                    Error = string.Format(await _localizationService.GetResource("Forum.Votes.MaxVotesReached"), _forumSettings.MaxVotesPerDay),
                    VoteCount = forumPost.VoteCount
                });

            await _forumService.InsertPostVote(new ForumPostVote
            {
                CustomerId = (await _workContext.GetCurrentCustomer()).Id,
                ForumPostId = postId,
                IsUp = isUp,
                CreatedOnUtc = DateTime.UtcNow
            });

            return Json(new { VoteCount = forumPost.VoteCount, IsUp = isUp });
        }

        #endregion
    }
}