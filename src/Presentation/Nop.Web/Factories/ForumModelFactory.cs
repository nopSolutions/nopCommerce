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

        protected CaptchaSettings CaptchaSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected ForumSettings ForumSettings { get; }
        private readonly IBBCodeHelper _bbCodeHelper;
        protected ICountryService CountryService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IForumService ForumService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IPictureService PictureService { get; }
        protected IWorkContext WorkContext { get; }
        protected MediaSettings MediaSettings { get; }

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
            CaptchaSettings = captchaSettings;
            CustomerSettings = customerSettings;
            ForumSettings = forumSettings;
            _bbCodeHelper = bbCodeHelper;
            CountryService = countryService;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            ForumService = forumService;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            PictureService = pictureService;
            WorkContext = workContext;
            MediaSettings = mediaSettings;
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
                    Text = await LocalizationService.GetResourceAsync("Forum.Normal"),
                    Value = ((int)ForumTopicType.Normal).ToString()
                },

                new SelectListItem
                {
                    Text = await LocalizationService.GetResourceAsync("Forum.Sticky"),
                    Value = ((int)ForumTopicType.Sticky).ToString()
                },

                new SelectListItem
                {
                    Text = await LocalizationService.GetResourceAsync("Forum.Announcement"),
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
            var forumGroups = await ForumService.GetAllForumGroupsAsync();

            foreach (var fg in forumGroups)
            {
                // Add the forum group with Value of 0 so it won't be used as a target forum
                forumsList.Add(new SelectListItem { Text = fg.Name, Value = "0" });

                var forums = await ForumService.GetAllForumsByGroupIdAsync(fg.Id);
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
                SeName = await ForumService.GetForumGroupSeNameAsync(forumGroup),
            };
            var forums = await ForumService.GetAllForumsByGroupIdAsync(forumGroup.Id);
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

            var forumGroups = await ForumService.GetAllForumGroupsAsync();
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
                ActiveDiscussionsFeedEnabled = ForumSettings.ActiveDiscussionsFeedEnabled,
                PostsPageSize = ForumSettings.PostsPageSize,
                AllowPostVoting = ForumSettings.AllowPostVoting
            };

            var topics = await ForumService.GetActiveTopicsAsync(0, 0, ForumSettings.HomepageActiveDiscussionsTopicCount);
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
                ActiveDiscussionsFeedEnabled = ForumSettings.ActiveDiscussionsFeedEnabled,
                PostsPageSize = ForumSettings.PostsPageSize,
                AllowPostVoting = ForumSettings.AllowPostVoting
            };

            var pageSize = ForumSettings.ActiveDiscussionsPageSize > 0 ? ForumSettings.ActiveDiscussionsPageSize : 50;

            var topics = await ForumService.GetActiveTopicsAsync(forumId, (page - 1), pageSize);
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
                SeName = await ForumService.GetForumSeNameAsync(forum),
                Description = forum.Description
            };

            var pageSize = ForumSettings.TopicsPageSize > 0 ? ForumSettings.TopicsPageSize : 10;

            model.AllowPostVoting = ForumSettings.AllowPostVoting;

            //subscription
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await ForumService.IsCustomerAllowedToSubscribeAsync(customer))
            {
                model.WatchForumText = await LocalizationService.GetResourceAsync("Forum.WatchForum");

                var forumSubscription = (await ForumService.GetAllSubscriptionsAsync(customer.Id, forum.Id, 0, 0, 1)).FirstOrDefault();
                if (forumSubscription != null)
                {
                    model.WatchForumText = await LocalizationService.GetResourceAsync("Forum.UnwatchForum");
                }
            }

            var topics = await ForumService.GetAllTopicsAsync(forum.Id, 0, string.Empty, ForumSearchType.All, 0, (page - 1), pageSize);
            model.TopicPageSize = topics.PageSize;
            model.TopicTotalRecords = topics.TotalCount;
            model.TopicPageIndex = topics.PageIndex;
            foreach (var topic in topics)
            {
                var topicModel = await PrepareForumTopicRowModelAsync(topic);
                model.ForumTopics.Add(topicModel);
            }
            model.IsCustomerAllowedToSubscribe = await ForumService.IsCustomerAllowedToSubscribeAsync(customer);
            model.ForumFeedsEnabled = ForumSettings.ForumFeedsEnabled;
            model.PostsPageSize = ForumSettings.PostsPageSize;
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
            var posts = await ForumService.GetAllPostsAsync(forumTopic.Id, 0, string.Empty,
                page - 1, ForumSettings.PostsPageSize);

            //prepare model
            var currentCustomer = await WorkContext.GetCurrentCustomerAsync();
            var model = new ForumTopicPageModel
            {
                Id = forumTopic.Id,
                Subject = forumTopic.Subject,
                SeName = await ForumService.GetTopicSeNameAsync(forumTopic),

                IsCustomerAllowedToEditTopic = await ForumService.IsCustomerAllowedToEditTopicAsync(currentCustomer, forumTopic),
                IsCustomerAllowedToDeleteTopic = await ForumService.IsCustomerAllowedToDeleteTopicAsync(currentCustomer, forumTopic),
                IsCustomerAllowedToMoveTopic = await ForumService.IsCustomerAllowedToMoveTopicAsync(currentCustomer, forumTopic),
                IsCustomerAllowedToSubscribe = await ForumService.IsCustomerAllowedToSubscribeAsync(currentCustomer)
            };

            if (model.IsCustomerAllowedToSubscribe)
            {
                model.WatchTopicText = await LocalizationService.GetResourceAsync("Forum.WatchTopic");

                var forumTopicSubscription = (await ForumService.GetAllSubscriptionsAsync(currentCustomer.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
                if (forumTopicSubscription != null)
                {
                    model.WatchTopicText = await LocalizationService.GetResourceAsync("Forum.UnwatchTopic");
                }
            }
            model.PostsPageIndex = posts.PageIndex;
            model.PostsPageSize = posts.PageSize;
            model.PostsTotalRecords = posts.TotalCount;
            foreach (var post in posts)
            {
                var customer = await CustomerService.GetCustomerByIdAsync(post.CustomerId);

                var customerIsGuest = await CustomerService.IsGuestAsync(customer);
                var customerIsModerator = !customerIsGuest && await _customerService.IsForumModeratorAsync(customer);

                var forumPostModel = new ForumPostModel
                {
                    Id = post.Id,
                    ForumTopicId = post.TopicId,
                    ForumTopicSeName = await ForumService.GetTopicSeNameAsync(forumTopic),
                    FormattedText = ForumService.FormatPostText(post),
                    IsCurrentCustomerAllowedToEditPost = await ForumService.IsCustomerAllowedToEditPostAsync(currentCustomer, post),
                    IsCurrentCustomerAllowedToDeletePost = await ForumService.IsCustomerAllowedToDeletePostAsync(currentCustomer, post),
                    CustomerId = post.CustomerId,
                    AllowViewingProfiles = CustomerSettings.AllowViewingProfiles && !customerIsGuest,
                    CustomerName = await CustomerService.FormatUsernameAsync(customer),
                    IsCustomerForumModerator = customerIsModerator,
                    ShowCustomersPostCount = ForumSettings.ShowCustomersPostCount,
                    ForumPostCount = await GenericAttributeService.GetAttributeAsync<Customer, int>(post.CustomerId, NopCustomerDefaults.ForumPostCountAttribute),
                    ShowCustomersJoinDate = CustomerSettings.ShowCustomersJoinDate && !customerIsGuest,
                    CustomerJoinDate = customer?.CreatedOnUtc ?? DateTime.Now,
                    AllowPrivateMessages = ForumSettings.AllowPrivateMessages && !customerIsGuest,
                    SignaturesEnabled = ForumSettings.SignaturesEnabled,
                    FormattedSignature = ForumService.FormatForumSignatureText(await GenericAttributeService.GetAttributeAsync<Customer, string>(post.CustomerId, NopCustomerDefaults.SignatureAttribute)),
                };
                //created on string
                var languageCode = (await WorkContext.GetWorkingLanguageAsync()).LanguageCulture;
                if (ForumSettings.RelativeDateTimeFormattingEnabled)
                {
                    var postCreatedAgo = post.CreatedOnUtc.RelativeFormat(languageCode);
                    forumPostModel.PostCreatedOnStr = string.Format(await LocalizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postCreatedAgo);
                }
                else
                    forumPostModel.PostCreatedOnStr =
                        (await DateTimeHelper.ConvertToUserTimeAsync(post.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
                //avatar
                if (CustomerSettings.AllowCustomersToUploadAvatars)
                {
                    forumPostModel.CustomerAvatarUrl = await PictureService.GetPictureUrlAsync(
                        await GenericAttributeService.GetAttributeAsync<Customer, int>(post.CustomerId, NopCustomerDefaults.AvatarPictureIdAttribute),
                        MediaSettings.AvatarPictureSize,
                        CustomerSettings.DefaultAvatarEnabled,
                        defaultPictureType: PictureType.Avatar);
                }
                //location
                forumPostModel.ShowCustomersLocation = CustomerSettings.ShowCustomersLocation && !customerIsGuest;
                if (CustomerSettings.ShowCustomersLocation)
                {
                    var countryId = await GenericAttributeService.GetAttributeAsync<Customer, int>(post.CustomerId, NopCustomerDefaults.CountryIdAttribute);
                    var country = await CountryService.GetCountryByIdAsync(countryId);
                    forumPostModel.CustomerLocation = country != null ? await LocalizationService.GetLocalizedAsync(country, x => x.Name) : string.Empty;
                }

                //votes
                if (ForumSettings.AllowPostVoting)
                {
                    forumPostModel.AllowPostVoting = true;
                    forumPostModel.VoteCount = post.VoteCount;
                    var postVote = await ForumService.GetPostVoteAsync(post.Id, currentCustomer);
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
                TopicSeName = await ForumService.GetTopicSeNameAsync(forumTopic),
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

            var customer = await WorkContext.GetCurrentCustomerAsync();
            model.IsEdit = false;
            model.ForumId = forum.Id;
            model.ForumName = forum.Name;
            model.ForumSeName = await ForumService.GetForumSeNameAsync(forum);
            model.ForumEditor = ForumSettings.ForumEditor;
            model.IsCustomerAllowedToSetTopicPriority = await ForumService.IsCustomerAllowedToSetTopicPriorityAsync(customer);
            model.TopicPriorities = await ForumTopicTypesListAsync();
            model.IsCustomerAllowedToSubscribe = await ForumService.IsCustomerAllowedToSubscribeAsync(customer);
            model.DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnForum;
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

            var forum = await ForumService.GetForumByIdAsync(forumTopic.ForumId);
            if (forum == null)
                throw new ArgumentException("forum cannot be loaded");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            model.IsEdit = true;
            model.Id = forumTopic.Id;
            model.TopicPriorities = await ForumTopicTypesListAsync();
            model.ForumName = forum.Name;
            model.ForumSeName = await ForumService.GetForumSeNameAsync(forum);
            model.ForumId = forum.Id;
            model.ForumEditor = ForumSettings.ForumEditor;
            model.DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnForum;
            model.IsCustomerAllowedToSetTopicPriority = await ForumService.IsCustomerAllowedToSetTopicPriorityAsync(customer);
            model.IsCustomerAllowedToSubscribe = await ForumService.IsCustomerAllowedToSubscribeAsync(customer);

            if (!excludeProperties)
            {
                var firstPost = await ForumService.GetFirstPostAsync(forumTopic);
                model.Text = firstPost.Text;
                model.Subject = forumTopic.Subject;
                model.TopicTypeId = forumTopic.TopicTypeId;
                //subscription            
                if (model.IsCustomerAllowedToSubscribe)
                {
                    var forumSubscription = (await ForumService.GetAllSubscriptionsAsync(customer.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
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

            var forum = await ForumService.GetForumByIdAsync(forumTopic.ForumId);
            if (forum == null)
                throw new ArgumentException("forum cannot be loaded");

            var currentCustomer = await WorkContext.GetCurrentCustomerAsync();
            var model = new EditForumPostModel
            {
                ForumTopicId = forumTopic.Id,
                IsEdit = false,
                ForumEditor = ForumSettings.ForumEditor,
                ForumName = forum.Name,
                ForumTopicSubject = forumTopic.Subject,
                ForumTopicSeName = await ForumService.GetTopicSeNameAsync(forumTopic),
                IsCustomerAllowedToSubscribe = await ForumService.IsCustomerAllowedToSubscribeAsync(currentCustomer),
                DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnForum
            };

            if (!excludeProperties)
            {
                //subscription            
                if (model.IsCustomerAllowedToSubscribe)
                {
                    var forumSubscription = (await ForumService.GetAllSubscriptionsAsync(currentCustomer.Id,
                        0, forumTopic.Id, 0, 1)).FirstOrDefault();
                    model.Subscribed = forumSubscription != null;
                }

                // Insert the quoted text
                var text = string.Empty;
                if (quote.HasValue)
                {
                    var quotePost = await ForumService.GetPostByIdAsync(quote.Value);
                    
                    if (quotePost != null && quotePost.TopicId == forumTopic.Id)
                    {
                        var customer = await CustomerService.GetCustomerByIdAsync(quotePost.CustomerId);

                        var quotePostText = quotePost.Text;

                        switch (ForumSettings.ForumEditor)
                        {
                            case EditorType.SimpleTextBox:
                                text = $"{await CustomerService.FormatUsernameAsync(customer)}:\n{quotePostText}\n";
                                break;
                            case EditorType.BBCodeEditor:
                                text = $"[quote={await CustomerService.FormatUsernameAsync(customer)}]{BBCodeHelper.RemoveQuotes(quotePostText)}[/quote]";
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

            var forumTopic = await ForumService.GetTopicByIdAsync(forumPost.TopicId);
            if (forumTopic == null)
                throw new ArgumentException("forum topic cannot be loaded");

            var forum = await ForumService.GetForumByIdAsync(forumTopic.ForumId);
            if (forum == null)
                throw new ArgumentException("forum cannot be loaded");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var model = new EditForumPostModel
            {
                Id = forumPost.Id,
                ForumTopicId = forumTopic.Id,
                IsEdit = true,
                ForumEditor = ForumSettings.ForumEditor,
                ForumName = forum.Name,
                ForumTopicSubject = forumTopic.Subject,
                ForumTopicSeName = await ForumService.GetTopicSeNameAsync(forumTopic),
                IsCustomerAllowedToSubscribe = await ForumService.IsCustomerAllowedToSubscribeAsync(customer),
                DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnForum
            };

            if (!excludeProperties)
            {
                model.Text = forumPost.Text;
                //subscription
                if (model.IsCustomerAllowedToSubscribe)
                {
                    var forumSubscription = (await ForumService.GetAllSubscriptionsAsync(customer.Id, 0, forumTopic.Id, 0, 1)).FirstOrDefault();
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
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.AllResults"),
                    Value = "0"
                },
                new SelectListItem
                {
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1day"),
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.7days"),
                    Value = "7"
                },
                new SelectListItem
                {
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.2weeks"),
                    Value = "14"
                },
                new SelectListItem
                {
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1month"),
                    Value = "30"
                },
                new SelectListItem
                {
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.3months"),
                    Value = "92"
                },
                new SelectListItem
                {
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.6months"),
                    Value = "183"
                },
                new SelectListItem
                {
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.LimitResultsToPrevious.1year"),
                    Value = "365"
                }
            };
            model.LimitList = limitList;

            // Create the values for the "Search in forum" select list
            var forumsSelectList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.SearchInForum.All"),
                    Value = "0",
                    Selected = true,
                }
            };

            var separator = "--";
            var forumGroups = await ForumService.GetAllForumGroupsAsync();
            foreach (var fg in forumGroups)
            {
                // Add the forum group with value as '-' so it can't be used as a target forum id
                forumsSelectList.Add(new SelectListItem { Text = fg.Name, Value = "-" });

                var forums = await ForumService.GetAllForumsByGroupIdAsync(fg.Id);
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
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.SearchWithin.All")
                },
                new SelectListItem
                {
                    Value = ((int) ForumSearchType.TopicTitlesOnly).ToString(),
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.SearchWithin.TopicTitlesOnly")
                },
                new SelectListItem
                {
                    Value = ((int) ForumSearchType.PostTextOnly).ToString(),
                    Text = await LocalizationService.GetResourceAsync("Forum.Search.SearchWithin.PostTextOnly")
                }
            };
            model.WithinList = withinList;

            _ = int.TryParse(forumId, out var forumIdSelected);
            model.ForumIdSelected = forumIdSelected;

            _ = int.TryParse(within, out var withinSelected);
            model.WithinSelected = withinSelected;

            _ = int.TryParse(limitDays, out var limitDaysSelected);
            model.LimitDaysSelected = limitDaysSelected;

            var searchTermMinimumLength = ForumSettings.ForumSearchTermMinimumLength;

            model.ShowAdvancedSearch = advs.GetValueOrDefault();
            model.SearchResultsVisible = false;
            model.NoResultsVisisble = false;
            model.PostsPageSize = ForumSettings.PostsPageSize;

            model.AllowPostVoting = ForumSettings.AllowPostVoting;

            try
            {
                if (!string.IsNullOrWhiteSpace(searchterms))
                {
                    searchterms = searchterms.Trim();
                    model.SearchTerms = searchterms;

                    if (searchterms.Length < searchTermMinimumLength)
                    {
                        throw new NopException(string.Format(await LocalizationService.GetResourceAsync("Forum.SearchTermMinimumLengthIsNCharacters"),
                            searchTermMinimumLength));
                    }

                    ForumSearchType searchWithin = 0;
                    var limitResultsToPrevious = 0;
                    if (advs.GetValueOrDefault())
                    {
                        searchWithin = (ForumSearchType)withinSelected;
                        limitResultsToPrevious = limitDaysSelected;
                    }

                    if (ForumSettings.SearchResultsPageSize > 0)
                    {
                        pageSize = ForumSettings.SearchResultsPageSize;
                    }

                    var topics = await ForumService.GetAllTopicsAsync(forumIdSelected, 0, searchterms, searchWithin,
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

            var topic = await ForumService.GetTopicByIdAsync(forumPost.TopicId);

            if (topic is null)
                return model;

            var customer = await CustomerService.GetCustomerByIdAsync(forumPost.CustomerId);

            model.Id = forumPost.Id;
            model.ForumTopicId = topic.Id;
            model.ForumTopicSeName = await ForumService.GetTopicSeNameAsync(topic);
            model.ForumTopicSubject = ForumService.StripTopicSubject(topic);
            model.CustomerId = forumPost.CustomerId;
            model.AllowViewingProfiles = CustomerSettings.AllowViewingProfiles && !await CustomerService.IsGuestAsync(customer);
            model.CustomerName = await CustomerService.FormatUsernameAsync(customer);
            //created on string
            var languageCode = (await WorkContext.GetWorkingLanguageAsync()).LanguageCulture;
            if (ForumSettings.RelativeDateTimeFormattingEnabled)
            {
                var postCreatedAgo = forumPost.CreatedOnUtc.RelativeFormat(languageCode);
                model.PostCreatedOnStr = string.Format(await LocalizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postCreatedAgo);
            }
            else
                model.PostCreatedOnStr = (await DateTimeHelper.ConvertToUserTimeAsync(forumPost.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");

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
                forumTopic = await ForumService.GetTopicByIdAsync(forumTopicId.Value);
                if (forumTopic != null)
                {
                    model.ForumTopicId = forumTopic.Id;
                    model.ForumTopicSubject = forumTopic.Subject;
                    model.ForumTopicSeName = await ForumService.GetTopicSeNameAsync(forumTopic);
                }
            }

            var forum = await ForumService.GetForumByIdAsync(forumTopic != null ? forumTopic.ForumId : (forumId ?? 0));
            if (forum != null)
            {
                model.ForumId = forum.Id;
                model.ForumName = forum.Name;
                model.ForumSeName = await ForumService.GetForumSeNameAsync(forum);
            }

            var forumGroup = await ForumService.GetForumGroupByIdAsync(forum != null ? forum.ForumGroupId : (forumGroupId ?? 0));
            if (forumGroup != null)
            {
                model.ForumGroupId = forumGroup.Id;
                model.ForumGroupName = forumGroup.Name;
                model.ForumGroupSeName = await ForumService.GetForumGroupSeNameAsync(forumGroup);
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

            var customer = await WorkContext.GetCurrentCustomerAsync();

            var pageSize = ForumSettings.ForumSubscriptionsPageSize;

            var list = await ForumService.GetAllSubscriptionsAsync(customer.Id, 0, 0, pageIndex, pageSize);

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
                    var forumTopic = await ForumService.GetTopicByIdAsync(forumTopicId);
                    if (forumTopic != null)
                    {
                        title = forumTopic.Subject;
                        slug = await ForumService.GetTopicSeNameAsync(forumTopic);
                    }
                }
                else
                {
                    var forum = await ForumService.GetForumByIdAsync(forumId);
                    if (forum != null)
                    {
                        title = forum.Name;
                        slug = await ForumService.GetForumSeNameAsync(forum);
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

            var customer = await CustomerService.GetCustomerByIdAsync(topic.CustomerId);

            var topicModel = new ForumTopicRowModel
            {
                Id = topic.Id,
                Subject = topic.Subject,
                SeName = await ForumService.GetTopicSeNameAsync(topic),
                LastPostId = topic.LastPostId,
                NumPosts = topic.NumPosts,
                Views = topic.Views,
                NumReplies = topic.NumPosts > 0 ? topic.NumPosts - 1 : 0,
                ForumTopicType = topic.ForumTopicType,
                CustomerId = topic.CustomerId,
                AllowViewingProfiles = CustomerSettings.AllowViewingProfiles && !await CustomerService.IsGuestAsync(customer),
                CustomerName = await CustomerService.FormatUsernameAsync(customer)
            };

            var forumPosts = await ForumService.GetAllPostsAsync(topic.Id, 0, string.Empty, 1, ForumSettings.PostsPageSize);
            topicModel.TotalPostPages = forumPosts.TotalPages;

            var firstPost = await ForumService.GetFirstPostAsync(topic);
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
                SeName = await ForumService.GetForumSeNameAsync(forum),
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