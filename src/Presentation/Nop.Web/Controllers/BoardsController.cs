using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Rss;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Boards;

namespace Nop.Web.Controllers
{
    [HttpsRequirement(SslRequirement.No)]
    public partial class BoardsController : BasePublicController
    {
        #region Fields

        private readonly ForumSettings _forumSettings;
        private readonly IForumModelFactory _forumModelFactory;
        private readonly IForumService _forumService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public BoardsController(ForumSettings forumSettings,
            IForumModelFactory forumModelFactory,
            IForumService forumService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _forumSettings = forumSettings;
            _forumModelFactory = forumModelFactory;
            _forumService = forumService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = _forumModelFactory.PrepareBoardsIndexModel();
            return View(model);
        }

        public virtual IActionResult ActiveDiscussions(int forumId = 0, int pageNumber = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = _forumModelFactory.PrepareActiveDiscussionsModel(forumId, pageNumber);
            return View(model);
        }

        public virtual IActionResult ActiveDiscussionsRss(int forumId = 0)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            if (!_forumSettings.ActiveDiscussionsFeedEnabled)
                return RedirectToRoute("Boards");

            var topics = _forumService.GetActiveTopics(forumId, 0, _forumSettings.ActiveDiscussionsFeedCount);
            var url = Url.RouteUrl("ActiveDiscussionsRSS", null, _webHelper.CurrentRequestProtocol);

            var feedTitle = _localizationService.GetResource("Forum.ActiveDiscussionsFeedTitle");
            var feedDescription = _localizationService.GetResource("Forum.ActiveDiscussionsFeedDescription");

            var feed = new RssFeed(
                string.Format(feedTitle, _localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name)),
                feedDescription,
                new Uri(url),
                DateTime.UtcNow);

            var items = new List<RssItem>();

            var viewsText = _localizationService.GetResource("Forum.Views");
            var repliesText = _localizationService.GetResource("Forum.Replies");

            foreach (var topic in topics)
            {
                var topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = _forumService.GetTopicSeName(topic) }, _webHelper.CurrentRequestProtocol);
                var content = $"{repliesText}: {topic.NumReplies.ToString()}, {viewsText}: {topic.Views.ToString()}";

                items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl),
                    $"urn:store:{_storeContext.CurrentStore.Id}:activeDiscussions:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
            }
            feed.Items = items;

            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

        public virtual IActionResult ForumGroup(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumGroup = _forumService.GetForumGroupById(id);
            if (forumGroup == null)
                return RedirectToRoute("Boards");

            var model = _forumModelFactory.PrepareForumGroupModel(forumGroup);
            return View(model);
        }

        public virtual IActionResult Forum(int id, int pageNumber = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = _forumService.GetForumById(id);
            if (forum == null)
                return RedirectToRoute("Boards");

            var model = _forumModelFactory.PrepareForumPageModel(forum, pageNumber);
            return View(model);
        }

        public virtual IActionResult ForumRss(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            if (!_forumSettings.ForumFeedsEnabled)
                return RedirectToRoute("Boards");

            var topicLimit = _forumSettings.ForumFeedCount;
            var forum = _forumService.GetForumById(id);

            if (forum != null)
            {
                //Order by newest topic posts & limit the number of topics to return
                var topics = _forumService.GetAllTopics(forum.Id, 0, string.Empty,
                     ForumSearchType.All, 0, 0, topicLimit);

                var url = Url.RouteUrl("ForumRSS", new { id = forum.Id }, _webHelper.CurrentRequestProtocol);

                var feedTitle = _localizationService.GetResource("Forum.ForumFeedTitle");
                var feedDescription = _localizationService.GetResource("Forum.ForumFeedDescription");

                var feed = new RssFeed(
                    string.Format(feedTitle, _localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name), forum.Name),
                    feedDescription,
                    new Uri(url),
                    DateTime.UtcNow);

                var items = new List<RssItem>();

                var viewsText = _localizationService.GetResource("Forum.Views");
                var repliesText = _localizationService.GetResource("Forum.Replies");

                foreach (var topic in topics)
                {
                    var topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = _forumService.GetTopicSeName(topic) }, _webHelper.CurrentRequestProtocol);
                    var content = $"{repliesText}: {topic.NumReplies}, {viewsText}: {topic.Views}";

                    items.Add(new RssItem(topic.Subject, content, new Uri(topicUrl), $"urn:store:{_storeContext.CurrentStore.Id}:forum:topic:{topic.Id}", topic.LastPostTime ?? topic.UpdatedOnUtc));
                }

                feed.Items = items;

                return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
            }

            return new RssActionResult(new RssFeed(new Uri(_webHelper.GetStoreLocation())), _webHelper.GetThisPageUrl(false));
        }

        [HttpPost]
        public virtual IActionResult ForumWatch(int id)
        {
            var watchTopic = _localizationService.GetResource("Forum.WatchForum");
            var unwatchTopic = _localizationService.GetResource("Forum.UnwatchForum");
            var returnText = watchTopic;

            var forum = _forumService.GetForumById(id);
            if (forum == null)
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            if (!_forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer))
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id,
                forum.Id, 0, 0, 1).FirstOrDefault();

            bool subscribed;
            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription
                {
                    SubscriptionGuid = Guid.NewGuid(),
                    CustomerId = _workContext.CurrentCustomer.Id,
                    ForumId = forum.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _forumService.InsertSubscription(forumSubscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                _forumService.DeleteSubscription(forumSubscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        public virtual IActionResult Topic(int id, int pageNumber = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var model = _forumModelFactory.PrepareForumTopicPageModel(forumTopic, pageNumber);
            //if no posts loaded, redirect to the first page
            if (!model.ForumPostModels.Any() && pageNumber > 1)
                return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = _forumService.GetTopicSeName(forumTopic) });

            //update view count
            forumTopic.Views += 1;
            _forumService.UpdateTopic(forumTopic);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult TopicWatch(int id)
        {
            var watchTopic = _localizationService.GetResource("Forum.WatchTopic");
            var unwatchTopic = _localizationService.GetResource("Forum.UnwatchTopic");
            var returnText = watchTopic;

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            if (!_forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer))
                return Json(new { Subscribed = false, Text = returnText, Error = true });

            var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id,
                0, forumTopic.Id, 0, 1).FirstOrDefault();

            bool subscribed;
            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription
                {
                    SubscriptionGuid = Guid.NewGuid(),
                    CustomerId = _workContext.CurrentCustomer.Id,
                    TopicId = forumTopic.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _forumService.InsertSubscription(forumSubscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                _forumService.DeleteSubscription(forumSubscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        public virtual IActionResult TopicMove(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var model = _forumModelFactory.PrepareTopicMove(forumTopic);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult TopicMove(TopicMoveModel model)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = _forumService.GetTopicById(model.Id);

            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var newForumId = model.ForumSelected;
            var forum = _forumService.GetForumById(newForumId);

            if (forum != null && forumTopic.ForumId != newForumId)
                _forumService.MoveTopic(forumTopic.Id, newForumId);

            return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = _forumService.GetTopicSeName(forumTopic) });
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult TopicDelete(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return Json(new
                {
                    redirect = Url.RouteUrl("Homepage"),
                });

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic != null)
            {
                if (!_forumService.IsCustomerAllowedToDeleteTopic(_workContext.CurrentCustomer, forumTopic))
                    return Challenge();

                var forum = _forumService.GetForumById(forumTopic.ForumId);

                _forumService.DeleteTopic(forumTopic);

                if (forum != null)
                    return Json(new
                    {
                        redirect = Url.RouteUrl("ForumSlug", new { id = forum.Id, slug = _forumService.GetForumSeName(forum) }),
                    });
            }

            return Json(new
            {
                redirect = Url.RouteUrl("Boards"),
            });
        }

        public virtual IActionResult TopicCreate(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = _forumService.GetForumById(id);
            if (forum == null)
                return RedirectToRoute("Boards");

            if (_forumService.IsCustomerAllowedToCreateTopic(_workContext.CurrentCustomer, forum) == false)
                return Challenge();

            var model = new EditForumTopicModel();
            _forumModelFactory.PrepareTopicCreateModel(forum, model);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult TopicCreate(EditForumTopicModel model)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forum = _forumService.GetForumById(model.ForumId);
            if (forum == null)
                return RedirectToRoute("Boards");

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_forumService.IsCustomerAllowedToCreateTopic(_workContext.CurrentCustomer, forum))
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
                    var ipAddress = _webHelper.GetCurrentIpAddress();
                    var nowUtc = DateTime.UtcNow;

                    if (_forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer))
                        topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

                    //forum topic
                    var forumTopic = new ForumTopic
                    {
                        ForumId = forum.Id,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        TopicTypeId = (int)topicType,
                        Subject = subject,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    _forumService.InsertTopic(forumTopic, true);

                    //forum post
                    var forumPost = new ForumPost
                    {
                        TopicId = forumTopic.Id,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    _forumService.InsertPost(forumPost, false);

                    //update forum topic
                    forumTopic.NumPosts = 1;
                    forumTopic.LastPostId = forumPost.Id;
                    forumTopic.LastPostCustomerId = forumPost.CustomerId;
                    forumTopic.LastPostTime = forumPost.CreatedOnUtc;
                    forumTopic.UpdatedOnUtc = nowUtc;
                    _forumService.UpdateTopic(forumTopic);

                    //subscription                
                    if (_forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer))
                    {
                        if (model.Subscribed)
                        {
                            var forumSubscription = new ForumSubscription
                            {
                                SubscriptionGuid = Guid.NewGuid(),
                                CustomerId = _workContext.CurrentCustomer.Id,
                                TopicId = forumTopic.Id,
                                CreatedOnUtc = nowUtc
                            };

                            _forumService.InsertSubscription(forumSubscription);
                        }
                    }

                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = _forumService.GetTopicSeName(forumTopic) });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            _forumModelFactory.PrepareTopicCreateModel(forum, model);
            return View(model);
        }

        public virtual IActionResult TopicEdit(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            if (!_forumService.IsCustomerAllowedToEditTopic(_workContext.CurrentCustomer, forumTopic))
                return Challenge();

            var model = new EditForumTopicModel();
            _forumModelFactory.PrepareTopicEditModel(forumTopic, model, false);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult TopicEdit(EditForumTopicModel model)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = _forumService.GetTopicById(model.Id);

            if (forumTopic == null)
                return RedirectToRoute("Boards");
            var forum = forumTopic.Forum;
            if (forum == null)
                return RedirectToRoute("Boards");

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_forumService.IsCustomerAllowedToEditTopic(_workContext.CurrentCustomer, forumTopic))
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
                    var ipAddress = _webHelper.GetCurrentIpAddress();
                    var nowUtc = DateTime.UtcNow;

                    if (_forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer))
                        topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);

                    //forum topic
                    forumTopic.TopicTypeId = (int)topicType;
                    forumTopic.Subject = subject;
                    forumTopic.UpdatedOnUtc = nowUtc;
                    _forumService.UpdateTopic(forumTopic);

                    //forum post                
                    var firstPost = _forumService.GetFirstPost(forumTopic);
                    if (firstPost != null)
                    {
                        firstPost.Text = text;
                        firstPost.UpdatedOnUtc = nowUtc;
                        _forumService.UpdatePost(firstPost);
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

                        _forumService.InsertPost(firstPost, false);
                    }

                    //subscription
                    if (_forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer))
                    {
                        var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id,
                            0, forumTopic.Id, 0, 1).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    CustomerId = _workContext.CurrentCustomer.Id,
                                    TopicId = forumTopic.Id,
                                    CreatedOnUtc = nowUtc
                                };

                                _forumService.InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                _forumService.DeleteSubscription(forumSubscription);
                            }
                        }
                    }

                    // redirect to the topic page with the topic slug
                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = _forumService.GetTopicSeName(forumTopic) });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            _forumModelFactory.PrepareTopicEditModel(forumTopic, model, true);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult PostDelete(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return Json(new
                {
                    redirect = Url.RouteUrl("Homepage"),
                });

            var forumPost = _forumService.GetPostById(id);
            if (forumPost != null)
            {
                if (!_forumService.IsCustomerAllowedToDeletePost(_workContext.CurrentCustomer, forumPost))
                    return Challenge();

                var forumTopic = forumPost.ForumTopic;
                var forumId = forumTopic.Forum.Id;
                var forumSlug = _forumService.GetForumSeName(forumTopic.Forum);

                _forumService.DeletePost(forumPost);

                //get topic one more time because it can be deleted (first or only post deleted)
                forumTopic = _forumService.GetTopicById(forumPost.TopicId);
                if (forumTopic == null)
                {
                    return Json(new
                    {
                        redirect = Url.RouteUrl("ForumSlug", new { id = forumId, slug = forumSlug }),
                    });
                }
                return Json(new
                {
                    redirect = Url.RouteUrl("TopicSlug", new { id = forumTopic.Id, slug = _forumService.GetTopicSeName(forumTopic) }),
                });
            }

            return Json(new
            {
                redirect = Url.RouteUrl("Boards"),
            });
        }

        public virtual IActionResult PostCreate(int id, int? quote)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            if (!_forumService.IsCustomerAllowedToCreatePost(_workContext.CurrentCustomer, forumTopic))
                return Challenge();

            var model = _forumModelFactory.PreparePostCreateModel(forumTopic, quote, false);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult PostCreate(EditForumPostModel model)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumTopic = _forumService.GetTopicById(model.ForumTopicId);
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_forumService.IsCustomerAllowedToCreatePost(_workContext.CurrentCustomer, forumTopic))
                        return Challenge();

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text.Substring(0, maxPostLength);

                    var ipAddress = _webHelper.GetCurrentIpAddress();

                    var nowUtc = DateTime.UtcNow;

                    var forumPost = new ForumPost
                    {
                        TopicId = forumTopic.Id,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    _forumService.InsertPost(forumPost, true);

                    //subscription
                    if (_forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer))
                    {
                        var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id,
                            0, forumPost.TopicId, 0, 1).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    CustomerId = _workContext.CurrentCustomer.Id,
                                    TopicId = forumPost.TopicId,
                                    CreatedOnUtc = nowUtc
                                };

                                _forumService.InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                _forumService.DeleteSubscription(forumSubscription);
                            }
                        }
                    }

                    var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;

                    var pageIndex = (_forumService.CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.Id) + 1);
                    var url = string.Empty;
                    if (pageIndex > 1)
                        url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.TopicId, slug = _forumService.GetTopicSeName(forumPost.ForumTopic), pageNumber = pageIndex });
                    else
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = _forumService.GetTopicSeName(forumPost.ForumTopic) });
                    return Redirect($"{url}#{forumPost.Id}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            _forumModelFactory.PreparePostCreateModel(forumTopic, 0, true);
            return View(model);
        }

        public virtual IActionResult PostEdit(int id)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumPost = _forumService.GetPostById(id);
            if (forumPost == null)
                return RedirectToRoute("Boards");

            if (!_forumService.IsCustomerAllowedToEditPost(_workContext.CurrentCustomer, forumPost))
                return Challenge();

            var model = _forumModelFactory.PreparePostEditModel(forumPost, false);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult PostEdit(EditForumPostModel model)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var forumPost = _forumService.GetPostById(model.Id);
            if (forumPost == null)
                return RedirectToRoute("Boards");

            if (!_forumService.IsCustomerAllowedToEditPost(_workContext.CurrentCustomer, forumPost))
                return Challenge();

            var forumTopic = forumPost.ForumTopic;
            if (forumTopic == null)
                return RedirectToRoute("Boards");

            var forum = forumTopic.Forum;
            if (forum == null)
                return RedirectToRoute("Boards");

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
                    _forumService.UpdatePost(forumPost);

                    //subscription
                    if (_forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer))
                    {
                        var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id,
                            0, forumPost.TopicId, 0, 1).FirstOrDefault();
                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    SubscriptionGuid = Guid.NewGuid(),
                                    CustomerId = _workContext.CurrentCustomer.Id,
                                    TopicId = forumPost.TopicId,
                                    CreatedOnUtc = nowUtc
                                };
                                _forumService.InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                _forumService.DeleteSubscription(forumSubscription);
                            }
                        }
                    }

                    var pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;
                    var pageIndex = (_forumService.CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.Id) + 1);
                    var url = string.Empty;
                    if (pageIndex > 1)
                    {
                        url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.TopicId, slug = _forumService.GetTopicSeName(forumPost.ForumTopic), pageNumber = pageIndex });
                    }
                    else
                    {
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = _forumService.GetTopicSeName(forumPost.ForumTopic) });
                    }
                    return Redirect($"{url}#{forumPost.Id}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //redisplay form
            _forumModelFactory.PreparePostEditModel(forumPost, true);
            return View(model);
        }

        public virtual IActionResult Search(string searchterms, bool? adv, string forumId,
            string within, string limitDays, int page = 1)
        {
            if (!_forumSettings.ForumsEnabled)
                return RedirectToRoute("Homepage");

            var model = _forumModelFactory.PrepareSearchModel(searchterms, adv, forumId, within, limitDays, page);
            return View(model);
        }

        public virtual IActionResult CustomerForumSubscriptions(int? pageNumber)
        {
            if (!_forumSettings.AllowCustomersToManageSubscriptions)
                return RedirectToRoute("CustomerInfo");

            var model = _forumModelFactory.PrepareCustomerForumSubscriptionsModel(pageNumber);
            return View(model);
        }

        [HttpPost, ActionName("CustomerForumSubscriptions")]
        public virtual IActionResult CustomerForumSubscriptionsPOST(IFormCollection formCollection)
        {
            foreach (var key in formCollection.Keys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("fs", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("fs", "").Trim();
                    if (int.TryParse(id, out int forumSubscriptionId))
                    {
                        var forumSubscription = _forumService.GetSubscriptionById(forumSubscriptionId);
                        if (forumSubscription != null && forumSubscription.CustomerId == _workContext.CurrentCustomer.Id)
                        {
                            _forumService.DeleteSubscription(forumSubscription);
                        }
                    }
                }
            }

            return RedirectToRoute("CustomerForumSubscriptions");
        }

        [HttpPost]
        public virtual IActionResult PostVote(int postId, bool isUp)
        {
            if (!_forumSettings.AllowPostVoting)
                return new NullJsonResult();

            var forumPost = _forumService.GetPostById(postId);
            if (forumPost == null)
                return new NullJsonResult();

            if (!_workContext.CurrentCustomer.IsRegistered())
                return Json(new
                {
                    Error = _localizationService.GetResource("Forum.Votes.Login"),
                    VoteCount = forumPost.VoteCount
                });

            if (_workContext.CurrentCustomer.Id == forumPost.CustomerId)
                return Json(new
                {
                    Error = _localizationService.GetResource("Forum.Votes.OwnPost"),
                    VoteCount = forumPost.VoteCount
                });

            var forumPostVote = _forumService.GetPostVote(postId, _workContext.CurrentCustomer);
            if (forumPostVote != null)
            {
                if ((forumPostVote.IsUp && isUp) || (!forumPostVote.IsUp && !isUp))
                    return Json(new
                    {
                        Error = _localizationService.GetResource("Forum.Votes.AlreadyVoted"),
                        VoteCount = forumPost.VoteCount
                    });

                _forumService.DeletePostVote(forumPostVote);
                return Json(new { VoteCount = forumPost.VoteCount });
            }

            if (_forumService.GetNumberOfPostVotes(_workContext.CurrentCustomer, DateTime.UtcNow.AddDays(-1)) >= _forumSettings.MaxVotesPerDay)
                return Json(new
                {
                    Error = string.Format(_localizationService.GetResource("Forum.Votes.MaxVotesReached"), _forumSettings.MaxVotesPerDay),
                    VoteCount = forumPost.VoteCount
                });

            _forumService.InsertPostVote(new ForumPostVote
            {
                CustomerId = _workContext.CurrentCustomer.Id,
                ForumPostId = postId,
                IsUp = isUp,
                CreatedOnUtc = DateTime.UtcNow
            });
            return Json(new { VoteCount = forumPost.VoteCount, IsUp = isUp });
        }

        #endregion
    }
}