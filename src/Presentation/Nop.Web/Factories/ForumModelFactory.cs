using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Extensions;
using Nop.Web.Models.Boards;
using Nop.Web.Models.Common;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the forum model factory
    /// </summary>
    public partial class ForumModelFactory : IForumModelFactory
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IBBCodeHelper _bbCodeHelper;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IForumService _forumService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public ForumModelFactory(CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            ForumSettings forumSettings,
            IBBCodeHelper bbCodeHelper,
            ICountryService countryService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IWorkContext workContext,
            MediaSettings mediaSettings)
        {
            _captchaSettings = captchaSettings;
            _customerSettings = customerSettings;
            _forumSettings = forumSettings;
            _bbCodeHelper = bbCodeHelper;
            _countryService = countryService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _forumService = forumService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get the list of forum topic types
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of the select list item
        /// </returns>
        protected virtual async Task<IEnumerable<SelectListItem>> ForumTopicTypesListAsync()
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Normal"),
                    Value = ((int)ForumTopicType.Normal).ToString()
                },

                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Sticky"),
                    Value = ((int)ForumTopicType.Sticky).ToString()
                },

                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Announcement"),
                    Value = ((int)ForumTopicType.Announcement).ToString()
                }
            };

            return list;
        }

        /// <summary>
        /// Get the list of forum groups
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of the select list item
        /// </returns>
        protected virtual async Task<IEnumerable<SelectListItem>> ForumGroupsForumsListAsync()
        {
            var forumsList = new List<SelectListItem>();
            var separator = "--";
            var forumGroups = await _forumService.GetAllForumGroupsAsync();

            foreach (var fg in forumGroups)
            {
                // Add the forum group with Value of 0 so it won't be used as a target forum
                forumsList.Add(new SelectListItem { Text = fg.Name, Value = "0" });

                var forums = await _forumService.GetAllForumsByGroupIdAsync(fg.Id);
                foreach (var f in forums)
                {
                    forumsList.Add(new SelectListItem { Text = $"{separator}{f.Name}", Value = f.Id.ToString() });
                }
            }

            return forumsList;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Prepare the forum group model
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum group model
        /// </returns>
        public virtual async Task<ForumGroupModel> PrepareForumGroupModelAsync(ForumGroup forumGroup)
        {
            if (forumGroup == null)
                throw new ArgumentNullException(nameof(forumGroup));

            var forumGroupModel = new ForumGroupModel
            {
                Id = forumGroup.Id,
                Name = forumGroup.Name,
                SeName = await _forumService.GetForumGroupSeNameAsync(forumGroup),
            };
            var forums = await _forumService.GetAllForumsByGroupIdAsync(forumGroup.Id);
            foreach (var forum in forums)
            {
                var forumModel = await PrepareForumRowModelAsync(forum);
                forumGroupModel.Forums.Add(forumModel);
            }

            return forumGroupModel;
        }

        /// <summary>
        /// Prepare the boards index model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the boards index model
        /// </returns>
        public virtual async Task<BoardsIndexModel> PrepareBoardsIndexModelAsync()
        {
            var model = new BoardsIndexModel();

            var forumGroups = await _forumService.GetAllForumGroupsAsync();
            foreach (var forumGroup in forumGroups)
            {
                var forumGroupModel = await PrepareForumGroupModelAsync(forumGroup);
                model.ForumGroups.Add(forumGroupModel);
            }
            return model;
        }

        /// <summary>
        /// Prepare the active discussions model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the active discussions model
        /// </returns>
        public virtual async Task<ActiveDiscussionsModel> PrepareActiveDiscussionsModelAsync()
        {
            var model = new ActiveDiscussionsModel
            {
                ViewAllLinkEnabled = true,
                ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled,
                PostsPageSize = _forumSettings.PostsPageSize,
                AllowPostVoting = _forumSettings.AllowPostVoting
            };

            var topics = await _forumService.GetActiveTopicsAsync(0, 0, _forumSettings.HomepageActiveDiscussionsTopicCount);
            foreach (var topic in topics)
            {
                var topicModel = await PrepareForumTopicRowModelAsync(topic);
                model.ForumTopics.Add(topicModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the active discussions model
        /// </summary>
        /// <param name="forumId">Forum identifier</param>
        /// <param name="page">Number of forum topics page</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the active discussions model
        /// </returns>
        public virtual async Task<ActiveDiscussionsModel> PrepareActiveDiscussionsModelAsync(int forumId, int page)
        {
            var model = new ActiveDiscussionsModel
            {
                ViewAllLinkEnabled = false,
                ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled,
                PostsPageSize = _forumSettings.PostsPageSize,
                AllowPostVoting = _forumSettings.AllowPostVoting
            };

            var pageSize = _forumSettings.ActiveDiscussionsPageSize > 0 ? _forumSettings.ActiveDiscussionsPageSize : 50;

            var topics = await _forumService.GetActiveTopicsAsync(forumId, (page - 1), pageSize);
            model.TopicPageSize = topics.PageSize;
            model.TopicTotalRecords = topics.TotalCount;
            model.TopicPageIndex = topics.PageIndex;
            foreach (var topic in topics)
            {
                var topicModel = await PrepareForumTopicRowModelAsync(topic);
                model.ForumTopics.Add(topicModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the forum page model
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <param name="page">Number of forum topics page</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum page model
        /// </returns>
        public virtual async Task<ForumPageModel> PrepareForumPageModelAsync(Forum forum, int page)
        {
            if (forum == null)
                throw new ArgumentNullException(nameof(forum));

            var model = new ForumPageModel
            {
                Id = forum.Id,
                Name = forum.Name,
                SeName = await _forumService.GetForumSeNameAsync(forum),
                Description = forum.Description
            };

            var pageSize = _forumSettings.TopicsPageSize > 0 ? _forumSettings.TopicsPageSize : 10;

            model.AllowPostVoting = _forumSettings.AllowPostVoting;

            //subscription
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _forumService.IsCustomerAllowedToSubscribeAsync(customer))
            {
                model.WatchForumText = await _localizationService.GetResourceAsync("Forum.WatchForum");

                var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, forum.Id, 0, 0, 1)).FirstOrDefault();
                if (forumSubscription != null)
                {
                    model.WatchForumText = await _localizationService.GetResourceAsync("Forum.UnwatchForum");
                }
            }

            var topics = await _forumService.GetAllTopicsAsync(forum.Id, 0, string.Empty, ForumSearchType.All, 0, (page - 1), pageSize);
            model.TopicPageSize = topics.PageSize;
            model.TopicTotalRecords = topics.TotalCount;
            model.TopicPageIndex = topics.PageIndex;
            foreach (var topic in topics)
            {
                var topicModel = await PrepareForumTopicRowModelAsync(topic);
                model.ForumTopics.Add(topicModel);
            }
            model.IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(customer);
            model.ForumFeedsEnabled = _forumSettings.ForumFeedsEnabled;
            model.PostsPageSize = _forumSettings.PostsPageSize;
            return model;
        }

        /// <summary>
        /// Prepare the forum topic page model
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="page">Number of forum posts page</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum topic page model
        /// </returns>
        public virtual async Task<ForumTopicPageModel> PrepareForumTopicPageModelAsync(ForumTopic forumTopic, int page)
        {
            if (forumTopic == null)
                throw new ArgumentNullException(nameof(forumTopic));

            //load posts
            var posts = await _forumService.GetAllPostsAsync(forumTopic.Id, 0, string.Empty,
                page - 1, _forumSettings.PostsPageSize);

            //prepare model
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var model = new ForumTopicPageModel
            {
                Id = forumTopic.Id,
                Subject = forumTopic.Subject,
                SeName = await _forumService.GetTopicSeNameAsync(forumTopic),

                IsCustomerAllowedToEditTopic = await _forumService.IsCustomerAllowedToEditTopicAsync(currentCustomer, forumTopic),
                IsCustomerAllowedToDeleteTopic = await _forumService.IsCustomerAllowedToDeleteTopicAsync(currentCustomer, forumTopic),
                IsCustomerAllowedToMoveTopic = await _forumService.IsCustomerAllowedToMoveTopicAsync(currentCustomer, forumTopic),
                IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(currentCustomer)
            };

            if (model.IsCustomerAllowedToSubscribe)
            {
                model.WatchTopicText = await _localizationService.GetResourceAsync("Forum.WatchTopic");

                var forumTopicSubscription = (await _forumService.GetAllSubscriptionsAsync(currentCustomer.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
                if (forumTopicSubscription != null)
                {
                    model.WatchTopicText = await _localizationService.GetResourceAsync("Forum.UnwatchTopic");
                }
            }
            model.PostsPageIndex = posts.PageIndex;
            model.PostsPageSize = posts.PageSize;
            model.PostsTotalRecords = posts.TotalCount;
            foreach (var post in posts)
            {
                var customer = await _customerService.GetCustomerByIdAsync(post.CustomerId);

                var customerIsGuest = await _customerService.IsGuestAsync(customer);
                var customerIsModerator = !customerIsGuest && await _customerService.IsForumModeratorAsync(customer);

                var forumPostModel = new ForumPostModel
                {
                    Id = post.Id,
                    ForumTopicId = post.TopicId,
                    ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
                    FormattedText = _forumService.FormatPostText(post),
                    IsCurrentCustomerAllowedToEditPost = await _forumService.IsCustomerAllowedToEditPostAsync(currentCustomer, post),
                    IsCurrentCustomerAllowedToDeletePost = await _forumService.IsCustomerAllowedToDeletePostAsync(currentCustomer, post),
                    CustomerId = post.CustomerId,
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !customerIsGuest,
                    CustomerName = await _customerService.FormatUsernameAsync(customer),
                    IsCustomerForumModerator = customerIsModerator,
                    ShowCustomersPostCount = _forumSettings.ShowCustomersPostCount,
                    ForumPostCount = await _genericAttributeService.GetAttributeAsync<Customer, int>(post.CustomerId, NopCustomerDefaults.ForumPostCountAttribute),
                    ShowCustomersJoinDate = _customerSettings.ShowCustomersJoinDate && !customerIsGuest,
                    CustomerJoinDate = customer?.CreatedOnUtc ?? DateTime.Now,
                    AllowPrivateMessages = _forumSettings.AllowPrivateMessages && !customerIsGuest,
                    SignaturesEnabled = _forumSettings.SignaturesEnabled,
                    FormattedSignature = _forumService.FormatForumSignatureText(await _genericAttributeService.GetAttributeAsync<Customer, string>(post.CustomerId, NopCustomerDefaults.SignatureAttribute)),
                };
                //created on string
                var languageCode = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;
                if (_forumSettings.RelativeDateTimeFormattingEnabled)
                {
                    var postCreatedAgo = post.CreatedOnUtc.RelativeFormat(languageCode);
                    forumPostModel.PostCreatedOnStr = string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postCreatedAgo);
                }
                else
                    forumPostModel.PostCreatedOnStr =
                        (await _dateTimeHelper.ConvertToUserTimeAsync(post.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
                //avatar
                if (_customerSettings.AllowCustomersToUploadAvatars)
                {
                    forumPostModel.CustomerAvatarUrl = await _pictureService.GetPictureUrlAsync(
                        await _genericAttributeService.GetAttributeAsync<Customer, int>(post.CustomerId, NopCustomerDefaults.AvatarPictureIdAttribute),
                        _mediaSettings.AvatarPictureSize,
                        _customerSettings.DefaultAvatarEnabled,
                        defaultPictureType: PictureType.Avatar);
                }
                //location
                forumPostModel.ShowCustomersLocation = _customerSettings.ShowCustomersLocation && !customerIsGuest;
                if (_customerSettings.ShowCustomersLocation)
                {
                    var countryId = await _genericAttributeService.GetAttributeAsync<Customer, int>(post.CustomerId, NopCustomerDefaults.CountryIdAttribute);
                    var country = await _countryService.GetCountryByIdAsync(countryId);
                    forumPostModel.CustomerLocation = country != null ? await _localizationService.GetLocalizedAsync(country, x => x.Name) : string.Empty;
                }

                //votes
                if (_forumSettings.AllowPostVoting)
                {
                    forumPostModel.AllowPostVoting = true;
                    forumPostModel.VoteCount = post.VoteCount;
                    var postVote = await _forumService.GetPostVoteAsync(post.Id, currentCustomer);
                    if (postVote != null)
                        forumPostModel.VoteIsUp = postVote.IsUp;
                }

                // page number is needed for creating post link in _ForumPost partial view
                forumPostModel.CurrentTopicPage = page;
                model.ForumPostModels.Add(forumPostModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the topic move model
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic move model
        /// </returns>
        public virtual async Task<TopicMoveModel> PrepareTopicMoveAsync(ForumTopic forumTopic)
        {
            if (forumTopic == null)
                throw new ArgumentNullException(nameof(forumTopic));

            var model = new TopicMoveModel
            {
                ForumList = await ForumGroupsForumsListAsync(),
                Id = forumTopic.Id,
                TopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
                ForumSelected = forumTopic.ForumId
            };

            return model;
        }

        /// <summary>
        /// Prepare the forum topic create model
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <param name="model">Edit forum topic model</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareTopicCreateModelAsync(Forum forum, EditForumTopicModel model)
        {
            if (forum == null)
                throw new ArgumentNullException(nameof(forum));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var customer = await _workContext.GetCurrentCustomerAsync();
            model.IsEdit = false;
            model.ForumId = forum.Id;
            model.ForumName = forum.Name;
            model.ForumSeName = await _forumService.GetForumSeNameAsync(forum);
            model.ForumEditor = _forumSettings.ForumEditor;
            model.IsCustomerAllowedToSetTopicPriority = await _forumService.IsCustomerAllowedToSetTopicPriorityAsync(customer);
            model.TopicPriorities = await ForumTopicTypesListAsync();
            model.IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(customer);
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum;
        }

        /// <summary>
        /// Prepare the forum topic edit model
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="model">Edit forum topic model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareTopicEditModelAsync(ForumTopic forumTopic, EditForumTopicModel model, bool excludeProperties)
        {
            if (forumTopic == null)
                throw new ArgumentNullException(nameof(forumTopic));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
            if (forum == null)
                throw new ArgumentException("forum cannot be loaded");

            var customer = await _workContext.GetCurrentCustomerAsync();
            model.IsEdit = true;
            model.Id = forumTopic.Id;
            model.TopicPriorities = await ForumTopicTypesListAsync();
            model.ForumName = forum.Name;
            model.ForumSeName = await _forumService.GetForumSeNameAsync(forum);
            model.ForumId = forum.Id;
            model.ForumEditor = _forumSettings.ForumEditor;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum;
            model.IsCustomerAllowedToSetTopicPriority = await _forumService.IsCustomerAllowedToSetTopicPriorityAsync(customer);
            model.IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(customer);

            if (!excludeProperties)
            {
                var firstPost = await _forumService.GetFirstPostAsync(forumTopic);
                model.Text = firstPost.Text;
                model.Subject = forumTopic.Subject;
                model.TopicTypeId = forumTopic.TopicTypeId;
                //subscription            
                if (model.IsCustomerAllowedToSubscribe)
                {
                    var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
                    model.Subscribed = forumSubscription != null;
                }
            }
        }

        /// <summary>
        /// Prepare the forum post create model
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="quote">Identifier of the quoted post; pass null to load the empty text</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the edit forum post model
        /// </returns>
        public virtual async Task<EditForumPostModel> PreparePostCreateModelAsync(ForumTopic forumTopic, int? quote, bool excludeProperties)
        {
            if (forumTopic == null)
                throw new ArgumentNullException(nameof(forumTopic));

            var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
            if (forum == null)
                throw new ArgumentException("forum cannot be loaded");

            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var model = new EditForumPostModel
            {
                ForumTopicId = forumTopic.Id,
                IsEdit = false,
                ForumEditor = _forumSettings.ForumEditor,
                ForumName = forum.Name,
                ForumTopicSubject = forumTopic.Subject,
                ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
                IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(currentCustomer),
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum
            };

            if (!excludeProperties)
            {
                //subscription            
                if (model.IsCustomerAllowedToSubscribe)
                {
                    var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(currentCustomer.Id,
                        0, forumTopic.Id, 0, 1)).FirstOrDefault();
                    model.Subscribed = forumSubscription != null;
                }

                // Insert the quoted text
                var text = string.Empty;
                if (quote.HasValue)
                {
                    var quotePost = await _forumService.GetPostByIdAsync(quote.Value);
                    
                    if (quotePost != null && quotePost.TopicId == forumTopic.Id)
                    {
                        var customer = await _customerService.GetCustomerByIdAsync(quotePost.CustomerId);

                        var quotePostText = quotePost.Text;

                        switch (_forumSettings.ForumEditor)
                        {
                            case EditorType.SimpleTextBox:
                                text = $"{await _customerService.FormatUsernameAsync(customer)}:\n{quotePostText}\n";
                                break;
                            case EditorType.BBCodeEditor:
                                text = $"[quote={await _customerService.FormatUsernameAsync(customer)}]{_bbCodeHelper.RemoveQuotes(quotePostText)}[/quote]";
                                break;
                        }
                        model.Text = text;
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the forum post edit model
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the edit forum post model
        /// </returns>
        public virtual async Task<EditForumPostModel> PreparePostEditModelAsync(ForumPost forumPost, bool excludeProperties)
        {
            if (forumPost == null)
                throw new ArgumentNullException(nameof(forumPost));

            var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);
            if (forumTopic == null)
                throw new ArgumentException("forum topic cannot be loaded");

            var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId);
            if (forum == null)
                throw new ArgumentException("forum cannot be loaded");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var model = new EditForumPostModel
            {
                Id = forumPost.Id,
                ForumTopicId = forumTopic.Id,
                IsEdit = true,
                ForumEditor = _forumSettings.ForumEditor,
                ForumName = forum.Name,
                ForumTopicSubject = forumTopic.Subject,
                ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic),
                IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(customer),
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForum
            };

            if (!excludeProperties)
            {
                model.Text = forumPost.Text;
                //subscription
                if (model.IsCustomerAllowedToSubscribe)
                {
                    var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
                    model.Subscribed = forumSubscription != null;
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the search model
        /// </summary>
        /// <param name="searchterms">Search terms</param>
        /// <param name="advs">Whether to use the advanced search</param>
        /// <param name="forumId">Forum identifier</param>
        /// <param name="within">String representation of int value of ForumSearchType</param>
        /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
        /// <param name="page">Number of items page</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the search model
        /// </returns>
        public virtual async Task<SearchModel> PrepareSearchModelAsync(string searchterms, bool? advs, string forumId,
            string within, string limitDays, int page)
        {
            var model = new SearchModel();

            var pageSize = 10;

            // Create the values for the "Limit results to previous" select list
            var limitList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.AllResults"),
                    Value = "0"
                },
                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1day"),
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.7days"),
                    Value = "7"
                },
                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.2weeks"),
                    Value = "14"
                },
                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1month"),
                    Value = "30"
                },
                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.3months"),
                    Value = "92"
                },
                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.6months"),
                    Value = "183"
                },
                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1year"),
                    Value = "365"
                }
            };
            model.LimitList = limitList;

            // Create the values for the "Search in forum" select list
            var forumsSelectList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Forum.Search.SearchInForum.All"),
                    Value = "0",
                    Selected = true,
                }
            };

            var separator = "--";
            var forumGroups = await _forumService.GetAllForumGroupsAsync();
            foreach (var fg in forumGroups)
            {
                // Add the forum group with value as '-' so it can't be used as a target forum id
                forumsSelectList.Add(new SelectListItem { Text = fg.Name, Value = "-" });

                var forums = await _forumService.GetAllForumsByGroupIdAsync(fg.Id);
                foreach (var f in forums)
                {
                    forumsSelectList.Add(
                        new SelectListItem
                        {
                            Text = $"{separator}{f.Name}",
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
                    Value = ((int) ForumSearchType.All).ToString(),
                    Text = await _localizationService.GetResourceAsync("Forum.Search.SearchWithin.All")
                },
                new SelectListItem
                {
                    Value = ((int) ForumSearchType.TopicTitlesOnly).ToString(),
                    Text = await _localizationService.GetResourceAsync("Forum.Search.SearchWithin.TopicTitlesOnly")
                },
                new SelectListItem
                {
                    Value = ((int) ForumSearchType.PostTextOnly).ToString(),
                    Text = await _localizationService.GetResourceAsync("Forum.Search.SearchWithin.PostTextOnly")
                }
            };
            model.WithinList = withinList;

            _ = int.TryParse(forumId, out var forumIdSelected);
            model.ForumIdSelected = forumIdSelected;

            _ = int.TryParse(within, out var withinSelected);
            model.WithinSelected = withinSelected;

            _ = int.TryParse(limitDays, out var limitDaysSelected);
            model.LimitDaysSelected = limitDaysSelected;

            var searchTermMinimumLength = _forumSettings.ForumSearchTermMinimumLength;

            model.ShowAdvancedSearch = advs.GetValueOrDefault();
            model.SearchResultsVisible = false;
            model.NoResultsVisisble = false;
            model.PostsPageSize = _forumSettings.PostsPageSize;

            model.AllowPostVoting = _forumSettings.AllowPostVoting;

            try
            {
                if (!string.IsNullOrWhiteSpace(searchterms))
                {
                    searchterms = searchterms.Trim();
                    model.SearchTerms = searchterms;

                    if (searchterms.Length < searchTermMinimumLength)
                    {
                        throw new NopException(string.Format(await _localizationService.GetResourceAsync("Forum.SearchTermMinimumLengthIsNCharacters"),
                            searchTermMinimumLength));
                    }

                    ForumSearchType searchWithin = 0;
                    var limitResultsToPrevious = 0;
                    if (advs.GetValueOrDefault())
                    {
                        searchWithin = (ForumSearchType)withinSelected;
                        limitResultsToPrevious = limitDaysSelected;
                    }

                    if (_forumSettings.SearchResultsPageSize > 0)
                    {
                        pageSize = _forumSettings.SearchResultsPageSize;
                    }

                    var topics = await _forumService.GetAllTopicsAsync(forumIdSelected, 0, searchterms, searchWithin,
                        limitResultsToPrevious, page - 1, pageSize);
                    model.TopicPageSize = topics.PageSize;
                    model.TopicTotalRecords = topics.TotalCount;
                    model.TopicPageIndex = topics.PageIndex;
                    foreach (var topic in topics)
                    {
                        var topicModel = await PrepareForumTopicRowModelAsync(topic);
                        model.ForumTopics.Add(topicModel);
                    }

                    model.SearchResultsVisible = (topics.Any());
                    model.NoResultsVisisble = !(model.SearchResultsVisible);

                    return model;
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

            return model;
        }

        /// <summary>
        /// Prepare the last post model
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <param name="showTopic">Whether to show topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the last post model
        /// </returns>
        public virtual async Task<LastPostModel> PrepareLastPostModelAsync(ForumPost forumPost, bool showTopic)
        {
            var model = new LastPostModel
            {
                ShowTopic = showTopic
            };

            //do not throw an exception here
            if (forumPost == null)
                return model;

            var topic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);

            if (topic is null)
                return model;

            var customer = await _customerService.GetCustomerByIdAsync(forumPost.CustomerId);

            model.Id = forumPost.Id;
            model.ForumTopicId = topic.Id;
            model.ForumTopicSeName = await _forumService.GetTopicSeNameAsync(topic);
            model.ForumTopicSubject = _forumService.StripTopicSubject(topic);
            model.CustomerId = forumPost.CustomerId;
            model.AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !await _customerService.IsGuestAsync(customer);
            model.CustomerName = await _customerService.FormatUsernameAsync(customer);
            //created on string
            var languageCode = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;
            if (_forumSettings.RelativeDateTimeFormattingEnabled)
            {
                var postCreatedAgo = forumPost.CreatedOnUtc.RelativeFormat(languageCode);
                model.PostCreatedOnStr = string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postCreatedAgo);
            }
            else
                model.PostCreatedOnStr = (await _dateTimeHelper.ConvertToUserTimeAsync(forumPost.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");

            return model;
        }

        /// <summary>
        /// Prepare the forum breadcrumb model
        /// </summary>
        /// <param name="forumGroupId">Forum group identifier; pass null to load nothing</param>
        /// <param name="forumId">Forum identifier; pass null to load breadcrumbs up to forum group</param>
        /// <param name="forumTopicId">Forum topic identifier; pass null to load breadcrumbs up to forum</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum breadcrumb model
        /// </returns>
        public virtual async Task<ForumBreadcrumbModel> PrepareForumBreadcrumbModelAsync(int? forumGroupId, int? forumId, int? forumTopicId)
        {
            var model = new ForumBreadcrumbModel();

            ForumTopic forumTopic = null;
            if (forumTopicId.HasValue)
            {
                forumTopic = await _forumService.GetTopicByIdAsync(forumTopicId.Value);
                if (forumTopic != null)
                {
                    model.ForumTopicId = forumTopic.Id;
                    model.ForumTopicSubject = forumTopic.Subject;
                    model.ForumTopicSeName = await _forumService.GetTopicSeNameAsync(forumTopic);
                }
            }

            var forum = await _forumService.GetForumByIdAsync(forumTopic != null ? forumTopic.ForumId : (forumId ?? 0));
            if (forum != null)
            {
                model.ForumId = forum.Id;
                model.ForumName = forum.Name;
                model.ForumSeName = await _forumService.GetForumSeNameAsync(forum);
            }

            var forumGroup = await _forumService.GetForumGroupByIdAsync(forum != null ? forum.ForumGroupId : (forumGroupId ?? 0));
            if (forumGroup != null)
            {
                model.ForumGroupId = forumGroup.Id;
                model.ForumGroupName = forumGroup.Name;
                model.ForumGroupSeName = await _forumService.GetForumGroupSeNameAsync(forumGroup);
            }

            return model;
        }

        /// <summary>
        /// Prepare the customer forum subscriptions model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer forum subscriptions model
        /// </returns>
        public virtual async Task<CustomerForumSubscriptionsModel> PrepareCustomerForumSubscriptionsModelAsync(int? page)
        {
            var pageIndex = 0;
            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            var customer = await _workContext.GetCurrentCustomerAsync();

            var pageSize = _forumSettings.ForumSubscriptionsPageSize;

            var list = await _forumService.GetAllSubscriptionsAsync(customer.Id, 0, 0, pageIndex, pageSize);

            var model = new CustomerForumSubscriptionsModel();

            foreach (var forumSubscription in list)
            {
                var forumTopicId = forumSubscription.TopicId;
                var forumId = forumSubscription.ForumId;
                var topicSubscription = false;
                var title = string.Empty;
                var slug = string.Empty;

                if (forumTopicId > 0)
                {
                    topicSubscription = true;
                    var forumTopic = await _forumService.GetTopicByIdAsync(forumTopicId);
                    if (forumTopic != null)
                    {
                        title = forumTopic.Subject;
                        slug = await _forumService.GetTopicSeNameAsync(forumTopic);
                    }
                }
                else
                {
                    var forum = await _forumService.GetForumByIdAsync(forumId);
                    if (forum != null)
                    {
                        title = forum.Name;
                        slug = await _forumService.GetForumSeNameAsync(forum);
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

            model.PagerModel = new PagerModel(_localizationService)
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerForumSubscriptions",
                UseRouteLinks = true,
                RouteValues = new ForumSubscriptionsRouteValues { pageNumber = pageIndex }
            };

            return model;
        }

        /// <summary>
        /// Prepare the forum topic row model
        /// </summary>
        /// <param name="topic">Forum topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum topic row model
        /// </returns>
        public virtual async Task<ForumTopicRowModel> PrepareForumTopicRowModelAsync(ForumTopic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            var customer = await _customerService.GetCustomerByIdAsync(topic.CustomerId);

            var topicModel = new ForumTopicRowModel
            {
                Id = topic.Id,
                Subject = topic.Subject,
                SeName = await _forumService.GetTopicSeNameAsync(topic),
                LastPostId = topic.LastPostId,
                NumPosts = topic.NumPosts,
                Views = topic.Views,
                NumReplies = topic.NumPosts > 0 ? topic.NumPosts - 1 : 0,
                ForumTopicType = topic.ForumTopicType,
                CustomerId = topic.CustomerId,
                AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !await _customerService.IsGuestAsync(customer),
                CustomerName = await _customerService.FormatUsernameAsync(customer)
            };

            var forumPosts = await _forumService.GetAllPostsAsync(topic.Id, 0, string.Empty, 1, _forumSettings.PostsPageSize);
            topicModel.TotalPostPages = forumPosts.TotalPages;

            var firstPost = await _forumService.GetFirstPostAsync(topic);
            topicModel.Votes = firstPost != null ? firstPost.VoteCount : 0;

            return topicModel;
        }

        /// <summary>
        /// Prepare the forum row model
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum row model
        /// </returns>
        public virtual async Task<ForumRowModel> PrepareForumRowModelAsync(Forum forum)
        {
            if (forum == null)
                throw new ArgumentNullException(nameof(forum));

            var forumModel = new ForumRowModel
            {
                Id = forum.Id,
                Name = forum.Name,
                SeName = await _forumService.GetForumSeNameAsync(forum),
                Description = forum.Description,
                NumTopics = forum.NumTopics,
                NumPosts = forum.NumPosts,
                LastPostId = forum.LastPostId,
            };

            return forumModel;
        }

        #endregion
    }
}