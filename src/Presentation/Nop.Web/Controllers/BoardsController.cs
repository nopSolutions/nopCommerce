using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Boards;

namespace Nop.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.No)]
    public partial class BoardsController : BasePublicController
    {
        #region Fields

        private readonly IForumService _forumService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IForumModelFactory _forumModelFactory;
        private readonly ForumSettings _forumSettings;

        #endregion

        #region Constructors

        public BoardsController(IForumService forumService,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IStoreContext storeContext,
            IForumModelFactory forumModelFactory,
            ForumSettings forumSettings)
        {
            this._forumService = forumService;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._forumModelFactory = forumModelFactory;
            this._forumSettings = forumSettings;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var model = _forumModelFactory.PrepareBoardsIndexModel();
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ActiveDiscussionsSmall()
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var model = _forumModelFactory.PrepareActiveDiscussionsModel();
            if (!model.ForumTopics.Any())
                return Content("");

            return PartialView(model);
        }

        public ActionResult ActiveDiscussions(int forumId = 0, int page = 1)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var model = _forumModelFactory.PrepareActiveDiscussionsModel(forumId, page);
            return View(model);
        }

        public ActionResult ActiveDiscussionsRss(int forumId = 0)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            if (!_forumSettings.ActiveDiscussionsFeedEnabled)
            {
                return RedirectToRoute("Boards");
            }

            var topics = _forumService.GetActiveTopics(forumId, 0, _forumSettings.ActiveDiscussionsFeedCount);
            string url = Url.RouteUrl("ActiveDiscussionsRSS", null, _webHelper.IsCurrentConnectionSecured() ? "https" : "http");

            var feedTitle = _localizationService.GetResource("Forum.ActiveDiscussionsFeedTitle");
            var feedDescription = _localizationService.GetResource("Forum.ActiveDiscussionsFeedDescription");

            var feed = new SyndicationFeed(
                                    string.Format(feedTitle, _storeContext.CurrentStore.GetLocalized(x => x.Name)),
                                    feedDescription,
                                    new Uri(url),
                                    string.Format("urn:store:{0}:activeDiscussions", _storeContext.CurrentStore.Id),
                                    DateTime.UtcNow);

            var items = new List<SyndicationItem>();

            var viewsText = _localizationService.GetResource("Forum.Views");
            var repliesText = _localizationService.GetResource("Forum.Replies");

            foreach (var topic in topics)
            {
                string topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = topic.GetSeName() }, _webHelper.IsCurrentConnectionSecured() ? "https" : "http");
                string content = String.Format("{2}: {0}, {3}: {1}", topic.NumReplies.ToString(), topic.Views.ToString(), repliesText, viewsText);

                items.Add(new SyndicationItem(topic.Subject, content, new Uri(topicUrl),
                    String.Format("urn:store:{0}:activeDiscussions:topic:{1}", _storeContext.CurrentStore.Id, topic.Id), (topic.LastPostTime ?? topic.UpdatedOnUtc)));
            }
            feed.Items = items;

            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

        public ActionResult ForumGroup(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumGroup = _forumService.GetForumGroupById(id);
            if (forumGroup == null)
            {
                return RedirectToRoute("Boards");
            }
            
            var model = _forumModelFactory.PrepareForumGroupModel(forumGroup);
            return View(model);
        }

        public ActionResult Forum(int id, int page = 1)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forum = _forumService.GetForumById(id);
            if (forum == null)
                return RedirectToRoute("Boards");

            var model = _forumModelFactory.PrepareForumPageModel(forum, page);
            return View(model);
        }

        public ActionResult ForumRss(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            if (!_forumSettings.ForumFeedsEnabled)
            {
                return RedirectToRoute("Boards");
            }

            int topicLimit = _forumSettings.ForumFeedCount;
            var forum = _forumService.GetForumById(id);

            if (forum != null)
            {
                //Order by newest topic posts & limit the number of topics to return
                var topics = _forumService.GetAllTopics(forum.Id, 0, string.Empty,
                     ForumSearchType.All, 0, 0, topicLimit);

                string url = Url.RouteUrl("ForumRSS", new { id = forum.Id }, _webHelper.IsCurrentConnectionSecured() ? "https" : "http");

                var feedTitle = _localizationService.GetResource("Forum.ForumFeedTitle");
                var feedDescription = _localizationService.GetResource("Forum.ForumFeedDescription");

                var feed = new SyndicationFeed(
                                        string.Format(feedTitle, _storeContext.CurrentStore.GetLocalized(x => x.Name), forum.Name),
                                        feedDescription,
                                        new Uri(url),
                                        string.Format("urn:store:{0}:forum", _storeContext.CurrentStore.Id),
                                        DateTime.UtcNow);

                var items = new List<SyndicationItem>();

                var viewsText = _localizationService.GetResource("Forum.Views");
                var repliesText = _localizationService.GetResource("Forum.Replies");

                foreach (var topic in topics)
                {
                    string topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = topic.GetSeName() }, _webHelper.IsCurrentConnectionSecured() ? "https" : "http");
                    string content = string.Format("{2}: {0}, {3}: {1}", topic.NumReplies.ToString(), topic.Views.ToString(), repliesText, viewsText);

                    items.Add(new SyndicationItem(topic.Subject, content, new Uri(topicUrl), String.Format("urn:store:{0}:forum:topic:{1}", _storeContext.CurrentStore.Id, topic.Id),
                        (topic.LastPostTime ?? topic.UpdatedOnUtc)));
                }

                feed.Items = items;

                return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
            }

            return new RssActionResult(new SyndicationFeed(), _webHelper.GetThisPageUrl(false));
        }

        [HttpPost]
        public ActionResult ForumWatch(int id)
        {
            string watchTopic = _localizationService.GetResource("Forum.WatchForum");
            string unwatchTopic = _localizationService.GetResource("Forum.UnwatchForum");
            string returnText = watchTopic;

            var forum = _forumService.GetForumById(id);
            if (forum == null)
            {
                return Json(new { Subscribed = false, Text = returnText, Error = true });
            }

            if (!_forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer))
            {
                return Json(new { Subscribed = false, Text = returnText, Error = true });
            }

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

        public ActionResult Topic(int id, int page = 1)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            var model = _forumModelFactory.PrepareForumTopicPageModel(forumTopic, page);
            //if no posts loaded, redirect to the first page
            if (!model.ForumPostModels.Any() && page > 1)
                return RedirectToRoute("TopicSlug", new {id = forumTopic.Id, slug = forumTopic.GetSeName()});
            
            //update view count
            forumTopic.Views += 1;
            _forumService.UpdateTopic(forumTopic);
            
            return View(model);
        }

        [HttpPost]
        public ActionResult TopicWatch(int id)
        {
            string watchTopic = _localizationService.GetResource("Forum.WatchTopic");
            string unwatchTopic = _localizationService.GetResource("Forum.UnwatchTopic");
            string returnText = watchTopic;

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
            {
                return Json(new { Subscribed = false, Text = returnText, Error = true });
            }

            if (!_forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer))
            {
                return Json(new { Subscribed = false, Text = returnText, Error = true });
            }

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

        public ActionResult TopicMove(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            var model = _forumModelFactory.PrepareTopicMove(forumTopic);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public ActionResult TopicMove(TopicMoveModel model)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumTopic = _forumService.GetTopicById(model.Id);

            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            var newForumId = model.ForumSelected;
            var forum = _forumService.GetForumById(newForumId);

            if (forum != null && forumTopic.ForumId != newForumId)
            {
                _forumService.MoveTopic(forumTopic.Id, newForumId);
            }

            return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = forumTopic.GetSeName() });
        }

        [HttpPost]
        [PublicAntiForgery]
        public ActionResult TopicDelete(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("HomePage"),
                });
            }

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic != null)
            {
                if (!_forumService.IsCustomerAllowedToDeleteTopic(_workContext.CurrentCustomer, forumTopic))
                {
                    return new HttpUnauthorizedResult();
                }
                var forum = _forumService.GetForumById(forumTopic.ForumId);

                _forumService.DeleteTopic(forumTopic);

                if (forum != null)
                {
                    return Json(new
                    {
                        redirect = Url.RouteUrl("ForumSlug", new { id = forum.Id, slug = forum.GetSeName() }),
                    });
                }
            }

            return Json(new
            {
                redirect = Url.RouteUrl("Boards"),
            });
        }

        public ActionResult TopicCreate(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forum = _forumService.GetForumById(id);
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            if (_forumService.IsCustomerAllowedToCreateTopic(_workContext.CurrentCustomer, forum) == false)
            {
                return new HttpUnauthorizedResult();
            }

            var model = new EditForumTopicModel();
            _forumModelFactory.PrepareTopicCreateModel(forum, model);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public ActionResult TopicCreate(EditForumTopicModel model)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forum = _forumService.GetForumById(model.ForumId);

            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_forumService.IsCustomerAllowedToCreateTopic(_workContext.CurrentCustomer, forum))
                    {
                        return new HttpUnauthorizedResult();
                    }

                    string subject = model.Subject;
                    var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject.Substring(0, maxSubjectLength);
                    }

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text.Substring(0, maxPostLength);
                    }

                    var topicType = ForumTopicType.Normal;

                    string ipAddress = _webHelper.GetCurrentIpAddress();

                    var nowUtc = DateTime.UtcNow;

                    if (_forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer))
                    {
                        topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);
                    }

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

                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = forumTopic.GetSeName() });
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

        public ActionResult TopicEdit(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            if (!_forumService.IsCustomerAllowedToEditTopic(_workContext.CurrentCustomer, forumTopic))
            {
                return new HttpUnauthorizedResult();
            }

            var model = new EditForumTopicModel();
            _forumModelFactory.PrepareTopicEditModel(forumTopic, model, false);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public ActionResult TopicEdit(EditForumTopicModel model)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumTopic = _forumService.GetTopicById(model.Id);

            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }
            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_forumService.IsCustomerAllowedToEditTopic(_workContext.CurrentCustomer, forumTopic))
                    {
                        return new HttpUnauthorizedResult();
                    }

                    string subject = model.Subject;
                    var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject.Substring(0, maxSubjectLength);
                    }

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text.Substring(0, maxPostLength);
                    }

                    var topicType = ForumTopicType.Normal;

                    string ipAddress = _webHelper.GetCurrentIpAddress();

                    DateTime nowUtc = DateTime.UtcNow;

                    if (_forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer))
                    {
                        topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);
                    }

                    //forum topic
                    forumTopic.TopicTypeId = (int)topicType;
                    forumTopic.Subject = subject;
                    forumTopic.UpdatedOnUtc = nowUtc;
                    _forumService.UpdateTopic(forumTopic);

                    //forum post                
                    var firstPost = forumTopic.GetFirstPost(_forumService);
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
                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = forumTopic.GetSeName() });
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
        public ActionResult PostDelete(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("HomePage"),
                });
            }

            var forumPost = _forumService.GetPostById(id);

            if (forumPost != null)
            {
                if (!_forumService.IsCustomerAllowedToDeletePost(_workContext.CurrentCustomer, forumPost))
                {
                    return new HttpUnauthorizedResult();
                }

                var forumTopic = forumPost.ForumTopic;
                var forumId = forumTopic.Forum.Id;
                var forumSlug = forumTopic.Forum.GetSeName();

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
                    redirect = Url.RouteUrl("TopicSlug", new { id = forumTopic.Id, slug = forumTopic.GetSeName() }),
                });
            }

            return Json(new
            {
                redirect = Url.RouteUrl("Boards"),
            });
        }

        public ActionResult PostCreate(int id, int? quote)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            if (!_forumService.IsCustomerAllowedToCreatePost(_workContext.CurrentCustomer, forumTopic))
            {
                return new HttpUnauthorizedResult();
            }


            var model = _forumModelFactory.PreparePostCreateModel(forumTopic, quote, false);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public ActionResult PostCreate(EditForumPostModel model)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumTopic = _forumService.GetTopicById(model.ForumTopicId);
            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_forumService.IsCustomerAllowedToCreatePost(_workContext.CurrentCustomer, forumTopic))
                        return new HttpUnauthorizedResult();

                    var text = model.Text;
                    var maxPostLength = _forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                        text = text.Substring(0, maxPostLength);

                    string ipAddress = _webHelper.GetCurrentIpAddress();

                    DateTime nowUtc = DateTime.UtcNow;

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

                    int pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;

                    int pageIndex = (_forumService.CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.Id) + 1);
                    var url = string.Empty;
                    if (pageIndex > 1)
                    {
                        url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.TopicId, slug = forumPost.ForumTopic.GetSeName(), page = pageIndex });
                    }
                    else
                    {
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = forumPost.ForumTopic.GetSeName() });
                    }
                    return Redirect(string.Format("{0}#{1}", url, forumPost.Id));
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

        public ActionResult PostEdit(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumPost = _forumService.GetPostById(id);
            if (forumPost == null)
            {
                return RedirectToRoute("Boards");
            }

            if (!_forumService.IsCustomerAllowedToEditPost(_workContext.CurrentCustomer, forumPost))
            {
                return new HttpUnauthorizedResult();
            }

            var model = _forumModelFactory.PreparePostEditModel(forumPost, false);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public ActionResult PostEdit(EditForumPostModel model)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumPost = _forumService.GetPostById(model.Id);
            if (forumPost == null)
            {
                return RedirectToRoute("Boards");
            }

            if (!_forumService.IsCustomerAllowedToEditPost(_workContext.CurrentCustomer, forumPost))
            {
                return new HttpUnauthorizedResult();
            }

            var forumTopic = forumPost.ForumTopic;
            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime nowUtc = DateTime.UtcNow;

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

                    int pageSize = _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10;
                    int pageIndex = (_forumService.CalculateTopicPageIndex(forumPost.TopicId, pageSize, forumPost.Id) + 1);
                    var url = string.Empty;
                    if (pageIndex > 1)
                    {
                        url = Url.RouteUrl("TopicSlugPaged", new { id = forumPost.TopicId, slug = forumPost.ForumTopic.GetSeName(), page = pageIndex });
                    }
                    else
                    {
                        url = Url.RouteUrl("TopicSlug", new { id = forumPost.TopicId, slug = forumPost.ForumTopic.GetSeName() });
                    }
                    return Redirect(string.Format("{0}#{1}", url, forumPost.Id));
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

        public ActionResult Search(string searchterms, bool? adv, string forumId,
            string within, string limitDays, int page = 1)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }
            
            var model = _forumModelFactory.PrepareSearchModel(searchterms,adv, forumId, within, limitDays, page);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult LastPost(int forumPostId, bool showTopic)
        {
            var forumPost = _forumService.GetPostById(forumPostId);
            var model = _forumModelFactory.PrepareLastPostModel(forumPost, showTopic);
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ForumBreadcrumb(int? forumGroupId, int? forumId, int? forumTopicId)
        {
            var model = _forumModelFactory.PrepareForumBreadcrumbModel(forumGroupId, forumId, forumTopicId);
            return PartialView(model);
        }


        public ActionResult CustomerForumSubscriptions(int? page)
        {
            if (!_forumSettings.AllowCustomersToManageSubscriptions)
            {
                return RedirectToRoute("CustomerInfo");
            }

            var model = _forumModelFactory.PrepareCustomerForumSubscriptionsModel(page);
            return View(model);
        }
        [HttpPost, ActionName("CustomerForumSubscriptions")]
        public ActionResult CustomerForumSubscriptionsPOST(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("fs", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("fs", "").Trim();
                    int forumSubscriptionId;
                    if (Int32.TryParse(id, out forumSubscriptionId))
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
        public ActionResult PostVote(int postId, bool isUp)
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
                else
                {
                    _forumService.DeletePostVote(forumPostVote);
                    return Json(new { VoteCount = forumPost.VoteCount });
                }
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
