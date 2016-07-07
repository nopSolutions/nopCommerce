using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Html;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Boards;
using Nop.Web.Models.Common;

namespace Nop.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.No)]
    public partial class BoardsController : BasePublicController
    {
        #region Fields

        private readonly IForumService _forumService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ICountryService _countryService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ForumSettings _forumSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Constructors

        public BoardsController(IForumService forumService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ICountryService countryService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IStoreContext storeContext,
            ForumSettings forumSettings,
            CustomerSettings customerSettings,
            MediaSettings mediaSettings,
            IDateTimeHelper dateTimeHelper)
        {
            this._forumService = forumService;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            this._countryService = countryService;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._forumSettings = forumSettings;
            this._customerSettings = customerSettings;
            this._mediaSettings = mediaSettings;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual ForumTopicRowModel PrepareForumTopicRowModel(ForumTopic topic)
        {
            var topicModel = new ForumTopicRowModel
            {
                Id = topic.Id,
                Subject = topic.Subject,
                SeName = topic.GetSeName(),
                LastPostId = topic.LastPostId,
                NumPosts = topic.NumPosts,
                Views = topic.Views,
                NumReplies = topic.NumReplies,
                ForumTopicType = topic.ForumTopicType,
                CustomerId = topic.CustomerId,
                AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !topic.Customer.IsGuest(),
                CustomerName = topic.Customer.FormatUserName()
            };

            var forumPosts = _forumService.GetAllPosts(topic.Id, 0, string.Empty, 1, _forumSettings.PostsPageSize);
            topicModel.TotalPostPages = forumPosts.TotalPages;

            var firstPost = topic.GetFirstPost(_forumService);
            topicModel.Votes = firstPost != null ? firstPost.VoteCount : 0;
            return topicModel;
        }

        [NonAction]
        protected virtual ForumRowModel PrepareForumRowModel(Forum forum)
        {
            var forumModel = new ForumRowModel
            {
                Id = forum.Id,
                Name = forum.Name,
                SeName = forum.GetSeName(),
                Description = forum.Description,
                NumTopics = forum.NumTopics,
                NumPosts = forum.NumPosts,
                LastPostId = forum.LastPostId,
            };
            return forumModel;
        }

        [NonAction]
        protected virtual ForumGroupModel PrepareForumGroupModel(ForumGroup forumGroup)
        {
            var forumGroupModel = new ForumGroupModel
            {
                Id = forumGroup.Id,
                Name = forumGroup.Name,
                SeName = forumGroup.GetSeName(),
            };
            var forums = _forumService.GetAllForumsByGroupId(forumGroup.Id);
            foreach (var forum in forums)
            {
                var forumModel = PrepareForumRowModel(forum);
                forumGroupModel.Forums.Add(forumModel);
            }
            return forumGroupModel;
        }

        [NonAction]
        protected virtual IEnumerable<SelectListItem> ForumTopicTypesList()
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
        protected virtual IEnumerable<SelectListItem> ForumGroupsForumsList()
        {
            var forumsList = new List<SelectListItem>();
            var separator = "--";
            var forumGroups = _forumService.GetAllForumGroups();

            foreach (var fg in forumGroups)
            {
                // Add the forum group with Value of 0 so it won't be used as a target forum
                forumsList.Add(new SelectListItem { Text = fg.Name, Value = "0" });

                var forums = _forumService.GetAllForumsByGroupId(fg.Id);
                foreach (var f in forums)
                {
                    forumsList.Add(new SelectListItem { Text = string.Format("{0}{1}", separator, f.Name), Value = f.Id.ToString() });
                }
            }

            return forumsList;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumGroups = _forumService.GetAllForumGroups();

            var model = new BoardsIndexModel();
            foreach (var forumGroup in forumGroups)
            {

                var forumGroupModel = PrepareForumGroupModel(forumGroup);
                model.ForumGroups.Add(forumGroupModel);
            }
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ActiveDiscussionsSmall()
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var topics = _forumService.GetActiveTopics(0, 0, _forumSettings.HomePageActiveDiscussionsTopicCount);
            if (!topics.Any())
                return Content("");

            var model = new ActiveDiscussionsModel();
            foreach (var topic in topics)
            {
                var topicModel = PrepareForumTopicRowModel(topic);
                model.ForumTopics.Add(topicModel);
            }
            model.ViewAllLinkEnabled = true;
            model.ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled;
            model.PostsPageSize = _forumSettings.PostsPageSize;
            model.AllowPostVoting = _forumSettings.AllowPostVoting;

            return PartialView(model);
        }

        public ActionResult ActiveDiscussions(int forumId = 0, int page = 1)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var model = new ActiveDiscussionsModel();

            int pageSize = _forumSettings.ActiveDiscussionsPageSize > 0 ? _forumSettings.ActiveDiscussionsPageSize : 50;

            var topics = _forumService.GetActiveTopics(forumId, (page - 1), pageSize);
            model.TopicPageSize = topics.PageSize;
            model.TopicTotalRecords = topics.TotalCount;
            model.TopicPageIndex = topics.PageIndex;
            foreach (var topic in topics)
            {
                var topicModel = PrepareForumTopicRowModel(topic);
                model.ForumTopics.Add(topicModel);
            }
            model.ViewAllLinkEnabled = false;
            model.ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled;
            model.PostsPageSize = _forumSettings.PostsPageSize;
            model.AllowPostVoting = _forumSettings.AllowPostVoting;
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

            return new RssActionResult { Feed = feed };
        }

        public ActionResult ForumGroup(int id)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forumGroup = _forumService.GetForumGroupById(id);
            if (forumGroup == null)
                return RedirectToRoute("Boards");

            var model = PrepareForumGroupModel(forumGroup);
            return View(model);
        }

        public ActionResult Forum(int id, int page = 1)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
            }

            var forum = _forumService.GetForumById(id);

            if (forum != null)
            {
                var model = new ForumPageModel();
                model.Id = forum.Id;
                model.Name = forum.Name;
                model.SeName = forum.GetSeName();
                model.Description = forum.Description;

                int pageSize = _forumSettings.TopicsPageSize > 0 ? _forumSettings.TopicsPageSize : 10;

                model.AllowPostVoting = _forumSettings.AllowPostVoting;

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

                var topics = _forumService.GetAllTopics(forum.Id, 0, string.Empty,
                    ForumSearchType.All, 0, (page - 1), pageSize);
                model.TopicPageSize = topics.PageSize;
                model.TopicTotalRecords = topics.TotalCount;
                model.TopicPageIndex = topics.PageIndex;
                foreach (var topic in topics)
                {
                    var topicModel = PrepareForumTopicRowModel(topic);
                    model.ForumTopics.Add(topicModel);
                }
                model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);
                model.ForumFeedsEnabled = _forumSettings.ForumFeedsEnabled;
                model.PostsPageSize = _forumSettings.PostsPageSize;
                return View(model);
            }

            return RedirectToRoute("Boards");
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

                return new RssActionResult { Feed = feed };
            }

            return new RssActionResult { Feed = new SyndicationFeed() };
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

            if (forumTopic != null)
            {
                //load posts
                var posts = _forumService.GetAllPosts(forumTopic.Id, 0, string.Empty,
                    page - 1, _forumSettings.PostsPageSize);
                //if not posts loaded, redirect to the first page
                if (!posts.Any() && page > 1)
                {
                    return RedirectToRoute("TopicSlug", new { id = forumTopic.Id, slug = forumTopic.GetSeName() });
                }

                //update view count
                forumTopic.Views += 1;
                _forumService.UpdateTopic(forumTopic);

                //prepare model
                var model = new ForumTopicPageModel();
                model.Id = forumTopic.Id;
                model.Subject = forumTopic.Subject;
                model.SeName = forumTopic.GetSeName();

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
                model.PostsPageIndex = posts.PageIndex;
                model.PostsPageSize = posts.PageSize;
                model.PostsTotalRecords = posts.TotalCount;
                foreach (var post in posts)
                {
                    var forumPostModel = new ForumPostModel
                    {
                        Id = post.Id,
                        ForumTopicId = post.TopicId,
                        ForumTopicSeName = forumTopic.GetSeName(),
                        FormattedText = post.FormatPostText(),
                        IsCurrentCustomerAllowedToEditPost = _forumService.IsCustomerAllowedToEditPost(_workContext.CurrentCustomer, post),
                        IsCurrentCustomerAllowedToDeletePost = _forumService.IsCustomerAllowedToDeletePost(_workContext.CurrentCustomer, post),
                        CustomerId = post.CustomerId,
                        AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !post.Customer.IsGuest(),
                        CustomerName = post.Customer.FormatUserName(),
                        IsCustomerForumModerator = post.Customer.IsForumModerator(),
                        ShowCustomersPostCount = _forumSettings.ShowCustomersPostCount,
                        ForumPostCount = post.Customer.GetAttribute<int>(SystemCustomerAttributeNames.ForumPostCount),
                        ShowCustomersJoinDate = _customerSettings.ShowCustomersJoinDate && !post.Customer.IsGuest(),
                        CustomerJoinDate = post.Customer.CreatedOnUtc,
                        AllowPrivateMessages = _forumSettings.AllowPrivateMessages && !post.Customer.IsGuest(),
                        SignaturesEnabled = _forumSettings.SignaturesEnabled,
                        FormattedSignature = post.Customer.GetAttribute<string>(SystemCustomerAttributeNames.Signature).FormatForumSignatureText(),
                    };
                    //created on string
                    if (_forumSettings.RelativeDateTimeFormattingEnabled)
                        forumPostModel.PostCreatedOnStr = post.CreatedOnUtc.RelativeFormat(true, "f");
                    else
                        forumPostModel.PostCreatedOnStr = _dateTimeHelper.ConvertToUserTime(post.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
                    //avatar
                    if (_customerSettings.AllowCustomersToUploadAvatars)
                    {
                        forumPostModel.CustomerAvatarUrl = _pictureService.GetPictureUrl(
                            post.Customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                            _mediaSettings.AvatarPictureSize,
                            _customerSettings.DefaultAvatarEnabled,
                            defaultPictureType: PictureType.Avatar);
                    }
                    //location
                    forumPostModel.ShowCustomersLocation = _customerSettings.ShowCustomersLocation && !post.Customer.IsGuest();
                    if (_customerSettings.ShowCustomersLocation)
                    {
                        var countryId = post.Customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                        var country = _countryService.GetCountryById(countryId);
                        forumPostModel.CustomerLocation = country != null ? country.GetLocalized(x => x.Name) : string.Empty;
                    }

                    //votes
                    if (_forumSettings.AllowPostVoting)
                    {
                        forumPostModel.AllowPostVoting = true;
                        forumPostModel.VoteCount = post.VoteCount;
                        var postVote = _forumService.GetPostVote(post.Id, _workContext.CurrentCustomer);
                        if (postVote != null)
                            forumPostModel.VoteIsUp = postVote.IsUp;
                    }

                    // page number is needed for creating post link in _ForumPost partial view
                    forumPostModel.CurrentTopicPage = page;
                    model.ForumPostModels.Add(forumPostModel);
                }

                return View(model);
            }

            return RedirectToRoute("Boards");
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

            var model = new TopicMoveModel();
            model.ForumList = ForumGroupsForumsList();
            model.Id = forumTopic.Id;
            model.TopicSeName = forumTopic.GetSeName();
            model.ForumSelected = forumTopic.ForumId;
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
            model.Id = 0;
            model.IsEdit = false;
            model.ForumId = forum.Id;
            model.ForumName = forum.Name;
            model.ForumSeName = forum.GetSeName();
            model.ForumEditor = _forumSettings.ForumEditor;
            model.IsCustomerAllowedToSetTopicPriority = _forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer);
            model.TopicPriorities = ForumTopicTypesList();
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);
            model.Subscribed = false;
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

            // redisplay form
            model.TopicPriorities = ForumTopicTypesList();
            model.IsEdit = false;
            model.ForumId = forum.Id;
            model.ForumName = forum.Name;
            model.ForumSeName = forum.GetSeName();
            model.Id = 0;
            model.IsCustomerAllowedToSetTopicPriority = _forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer);
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);
            model.ForumEditor = _forumSettings.ForumEditor;

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

            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            var model = new EditForumTopicModel();
            model.IsEdit = true;
            model.TopicPriorities = ForumTopicTypesList();
            model.ForumName = forum.Name;
            model.ForumSeName = forum.GetSeName();
            var firstPost = forumTopic.GetFirstPost(_forumService);
            model.Text = firstPost.Text;
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

            // redisplay form
            model.TopicPriorities = ForumTopicTypesList();
            model.IsEdit = true;
            model.ForumName = forum.Name;
            model.ForumSeName = forum.GetSeName();
            model.ForumId = forum.Id;
            model.ForumEditor = _forumSettings.ForumEditor;

            model.IsCustomerAllowedToSetTopicPriority = _forumService.IsCustomerAllowedToSetTopicPriority(_workContext.CurrentCustomer);
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);

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

            var forum = forumTopic.Forum;
            if (forum == null)
            {
                return RedirectToRoute("Boards");
            }

            var model = new EditForumPostModel
            {
                Id = 0,
                ForumTopicId = forumTopic.Id,
                IsEdit = false,
                ForumEditor = _forumSettings.ForumEditor,
                ForumName = forum.Name,
                ForumTopicSubject = forumTopic.Subject,
                ForumTopicSeName = forumTopic.GetSeName(),
                IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer),
                Subscribed = false,
            };

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
                    var quotePostText = quotePost.Text;

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

            // redisplay form
            var forum = forumTopic.Forum;
            if (forum == null)
                return RedirectToRoute("Boards");

            model.IsEdit = false;
            model.ForumName = forum.Name;
            model.ForumTopicId = forumTopic.Id;
            model.ForumTopicSubject = forumTopic.Subject;
            model.ForumTopicSeName = forumTopic.GetSeName();
            model.Id = 0;
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);
            model.ForumEditor = _forumSettings.ForumEditor;

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


            var model = new EditForumPostModel
            {
                Id = forumPost.Id,
                ForumTopicId = forumTopic.Id,
                IsEdit = true,
                ForumEditor = _forumSettings.ForumEditor,
                ForumName = forum.Name,
                ForumTopicSubject = forumTopic.Subject,
                ForumTopicSeName = forumTopic.GetSeName(),
                IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer),
                Subscribed = false,
                Text = forumPost.Text,
            };

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
            model.IsEdit = true;
            model.ForumName = forum.Name;
            model.ForumTopicId = forumTopic.Id;
            model.ForumTopicSubject = forumTopic.Subject;
            model.ForumTopicSeName = forumTopic.GetSeName();
            model.Id = forumPost.Id;
            model.IsCustomerAllowedToSubscribe = _forumService.IsCustomerAllowedToSubscribe(_workContext.CurrentCustomer);
            model.ForumEditor = _forumSettings.ForumEditor;

            return View(model);
        }

        public ActionResult Search(string searchterms, bool? adv, string forumId,
            string within, string limitDays, int page = 1)
        {
            if (!_forumSettings.ForumsEnabled)
            {
                return RedirectToRoute("HomePage");
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

                var forums = _forumService.GetAllForumsByGroupId(fg.Id);
                foreach (var f in forums)
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

            model.AllowPostVoting = _forumSettings.AllowPostVoting;

            try
            {
                if (!String.IsNullOrWhiteSpace(searchterms))
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
                    if (adv.GetValueOrDefault())
                    {
                        searchWithin = (ForumSearchType)withinSelected;
                        limitResultsToPrevious = limitDaysSelected;
                    }

                    if (_forumSettings.SearchResultsPageSize > 0)
                    {
                        pageSize = _forumSettings.SearchResultsPageSize;
                    }

                    var topics = _forumService.GetAllTopics(forumIdSelected, 0, searchterms, searchWithin,
                        limitResultsToPrevious, page - 1, pageSize);
                    model.TopicPageSize = topics.PageSize;
                    model.TopicTotalRecords = topics.TotalCount;
                    model.TopicPageIndex = topics.PageIndex;
                    foreach (var topic in topics)
                    {
                        var topicModel = PrepareForumTopicRowModel(topic);
                        model.ForumTopics.Add(topicModel);
                    }

                    model.SearchResultsVisible = (topics.Any());
                    model.NoResultsVisisble = !(model.SearchResultsVisible);

                    return View(model);
                }
                model.SearchResultsVisible = false;
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
            }

            //some exception raised
            model.TopicPageSize = pageSize;
            model.TopicTotalRecords = 0;
            model.TopicPageIndex = page - 1;

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult LastPost(int forumPostId, bool showTopic)
        {
            var post = _forumService.GetPostById(forumPostId);
            var model = new LastPostModel();
            if (post != null)
            {
                model.Id = post.Id;
                model.ForumTopicId = post.TopicId;
                model.ForumTopicSeName = post.ForumTopic.GetSeName();
                model.ForumTopicSubject = post.ForumTopic.StripTopicSubject();
                model.CustomerId = post.CustomerId;
                model.AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !post.Customer.IsGuest();
                model.CustomerName = post.Customer.FormatUserName();
                //created on string
                if (_forumSettings.RelativeDateTimeFormattingEnabled)
                    model.PostCreatedOnStr = post.CreatedOnUtc.RelativeFormat(true, "f");
                else
                    model.PostCreatedOnStr = _dateTimeHelper.ConvertToUserTime(post.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
            }
            model.ShowTopic = showTopic;
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ForumBreadcrumb(int? forumGroupId, int? forumId, int? forumTopicId)
        {
            var model = new ForumBreadcrumbModel();

            ForumTopic forumTopic = null;
            if (forumTopicId.HasValue)
            {
                forumTopic = _forumService.GetTopicById(forumTopicId.Value);
                if (forumTopic != null)
                {
                    model.ForumTopicId = forumTopic.Id;
                    model.ForumTopicSubject = forumTopic.Subject;
                    model.ForumTopicSeName = forumTopic.GetSeName();
                }
            }

            Forum forum = _forumService.GetForumById(forumTopic != null ? forumTopic.ForumId : (forumId.HasValue ? forumId.Value : 0));
            if (forum != null)
            {
                model.ForumId = forum.Id;
                model.ForumName = forum.Name;
                model.ForumSeName = forum.GetSeName();
            }

            var forumGroup = _forumService.GetForumGroupById(forum != null ? forum.ForumGroupId : (forumGroupId.HasValue ? forumGroupId.Value : 0));
            if (forumGroup != null)
            {
                model.ForumGroupId = forumGroup.Id;
                model.ForumGroupName = forumGroup.Name;
                model.ForumGroupSeName = forumGroup.GetSeName();
            }

            return PartialView(model);
        }


        public ActionResult CustomerForumSubscriptions(int? page)
        {
            if (!_forumSettings.AllowCustomersToManageSubscriptions)
            {
                return RedirectToRoute("CustomerInfo");
            }

            int pageIndex = 0;
            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            var customer = _workContext.CurrentCustomer;

            var pageSize = _forumSettings.ForumSubscriptionsPageSize;

            var list = _forumService.GetAllSubscriptions(customer.Id, 0, 0, pageIndex, pageSize);

            var model = new CustomerForumSubscriptionsModel();

            foreach (var forumSubscription in list)
            {
                var forumTopicId = forumSubscription.TopicId;
                var forumId = forumSubscription.ForumId;
                bool topicSubscription = false;
                var title = string.Empty;
                var slug = string.Empty;

                if (forumTopicId > 0)
                {
                    topicSubscription = true;
                    var forumTopic = _forumService.GetTopicById(forumTopicId);
                    if (forumTopic != null)
                    {
                        title = forumTopic.Subject;
                        slug = forumTopic.GetSeName();
                    }
                }
                else
                {
                    var forum = _forumService.GetForumById(forumId);
                    if (forum != null)
                    {
                        title = forum.Name;
                        slug = forum.GetSeName();
                    }
                }

                model.ForumSubscriptions.Add(new CustomerForumSubscriptionsModel.ForumSubscriptionModel
                {
                    Id = forumSubscription.Id,
                    ForumTopicId = forumTopicId,
                    ForumId = forumSubscription.ForumId,
                    TopicSubscription = topicSubscription,
                    Title = title,
                    Slug = slug,
                });
            }

            model.PagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerForumSubscriptionsPaged",
                UseRouteLinks = true,
                RouteValues = new ForumSubscriptionsRouteValues { page = pageIndex }
            };

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
