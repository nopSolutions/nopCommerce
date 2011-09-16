using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Html;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Framework;
using Nop.Web.Models.Boards;

namespace Nop.Web.Controllers
{
    public class BoardsController : BaseNopController
    {
        private readonly IForumService _forumService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ICountryService _countryService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ForumSettings _forumSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly MediaSettings _mediaSettings;

        public BoardsController(IForumService forumService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ICountryService countryService,
            IWebHelper webHelper,
            IWorkContext workContext,
            ForumSettings forumSettings,
            StoreInformationSettings storeInformationSettings,
            CustomerSettings customerSettings,
            MediaSettings mediaSettings)
        {
            this._forumService = forumService;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            this._countryService = countryService;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._forumSettings = forumSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._customerSettings = customerSettings;
            this._mediaSettings = mediaSettings;
        }

        [NonAction]
        private bool ForumsEnabled()
        {
            return _forumSettings.ForumsEnabled;
        }

        [NonAction]
        protected IEnumerable<SelectListItem> ForumTopicTypesList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Forum.Normal"),
                Value = ((int)ForumTopicType.Normal).ToString()
            });

            list.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Forum.Sticky"),
                Value = ((int)ForumTopicType.Sticky).ToString()
            });

            list.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Forum.Announcement"),
                Value = ((int)ForumTopicType.Announcement).ToString()
            });

            return list;
        }

        [NonAction]
        private IEnumerable<SelectListItem> ForumGroupsForumsList()
        {
            var forumsList = new List<SelectListItem>();
            var separator = "--";
            var forumGroups = _forumService.GetAllForumGroups();

            foreach (var fg in forumGroups.OrderBy(fg => fg.DisplayOrder))
            {
                // Add the forum group with Value of 0 so it won't be used as a target forum
                forumsList.Add(new SelectListItem { Text = fg.Name, Value = "0" });

                foreach (var f in fg.Forums.OrderBy(f => f.DisplayOrder))
                {
                    forumsList.Add(new SelectListItem { Text = string.Format("{0}{1}", separator, f.Name), Value = f.Id.ToString() });
                }
            }

            return forumsList;
        }

        public ActionResult Index()
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            var forumGroups = _forumService.GetAllForumGroups();
            var topicCount = _forumSettings.HomePageActiveDiscussionsTopicCount;
            var topics = _forumService.GetActiveTopics(0, topicCount);

            var activeDiscussionsModel = new ActiveDiscussionsModel(topics);
            activeDiscussionsModel.ViewAllLinkEnabled = false;
            activeDiscussionsModel.ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled;
            activeDiscussionsModel.PostsPageSize = _forumSettings.PostsPageSize;
            activeDiscussionsModel.AllowViewingProfiles = _customerSettings.AllowViewingProfiles;

            var model = new BoardsIndexModel(forumGroups, activeDiscussionsModel);
            model.AllowViewingProfiles = _customerSettings.AllowViewingProfiles;
            model.RelativeDateTimeFormattingEnabled = _forumSettings.RelativeDateTimeFormattingEnabled;
            return View(model);
        }

        public ActionResult ActiveDiscussions(int forumId = 0)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            int topicLimit = _forumSettings.ActiveDiscussionsPageTopicCount;
            var topics = _forumService.GetActiveTopics(forumId, topicLimit);

            var model = new ActiveDiscussionsModel(topics);

            model.ViewAllLinkEnabled = true;
            model.ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled;
            model.PostsPageSize = _forumSettings.PostsPageSize;
            model.AllowViewingProfiles = _customerSettings.AllowViewingProfiles;
            model.RelativeDateTimeFormattingEnabled = _forumSettings.RelativeDateTimeFormattingEnabled;
            return View(model);
        }

        public ActionResult ActiveDiscussionsRss(int forumId = 0)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            if (!_forumSettings.ActiveDiscussionsFeedEnabled)
            {
                return RedirectToRoute("Boards");
            }

            int topicLimit = _forumSettings.ActiveDiscussionsFeedCount;
            var topics = _forumService.GetActiveTopics(forumId, topicLimit);
            string url = Url.Action("ActiveDiscussionsRSS", "Boards", null, "http");

            var feedTitle = _localizationService.GetResource("Forum.ActiveDiscussionsFeedTitle");
            var feedDescription = _localizationService.GetResource("Forum.ActiveDiscussionsFeedDescription");

            SyndicationFeed feed = new SyndicationFeed(
                                    string.Format(feedTitle, _storeInformationSettings.StoreName),
                                    feedDescription,
                                    new Uri(url),
                                    "ActiveDiscussionsRSS",
                                    DateTime.UtcNow);

            List<SyndicationItem> items = new List<SyndicationItem>();

            var viewsText = _localizationService.GetResource("Forum.Views");
            var repliesText = _localizationService.GetResource("Forum.Replies");

            foreach (var topic in topics)
            {
                string topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = topic.GetSeName() }, "http");
                string content = String.Format("{2}: {0}, {3}: {1}", topic.NumReplies.ToString(), topic.Views.ToString(), repliesText, viewsText);

                items.Add(new SyndicationItem(topic.Subject, content, new Uri(topicUrl),
                    String.Format("Topic:{0}", topic.Id), (topic.LastPostTime ?? topic.UpdatedOnUtc)));
            }
            feed.Items = items;

            return new RssActionResult() { Feed = feed };
        }

        public ActionResult ForumGroup(int id)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            var forumGroup = _forumService.GetForumGroupById(id);

            if (forumGroup != null)
            {
                var model = new ForumGroupModel(forumGroup);
                model.AllowViewingProfiles = _customerSettings.AllowViewingProfiles;
                model.RelativeDateTimeFormattingEnabled = _forumSettings.RelativeDateTimeFormattingEnabled;
                return View(model);
            }

            return RedirectToRoute("Boards");
        }

        public ActionResult Forum(int id, int page = 1)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            var forum = _forumService.GetForumById(id);

            if (forum != null)
            {
                var model = new ForumModel(forum);

                int pageSize = 10;
                if (_forumSettings.TopicsPageSize > 0)
                {
                    pageSize = _forumSettings.TopicsPageSize;
                }

                //subscription                
                if (_forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer))
                {
                    model.WatchForumText = _localizationService.GetResource("Forum.WatchForum");

                    var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id, forum.Id, 0, 0, 1).FirstOrDefault();
                    if (forumSubscription != null)
                    {
                        model.WatchForumText = _localizationService.GetResource("Forum.UnwatchForum");
                    }
                }

                // Order the topics by the topicTypeId, LastPostTime, TopicId
                var topics = forum.ForumTopics.OrderByDescending(t => t.TopicTypeId).ThenByDescending(t => t.LastPostTime).ThenByDescending(t => t.Id);
                var pagedList = new PagedList<ForumTopic>(topics.AsQueryable(), (page - 1), pageSize);
                model.PagedList = pagedList;
                model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);
                model.AllowViewingProfiles = _customerSettings.AllowViewingProfiles;
                model.ForumFeedsEnabled = _forumSettings.ForumFeedsEnabled;
                model.PostsPageSize = _forumSettings.PostsPageSize;
                model.RelativeDateTimeFormattingEnabled = _forumSettings.RelativeDateTimeFormattingEnabled;
                return View(model);
            }

            return RedirectToRoute("Boards");
        }

        public ActionResult ForumRss(int id)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            if (!_forumSettings.ForumFeedsEnabled)
            {
                return RedirectToRoute("Boards");
            }

            int topicLimit = _forumSettings.ForumFeedCount;
            var forum = _forumService.GetForumById(id);

            if (forum != null)
            {
                // Order by newest topic posts & limit the number of topics to return
                var topics = forum.ForumTopics.OrderByDescending(t => t.LastPostTime).ThenByDescending(t => t.Id).Take(topicLimit);

                string url = Url.Action("ForumRSS", "Boards", new { id = forum.Id }, "http");

                var feedTitle = _localizationService.GetResource("Forum.ForumFeedTitle");
                var feedDescription = _localizationService.GetResource("Forum.ForumFeedDescription");

                SyndicationFeed feed = new SyndicationFeed(
                                        string.Format(feedTitle, _storeInformationSettings.StoreName, forum.Name),
                                        feedDescription,
                                        new Uri(url),
                                        string.Format("ForumRSS:{0}", forum.Id),
                                        DateTime.UtcNow);

                List<SyndicationItem> items = new List<SyndicationItem>();

                var viewsText = _localizationService.GetResource("Forum.Views");
                var repliesText = _localizationService.GetResource("Forum.Replies");

                foreach (var topic in topics)
                {
                    string topicUrl = Url.RouteUrl("TopicSlug", new { id = topic.Id, slug = topic.GetSeName() }, "http");
                    string content = string.Format("{2}: {0}, {3}: {1}", topic.NumReplies.ToString(), topic.Views.ToString(), repliesText, viewsText);

                    items.Add(new SyndicationItem(topic.Subject, content, new Uri(topicUrl), String.Format("Topic:{0}", topic.Id),
                        (topic.LastPostTime ?? topic.UpdatedOnUtc)));
                }

                feed.Items = items;

                return new RssActionResult() { Feed = feed };
            }

            return new RssActionResult() { Feed = new SyndicationFeed() };
        }

        [HttpPost]
        public ActionResult ForumWatch(int id)
        {
            bool subscribed = false;
            string watchTopic = _localizationService.GetResource("Forum.WatchForum");
            string unwatchTopic = _localizationService.GetResource("Forum.UnwatchForum");
            string returnText = watchTopic;

            var forum = _forumService.GetForumById(id);
            if (forum == null)
            {
                return Json(new { Subscribed = subscribed, Text = returnText, Error = true });
            }

            if (!_forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer))
            {
                return Json(new { Subscribed = subscribed, Text = returnText, Error = true });
            }

            var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id,
                forum.Id, 0, 0, 1).FirstOrDefault();

            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription()
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
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            var forumTopic = _forumService.GetTopicById(id);

            if (forumTopic != null)
            {
                forumTopic.Views += 1;
                _forumService.UpdateTopic(forumTopic);

                var model = new ForumTopicPostsModel(forumTopic);

                // page number is needed for creating post link in _ForumPost partial view
                ViewBag.Page = page;

                model.IsCustomerAllowedToEditTopic = _forumService.IsCustomerAllowedToEditTopic(_workContext.CurrentCustomer, forumTopic);
                model.IsCustomerAllowedToDeleteTopic = _forumService.IsCustomerAllowedToDeleteTopic(_workContext.CurrentCustomer, forumTopic);
                model.IsCustomerAllowedToMoveTopic = _forumService.IsCustomerAllowedToMoveTopic(_workContext.CurrentCustomer, forumTopic);
                model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);

                if (model.IsCustomerAllowedToSubscribe)
                {
                    model.WatchTopicText = _localizationService.GetResource("Forum.WatchTopic");

                    var forumTopicSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id,
                        0, forumTopic.Id, 0, 1).FirstOrDefault();
                    if (forumTopicSubscription != null)
                    {
                        model.WatchTopicText = _localizationService.GetResource("Forum.UnwatchTopic");
                    }
                }
                var pagedForumPostList = new PagedList<ForumPost>(forumTopic.ForumPosts.OrderBy(
                    fp => fp.CreatedOnUtc).ToList(), (page - 1), _forumSettings.PostsPageSize);

                var defaultAvatarUrl = _pictureService.GetDefaultPictureUrl(_mediaSettings.AvatarPictureSize, PictureType.Avatar);

                var commonTopicSettings = new CommonTopicSettingsModel()
                {
                    ShowCustomersPostCount = _forumSettings.ShowCustomersPostCount,
                    AllowPrivateMessages = _forumSettings.AllowPrivateMessages,
                    RelativeDateTimeFormattingEnabled = _forumSettings.RelativeDateTimeFormattingEnabled,
                    ForumEditor = _forumSettings.ForumEditor,
                    SignaturesEnabled = _forumSettings.SignaturesEnabled,
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles,
                    AllowCustomersToUploadAvatars = _customerSettings.AllowCustomersToUploadAvatars,
                    ShowCustomersJoinDate = _customerSettings.ShowCustomersJoinDate,
                    ShowCustomersLocation = _customerSettings.ShowCustomersLocation,
                    DefaultAvatarEnabled = _customerSettings.DefaultAvatarEnabled,
                    DefaultAvatarUrl = defaultAvatarUrl
                };

                model.PagedList = pagedForumPostList;

                foreach (var post in model.PagedList)
                {
                    // edit/delete permissions
                    var isCustomerAllowedToEditPost = _forumService.IsCustomerAllowedToEditPost(_workContext.CurrentCustomer, post);
                    var isCustomerAllowedToDeletePost = _forumService.IsCustomerAllowedToDeletePost(_workContext.CurrentCustomer, post);

                    var avatarUrl = _pictureService.GetPictureUrl(post.Customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                         _mediaSettings.AvatarPictureSize, false);
                    var signature = post.Customer.GetAttribute<string>(SystemCustomerAttributeNames.Signature);
                    var countryId = post.Customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                    var country = _countryService.GetCountryById(countryId);
                    var location = country != null ? country.Name : string.Empty;

                    model.ForumPostModels.Add(new ForumPostModel()
                    {
                        Id = post.Id,
                        ForumPost = post,
                        IsCustomerAllowedToEditPost = isCustomerAllowedToEditPost,
                        IsCustomerAllowedToDeletePost = isCustomerAllowedToDeletePost,
                        CommonTopicSettingsModel = commonTopicSettings,
                        AvatarUrl = avatarUrl,
                        Signature = signature.FormatForumSignatureText(),
                        Location = location
                    });
                }

                var forumBreadcrumbModel = new ForumBreadcrumbModel()
                {
                    ForumGroupModel = new ForumGroupModel(forumTopic.Forum.ForumGroup),
                    ForumModel = new ForumModel(forumTopic.Forum),
                    ForumTopicPostsModel = new ForumTopicPostsModel(forumTopic),
                    Separator = " / ",
                    StoreLocation = _webHelper.GetStoreLocation()
                };
                model.ForumBreadcrumbModel = forumBreadcrumbModel;

                return View(model);
            }

            return RedirectToRoute("Boards");
        }

        [HttpPost]
        public ActionResult TopicWatch(int id)
        {
            bool subscribed = false;
            string watchTopic = _localizationService.GetResource("Forum.WatchTopic");
            string unwatchTopic = _localizationService.GetResource("Forum.UnwatchTopic");
            string returnText = watchTopic;

            var forumTopic = _forumService.GetTopicById(id);
            if (forumTopic == null)
            {
                return Json(new { Subscribed = subscribed, Text = returnText, Error = true });
            }

            if (!_forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer))
            {
                return Json(new { Subscribed = subscribed, Text = returnText, Error = true });
            }

            var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id,
                0, forumTopic.Id, 0, 1).FirstOrDefault();

            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription()
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
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            var forumTopic = _forumService.GetTopicById(id);

            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            var model = new TopicMoveModel();
            model.ForumList = ForumGroupsForumsList();
            model.Id = forumTopic.Id;
            model.ForumSelected = forumTopic.ForumId;
            model.ForumBreadCrumbModel = new ForumBreadcrumbModel
            {
                ForumGroupModel = new ForumGroupModel(forumTopic.Forum.ForumGroup),
                ForumModel = new ForumModel(forumTopic.Forum),
                ForumTopicPostsModel = new ForumTopicPostsModel(forumTopic),
                Separator = " / ",
                StoreLocation = _webHelper.GetStoreLocation()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult TopicMove(TopicMoveModel model)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
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

        public ActionResult TopicDelete(int id)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
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
                    return RedirectToRoute("ForumSlug", new { id = forum.Id, slug = forum.GetSeName() });
                }
            }

            return RedirectToRoute("Boards");
        }

        public ActionResult TopicCreate(int id)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
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

            var model = new ForumTopicModel();
            model.TopicPriorities = ForumTopicTypesList();
            model.Forum = forum;
            model.ForumId = forum.Id;
            model.Id = 0;
            model.Subscribed = false;

            model.IsCustomerAllowedToSetTopicPriority = _forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer);
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);
            model.ForumEditor = _forumSettings.ForumEditor;

            var forumBreadcrumbModel = new ForumBreadcrumbModel()
            {
                ForumGroupModel = new ForumGroupModel(forum.ForumGroup),
                ForumModel = new ForumModel(forum),
                ForumTopicPostsModel = null,
                Separator = " / ",
                StoreLocation = _webHelper.GetStoreLocation()
            };
            model.ForumBreadcrumbModel = forumBreadcrumbModel;

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TopicCreate(ForumTopicModel model)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            var forum = _forumService.GetForumById(model.ForumId);

            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            try
            {
                if (!_forumService.IsCustomerAllowedToCreateTopic(_workContext.CurrentCustomer, forum))
                {
                    return new HttpUnauthorizedResult();
                }

                string subject = model.Subject;
                if (subject != null)
                {
                    subject = subject.Trim();
                }

                if (String.IsNullOrEmpty(subject))
                {
                    throw new NopException(_localizationService.GetResource("Forum.TopicSubjectCannotBeEmpty"));
                }

                var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                {
                    subject = subject.Substring(0, maxSubjectLength);
                }

                var text = model.Text;
                if (text != null)
                {
                    text = text.Trim();
                }

                if (String.IsNullOrEmpty(text))
                {
                    throw new NopException(_localizationService.GetResource("Forum.TextCannotBeEmpty"));
                }

                var maxPostLength = _forumSettings.PostMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                {
                    text = text.Substring(0, maxPostLength);
                }

                var topicType = ForumTopicType.Normal;

                string IPAddress = _webHelper.GetCurrentIpAddress();

                var nowUtc = DateTime.UtcNow;

                if (_forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer))
                {
                    topicType = (ForumTopicType)Enum.ToObject(typeof(ForumTopicType), model.TopicTypeId);
                }

                //forum topic
                var forumTopic = new ForumTopic()
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
                var forumPost = new ForumPost()
                {
                    TopicId = forumTopic.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Text = text,
                    IPAddress = IPAddress,
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
                        var forumSubscription = new ForumSubscription()
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
                model.PostError = ex.Message;
            }

            // redisplay form
            model.ForumBreadcrumbModel = new ForumBreadcrumbModel()
            {
                ForumGroupModel = new ForumGroupModel(forum.ForumGroup),
                ForumModel = new ForumModel(forum),
                ForumTopicPostsModel = null,
                Separator = " / ",
                StoreLocation = _webHelper.GetStoreLocation()
            };
            model.TopicPriorities = ForumTopicTypesList();
            model.Forum = forum;
            model.ForumId = forum.Id;
            model.Id = 0;

            model.IsCustomerAllowedToSetTopicPriority = _forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer);
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);
            model.ForumEditor = _forumSettings.ForumEditor;

            return View(model);
        }

        public ActionResult TopicEdit(int id)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
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

            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            var model = new ForumTopicModel();
            model.ForumBreadcrumbModel = new ForumBreadcrumbModel()
            {
                ForumGroupModel = new ForumGroupModel(forum.ForumGroup),
                ForumModel = new ForumModel(forum),
                ForumTopicPostsModel = null,
                Separator = " / ",
                StoreLocation = _webHelper.GetStoreLocation()
            };
            model.TopicPriorities = ForumTopicTypesList();
            model.Forum = forum;
            model.Text = Server.HtmlDecode(forumTopic.FirstPost.Text);
            model.Subject = forumTopic.Subject;
            model.TopicTypeId = forumTopic.TopicTypeId;
            model.Id = forumTopic.Id;
            model.ForumId = forum.Id;
            model.ForumEditor = _forumSettings.ForumEditor;

            model.IsCustomerAllowedToSetTopicPriority = _forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer);
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);

            //subscription            
            if (model.IsCustomerAllowedToSubscribe)
            {
                var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id,
                    0, forumTopic.Id, 0, 1).FirstOrDefault();
                model.Subscribed = forumSubscription != null;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TopicEdit(ForumTopicModel model)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            var forumTopic = _forumService.GetTopicById(model.Id);

            if (forumTopic == null)
            {
                return RedirectToAction("Index", "Boards");
            }
            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToAction("Index", "Boards");
            }

            try
            {
                if (!_forumService.IsCustomerAllowedToEditTopic(_workContext.CurrentCustomer, forumTopic))
                {
                    return new HttpUnauthorizedResult();
                }

                string subject = model.Subject;
                if (subject != null)
                {
                    subject = subject.Trim();
                }

                if (String.IsNullOrEmpty(subject))
                {
                    throw new NopException(_localizationService.GetResource("Forum.TopicSubjectCannotBeEmpty"));
                }

                var maxSubjectLength = _forumSettings.TopicSubjectMaxLength;
                if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                {
                    subject = subject.Substring(0, maxSubjectLength);
                }

                var text = model.Text;
                if (text != null)
                {
                    text = text.Trim();
                }

                if (String.IsNullOrEmpty(text))
                {
                    throw new NopException(_localizationService.GetResource("Forum.TextCannotBeEmpty"));
                }

                var maxPostLength = _forumSettings.PostMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                {
                    text = text.Substring(0, maxPostLength);
                }

                var topicType = ForumTopicType.Normal;

                string IPAddress = _webHelper.GetCurrentIpAddress();

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
                var firstPost = forumTopic.FirstPost;
                if (firstPost != null)
                {
                    firstPost.Text = text;
                    firstPost.UpdatedOnUtc = nowUtc;
                    _forumService.UpdatePost(firstPost);
                }
                else
                {
                    //error (not possible)
                    firstPost = new ForumPost()
                    {
                        TopicId = forumTopic.Id,
                        CustomerId = forumTopic.CustomerId,
                        Text = text,
                        IPAddress = IPAddress,
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
                            forumSubscription = new ForumSubscription()
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
                model.PostError = ex.Message;
            }

            // redisplay form
            var forumBreadcrumbModel = new ForumBreadcrumbModel()
            {
                ForumGroupModel = new ForumGroupModel(forum.ForumGroup),
                ForumModel = new ForumModel(forum),
                ForumTopicPostsModel = null,
                Separator = " / ",
                StoreLocation = _webHelper.GetStoreLocation()
            };
            model.ForumBreadcrumbModel = forumBreadcrumbModel;
            model.TopicPriorities = ForumTopicTypesList();
            model.Forum = forum;
            model.ForumId = forum.Id;
            model.ForumEditor = _forumSettings.ForumEditor;

            model.IsCustomerAllowedToSetTopicPriority = _forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer);
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);

            return View(model);
        }

        public ActionResult PostDelete(int id)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
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

                string url = string.Empty;
                //get topic one more time because it can be deleted (first or only post deleted)
                forumTopic = _forumService.GetTopicById(forumPost.TopicId);
                if (forumTopic == null)
                {
                    return RedirectToRoute("ForumSlug", new { id = forumId, slug = forumSlug });
                }
                else
                {
                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = forumTopic.GetSeName() });
                }
            }

            return RedirectToRoute("Boards");
        }

        public ActionResult PostCreate(int id, int? quote)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
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

            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            var model = new ForumPostModel();
            model.ForumTopic = forumTopic;
            model.ForumTopicId = forumTopic.Id;
            model.ForumName = forum.Name;
            model.Id = 0;
            model.Subscribed = false;
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);
            model.ForumEditor = _forumSettings.ForumEditor;

            var forumBreadcrumbModel = new ForumBreadcrumbModel()
            {
                ForumGroupModel = new ForumGroupModel(forum.ForumGroup),
                ForumModel = new ForumModel(forum),
                ForumTopicPostsModel = new ForumTopicPostsModel(forumTopic),
                Separator = " / ",
                StoreLocation = _webHelper.GetStoreLocation()
            };

            model.ForumBreadcrumbModel = forumBreadcrumbModel;

            //subscription            
            if (model.IsCustomerAllowedToSubscribe)
            {
                var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id,
                    0, forumTopic.Id, 0, 1).FirstOrDefault();
                model.Subscribed = forumSubscription != null;
            }

            // Insert the quoted text
            string text = string.Empty;
            if (quote.HasValue)
            {
                var quotePost = _forumService.GetPostById(quote.Value);
                if (quotePost != null && quotePost.TopicId == forumTopic.Id)
                {
                    var quotePostText = Server.HtmlDecode(quotePost.Text);

                    switch (_forumSettings.ForumEditor)
                    {
                        case EditorType.SimpleTextBox:
                            text = String.Format("{0}:\n{1}\n", quotePost.Customer.FormatUserName(), quotePostText);
                            break;
                        case EditorType.BBCodeEditor:
                            text = String.Format("[quote={0}]{1}[/quote]", quotePost.Customer.FormatUserName(), BBCodeHelper.RemoveQuotes(quotePostText));
                            break;
                    }
                    model.Text = text;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PostCreate(ForumPostModel model)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            var forumTopic = _forumService.GetTopicById(model.ForumTopicId);
            if (forumTopic == null)
            {
                return RedirectToRoute("Boards");
            }

            try
            {
                if (!_forumService.IsCustomerAllowedToCreatePost(_workContext.CurrentCustomer, forumTopic))
                {
                    return new HttpUnauthorizedResult();
                }

                var text = model.Text;
                if (text != null)
                {
                    text = text.Trim();
                }

                if (String.IsNullOrEmpty(text))
                {
                    throw new NopException(_localizationService.GetResource("Forum.TextCannotBeEmpty"));
                }

                var maxPostLength = _forumSettings.PostMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                {
                    text = text.Substring(0, maxPostLength);
                }

                string IPAddress = _webHelper.GetCurrentIpAddress();

                DateTime nowUtc = DateTime.UtcNow;

                var forumPost = new ForumPost()
                {
                    TopicId = forumTopic.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Text = text,
                    IPAddress = IPAddress,
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
                            forumSubscription = new ForumSubscription()
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

                int pageSize = 10;
                if (_forumSettings.PostsPageSize > 0)
                {
                    pageSize = _forumSettings.PostsPageSize;
                }
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
                model.PostError = ex.Message;
            }

            // redisplay form
            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            model.ForumTopic = forumTopic;
            model.ForumName = forum.Name;
            model.ForumTopicId = forumTopic.Id;
            model.Id = 0;
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);
            model.ForumEditor = _forumSettings.ForumEditor;
            model.ForumBreadcrumbModel = new ForumBreadcrumbModel()
            {
                ForumGroupModel = new ForumGroupModel(forum.ForumGroup),
                ForumModel = new ForumModel(forum),
                ForumTopicPostsModel = new ForumTopicPostsModel(forumTopic),
                Separator = " / ",
                StoreLocation = _webHelper.GetStoreLocation()
            };

            return View(model);
        }

        public ActionResult PostEdit(int id)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
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

            var model = new ForumPostModel();
            model.Text = Server.HtmlDecode(forumPost.Text);
            model.ForumTopic = forumTopic;
            model.ForumTopicId = forumTopic.Id;
            model.ForumName = forum.Name;
            model.Id = forumPost.Id;
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);
            model.ForumEditor = _forumSettings.ForumEditor;

            var forumBreadcrumbModel = new ForumBreadcrumbModel()
            {
                ForumGroupModel = new ForumGroupModel(forum.ForumGroup),
                ForumModel = new ForumModel(forum),
                ForumTopicPostsModel = new ForumTopicPostsModel(forumTopic),
                Separator = " / ",
                StoreLocation = _webHelper.GetStoreLocation()
            };
            model.ForumBreadcrumbModel = forumBreadcrumbModel;

            //subscription
            if (model.IsCustomerAllowedToSubscribe)
            {
                var forumSubscription = _forumService.GetAllSubscriptions(_workContext.CurrentCustomer.Id,
                    0, forumTopic.Id, 0, 1).FirstOrDefault();
                model.Subscribed = forumSubscription != null;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PostEdit(ForumPostModel model)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
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

            try
            {
                DateTime nowUtc = DateTime.UtcNow;

                var text = model.Text;
                if (text != null)
                {
                    text = text.Trim();
                }

                if (String.IsNullOrEmpty(text))
                {
                    throw new NopException(_localizationService.GetResource("Forum.TextCannotBeEmpty"));
                }

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
                            forumSubscription = new ForumSubscription()
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

                int pageSize = 10;
                if (_forumSettings.PostsPageSize > 0)
                {
                    pageSize = _forumSettings.PostsPageSize;
                }
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
                model.PostError = ex.Message;
            }

            // redisplay form            
            model.ForumTopic = forumTopic;
            model.ForumTopicId = forumTopic.Id;
            model.ForumName = forum.Name;
            model.Id = forumPost.Id;
            model.ForumEditor = _forumSettings.ForumEditor;
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);

            model.ForumBreadcrumbModel = new ForumBreadcrumbModel()
            {
                ForumGroupModel = new ForumGroupModel(forum.ForumGroup),
                ForumModel = new ForumModel(forum),
                ForumTopicPostsModel = new ForumTopicPostsModel(forumTopic),
                Separator = " / ",
                StoreLocation = _webHelper.GetStoreLocation()
            };

            return View(model);
        }

        public ActionResult Search(string searchterms, bool? adv, string forumId,
            string within, string limitDays, int page = 1)
        {
            if (!ForumsEnabled())
            {
                return RedirectToAction("index", "home");
            }

            int pageSize = 10;

            var model = new SearchModel();

            // Create the values for the "Limit results to previous" select list
            var limitList = new List<SelectListItem>
                            {
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.AllResults"),
                                    Value = "0"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.1day"),
                                    Value = "1"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.7days"),
                                    Value = "7"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.2weeks"),
                                    Value = "14"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.1month"),
                                    Value = "30"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.3months"),
                                    Value = "92"
                                },
                                new SelectListItem
                                {
                                    Text= _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.6months"),
                                    Value = "183"
                                },
                                new SelectListItem
                                {
                                    Text = _localizationService.GetResource("Forum.Search.LimitResultsToPrevious.1year"),
                                    Value = "365"
                                }
                            };
            model.LimitList = limitList;

            // Create the values for the "Search in forum" select list
            var forumsSelectList = new List<SelectListItem>();
            forumsSelectList.Add(
                new SelectListItem
                {
                    Text = _localizationService.GetResource("Forum.Search.SearchInForum.All"),
                    Value = "0",
                    Selected = true,
                });
            var separator = "--";
            var forumGroups = _forumService.GetAllForumGroups();
            foreach (var fg in forumGroups)
            {
                // Add the forum group with value as '-' so it can't be used as a target forum id
                forumsSelectList.Add(new SelectListItem { Text = fg.Name, Value = "-" });

                foreach (var f in fg.Forums.OrderBy(f => f.DisplayOrder))
                {
                    forumsSelectList.Add(
                        new SelectListItem
                        {
                            Text = string.Format("{0}{1}", separator, f.Name),
                            Value = f.Id.ToString()
                        });
                }
            }
            model.ForumList = forumsSelectList;

            // Create the values for "Search within" select list            
            var withinList = new List<SelectListItem>
                                {
                                    new SelectListItem
                                    {
                                        Value = ((int)ForumSearchType.All).ToString(),
                                        Text = _localizationService.GetResource("Forum.Search.SearchWithin.All")
                                    },
                                    new SelectListItem
                                    {
                                        Value = ((int)ForumSearchType.TopicTitlesOnly).ToString(),
                                        Text = _localizationService.GetResource("Forum.Search.SearchWithin.TopicTitlesOnly")
                                    },
                                    new SelectListItem
                                    {
                                        Value = ((int)ForumSearchType.PostTextOnly).ToString(),
                                        Text = _localizationService.GetResource("Forum.Search.SearchWithin.PostTextOnly")
                                    }
                                };
            model.WithinList = withinList;

            int forumIdSelected;
            int.TryParse(forumId, out forumIdSelected);
            model.ForumIdSelected = forumIdSelected;

            int withinSelected;
            int.TryParse(within, out withinSelected);
            model.WithinSelected = withinSelected;

            int limitDaysSelected;
            int.TryParse(limitDays, out limitDaysSelected);
            model.LimitDaysSelected = limitDaysSelected;

            int searchTermMinimumLength = _forumSettings.ForumSearchTermMinimumLength;

            model.ShowAdvancedSearch = adv.GetValueOrDefault();
            model.SearchResultsVisible = false;
            model.NoResultsVisisble = false;
            model.PostsPageSize = _forumSettings.PostsPageSize;
            model.AllowViewingProfiles = _customerSettings.AllowViewingProfiles;
            model.RelativeDateTimeFormattingEnabled = _forumSettings.RelativeDateTimeFormattingEnabled;

            var forumBreadcrumbModel = new ForumBreadcrumbModel()
            {
                Separator = " / ",
                StoreLocation = _webHelper.GetStoreLocation()
            };
            model.ForumBreadcrumbModel = forumBreadcrumbModel;

            try
            {
                if (String.IsNullOrWhiteSpace(searchterms) == false)
                {
                    searchterms = searchterms.Trim();
                    model.SearchTerms = searchterms;

                    if (searchterms.Length < searchTermMinimumLength)
                    {
                        throw new NopException(string.Format(_localizationService.GetResource("Forum.SearchTermMinimumLengthIsNCharacters"),
                            searchTermMinimumLength));
                    }

                    ForumSearchType searchWithin = 0;
                    int limitResultsToPrevious = 0;
                    if (adv.GetValueOrDefault() == true)
                    {
                        searchWithin = (ForumSearchType)withinSelected;
                        limitResultsToPrevious = limitDaysSelected;
                    }

                    if (_forumSettings.SearchResultsPageSize > 0)
                    {
                        pageSize = _forumSettings.SearchResultsPageSize;
                    }

                    var forumTopics = _forumService.GetAllTopics(forumIdSelected, 0, searchterms, searchWithin,
                        limitResultsToPrevious, page, pageSize);

                    model.SearchResultsVisible = (forumTopics.Count > 0);
                    model.NoResultsVisisble = !(model.SearchResultsVisible);

                    var pagedList = new PagedList<ForumTopic>(forumTopics.ToList<ForumTopic>(), page - 1, pageSize);
                    model.PagedList = pagedList;

                    return View(model);
                }
                else
                {
                    ViewBag.SearchResultsVisible = false;
                }
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
            }
            model.PagedList = new PagedList<ForumTopic>(new List<ForumTopic>(), page, pageSize);

            return View(model);
        }
    }
}
