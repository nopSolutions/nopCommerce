using System.Globalization;
using System.Text.Encodings.Web;
using Humanizer;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Http;
using Nop.Plugin.Misc.Forums.Domain;
using Nop.Plugin.Misc.Forums.Public.Models;
using Nop.Plugin.Misc.Forums.Public.Models.JsonLD;
using Nop.Plugin.Misc.Forums.Services;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;
using Nop.Web.Models.Common;
using Nop.Web.Models.JsonLD;

namespace Nop.Plugin.Misc.Forums.Public.Factories;

/// <summary>
/// Represents the forum model factory
/// </summary>
public class ForumModelFactory
{
    #region Fields

    private readonly BBCodeHelper _bbCodeHelper;
    private readonly CaptchaSettings _captchaSettings;
    private readonly CustomerSettings _customerSettings;
    private readonly ForumService _forumService;
    private readonly ForumSettings _forumSettings;
    private readonly ICountryService _countryService;
    private readonly ICustomerService _customerService;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IPictureService _pictureService;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;
    private readonly MediaSettings _mediaSettings;
    private readonly PrivateMessageSettings _privateMessageSettings;
    private readonly SeoSettings _seoSettings;

    #endregion

    #region Ctor

    public ForumModelFactory(BBCodeHelper bbCodeHelper, 
        CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        ForumService forumService,
        ForumSettings forumSettings,
        ICountryService countryService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        IPictureService pictureService,
        IWebHelper webHelper,
        IWorkContext workContext,
        PrivateMessageSettings privateMessageSettings,
        MediaSettings mediaSettings,
        SeoSettings seoSettings)
    {
        _bbCodeHelper = bbCodeHelper;
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;
        _forumService = forumService;
        _forumSettings = forumSettings;
        _countryService = countryService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _pictureService = pictureService;
        _webHelper = webHelper;
        _workContext = workContext;
        _privateMessageSettings = privateMessageSettings;
        _mediaSettings = mediaSettings;
        _seoSettings = seoSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get the list of forum groups
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collection of the select list item
    /// </returns>
    private async Task<List<SelectListItem>> ForumGroupsForumsListAsync()
    {
        var forumsList = new List<SelectListItem>();
        var separator = "--";
        var forumGroups = await _forumService.GetAllForumGroupsAsync();

        foreach (var fg in forumGroups)
        {
            // Add the forum group with Value of 0 so it won't be used as a target forum
            forumsList.Add(new() { Text = fg.Name, Value = "0" });

            var forums = await _forumService.GetAllForumsByGroupIdAsync(fg.Id);
            foreach (var f in forums)
            {
                forumsList.Add(new() { Text = $"{separator}{f.Name}", Value = f.Id.ToString() });
            }
        }

        return forumsList;
    }

    /// <summary>
    /// Prepare JSON-LD forum topic model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <param name="firstPost">The first post on forum topic</param>
    /// <param name="model">Forum topic page model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains JSON-LD forum topic model
    /// </returns>
    private async Task<JsonLdForumTopicModel> PrepareJsonLdForumTopicAsync(ForumTopic forumTopic, ForumPost firstPost, ForumTopicPageModel model)
    {
        var forumTopicCustomer = await _customerService.GetCustomerByIdAsync(forumTopic.CustomerId);
        var customerName = await _customerService.FormatUsernameAsync(forumTopicCustomer);
        var createdOn = await _dateTimeHelper.ConvertToUserTimeAsync(forumTopic.CreatedOnUtc, DateTimeKind.Utc);

        var forumTopicModel = new JsonLdForumTopicModel
        {
            Author = new()
            {
                Name = JavaScriptEncoder.Default.Encode(customerName),
                Url = _nopUrlHelper.RouteUrl(NopRouteNames.Standard.CUSTOMER_PROFILE, new { id = forumTopic.CustomerId }, _webHelper.GetCurrentRequestProtocol()),
            },
            DatePublished = new DateTimeOffset(createdOn).ToString("O", CultureInfo.InvariantCulture),
            Subject = JavaScriptEncoder.Default.Encode(model.Subject),
            Text = _forumService.FormatPostText(firstPost),
            Url = _nopUrlHelper.RouteUrl(ForumDefaults.Routes.Public.TOPIC_SLUG, new { id = model.Id, slug = model.SeName }, _webHelper.GetCurrentRequestProtocol()),
            Comments = model.ForumPostModels.Where(pm => pm.Id != firstPost.Id).Select(postModel =>
            {
                var commentModel = new JsonLdForumTopicCommentModel
                {
                    Author = new()
                    {
                        Name = JavaScriptEncoder.Default.Encode(postModel.CustomerName),
                        Url = _nopUrlHelper.RouteUrl(NopRouteNames.Standard.CUSTOMER_PROFILE, new { id = postModel.CustomerId }, _webHelper.GetCurrentRequestProtocol()),
                    },
                    DatePublished = new DateTimeOffset(postModel.PostCreatedOn).ToString("O", CultureInfo.InvariantCulture),
                    Url = postModel.CurrentTopicPage > 1
                        ? _nopUrlHelper.RouteUrl(ForumDefaults.Routes.Public.TOPIC_SLUG_PAGED, new { id = model.Id, slug = model.SeName, pageNumber = postModel.CurrentTopicPage }, _webHelper.GetCurrentRequestProtocol(), null, postModel.Id.ToString())
                        : _nopUrlHelper.RouteUrl(ForumDefaults.Routes.Public.TOPIC_SLUG, new { id = model.Id, slug = model.SeName }, _webHelper.GetCurrentRequestProtocol(), null, postModel.Id.ToString()),
                    Text = postModel.FormattedText
                };

                if (_forumSettings.AllowPostVoting)
                {
                    commentModel.InteractionStatistic = new()
                    {
                        InteractionType = postModel.VoteCount >= 0
                            ? "https://schema.org/LikeAction"
                            : "https://schema.org/DislikeAction",
                        UserInteractionCount = Math.Abs(postModel.VoteCount)
                    };
                }

                return commentModel;
            }).ToList(),
            InteractionStatistic = new List<JsonLdInteractionStatisticModel>
            {
                new()
                {
                    InteractionType = "https://schema.org/CommentAction", UserInteractionCount = Math.Max(forumTopic.NumPosts - 1, 0)
                },
                new()
                {
                    InteractionType = "https://schema.org/ViewAction", UserInteractionCount = forumTopic.Views
                }
            }
        };

        return forumTopicModel;
    }

    /// <summary>
    /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
    /// </summary>
    /// <param name="source">Source (UTC format)</param>
    /// <param name="languageCode">Language culture code</param>
    /// <returns>Formatted date and time string</returns>
    public static string RelativeFormat(DateTime source, string languageCode = "en-US")
    {
        var ts = new TimeSpan(DateTime.UtcNow.Ticks - source.Ticks);
        var delta = ts.TotalSeconds;

        CultureInfo culture;
        try
        {
            culture = new CultureInfo(languageCode);
        }
        catch (CultureNotFoundException)
        {
            culture = new CultureInfo("en-US");
        }
        return TimeSpan.FromSeconds(delta).Humanize(precision: 1, culture: culture, maxUnit: TimeUnit.Year);
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
    public async Task<ForumGroupModel> PrepareForumGroupModelAsync(ForumGroup forumGroup)
    {
        ArgumentNullException.ThrowIfNull(forumGroup);

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
    public async Task<BoardsIndexModel> PrepareBoardsIndexModelAsync()
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
    public async Task<ActiveDiscussionsModel> PrepareActiveDiscussionsModelAsync()
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
    public async Task<ActiveDiscussionsModel> PrepareActiveDiscussionsModelAsync(int forumId, int page)
    {
        var model = new ActiveDiscussionsModel
        {
            ViewAllLinkEnabled = false,
            ActiveDiscussionsFeedEnabled = _forumSettings.ActiveDiscussionsFeedEnabled,
            PostsPageSize = _forumSettings.PostsPageSize,
            AllowPostVoting = _forumSettings.AllowPostVoting
        };

        var pageSize = _forumSettings.ActiveDiscussionsPageSize > 0 ? _forumSettings.ActiveDiscussionsPageSize : 50;

        var topics = await _forumService.GetActiveTopicsAsync(forumId, page - 1, pageSize);
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
    public async Task<ForumPageModel> PrepareForumPageModelAsync(Forum forum, int page)
    {
        ArgumentNullException.ThrowIfNull(forum);

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
            model.WatchForumText = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.WatchForum");

            var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(customer.Id, forum.Id, 0, 0, 1)).FirstOrDefault();
            if (forumSubscription != null)
                model.WatchForumText = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.UnwatchForum");
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
    public async Task<ForumTopicPageModel> PrepareForumTopicPageModelAsync(ForumTopic forumTopic, int page)
    {
        ArgumentNullException.ThrowIfNull(forumTopic);

        //load first post
        var firstPost = (await _forumService.GetAllPostsAsync(forumTopic.Id, 0, string.Empty, 0, 1))
            .FirstOrDefault();

        //load posts
        var posts = await _forumService.GetAllPostsAsync(forumTopic.Id, 0, string.Empty, page - 1, _forumSettings.PostsPageSize);

        //prepare model
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();

        var firstPostText = posts.FirstOrDefault()?.Text?.Replace(Environment.NewLine, string.Empty);

        var model = new ForumTopicPageModel
        {
            Id = forumTopic.Id,
            Subject = forumTopic.Subject,
            SeName = await _forumService.GetTopicSeNameAsync(forumTopic),

            IsCustomerAllowedToEditTopic = await _forumService.IsCustomerAllowedToEditTopicAsync(currentCustomer, forumTopic),
            IsCustomerAllowedToDeleteTopic = await _forumService.IsCustomerAllowedToDeleteTopicAsync(currentCustomer, forumTopic),
            IsCustomerAllowedToMoveTopic = await _forumService.IsCustomerAllowedToMoveTopicAsync(currentCustomer, forumTopic),
            IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(currentCustomer),

            MetaTitle = forumTopic.Subject,
            MetaDescription = CommonHelper.EnsureMaximumLength(firstPostText, _forumSettings.TopicMetaDescriptionLength, await _localizationService.GetResourceAsync("Plugins.Misc.Forums.TruncatePostfix"))
        };

        if (model.IsCustomerAllowedToSubscribe)
        {
            model.WatchTopicText = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.WatchTopic");

            var forumTopicSubscription = (await _forumService.GetAllSubscriptionsAsync(currentCustomer.Id, 0, forumTopic.Id, 0, 1))
                .FirstOrDefault();
            if (forumTopicSubscription != null)
                model.WatchTopicText = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.UnwatchTopic");
        }
        model.ForumEditor = _forumSettings.ForumEditor;
        model.PostsPageIndex = posts.PageIndex;
        model.PostsPageSize = posts.PageSize;
        model.PostsTotalRecords = posts.TotalCount;
        foreach (var post in posts)
        {
            var customer = await _customerService.GetCustomerByIdAsync(post.CustomerId);

            var customerIsGuest = await _customerService.IsGuestAsync(customer);
            var customerIsModerator = !customerIsGuest && await _forumService.IsForumModeratorAsync(customer);

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
                ForumPostCount = await _genericAttributeService.GetAttributeAsync<Customer, int>(post.CustomerId, ForumDefaults.ForumPostCountAttribute),
                ShowCustomersJoinDate = _customerSettings.ShowCustomersJoinDate && !customerIsGuest,
                CustomerJoinDate = customer?.CreatedOnUtc ?? DateTime.Now,
                AllowPrivateMessages = _privateMessageSettings.AllowPrivateMessages && !customerIsGuest,
                SignaturesEnabled = _forumSettings.SignaturesEnabled,
                FormattedSignature = _forumService
                    .FormatForumSignatureText(await _genericAttributeService.GetAttributeAsync<Customer, string>(post.CustomerId, ForumDefaults.SignatureAttribute)),
                //created on string
                PostCreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(post.CreatedOnUtc, DateTimeKind.Utc)
            };
            if (_forumSettings.RelativeDateTimeFormattingEnabled)
            {
                var languageCode = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;
                var postCreatedAgo = RelativeFormat(post.CreatedOnUtc, languageCode);
                forumPostModel.PostCreatedOnStr = string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postCreatedAgo);
            }
            else
            {
                forumPostModel.PostCreatedOnStr = forumPostModel.PostCreatedOn.ToString("f");
            }

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
                var country = await _countryService.GetCountryByIdAsync(customer.CountryId);
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

        if (_seoSettings.MicrodataEnabled)
        {
            var jsonLdModel = await PrepareJsonLdForumTopicAsync(forumTopic, firstPost, model);
            model.JsonLd = JsonConvert.SerializeObject(jsonLdModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        return model;
    }

    /// <summary>
    /// Prepare the topic move model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic move model
    /// </returns>
    public async Task<TopicMoveModel> PrepareTopicMoveAsync(ForumTopic forumTopic)
    {
        ArgumentNullException.ThrowIfNull(forumTopic);

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
    public async Task PrepareTopicCreateModelAsync(Forum forum, EditForumTopicModel model)
    {
        ArgumentNullException.ThrowIfNull(forum);

        ArgumentNullException.ThrowIfNull(model);

        var customer = await _workContext.GetCurrentCustomerAsync();
        model.IsEdit = false;
        model.ForumId = forum.Id;
        model.ForumName = forum.Name;
        model.ForumSeName = await _forumService.GetForumSeNameAsync(forum);
        model.ForumEditor = _forumSettings.ForumEditor;
        model.IsCustomerAllowedToSetTopicPriority = await _forumService.IsCustomerAllowedToSetTopicPriorityAsync(customer);
        model.TopicPriorities = (await ForumTopicType.Normal.ToSelectListAsync(false)).ToList();
        model.IsCustomerAllowedToSubscribe = await _forumService.IsCustomerAllowedToSubscribeAsync(customer);
        model.DisplayCaptcha = _captchaSettings.Enabled && _forumSettings.ShowCaptcha;
    }

    /// <summary>
    /// Prepare the forum topic edit model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <param name="model">Edit forum topic model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task PrepareTopicEditModelAsync(ForumTopic forumTopic, EditForumTopicModel model, bool excludeProperties)
    {
        ArgumentNullException.ThrowIfNull(forumTopic);

        ArgumentNullException.ThrowIfNull(model);

        var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId)
            ?? throw new ArgumentException("forum cannot be loaded");

        var customer = await _workContext.GetCurrentCustomerAsync();
        model.IsEdit = true;
        model.Id = forumTopic.Id;
        model.TopicPriorities = (await ForumTopicType.Normal.ToSelectListAsync(false)).ToList();
        model.ForumName = forum.Name;
        model.ForumSeName = await _forumService.GetForumSeNameAsync(forum);
        model.ForumId = forum.Id;
        model.ForumEditor = _forumSettings.ForumEditor;
        model.DisplayCaptcha = _captchaSettings.Enabled && _forumSettings.ShowCaptcha;
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
    public async Task<EditForumPostModel> PreparePostCreateModelAsync(ForumTopic forumTopic, int? quote, bool excludeProperties)
    {
        ArgumentNullException.ThrowIfNull(forumTopic);

        var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId)
            ?? throw new ArgumentException("forum cannot be loaded");

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
            DisplayCaptcha = _captchaSettings.Enabled && _forumSettings.ShowCaptcha
        };

        if (!excludeProperties)
        {
            //subscription            
            if (model.IsCustomerAllowedToSubscribe)
            {
                var forumSubscription = (await _forumService.GetAllSubscriptionsAsync(currentCustomer.Id, 0, forumTopic.Id, 0, 1))
                    .FirstOrDefault();
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
                    var username = await _customerService.FormatUsernameAsync(customer);
                    var quotePostText = quotePost.Text;

                    switch (_forumSettings.ForumEditor)
                    {
                        case EditorType.SimpleTextBox:
                            text = $"{username}:\n{quotePostText}\n";
                            break;
                        case EditorType.BBCodeEditor:
                            text = $"[quote={username}]{_bbCodeHelper.RemoveQuotes(quotePostText)}[/quote]";
                            break;
                        case EditorType.MarkdownEditor:
                            var quotedLines = quotePostText.Split('\n').Select(line => $"> {line}");
                            text = $"**{username}:**\n\n{string.Join("\n", quotedLines)}\n\n";
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
    public async Task<EditForumPostModel> PreparePostEditModelAsync(ForumPost forumPost, bool excludeProperties)
    {
        ArgumentNullException.ThrowIfNull(forumPost);

        var forumTopic = await _forumService.GetTopicByIdAsync(forumPost.TopicId)
            ?? throw new ArgumentException("forum topic cannot be loaded");

        var forum = await _forumService.GetForumByIdAsync(forumTopic.ForumId)
            ?? throw new ArgumentException("forum cannot be loaded");

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
            DisplayCaptcha = _captchaSettings.Enabled && _forumSettings.ShowCaptcha
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
    public async Task<SearchModel> PrepareSearchModelAsync(string searchterms, bool? advs, string forumId,
        string within, string limitDays, int page)
    {
        var model = new SearchModel();

        var pageSize = 10;

        // Create the values for the "Limit results to previous" select list
        model.LimitList = new()
        {
            new(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Search.LimitResultsToPrevious.AllResults"), "0"),
            new(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Search.LimitResultsToPrevious.1day"), "1"),
            new(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Search.LimitResultsToPrevious.7days"), "7"),
            new(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Search.LimitResultsToPrevious.2weeks"), "14"),
            new(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Search.LimitResultsToPrevious.1month"), "30"),
            new(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Search.LimitResultsToPrevious.3months"), "92"),
            new(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Search.LimitResultsToPrevious.6months"), "183"),
            new(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Search.LimitResultsToPrevious.1year"), "365")
        };

        // Create the values for the "Search in forum" select list
        var forumsSelectList = new List<SelectListItem>
        {
            new(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Search.SearchInForum.All"), "0", true)
        };

        var separator = "--";
        var forumGroups = await _forumService.GetAllForumGroupsAsync();
        foreach (var fg in forumGroups)
        {
            // Add the forum group with value as '-' so it can't be used as a target forum id
            forumsSelectList.Add(new(fg.Name, "-"));

            var forums = await _forumService.GetAllForumsByGroupIdAsync(fg.Id);
            foreach (var f in forums)
            {
                forumsSelectList.Add(new($"{separator}{f.Name}", f.Id.ToString()));
            }
        }
        model.ForumList = forumsSelectList;

        // Create the values for "Search within" select list
        model.WithinList = (await ForumSearchType.All.ToSelectListAsync(false)).ToList();

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
                    throw new NopException(string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.SearchTermMinimumLengthIsNCharacters"), searchTermMinimumLength));

                ForumSearchType searchWithin = 0;
                var limitResultsToPrevious = 0;
                if (advs.GetValueOrDefault())
                {
                    searchWithin = (ForumSearchType)withinSelected;
                    limitResultsToPrevious = limitDaysSelected;
                }

                if (_forumSettings.SearchResultsPageSize > 0)
                    pageSize = _forumSettings.SearchResultsPageSize;

                var topics = await _forumService
                    .GetAllTopicsAsync(forumIdSelected, 0, searchterms, searchWithin, limitResultsToPrevious, page - 1, pageSize);
                model.TopicPageSize = topics.PageSize;
                model.TopicTotalRecords = topics.TotalCount;
                model.TopicPageIndex = topics.PageIndex;
                foreach (var topic in topics)
                {
                    var topicModel = await PrepareForumTopicRowModelAsync(topic);
                    model.ForumTopics.Add(topicModel);
                }

                model.SearchResultsVisible = topics.Any();
                model.NoResultsVisisble = !model.SearchResultsVisible;

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
    public async Task<LastPostModel> PrepareLastPostModelAsync(ForumPost forumPost, bool showTopic)
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
            var postCreatedAgo = RelativeFormat(forumPost.CreatedOnUtc, languageCode);
            model.PostCreatedOnStr = string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postCreatedAgo);
        }
        else
        {
            model.PostCreatedOnStr = (await _dateTimeHelper.ConvertToUserTimeAsync(forumPost.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
        }

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
    public async Task<ForumBreadcrumbModel> PrepareForumBreadcrumbModelAsync(int? forumGroupId, int? forumId, int? forumTopicId)
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
    public async Task<CustomerForumSubscriptionsModel> PrepareCustomerForumSubscriptionsModelAsync(int? page)
    {
        var pageIndex = 0;
        if (page > 0)
            pageIndex = page.Value - 1;

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

            model.ForumSubscriptions.Add(new()
            {
                Id = forumSubscription.Id,
                ForumTopicId = forumTopicId,
                ForumId = forumSubscription.ForumId,
                TopicSubscription = topicSubscription,
                Title = title,
                Slug = slug,
            });
        }

        model.PagerModel = new(_localizationService)
        {
            PageSize = list.PageSize,
            TotalRecords = list.TotalCount,
            PageIndex = list.PageIndex,
            ShowTotalSummary = false,
            RouteActionName = ForumDefaults.Routes.Public.CUSTOMER_FORUM_SUBSCRIPTIONS,
            UseRouteLinks = true,
            RouteValues = new ForumSubscriptionsRouteValues { PageNumber = pageIndex }
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
    public async Task<ForumTopicRowModel> PrepareForumTopicRowModelAsync(ForumTopic topic)
    {
        ArgumentNullException.ThrowIfNull(topic);

        var customer = await _customerService.GetCustomerByIdAsync(topic.CustomerId);
        var firstPost = await _forumService.GetFirstPostAsync(topic);

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
            CustomerName = await _customerService.FormatUsernameAsync(customer),
            TotalPostPages = (topic.NumPosts / _forumSettings.PostsPageSize) + 1,
            Votes = firstPost?.VoteCount ?? 0
        };

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
    public async Task<ForumRowModel> PrepareForumRowModelAsync(Forum forum)
    {
        ArgumentNullException.ThrowIfNull(forum);

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

    /// <summary>
    /// Prepare the profile posts model
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="page">Number of posts page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the profile posts model
    /// </returns>
    public async Task<ProfilePostsModel> PrepareProfilePostsModelAsync(Customer customer, int page)
    {
        ArgumentNullException.ThrowIfNull(customer);

        if (page > 0)
            page -= 1;

        var pageSize = _forumSettings.LatestCustomerPostsPageSize;

        var list = await _forumService.GetAllPostsAsync(0, customer.Id, string.Empty, false, page, pageSize);

        var latestPosts = new List<PostsModel>();

        foreach (var forumPost in list)
        {
            string posted;
            if (_forumSettings.RelativeDateTimeFormattingEnabled)
            {
                var languageCode = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;
                var postedAgo = RelativeFormat(forumPost.CreatedOnUtc, languageCode);
                posted = string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postedAgo);
            }
            else
            {
                posted = (await _dateTimeHelper.ConvertToUserTimeAsync(forumPost.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
            }

            var topic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);

            latestPosts.Add(new()
            {
                ForumTopicId = topic.Id,
                ForumTopicTitle = topic.Subject,
                ForumTopicSlug = await _forumService.GetTopicSeNameAsync(topic),
                ForumPostText = _forumService.FormatPostText(forumPost),
                Posted = posted
            });
        }

        var pagerModel = new PagerModel(_localizationService)
        {
            PageSize = list.PageSize,
            TotalRecords = list.TotalCount,
            PageIndex = list.PageIndex,
            ShowTotalSummary = false,
            RouteActionName = ForumDefaults.Routes.Public.FORUM_PROFILE_POSTS_PAGED,
            UseRouteLinks = true,
            RouteValues = new SlugRouteValues { PageNumber = page, Id = customer.Id }
        };

        var model = new ProfilePostsModel
        {
            CustomerName = await _customerService.FormatUsernameAsync(customer),
            CustomerId = customer.Id,
            PagerModel = pagerModel,
            Posts = latestPosts,
        };

        return model;
    }

    /// <summary>
    /// Prepare forum account info model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum account info model
    /// </returns>
    public async Task<ForumAccountInfoModel> PrepareForumAccountInfoModelAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        return new()
        {
            Id = customer.Id,
            Signature = await _genericAttributeService.GetAttributeAsync<string>(customer, ForumDefaults.SignatureAttribute)
        };
    }

    #endregion

    #region Nested class

    /// <summary>
    /// record that has only page for route value. Used for (My Account) Forum Subscriptions pagination
    /// </summary>
    public record ForumSubscriptionsRouteValues : BaseRouteValues;

    /// <summary>
    /// record that has search options for route values. Used for Search result pagination
    /// </summary>
    public record ForumSearchRouteValues : BaseRouteValues
    {
        public string Searchterms { get; set; }
        public string Advs { get; set; }
        public string ForumId { get; set; }
        public string Within { get; set; }
        public string LimitDays { get; set; }
    }

    /// <summary>
    /// record that has only page for route value. Used for Active Discussions (forums) pagination
    /// </summary>
    public record ForumActiveDiscussionsRouteValues : BaseRouteValues;

    #endregion
}