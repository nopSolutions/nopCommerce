using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Http;
using Nop.Core.Rss;
using Nop.Plugin.Misc.Forums.Domain;
using Nop.Plugin.Misc.Forums.Public.Factories;
using Nop.Plugin.Misc.Forums.Public.Models;
using Nop.Plugin.Misc.Forums.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.Forums.Public.Controllers;

[WwwRequirement]
[CheckLanguageSeoCode]
[CheckAccessPublicStore]
[CheckAccessClosedStore]
[CheckDiscountCoupon]
[CheckAffiliate]
[AutoValidateAntiforgeryToken]
public class BoardsController : BasePluginController
{
    #region Fields

    private readonly CaptchaSettings _captchaSettings;
    private readonly CustomerSettings _customerSettings;
    private readonly ForumModelFactory _forumModelFactory;
    private readonly ForumService _forumService;
    private readonly ForumSettings _forumSettings;
    private readonly ICustomerActivityService _customerActivityService;
    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IStoreContext _storeContext;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public BoardsController(CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        ForumModelFactory forumModelFactory,
        ForumService forumService,
        ForumSettings forumSettings,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IStoreContext storeContext,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;
        _forumModelFactory = forumModelFactory;
        _forumService = forumService;
        _forumSettings = forumSettings;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _storeContext = storeContext;
        _webHelper = webHelper;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Index()
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _forumModelFactory.PrepareBoardsIndexModelAsync();

        return View("~/Plugins/Misc.Forums/Public/Views/Index.cshtml", model);
    }

    public async Task<IActionResult> ActiveDiscussions(int forumId = 0, int pageNumber = 1)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _forumModelFactory.PrepareActiveDiscussionsModelAsync(forumId, pageNumber);

        return View("~/Plugins/Misc.Forums/Public/Views/ActiveDiscussions.cshtml", model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public async Task<IActionResult> ActiveDiscussionsRss(int forumId = 0)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (!_forumSettings.ActiveDiscussionsFeedEnabled)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        var topics = await _forumService.GetActiveTopicsAsync(forumId, 0, _forumSettings.ActiveDiscussionsFeedCount);
        var url = Url.RouteUrl(ForumDefaults.Routes.Public.ACTIVE_DISCUSSIONS_RSS, null, _webHelper.GetCurrentRequestProtocol());

        var feedTitle = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ActiveDiscussionsFeedTitle");
        var feedDescription = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ActiveDiscussionsFeedDescription");

        var store = await _storeContext.GetCurrentStoreAsync();
        var feed = new RssFeed(
            string.Format(feedTitle, await _localizationService.GetLocalizedAsync(store, x => x.Name)),
            feedDescription,
            new Uri(url),
            DateTime.UtcNow);

        var items = new List<RssItem>();

        var viewsText = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Views");
        var repliesText = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Replies");

        foreach (var topic in topics)
        {
            var topicUrl = Url.RouteUrl(ForumDefaults.Routes.Public.TOPIC_SLUG, new { id = topic.Id, slug = await _forumService.GetTopicSeNameAsync(topic) }, _webHelper.GetCurrentRequestProtocol());
            var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";

            items.Add(new(topic.Subject, content, new Uri(topicUrl), $"urn:store:{store.Id}:activeDiscussions:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
        }
        feed.Items = items;

        return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
    }

    public async Task<IActionResult> ForumGroup(int id)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumGroup = await _forumService.GetForumGroupByIdAsync(id);
        if (forumGroup == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        var model = await _forumModelFactory.PrepareForumGroupModelAsync(forumGroup);

        return View("~/Plugins/Misc.Forums/Public/Views/ForumGroup.cshtml", model);
    }

    public async Task<IActionResult> Forum(int id, int pageNumber = 1)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forum = await _forumService.GetForumByIdAsync(id);
        if (forum == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        var model = await _forumModelFactory.PrepareForumPageModelAsync(forum, pageNumber);

        return View("~/Plugins/Misc.Forums/Public/Views/Forum.cshtml", model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public async Task<IActionResult> ForumRss(int id)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (!_forumSettings.ForumFeedsEnabled)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        var topicLimit = _forumSettings.ForumFeedCount;
        var forum = await _forumService.GetForumByIdAsync(id);

        if (forum != null)
        {
            //Order by newest topic posts & limit the number of topics to return
            var topics = await _forumService.GetAllTopicsAsync(forum.Id, 0, string.Empty, ForumSearchType.All, 0, 0, topicLimit);

            var url = Url.RouteUrl(ForumDefaults.Routes.Public.FORUM_RSS, new { id = forum.Id }, _webHelper.GetCurrentRequestProtocol());

            var feedTitle = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ForumFeedTitle");
            var feedDescription = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ForumFeedDescription");

            var store = await _storeContext.GetCurrentStoreAsync();
            var feed = new RssFeed(
                string.Format(feedTitle, await _localizationService.GetLocalizedAsync(store, x => x.Name), forum.Name),
                feedDescription,
                new Uri(url),
                DateTime.UtcNow);

            var items = new List<RssItem>();

            var viewsText = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Views");
            var repliesText = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Replies");

            foreach (var topic in topics)
            {
                var topicUrl = Url.RouteUrl(ForumDefaults.Routes.Public.TOPIC_SLUG, new { id = topic.Id, slug = await _forumService.GetTopicSeNameAsync(topic) }, _webHelper.GetCurrentRequestProtocol());
                var content = $"{repliesText}: {(topic.NumPosts > 0 ? topic.NumPosts - 1 : 0)}, {viewsText}: {topic.Views}";

                items.Add(new(topic.Subject, content, new Uri(topicUrl), $"urn:store:{store.Id}:forum:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
            }

            feed.Items = items;

            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

        return new RssActionResult(new RssFeed(new Uri(_webHelper.GetStoreLocation())), _webHelper.GetThisPageUrl(false));
    }

    [HttpPost]
    public async Task<IActionResult> ForumWatch(int id)
    {
        var watchTopic = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.WatchForum");
        var unwatchTopic = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.UnwatchForum");
        var returnText = watchTopic;

        var forum = await _forumService.GetForumByIdAsync(id);
        if (forum == null)
            return Json(new { Subscribed = false, Text = returnText, Error = true });

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
            return Json(new { Subscribed = false, Text = returnText, Error = true });

        var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, forum.Id, 0, 0, 1)).FirstOrDefault();

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
            await _forumService.InsertSubscriptionAsync(forumSubscription);
            subscribed = true;
            returnText = unwatchTopic;
        }
        else
        {
            await _forumService.DeleteSubscriptionAsync(forumSubscription);
            subscribed = false;
        }

        return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
    }

    public async Task<IActionResult> Topic(int id, int pageNumber = 1)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
        if (forumTopic == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        var model = await _forumModelFactory.PrepareForumTopicPageModelAsync(forumTopic, pageNumber);
        //if no posts loaded, redirect to the first page
        if (!model.ForumPostModels.Any() && pageNumber > 1)
            return RedirectToRoute(ForumDefaults.Routes.Public.TOPIC_SLUG, new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });

        //update view count
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!customer.IsSearchEngineAccount())
        {
            forumTopic.Views += 1;
            await _forumService.UpdateTopicAsync(forumTopic);
        }

        return View("~/Plugins/Misc.Forums/Public/Views/Topic.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> TopicWatch(int id)
    {
        var watchTopic = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.WatchTopic");
        var unwatchTopic = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.UnwatchTopic");
        var returnText = watchTopic;

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
        if (forumTopic == null)
            return Json(new { Subscribed = false, Text = returnText, Error = true });

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
            return Json(new { Subscribed = false, Text = returnText, Error = true });

        var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();

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
            await _forumService.InsertSubscriptionAsync(forumSubscription);
            subscribed = true;
            returnText = unwatchTopic;
        }
        else
        {
            await _forumService.DeleteSubscriptionAsync(forumSubscription);
            subscribed = false;
        }

        return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
    }

    public async Task<IActionResult> TopicMove(int id)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
        if (forumTopic == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        if (!await _forumService.IsCustomerAllowedToMoveTopicAsync(await _workContext.GetCurrentCustomerAsync(), forumTopic))
            return Challenge();

        var model = await _forumModelFactory.PrepareTopicMoveAsync(forumTopic);

        return View("~/Plugins/Misc.Forums/Public/Views/TopicMove.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> TopicMove(TopicMoveModel model)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(model.Id);

        if (forumTopic == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        var newForumId = model.ForumSelected;
        var forum = await _forumService.GetForumByIdAsync(newForumId);

        if (forum != null && forumTopic.ForumId != newForumId)
            await _forumService.MoveTopicAsync(forumTopic.Id, newForumId);

        return RedirectToRoute(ForumDefaults.Routes.Public.TOPIC_SLUG, new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
    }

    [HttpPost]
    public async Task<IActionResult> TopicDelete(int id)
    {
        if (!_forumSettings.ForumsEnabled)
        {
            return Json(new
            {
                redirect = Url.RouteUrl(NopRouteNames.General.HOMEPAGE),
            });
        }

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
        if (forumTopic != null)
        {
            if (!await _forumService.IsCustomerAllowedToDeleteTopicAsync(await _workContext.GetCurrentCustomerAsync(), forumTopic))
                return Challenge();

            var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);

            await _forumService.DeleteTopicAsync(forumTopic);

            //activity log
            await _customerActivityService.InsertActivityAsync(ForumDefaults.ActivityLogTypeSystemNames.DeleteForumTopic,
                string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ActivityLog.DeleteForumTopic"), forumTopic.Id));

            if (forum != null)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl(ForumDefaults.Routes.Public.FORUM_SLUG, new { id = forum.Id, slug = await _forumService.GetForumSeNameAsync(forum) }),
                });
            }
        }

        return Json(new
        {
            redirect = Url.RouteUrl(ForumDefaults.Routes.Public.BOARDS),
        });
    }

    public async Task<IActionResult> TopicCreate(int id)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forum = await _forumService.GetForumByIdAsync(id);
        if (forum == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        if (!await _forumService.IsCustomerAllowedToCreateTopicAsync(await _workContext.GetCurrentCustomerAsync(), forum))
            return Challenge();

        var model = new EditForumTopicModel();
        await _forumModelFactory.PrepareTopicCreateModelAsync(forum, model);

        return View("~/Plugins/Misc.Forums/Public/Views/TopicCreate.cshtml", model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public async Task<IActionResult> TopicCreate(EditForumTopicModel model, bool captchaValid)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forum = await _forumService.GetForumByIdAsync(model.ForumId);
        if (forum == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _forumSettings.ShowCaptcha && !captchaValid)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

        if (ModelState.IsValid)
        {
            try
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                if (!await _forumService.IsCustomerAllowedToCreateTopicAsync(customer, forum))
                    return Challenge();

                var subject = model.Subject;
                var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    subject = subject[0..maxSubjectLength];

                var text = model.Text;
                var maxPostLength = _forumSettings.PostMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                    text = text[0..maxPostLength];

                var topicType = ForumTopicType.Normal;
                var nowUtc = DateTime.UtcNow;

                if (await _forumService.IsCustomerAllowedToSetTopicPriorityAsync(customer))
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
                await _forumService.InsertTopicAsync(forumTopic);

                //activity log
                await _customerActivityService.InsertActivityAsync(ForumDefaults.ActivityLogTypeSystemNames.AddForumTopic,
                    string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ActivityLog.AddForumTopic"), forumTopic.Id));

                //forum post
                var forumPost = new ForumPost
                {
                    TopicId = forumTopic.Id,
                    CustomerId = customer.Id,
                    Text = text,
                    IPAddress = _customerSettings.StoreIpAddresses ? _webHelper.GetCurrentIpAddress() : string.Empty,
                    CreatedOnUtc = nowUtc,
                    UpdatedOnUtc = nowUtc
                };
                await _forumService.InsertPostAsync(forumPost, false);

                //update forum topic
                forumTopic.NumPosts = 1;
                forumTopic.LastPostId = forumPost.Id;
                forumTopic.LastPostCustomerId = forumPost.CustomerId;
                forumTopic.LastPostTime = forumPost.CreatedOnUtc;
                forumTopic.UpdatedOnUtc = nowUtc;
                await _forumService.UpdateTopicAsync(forumTopic);

                //subscription                
                if (await _forumService.IsCustomerAllowedToSubscribeAsync(customer) && model.Subscribed)
                {
                    await _forumService.InsertSubscriptionAsync(new()
                    {
                        SubscriptionGuid = Guid.NewGuid(),
                        CustomerId = customer.Id,
                        TopicId = forumTopic.Id,
                        CreatedOnUtc = nowUtc
                    });
                }

                return RedirectToRoute(ForumDefaults.Routes.Public.TOPIC_SLUG, new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        //redisplay form
        await _forumModelFactory.PrepareTopicCreateModelAsync(forum, model);

        return View("~/Plugins/Misc.Forums/Public/Views/TopicCreate.cshtml", model);
    }

    public async Task<IActionResult> TopicEdit(int id)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
        if (forumTopic == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        if (!await _forumService.IsCustomerAllowedToEditTopicAsync(await _workContext.GetCurrentCustomerAsync(), forumTopic))
            return Challenge();

        var model = new EditForumTopicModel();
        await _forumModelFactory.PrepareTopicEditModelAsync(forumTopic, model, false);

        return View("~/Plugins/Misc.Forums/Public/Views/TopicEdit.cshtml", model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public async Task<IActionResult> TopicEdit(EditForumTopicModel model, bool captchaValid)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(model.Id);

        if (forumTopic == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
        if (forum == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _forumSettings.ShowCaptcha && !captchaValid)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

        if (ModelState.IsValid)
        {
            try
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                if (!await _forumService.IsCustomerAllowedToEditTopicAsync(customer, forumTopic))
                    return Challenge();

                var subject = model.Subject;
                var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    subject = subject[0..maxSubjectLength];

                var text = model.Text;
                var maxPostLength = _forumSettings.PostMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                    text = text[0..maxPostLength];

                var topicType = ForumTopicType.Normal;
                var nowUtc = DateTime.UtcNow;

                if (await _forumService.IsCustomerAllowedToSetTopicPriorityAsync(customer))
                    topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

                //forum topic
                forumTopic.TopicTypeId = (int)topicType;
                forumTopic.Subject = subject;
                forumTopic.UpdatedOnUtc = nowUtc;
                await _forumService.UpdateTopicAsync(forumTopic);

                //activity log
                await _customerActivityService.InsertActivityAsync(ForumDefaults.ActivityLogTypeSystemNames.EditForumTopic,
                    string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ActivityLog.EditForumTopic"), forumTopic.Id));

                //forum post                
                var firstPost = await _forumService.GetFirstPostAsync(forumTopic);
                if (firstPost != null)
                {
                    firstPost.Text = text;
                    firstPost.UpdatedOnUtc = nowUtc;
                    await _forumService.UpdatePostAsync(firstPost);
                }
                else
                {
                    //error (not possible)
                    firstPost = new ForumPost
                    {
                        TopicId = forumTopic.Id,
                        CustomerId = forumTopic.CustomerId,
                        Text = text,
                        IPAddress = _customerSettings.StoreIpAddresses ? _webHelper.GetCurrentIpAddress() : string.Empty,
                        UpdatedOnUtc = nowUtc
                    };

                    await _forumService.InsertPostAsync(firstPost, false);
                }

                //subscription
                if (await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
                {
                    var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
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

                            await _forumService.InsertSubscriptionAsync(forumSubscription);
                        }
                    }
                    else
                    {
                        if (forumSubscription != null)
                            await _forumService.DeleteSubscriptionAsync(forumSubscription);
                    }
                }

                // redirect to the topic page with the topic slug
                return RedirectToRoute(ForumDefaults.Routes.Public.TOPIC_SLUG, new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        //redisplay form
        await _forumModelFactory.PrepareTopicEditModelAsync(forumTopic, model, true);

        return View("~/Plugins/Misc.Forums/Public/Views/TopicEdit.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> PostDelete(int id)
    {
        if (!_forumSettings.ForumsEnabled)
        {
            return Json(new
            {
                redirect = Url.RouteUrl(NopRouteNames.General.HOMEPAGE),
            });
        }

        var forumPost = await _forumService.GetPostByIdAsync(id);

        if (forumPost == null)
            return Json(new { redirect = Url.RouteUrl(ForumDefaults.Routes.Public.BOARDS) });

        if (!await _forumService.IsCustomerAllowedToDeletePostAsync(await _workContext.GetCurrentCustomerAsync(), forumPost))
            return Challenge();

        var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
        var forumId = forumTopic.ForumId;
        var forum = await _forumService.GetForumByIdAsync(forumId);
        var forumSlug = await _forumService.GetForumSeNameAsync(forum);

        await _forumService.DeletePostAsync(forumPost);

        //activity log
        await _customerActivityService.InsertActivityAsync(ForumDefaults.ActivityLogTypeSystemNames.DeleteForumPost,
            string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ActivityLog.DeleteForumPost"), forumPost.Id));

        //get topic one more time because it can be deleted (first or only post deleted)
        forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
        if (forumTopic == null)
        {
            return Json(new
            {
                redirect = Url.RouteUrl(ForumDefaults.Routes.Public.FORUM_SLUG, new { id = forumId, slug = forumSlug }),
            });
        }

        return Json(new
        {
            redirect = Url.RouteUrl(ForumDefaults.Routes.Public.TOPIC_SLUG, new { id = forumTopic.Id, slug = await _forumService.GetTopicSeNameAsync(forumTopic) }),
        });

    }

    public async Task<IActionResult> PostCreate(int id, int? quote)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(id);
        if (forumTopic == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        if (!await _forumService.IsCustomerAllowedToCreatePostAsync(await _workContext.GetCurrentCustomerAsync(), forumTopic))
            return Challenge();

        var model = await _forumModelFactory.PreparePostCreateModelAsync(forumTopic, quote, false);

        return View("~/Plugins/Misc.Forums/Public/Views/PostCreate.cshtml", model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public async Task<IActionResult> PostCreate(EditForumPostModel model, bool captchaValid)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumTopic = await _forumService.GetTopicByIdAsync(model.ForumTopicId);
        if (forumTopic == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _forumSettings.ShowCaptcha && !captchaValid)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

        if (ModelState.IsValid)
        {
            try
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                if (!await _forumService.IsCustomerAllowedToCreatePostAsync(customer, forumTopic))
                    return Challenge();

                var text = model.Text;
                var maxPostLength = _forumSettings.PostMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                    text = text[0..maxPostLength];

                var nowUtc = DateTime.UtcNow;

                var forumPost = new ForumPost
                {
                    TopicId = forumTopic.Id,
                    CustomerId = customer.Id,
                    Text = text,
                    IPAddress = _customerSettings.StoreIpAddresses ? _webHelper.GetCurrentIpAddress() : string.Empty,
                    CreatedOnUtc = nowUtc,
                    UpdatedOnUtc = nowUtc
                };
                await _forumService.InsertPostAsync(forumPost, true);

                //activity log
                await _customerActivityService.InsertActivityAsync(ForumDefaults.ActivityLogTypeSystemNames.AddForumPost,
                    string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ActivityLog.AddForumPost"), forumPost.Id));

                //subscription
                if (await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
                {
                    var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, 0, forumPost.TopicId, 0, 1)).FirstOrDefault();
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

                            await _forumService.InsertSubscriptionAsync(forumSubscription);
                        }
                    }
                    else
                    {
                        if (forumSubscription != null)
                            await _forumService.DeleteSubscriptionAsync(forumSubscription);
                    }
                }

                var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;

                var pageIndex = await _forumService.CalculateTopicPageIndexAsync(forumPost.TopicId, pageSize, forumPost.Id) + 1;
                var url = pageIndex > 1
                    ? Url.RouteUrl(ForumDefaults.Routes.Public.TOPIC_SLUG_PAGED, new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic), pageNumber = pageIndex })
                    : Url.RouteUrl(ForumDefaults.Routes.Public.TOPIC_SLUG, new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });
                return LocalRedirect($"{url}#{forumPost.Id}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        //redisplay form
        model = await _forumModelFactory.PreparePostCreateModelAsync(forumTopic, 0, true);

        return View("~/Plugins/Misc.Forums/Public/Views/PostCreate.cshtml", model);
    }

    public async Task<IActionResult> PostEdit(int id)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumPost = await _forumService.GetPostByIdAsync(id);
        if (forumPost == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        if (!await _forumService.IsCustomerAllowedToEditPostAsync(await _workContext.GetCurrentCustomerAsync(), forumPost))
            return Challenge();

        var model = await _forumModelFactory.PreparePostEditModelAsync(forumPost, false);

        return View("~/Plugins/Misc.Forums/Public/Views/PostEdit.cshtml", model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public async Task<IActionResult> PostEdit(EditForumPostModel model, bool captchaValid)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var forumPost = await _forumService.GetPostByIdAsync(model.Id);
        if (forumPost == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _forumService.IsCustomerAllowedToEditPostAsync(customer, forumPost))
            return Challenge();

        var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
        if (forumTopic == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
        if (forum == null)
            return RedirectToRoute(ForumDefaults.Routes.Public.BOARDS);

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _forumSettings.ShowCaptcha && !captchaValid)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

        if (ModelState.IsValid)
        {
            try
            {
                var nowUtc = DateTime.UtcNow;

                var text = model.Text;
                var maxPostLength = _forumSettings.PostMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                    text = text[0..maxPostLength];

                forumPost.UpdatedOnUtc = nowUtc;
                forumPost.Text = text;
                await _forumService.UpdatePostAsync(forumPost);

                //activity log
                await _customerActivityService.InsertActivityAsync(ForumDefaults.ActivityLogTypeSystemNames.EditForumPost,
                    string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ActivityLog.EditForumPost"), forumPost.Id));

                //subscription
                if (await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
                {
                    var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, 0, forumPost.TopicId, 0, 1)).FirstOrDefault();
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
                            await _forumService.InsertSubscriptionAsync(forumSubscription);
                        }
                    }
                    else
                    {
                        if (forumSubscription != null)
                            await _forumService.DeleteSubscriptionAsync(forumSubscription);
                    }
                }

                var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;
                var pageIndex = (await _forumService.CalculateTopicPageIndexAsync(forumPost.TopicId, pageSize, forumPost.Id) + 1);
                var url = pageIndex > 1
                    ? Url.RouteUrl(ForumDefaults.Routes.Public.TOPIC_SLUG_PAGED, new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic), pageNumber = pageIndex })
                    : Url.RouteUrl(ForumDefaults.Routes.Public.TOPIC_SLUG, new { id = forumPost.TopicId, slug = await _forumService.GetTopicSeNameAsync(forumTopic) });

                return LocalRedirect($"{url}#{forumPost.Id}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        //redisplay form
        model = await _forumModelFactory.PreparePostEditModelAsync(forumPost, true);

        return View("~/Plugins/Misc.Forums/Public/Views/PostEdit.cshtml", model);
    }

    public async Task<IActionResult> Search(string searchterms, bool? advs, string forumId,
        string within, string limitDays, int pageNumber = 1)
    {
        if (!_forumSettings.ForumsEnabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _forumModelFactory.PrepareSearchModelAsync(searchterms, advs, forumId, within, limitDays, pageNumber);

        return View("~/Plugins/Misc.Forums/Public/Views/Search.cshtml", model);
    }

    public async Task<IActionResult> CustomerForumSubscriptions(int? pageNumber)
    {
        if (!_forumSettings.AllowCustomersToManageSubscriptions)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var model = await _forumModelFactory.PrepareCustomerForumSubscriptionsModelAsync(pageNumber);

        return View("~/Plugins/Misc.Forums/Public/Views/CustomerForumSubscriptions.cshtml", model);
    }

    [HttpPost, ActionName("CustomerForumSubscriptions")]
    public async Task<IActionResult> CustomerForumSubscriptionsPOST(IFormCollection formCollection)
    {
        foreach (var key in formCollection.Keys)
        {
            var value = formCollection[key];

            if (value.Equals("on") && key.StartsWith("fs", StringComparison.InvariantCultureIgnoreCase))
            {
                var id = key.Replace("fs", "").Trim();
                if (int.TryParse(id, out var forumSubscriptionId))
                {
                    var forumSubscription = await _forumService.GetSubscriptionByIdAsync(forumSubscriptionId);
                    var customer = await _workContext.GetCurrentCustomerAsync();

                    if (forumSubscription != null && forumSubscription.CustomerId == customer.Id)
                        await _forumService.DeleteSubscriptionAsync(forumSubscription);
                }
            }
        }

        return RedirectToRoute(ForumDefaults.Routes.Public.CUSTOMER_FORUM_SUBSCRIPTIONS);
    }

    [HttpPost]
    public async Task<IActionResult> PostVote(int postId, bool isUp)
    {
        if (!_forumSettings.AllowPostVoting)
            return new NullJsonResult();

        var forumPost = await _forumService.GetPostByIdAsync(postId);
        if (forumPost == null)
            return new NullJsonResult();

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
        {
            return Json(new
            {
                Error = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Votes.Login"),
                VoteCount = forumPost.VoteCount
            });
        }

        if (customer.Id == forumPost.CustomerId)
        {
            return Json(new
            {
                Error = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Votes.OwnPost"),
                VoteCount = forumPost.VoteCount
            });
        }

        var forumPostVote = await _forumService.GetPostVoteAsync(postId, customer);
        if (forumPostVote != null)
        {
            if ((forumPostVote.IsUp && isUp) || (!forumPostVote.IsUp && !isUp))
            {
                return Json(new
                {
                    Error = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Votes.AlreadyVoted"),
                    VoteCount = forumPost.VoteCount
                });
            }

            await _forumService.DeletePostVoteAsync(forumPostVote);
            return Json(new { VoteCount = forumPost.VoteCount });
        }

        if (await _forumService.GetNumberOfPostVotesAsync(customer, DateTime.UtcNow.AddDays(-1)) >= _forumSettings.MaxVotesPerDay)
        {
            return Json(new
            {
                Error = string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Votes.MaxVotesReached"), _forumSettings.MaxVotesPerDay),
                VoteCount = forumPost.VoteCount
            });
        }

        await _forumService.InsertPostVoteAsync(new()
        {
            CustomerId = customer.Id,
            ForumPostId = postId,
            IsUp = isUp,
            CreatedOnUtc = DateTime.UtcNow
        });

        return Json(new { VoteCount = forumPost.VoteCount, IsUp = isUp });
    }

    [HttpPost]
    public async Task<IActionResult> SaveForumAccountInfo(ForumAccountInfoModel model)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        await _genericAttributeService.SaveAttributeAsync(customer, ForumDefaults.SignatureAttribute, model.Signature);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Account.Signature.Saved"));

        return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
    }

    public async Task<IActionResult> ProfilePosts(int id, int pageNumber)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer is null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = await _forumModelFactory.PrepareProfilePostsModelAsync(customer, pageNumber);

        return View("~/Plugins/Misc.Forums/Public/Views/ProfilePosts.cshtml", model);
    }

    #endregion
}