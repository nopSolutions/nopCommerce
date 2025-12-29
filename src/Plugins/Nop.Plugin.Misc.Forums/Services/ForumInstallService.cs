using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Plugin.Misc.Forums.Domain;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.Forums.Services;

/// <summary>
/// Plugin installation service
/// </summary>
public class ForumInstallService
{
    #region Fields

    private readonly EmailAccountSettings _emailAccountSettings;
    private readonly ForumService _forumService;
    private readonly ICustomerService _customerService;
    private readonly IEmailAccountService _emailAccountService;
    private readonly ILocalizationService _localizationService;
    private readonly IMessageTemplateService _messageTemplateService;
    private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
    private readonly IRepository<PermissionRecord> _permissionRepository;
    private readonly IRepository<PermissionRecordCustomerRoleMapping> _permissionMappingRepository;
    private readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public ForumInstallService(EmailAccountSettings emailAccountSettings,
        ForumService forumService,
        ICustomerService customerService,
        IEmailAccountService emailAccountService,
        ILocalizationService localizationService,
        IMessageTemplateService messageTemplateService,
        IRepository<ActivityLogType> activityLogTypeRepository,
        IRepository<PermissionRecord> permissionRepository,
        IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository,
        ISettingService settingService)
    {
        _emailAccountSettings = emailAccountSettings;
        _forumService = forumService;
        _customerService = customerService;
        _emailAccountService = emailAccountService;
        _localizationService = localizationService;
        _messageTemplateService = messageTemplateService;
        _activityLogTypeRepository = activityLogTypeRepository;
        _permissionRepository = permissionRepository;
        _permissionMappingRepository = permissionRecordCustomerRoleMappingRepository;
        _settingService = settingService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Initialize settings
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task InsertSettingsAsync()
    {
        var forumSettings = await _settingService.LoadSettingAsync<ForumSettings>();

        if (!await _settingService.SettingExistsAsync(forumSettings, s => s.ForumsEnabled))
        {
            await _settingService.SaveSettingAsync(new ForumSettings
            {
                ForumsEnabled = true,
                RelativeDateTimeFormattingEnabled = true,
                AllowCustomersToDeletePosts = false,
                AllowCustomersToEditPosts = false,
                AllowCustomersToManageSubscriptions = false,
                AllowGuestsToCreatePosts = false,
                AllowGuestsToCreateTopics = false,
                AllowPostVoting = true,
                MaxVotesPerDay = 30,
                TopicSubjectMaxLength = 450,
                PostMaxLength = 4000,
                StrippedTopicMaxLength = 45,
                TopicsPageSize = 10,
                PostsPageSize = 10,
                SearchResultsPageSize = 10,
                ActiveDiscussionsPageSize = 50,
                LatestCustomerPostsPageSize = 10,
                ShowCustomersPostCount = true,
                ForumEditor = EditorType.MarkdownEditor,
                SignaturesEnabled = true,
                ForumSubscriptionsPageSize = 10,
                HomepageActiveDiscussionsTopicCount = 5,
                ActiveDiscussionsFeedEnabled = false,
                ActiveDiscussionsFeedCount = 25,
                ForumFeedsEnabled = false,
                ForumFeedCount = 10,
                ForumSearchTermMinimumLength = 3,
                TopicMetaDescriptionLength = 160,
                ShowCaptcha = false
            });
        }
        else
        {
            var showCaptchaOnForums = await _settingService.GetSettingAsync($"{nameof(CaptchaSettings)}.ShowOnForum");
            if (showCaptchaOnForums is not null)
            {
                forumSettings.ShowCaptcha = CommonHelper.To<bool>(showCaptchaOnForums.Value);
                await _settingService.SaveSettingAsync(forumSettings, settings => settings.ShowCaptcha, clearCache: false);
                await _settingService.DeleteSettingAsync(showCaptchaOnForums);
            }
        }
    }

    /// <summary>
    /// Add or update locales
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task InsertLocalesAsync()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Admin.ContentManagement.MessageTemplates.Description.Forums.NewForumPost"] = "This message template is used when a new forum post in certain forum topic is created. The message is received by a store owner.",
            ["Admin.ContentManagement.MessageTemplates.Description.Forums.NewForumTopic"] = "This message template is used when a new forum topic is created. The message is received by a store owner.",

            ["Enums.Nop.Plugin.Misc.Forums.Domain.EditorType.BBCodeEditor"] = "BBCode editor",
            ["Enums.Nop.Plugin.Misc.Forums.Domain.EditorType.MarkdownEditor"] = "Markdown editor",
            ["Enums.Nop.Plugin.Misc.Forums.Domain.EditorType.SimpleTextBox"] = "Simple textbox",
            ["Enums.Nop.Plugin.Misc.Forums.Domain.ForumSearchType.All"] = "Topic titles and post text",
            ["Enums.Nop.Plugin.Misc.Forums.Domain.ForumSearchType.PostTextOnly"] = "Post text only",
            ["Enums.Nop.Plugin.Misc.Forums.Domain.ForumSearchType.TopicTitlesOnly"] = "Topic titles only",
            ["Enums.Nop.Plugin.Misc.Forums.Domain.ForumTopicType.Announcement"] = "Announcement",
            ["Enums.Nop.Plugin.Misc.Forums.Domain.ForumTopicType.Normal"] = "Normal",
            ["Enums.Nop.Plugin.Misc.Forums.Domain.ForumTopicType.Sticky"] = "Sticky",

            ["Plugins.Misc.Forums.Account.Forum"] = "Forum",
            ["Plugins.Misc.Forums.Account.ForumSubscriptions"] = "Forum subscriptions",
            ["Plugins.Misc.Forums.Account.ForumSubscriptions.DeleteSelected"] = "Delete Selected",
            ["Plugins.Misc.Forums.Account.ForumSubscriptions.Description"] = "You will receive an email when a new forum topic/post is created.",
            ["Plugins.Misc.Forums.Account.ForumSubscriptions.InfoColumn"] = "Forum/Topic",
            ["Plugins.Misc.Forums.Account.ForumSubscriptions.NoSubscriptions"] = "You are not currently subscribed to any forums",
            ["Plugins.Misc.Forums.Account.ForumSubscriptions.PageTitle"] = "Forum Subscriptions",
            ["Plugins.Misc.Forums.Account.Signature"] = "Signature",
            ["Plugins.Misc.Forums.Account.Signature.Saved"] = "The signature updated successfully.",

            ["Plugins.Misc.Forums.ActivityLog.AddForumTopic"] = "Added a new forum topic (ID = {0})",
            ["Plugins.Misc.Forums.ActivityLog.EditForumTopic"] = "Edited a forum topic (ID = {0})",
            ["Plugins.Misc.Forums.ActivityLog.DeleteForumTopic"] = "Deleted a forum topic (ID = {0})",
            ["Plugins.Misc.Forums.ActivityLog.AddForumPost"] = "Added a new forum post (ID = {0})",
            ["Plugins.Misc.Forums.ActivityLog.EditForumPost"] = "Edited a forum post (ID = {0})",
            ["Plugins.Misc.Forums.ActivityLog.DeleteForumPost"] = "Deleted a forum post (ID = {0})",

            ["Plugins.Misc.Forums.Admin.Documentation.Reference.Forums"] = "Learn more about <a target=\"_blank\" href=\"{0}\">forums</a>",

            ["Plugins.Misc.Forums.Configuration.ActiveDiscussionsFeedCount"] = "Active discussions feed count",
            ["Plugins.Misc.Forums.Configuration.ActiveDiscussionsFeedCount.Hint"] = "The count of topics to include in the Active Discussion feed.",
            ["Plugins.Misc.Forums.Configuration.ActiveDiscussionsFeedEnabled"] = "Active discussions feed enabled",
            ["Plugins.Misc.Forums.Configuration.ActiveDiscussionsFeedEnabled.Hint"] = "Enables RSS feed for Active Discussions topics.",
            ["Plugins.Misc.Forums.Configuration.ActiveDiscussionsPageSize"] = "Active discussions page size",
            ["Plugins.Misc.Forums.Configuration.ActiveDiscussionsPageSize.Hint"] = "Set the page size for active discussions page e.g. '10' results per page.",
            ["Plugins.Misc.Forums.Configuration.AllowCustomersToDeletePosts"] = "Allow customers to delete posts",
            ["Plugins.Misc.Forums.Configuration.AllowCustomersToDeletePosts.Hint"] = "A value indicating whether customers are allowed to delete posts that they created.",
            ["Plugins.Misc.Forums.Configuration.AllowCustomersToEditPosts"] = "Allow customers to edit posts",
            ["Plugins.Misc.Forums.Configuration.AllowCustomersToEditPosts.Hint"] = "A value indicating whether customers are allowed to edit posts that they created.",
            ["Plugins.Misc.Forums.Configuration.AllowCustomersToManageSubscriptions"] = "Allow customers to manage forum subscriptions",
            ["Plugins.Misc.Forums.Configuration.AllowCustomersToManageSubscriptions.Hint"] = "Check if you want to allow customers to manage forum subscriptions.",
            ["Plugins.Misc.Forums.Configuration.AllowGuestsToCreatePosts"] = "Allow guests to create posts",
            ["Plugins.Misc.Forums.Configuration.AllowGuestsToCreatePosts.Hint"] = "Set if you want to allow guests to create posts.",
            ["Plugins.Misc.Forums.Configuration.AllowGuestsToCreateTopics"] = "Allow guests to create topics",
            ["Plugins.Misc.Forums.Configuration.AllowGuestsToCreateTopics.Hint"] = "Set if you want to allow guests to create topics.",
            ["Plugins.Misc.Forums.Configuration.AllowPostVoting"] = "Allow users to vote for posts",
            ["Plugins.Misc.Forums.Configuration.AllowPostVoting.Hint"] = "Set if you want to allow users to vote for posts.",
            ["Plugins.Misc.Forums.Configuration.BlockTitle.Common"] = "Common",
            ["Plugins.Misc.Forums.Configuration.BlockTitle.Feeds"] = "Feeds",
            ["Plugins.Misc.Forums.Configuration.BlockTitle.PageSizes"] = "Page sizes",
            ["Plugins.Misc.Forums.Configuration.BlockTitle.Permissions"] = "Permissions",
            ["Plugins.Misc.Forums.Configuration.ForumEditor"] = "Forum editor",
            ["Plugins.Misc.Forums.Configuration.ForumEditor.Hint"] = "Forum editor type. WARNING: not recommended to change in production environment.",
            ["Plugins.Misc.Forums.Configuration.ForumFeedCount"] = "Forum feed count",
            ["Plugins.Misc.Forums.Configuration.ForumFeedCount.Hint"] = "The count of topics to include in the forum feed.",
            ["Plugins.Misc.Forums.Configuration.ForumFeedsEnabled"] = "Forum feeds enabled",
            ["Plugins.Misc.Forums.Configuration.ForumFeedsEnabled.Hint"] = "Enables RSS feed for each forum.",
            ["Plugins.Misc.Forums.Configuration.ForumsEnabled"] = "Forums enabled",
            ["Plugins.Misc.Forums.Configuration.ForumsEnabled.Hint"] = "Check to enable forums.",
            ["Plugins.Misc.Forums.Configuration.MaxVotesPerDay"] = "Maximum votes per day",
            ["Plugins.Misc.Forums.Configuration.MaxVotesPerDay.Hint"] = "Maximum number of votes for user per day.",
            ["Plugins.Misc.Forums.Configuration.PostsPageSize"] = "Posts page size",
            ["Plugins.Misc.Forums.Configuration.PostsPageSize.Hint"] = "Set the page size for posts in topics e.g. '10' posts per page.",
            ["Plugins.Misc.Forums.Configuration.RelativeDateTimeFormattingEnabled"] = "Relative date and time formatting",
            ["Plugins.Misc.Forums.Configuration.RelativeDateTimeFormattingEnabled.Hint"] = "Click to enable relative date and time formatting (e.g. 2 hours ago, a month ago).",
            ["Plugins.Misc.Forums.Configuration.SearchResultsPageSize"] = "Search results page size",
            ["Plugins.Misc.Forums.Configuration.SearchResultsPageSize.Hint"] = "Set the page size for search results e.g. '10' results per page.",
            ["Plugins.Misc.Forums.Configuration.ShowCustomersPostCount"] = "Show customers post count",
            ["Plugins.Misc.Forums.Configuration.ShowCustomersPostCount.Hint"] = "A value indicating whether to show customers post count.",
            ["Plugins.Misc.Forums.Configuration.SignaturesEnabled"] = "Signature enabled",
            ["Plugins.Misc.Forums.Configuration.SignaturesEnabled.Hint"] = "Add an opportunity for customers to specify signature. Signature will be displayed below each forum post.",
            ["Plugins.Misc.Forums.Configuration.TopicsPageSize"] = "Topics page size",
            ["Plugins.Misc.Forums.Configuration.TopicsPageSize.Hint"] = "Set the page size for topics in forums e.g. '10' topics per page.",
            ["Plugins.Misc.Forums.Configuration.ShowCaptcha"] = "Show CAPTCHA on forum",
            ["Plugins.Misc.Forums.Configuration.ShowCaptcha.Hint"] = "Check to show CAPTCHA on forum, when editing and creating a topic or post.",
            ["Plugins.Misc.Forums.Configuration.ShowCaptcha.Warning"] = "Don't forget to enable CAPTCHA in the <a href=\"{0}\" target=\"_blank\">General settings</a> for correct working.",

            ["Plugins.Misc.Forums.ActiveDiscussions"] = "Active discussions",
            ["Plugins.Misc.Forums.ActiveDiscussions.ViewAll"] = "View all",
            ["Plugins.Misc.Forums.ActiveDiscussionsFeedDescription"] = "Topics with active discussions.",
            ["Plugins.Misc.Forums.ActiveDiscussionsFeedTitle"] = "{0} - Active Discussions",
            ["Plugins.Misc.Forums.ActiveDiscussionsRSSLinkTitle"] = "Click here to be informed automatically of active discussion topics",
            ["Plugins.Misc.Forums.AdvancedSearch"] = "Advanced search",
            ["Plugins.Misc.Forums.Announcement"] = "Announcement",
            ["Plugins.Misc.Forums.Author"] = "Author",
            ["Plugins.Misc.Forums.Breadcrumb.ForumGroupTitle"] = "Forum Group",
            ["Plugins.Misc.Forums.Breadcrumb.ForumHomeTitle"] = "Forum Home Page",
            ["Plugins.Misc.Forums.Breadcrumb.ForumTitle"] = "Forum",
            ["Plugins.Misc.Forums.Breadcrumb.HomeTitle"] = "Home Page",
            ["Plugins.Misc.Forums.Breadcrumb.TopicTitle"] = "Topic",
            ["Plugins.Misc.Forums.By"] = "By",
            ["Plugins.Misc.Forums.Cancel"] = "Cancel",
            ["Plugins.Misc.Forums.DeletePost"] = "Delete Post",
            ["Plugins.Misc.Forums.DeleteTopic"] = "Delete Topic",
            ["Plugins.Misc.Forums.EditPost"] = "Edit Post",
            ["Plugins.Misc.Forums.EditTopic"] = "Edit Topic",
            ["Plugins.Misc.Forums.ForumFeedDescription"] = "Forum's topics with newest posts.",
            ["Plugins.Misc.Forums.ForumFeedTitle"] = "{0} - Forum: {1}",
            ["Plugins.Misc.Forums.ForumName"] = "Forum Name",
            ["Plugins.Misc.Forums.ForumRSSLinkTitle"] = "Click here to be informed automatically of new posts and topics in the forum",
            ["Plugins.Misc.Forums.Forums"] = "Forums",
            ["Plugins.Misc.Forums.Home"] = "Home",
            ["Plugins.Misc.Forums.In"] = "In",
            ["Plugins.Misc.Forums.Joined"] = "Joined",
            ["Plugins.Misc.Forums.LatestPost"] = "Latest Post",
            ["Plugins.Misc.Forums.Location"] = "Location",
            ["Plugins.Misc.Forums.MarkdownEditor.TabWrite"] = "Write",
            ["Plugins.Misc.Forums.MarkdownEditor.TabPreview"] = "Preview",
            ["Plugins.Misc.Forums.Moderator"] = "Moderator",
            ["Plugins.Misc.Forums.MoveTopic"] = "Move Topic",
            ["Plugins.Misc.Forums.NewPost"] = "New Post",
            ["Plugins.Misc.Forums.NewTopic"] = "New Topic",
            ["Plugins.Misc.Forums.NoPosts"] = "No Posts",
            ["Plugins.Misc.Forums.Normal"] = "Normal",
            ["Plugins.Misc.Forums.NotifyWhenSomeonePostsInThisTopic"] = "Notify me via email when someone posts in this topic",
            ["Plugins.Misc.Forums.PageTitle.ActiveDiscussions"] = "Active discussions",
            ["Plugins.Misc.Forums.PageTitle.Default"] = "Forums",
            ["Plugins.Misc.Forums.PageTitle.MoveTopic"] = "Move Topic",
            ["Plugins.Misc.Forums.PageTitle.PostEdit"] = "Edit Post",
            ["Plugins.Misc.Forums.PageTitle.PostNew"] = "New Post",
            ["Plugins.Misc.Forums.PageTitle.Search"] = "Search Forums",
            ["Plugins.Misc.Forums.PageTitle.TopicEdit"] = "Edit Topic",
            ["Plugins.Misc.Forums.PageTitle.TopicNew"] = "New Topic",
            ["Plugins.Misc.Forums.Post.IsNotUseful"] = "This post/answer is not useful",
            ["Plugins.Misc.Forums.Post.IsUseful"] = "This post/answer is useful",
            ["Plugins.Misc.Forums.Post.Text"] = "Post",
            ["Plugins.Misc.Forums.Posted"] = "Posted",
            ["Plugins.Misc.Forums.PostLinkTitle"] = "Link to this post",
            ["Plugins.Misc.Forums.Posts"] = "Posts",
            ["Plugins.Misc.Forums.Priority"] = "Priority",
            ["Plugins.Misc.Forums.PrivateMessages.PM"] = "PM",
            ["Plugins.Misc.Forums.Profile.Posts"] = "Forum posts",
            ["Plugins.Misc.Forums.Profile.Posts.By"] = "Posts by {0}",
            ["Plugins.Misc.Forums.QuotePost"] = "Quote",
            ["Plugins.Misc.Forums.Replies"] = "Replies",
            ["Plugins.Misc.Forums.Reply"] = "Reply",
            ["Plugins.Misc.Forums.RSS"] = "RSS",
            ["Plugins.Misc.Forums.Search"] = "Search Forums",
            ["Plugins.Misc.Forums.Search.AdvancedSearch"] = "Advanced search",
            ["Plugins.Misc.Forums.Search.LimitResultsToPrevious"] = "Limit results to previous:",
            ["Plugins.Misc.Forums.Search.LimitResultsToPrevious.1day"] = "1 day",
            ["Plugins.Misc.Forums.Search.LimitResultsToPrevious.1month"] = "1 month",
            ["Plugins.Misc.Forums.Search.LimitResultsToPrevious.1year"] = "1 year",
            ["Plugins.Misc.Forums.Search.LimitResultsToPrevious.2weeks"] = "2 weeks",
            ["Plugins.Misc.Forums.Search.LimitResultsToPrevious.3months"] = "3 months",
            ["Plugins.Misc.Forums.Search.LimitResultsToPrevious.6months"] = "6 months",
            ["Plugins.Misc.Forums.Search.LimitResultsToPrevious.7days"] = "7 days",
            ["Plugins.Misc.Forums.Search.LimitResultsToPrevious.AllResults"] = "All results",
            ["Plugins.Misc.Forums.Search.SearchInForum"] = "Search in forum:",
            ["Plugins.Misc.Forums.Search.SearchInForum.All"] = "All forums",
            ["Plugins.Misc.Forums.Search.SearchKeyword"] = "Search keyword:",
            ["Plugins.Misc.Forums.Search.SearchWithin"] = "Search within:",
            ["Plugins.Misc.Forums.Search.SearchWithin.All"] = "Topic titles and post text",
            ["Plugins.Misc.Forums.Search.SearchWithin.PostTextOnly"] = "Post text only",
            ["Plugins.Misc.Forums.Search.SearchWithin.TopicTitlesOnly"] = "Topic titles only",
            ["Plugins.Misc.Forums.SearchButton"] = "Search",
            ["Plugins.Misc.Forums.SearchForumsTooltip"] = "Search forums",
            ["Plugins.Misc.Forums.SearchNoResultsText"] = "No posts were found that matched your criteria.",
            ["Plugins.Misc.Forums.SearchTermMinimumLengthIsNCharacters"] = "Search term minimum length is {0} characters",
            ["Plugins.Misc.Forums.SelectTheForumToMoveTopic"] = "Select the forum you want to move the post to",
            ["Plugins.Misc.Forums.Status"] = "Status",
            ["Plugins.Misc.Forums.Sticky"] = "Sticky",
            ["Plugins.Misc.Forums.Submit"] = "Submit",
            ["Plugins.Misc.Forums.TextCannotBeEmpty"] = "Text cannot be empty",
            ["Plugins.Misc.Forums.Topics"] = "Topics",
            ["Plugins.Misc.Forums.Topics.GotoPostPager"] = "[Go to page: {0}]",
            ["Plugins.Misc.Forums.TopicSubjectCannotBeEmpty"] = "Topic subject can not be empty",
            ["Plugins.Misc.Forums.TopicTitle"] = "Topic Title",
            ["Plugins.Misc.Forums.TruncatePostfix"] = "...",
            ["Plugins.Misc.Forums.TotalPosts"] = "Total Posts",
            ["Plugins.Misc.Forums.UnwatchForum"] = "Unwatch Forum",
            ["Plugins.Misc.Forums.UnwatchTopic"] = "Unwatch Topic",
            ["Plugins.Misc.Forums.Views"] = "Views",
            ["Plugins.Misc.Forums.Votes"] = "Votes",
            ["Plugins.Misc.Forums.Votes.AlreadyVoted"] = "You already voted for this post",
            ["Plugins.Misc.Forums.Votes.Login"] = "You need to log in to vote for post",
            ["Plugins.Misc.Forums.Votes.MaxVotesReached"] = "A maximum of {0} votes can be cast per user per day",
            ["Plugins.Misc.Forums.Votes.OwnPost"] = "You cannot vote for your own post",
            ["Plugins.Misc.Forums.WatchForum"] = "Watch Forum",
            ["Plugins.Misc.Forums.WatchTopic"] = "Watch Topic",
            ["Plugins.Misc.Forums.WelcomeMesage"] = "<p>Put your welcome message here. You can edit this in the admin site.</p>",

            ["Plugins.Misc.Forums"] = "Forums",
            ["Plugins.Misc.Forums.Forum"] = "Forum",
            ["Plugins.Misc.Forums.Forum.Added"] = "The new forum has been added successfully.",
            ["Plugins.Misc.Forums.Forum.BackToList"] = "back to forum list",
            ["Plugins.Misc.Forums.Forum.CreateForum"] = "Add New Forum",
            ["Plugins.Misc.Forums.Forum.Deleted"] = "The forum has been deleted successfully.",
            ["Plugins.Misc.Forums.Forum.EditForum"] = "Edit forum details",
            ["Plugins.Misc.Forums.Forum.Fields.CreatedOn"] = "Created on",
            ["Plugins.Misc.Forums.Forum.Fields.Description"] = "Description",
            ["Plugins.Misc.Forums.Forum.Fields.Description.Hint"] = "The description of the forum. This is the description that the customer will see.",
            ["Plugins.Misc.Forums.Forum.Fields.DisplayOrder"] = "Display Order",
            ["Plugins.Misc.Forums.Forum.Fields.DisplayOrder.Hint"] = "The display order of the forum. 1 represents the top of the list.",
            ["Plugins.Misc.Forums.Forum.Fields.ForumGroupId"] = "Forum Group",
            ["Plugins.Misc.Forums.Forum.Fields.ForumGroupId.Hint"] = "Choose a forum group.",
            ["Plugins.Misc.Forums.Forum.Fields.ForumGroupId.Required"] = "Forum group is required.",
            ["Plugins.Misc.Forums.Forum.Fields.Name"] = "Name",
            ["Plugins.Misc.Forums.Forum.Fields.Name.Hint"] = "The name of this forum. This is the name that the customer will see.",
            ["Plugins.Misc.Forums.Forum.Fields.Name.Required"] = "Forum name is required.",
            ["Plugins.Misc.Forums.Forum.Updated"] = "The forum has been updated successfully.",
            ["Plugins.Misc.Forums.ForumGroup"] = "Forum Group",
            ["Plugins.Misc.Forums.ForumGroup.Added"] = "The new forum group has been added successfully.",
            ["Plugins.Misc.Forums.ForumGroup.BackToList"] = "back to forum group list",
            ["Plugins.Misc.Forums.ForumGroup.CreateForumGroup"] = "Add New Forum Group",
            ["Plugins.Misc.Forums.ForumGroup.Deleted"] = "The forum group has been deleted successfully.",
            ["Plugins.Misc.Forums.ForumGroup.EditForumGroup"] = "Edit Forum Group Details",
            ["Plugins.Misc.Forums.ForumGroup.Fields.CreatedOn"] = "Created on",
            ["Plugins.Misc.Forums.ForumGroup.Fields.DisplayOrder"] = "Display Order",
            ["Plugins.Misc.Forums.ForumGroup.Fields.DisplayOrder.Hint"] = "The display order of the forum group. 1 represents the top of the list.",
            ["Plugins.Misc.Forums.ForumGroup.Fields.Name"] = "Name",
            ["Plugins.Misc.Forums.ForumGroup.Fields.Name.Hint"] = "The name of this forum group. This is the name that the customer will see.",
            ["Plugins.Misc.Forums.ForumGroup.Fields.Name.Required"] = "Forum Group Name is required.",
            ["Plugins.Misc.Forums.ForumGroup.Updated"] = "The forum group has been updated successfully.",

            ["Security.Permission.Forums.Manage"] = "Admin area. Forums. Create, edit, delete",
            ["Security.Permission.Forums.View"] = "Admin area. Forums. View",

            ["Plugins.Misc.Forums.Profile.LatestPosts"] = "Latest Posts",
            ["Plugins.Misc.Forums.Profile.LatestPosts.NoPosts"] = "No posts found",
        });
    }

    /// <summary>
    /// Add a message template for default email account (if it doesn't exist)
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task InsertMessageTemplateAsync()
    {
        var eaGeneral = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
            ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();

        if (eaGeneral is null)
            return;

        if (!(await _messageTemplateService.GetMessageTemplatesByNameAsync(ForumDefaults.NEW_FORUM_POST_MESSAGE)).Any())
        {
            await _messageTemplateService.InsertMessageTemplateAsync(new()
            {
                Name = ForumDefaults.NEW_FORUM_POST_MESSAGE,
                Subject = "%Store.Name%. New Post Notification.",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new post has been created in the topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Post author: %Forums.PostAuthor%{Environment.NewLine}<br />{Environment.NewLine}Post body: %Forums.PostBody%{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

        if (!(await _messageTemplateService.GetMessageTemplatesByNameAsync(ForumDefaults.NEW_FORUM_TOPIC_MESSAGE)).Any())
        {
            await _messageTemplateService.InsertMessageTemplateAsync(new()
            {
                Name = ForumDefaults.NEW_FORUM_TOPIC_MESSAGE,
                Subject = "%Store.Name%. New Topic Notification.",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> has been created at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }
    }

    /// <summary>
    /// Add activity log types
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task InserActivityLogTypesAsync()
    {
        var types = new List<string>
        {
            ForumDefaults.ActivityLogTypeSystemNames.AddForumTopic,
            ForumDefaults.ActivityLogTypeSystemNames.EditForumTopic,
            ForumDefaults.ActivityLogTypeSystemNames.DeleteForumTopic,
            ForumDefaults.ActivityLogTypeSystemNames.AddForumPost,
            ForumDefaults.ActivityLogTypeSystemNames.EditForumPost,
            ForumDefaults.ActivityLogTypeSystemNames.DeleteForumPost,
        };
        var existingActivityLogTypes = await _activityLogTypeRepository
            .GetAllAsync(query => query.Where(alt => types.Contains(alt.SystemKeyword, StringComparer.InvariantCultureIgnoreCase)));

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, ForumDefaults.ActivityLogTypeSystemNames.AddForumTopic, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = ForumDefaults.ActivityLogTypeSystemNames.AddForumTopic,
                Enabled = false,
                Name = "Public store. Add forum topic"
            });
        }

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, ForumDefaults.ActivityLogTypeSystemNames.EditForumTopic, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = ForumDefaults.ActivityLogTypeSystemNames.EditForumTopic,
                Enabled = false,
                Name = "Public store. Edit forum topic"
            });
        }

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, ForumDefaults.ActivityLogTypeSystemNames.DeleteForumTopic, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = ForumDefaults.ActivityLogTypeSystemNames.DeleteForumTopic,
                Enabled = false,
                Name = "Public store. Delete forum topic"
            });
        }

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, ForumDefaults.ActivityLogTypeSystemNames.AddForumPost, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = ForumDefaults.ActivityLogTypeSystemNames.AddForumPost,
                Enabled = false,
                Name = "Public store. Add forum post"
            });
        }

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, ForumDefaults.ActivityLogTypeSystemNames.EditForumPost, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = ForumDefaults.ActivityLogTypeSystemNames.EditForumPost,
                Enabled = false,
                Name = "Public store. Edit forum post"
            });
        }

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, ForumDefaults.ActivityLogTypeSystemNames.DeleteForumPost, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = ForumDefaults.ActivityLogTypeSystemNames.DeleteForumPost,
                Enabled = false,
                Name = "Public store. Delete forum post"
            });
        }
    }

    /// <summary>
    /// Create permission records (if it doesn't exist)
    /// </summary>
    /// <param name="oldSystemName">Old name of the permission record</param>
    /// <param name="newSystemName">New name of the permission record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task UpdatePermissionMappingsAsync(string oldSystemName, string newSystemName)
    {
        ArgumentException.ThrowIfNullOrEmpty(oldSystemName);
        ArgumentException.ThrowIfNullOrEmpty(newSystemName);

        if (_permissionRepository.Table.FirstOrDefault(pr => pr.SystemName == oldSystemName) is PermissionRecord oldRec)
        {
            var roleIds = _permissionMappingRepository.Table
                .Where(pm => pm.PermissionRecordId == oldRec.Id)
                .Select(pm => pm.CustomerRoleId)
                .ToArray();

            if (roleIds.Any() && _permissionRepository.Table.FirstOrDefault(pr => pr.SystemName == newSystemName) is PermissionRecord newRec)
            {
                foreach (var roleId in roleIds)
                {
                    try
                    {
                        await _permissionMappingRepository.InsertAsync(new PermissionRecordCustomerRoleMapping
                        {
                            CustomerRoleId = roleId,
                            PermissionRecordId = newRec.Id
                        });
                    }
                    catch
                    {
                        //exist
                    }
                }

                await _permissionMappingRepository.DeleteAsync(pr => pr.PermissionRecordId == oldRec.Id);
                await _permissionRepository.DeleteAsync(pr => pr.Id == oldRec.Id);
            }
        }
    }

    /// <summary>
    /// Update permission record names
    /// </summary>
    /// <returns></returns>
    private async Task PreparePermissionMappingsAsync()
    {
        await UpdatePermissionMappingsAsync("ContentManagement.ForumsView", ForumDefaults.Permissions.FORUMS_VIEW);
        await UpdatePermissionMappingsAsync("ContentManagement.ForumsCreateEditDelete", ForumDefaults.Permissions.FORUMS_MANAGE);
    }

    /// <summary>
    /// Configure moderator role
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task ConfigureModeratorRoleAsync()
    {
        var moderatorRole = await _customerService.GetCustomerRoleBySystemNameAsync(ForumDefaults.ForumModeratorsRoleName);

        if (moderatorRole is null)
        {
            moderatorRole = new CustomerRole
            {
                Name = "Forum Moderators",
                Active = true,
                IsSystemRole = true,
                SystemName = ForumDefaults.ForumModeratorsRoleName
            };

            await _customerService.InsertCustomerRoleAsync(moderatorRole);

            var adminRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName);
            if (adminRole is null)
                return;

            var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: [adminRole.Id]);

            foreach (var customer in customers)
                await _customerService.AddCustomerRoleMappingAsync(new() { CustomerRoleId = moderatorRole.Id, CustomerId = customer.Id });
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds the necessary data for the plugin to work correctly
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InstallRequiredDataAsync()
    {
        await ConfigureModeratorRoleAsync();

        await InsertSettingsAsync();

        await InsertLocalesAsync();

        await InsertMessageTemplateAsync();

        await InserActivityLogTypesAsync();

        await PreparePermissionMappingsAsync();
    }

    /// <summary>
    /// Removes the data inserted in <see cref="InstallRequiredDataAsync"/>
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UninstallRequiredDataAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<ForumSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Enums.Nop.Plugin.Misc.Forums.Domain.");
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.Forums.");
        await _localizationService.DeleteLocaleResourcesAsync("Security.Permission.Forums.");
        await _localizationService.DeleteLocaleResourcesAsync("Admin.ContentManagement.MessageTemplates.Description.Forums.");

        //message templates
        foreach (var mt in await _messageTemplateService.GetMessageTemplatesByNameAsync(ForumDefaults.NEW_FORUM_TOPIC_MESSAGE))
            await _messageTemplateService.DeleteMessageTemplateAsync(mt);

        foreach (var mt in await _messageTemplateService.GetMessageTemplatesByNameAsync(ForumDefaults.NEW_FORUM_POST_MESSAGE))
            await _messageTemplateService.DeleteMessageTemplateAsync(mt);

        //activity log type
        await _activityLogTypeRepository.DeleteAsync(at => at.SystemKeyword == ForumDefaults.ActivityLogTypeSystemNames.AddForumPost);
        await _activityLogTypeRepository.DeleteAsync(at => at.SystemKeyword == ForumDefaults.ActivityLogTypeSystemNames.EditForumPost);
        await _activityLogTypeRepository.DeleteAsync(at => at.SystemKeyword == ForumDefaults.ActivityLogTypeSystemNames.DeleteForumPost);
        await _activityLogTypeRepository.DeleteAsync(at => at.SystemKeyword == ForumDefaults.ActivityLogTypeSystemNames.AddForumTopic);
        await _activityLogTypeRepository.DeleteAsync(at => at.SystemKeyword == ForumDefaults.ActivityLogTypeSystemNames.EditForumTopic);
        await _activityLogTypeRepository.DeleteAsync(at => at.SystemKeyword == ForumDefaults.ActivityLogTypeSystemNames.DeleteForumTopic);

        //permission
        await _permissionRepository.DeleteAsync(record => record.SystemName == ForumDefaults.Permissions.FORUMS_VIEW
            || record.SystemName == ForumDefaults.Permissions.FORUMS_MANAGE);

    }

    /// <summary>
    /// Install sample forum data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InstallSampleDataAsync()
    {
        var forumGroup = new ForumGroup
        {
            Name = "General",
            DisplayOrder = 1,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };

        await _forumService.InsertForumGroupAsync(forumGroup);

        await _forumService.InsertForumAsync(new()
        {
            ForumGroupId = forumGroup.Id,
            Name = "New Products",
            Description = "Discuss new products and industry trends",
            NumTopics = 0,
            NumPosts = 0,
            LastPostCustomerId = 0,
            LastPostTime = null,
            DisplayOrder = 5,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        });

        await _forumService.InsertForumAsync(new()
        {
            ForumGroupId = forumGroup.Id,
            Name = "Mobile Devices Forum",
            Description = "Discuss the mobile phone market",
            NumTopics = 0,
            NumPosts = 0,
            LastPostCustomerId = 0,
            LastPostTime = null,
            DisplayOrder = 10,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        });

        await _forumService.InsertForumAsync(new()
        {
            ForumGroupId = forumGroup.Id,
            Name = "Packaging & Shipping",
            Description = "Discuss packaging & shipping",
            NumTopics = 0,
            NumPosts = 0,
            LastPostCustomerId = 0,
            LastPostTime = null,
            DisplayOrder = 15,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        });
    }

    #endregion
}