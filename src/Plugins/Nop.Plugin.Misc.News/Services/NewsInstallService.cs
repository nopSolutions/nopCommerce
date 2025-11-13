using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Data;
using Nop.Plugin.Misc.News.Domain;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Seo;

namespace Nop.Plugin.Misc.News.Services;

/// <summary>
/// Plugin installation service
/// </summary>
public class NewsInstallService
{
    #region Fields

    private readonly EmailAccountSettings _emailAccountSettings;
    private readonly ICustomerService _customerService;
    private readonly IEmailAccountService _emailAccountService;
    private readonly ILocalizationService _localizationService;
    private readonly IMessageTemplateService _messageTemplateService;
    private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
    private readonly IRepository<PermissionRecord> _permissionRepository;
    private readonly IRepository<PermissionRecordCustomerRoleMapping> _permissionMappingRepository;
    private readonly ISettingService _settingService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IWorkContext _workContext;
    private readonly NewsService _newsService;

    #endregion

    #region Ctor

    public NewsInstallService(EmailAccountSettings emailAccountSettings,
        ICustomerService customerService,
        IEmailAccountService emailAccountService,
        ILocalizationService localizationService,
        IMessageTemplateService messageTemplateService,
        IRepository<ActivityLogType> activityLogTypeRepository,
        IRepository<PermissionRecord> permissionRepository,
        IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository,
        ISettingService settingService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        NewsService newsService)
    {
        _emailAccountSettings = emailAccountSettings;
        _customerService = customerService;
        _emailAccountService = emailAccountService;
        _localizationService = localizationService;
        _messageTemplateService = messageTemplateService;
        _activityLogTypeRepository = activityLogTypeRepository;
        _permissionRepository = permissionRepository;
        _permissionMappingRepository = permissionRecordCustomerRoleMappingRepository;
        _settingService = settingService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _newsService = newsService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Initialize settings
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task InsertSettingsAsync()
    {
        var newsSettings = await _settingService.LoadSettingAsync<NewsSettings>();

        if (!await _settingService.SettingExistsAsync(newsSettings, s => s.Enabled))
        {
            await _settingService.SaveSettingAsync(new NewsSettings
            {
                Enabled = true,
                AllowNotRegisteredUsersToLeaveComments = true,
                NotifyAboutNewNewsComments = false,
                ShowNewsOnMainPage = true,
                MainPageNewsCount = 3,
                NewsArchivePageSize = 10,
                ShowHeaderRssUrl = false,
                NewsCommentsMustBeApproved = false,
                ShowCaptchaOnNewsCommentPage = false,
                SitemapIncludeNews = false,
                SitemapXmlIncludeNews = true,
                ShowNewsCommentsPerStore = false
            });
        }
        else
        {
            var sitemapIncludeNews = await _settingService.GetSettingAsync($"{nameof(SitemapSettings)}.SitemapIncludeNews");
            if (sitemapIncludeNews is not null)
            {
                newsSettings.SitemapIncludeNews = CommonHelper.To<bool>(sitemapIncludeNews.Value);
                await _settingService.SaveSettingAsync(newsSettings, settings => settings.SitemapIncludeNews, clearCache: false);
                await _settingService.DeleteSettingAsync(sitemapIncludeNews);
            }

            var sitemapXmlIncludeNews = await _settingService.GetSettingAsync($"{nameof(SitemapXmlSettings)}.SitemapXmlIncludeNews");
            if (sitemapXmlIncludeNews is not null)
            {
                newsSettings.SitemapXmlIncludeNews = CommonHelper.To<bool>(sitemapXmlIncludeNews.Value);
                await _settingService.SaveSettingAsync(newsSettings, settings => settings.SitemapXmlIncludeNews, clearCache: false);
                await _settingService.DeleteSettingAsync(sitemapXmlIncludeNews);
            }

            var showCaptchaOnNewsCommentPage = await _settingService.GetSettingAsync($"{nameof(CaptchaSettings)}.ShowOnNewsCommentPage");
            if (showCaptchaOnNewsCommentPage is not null)
            {
                newsSettings.ShowCaptchaOnNewsCommentPage = CommonHelper.To<bool>(showCaptchaOnNewsCommentPage.Value);
                await _settingService.SaveSettingAsync(newsSettings, settings => settings.ShowCaptchaOnNewsCommentPage, clearCache: false);
                await _settingService.DeleteSettingAsync(showCaptchaOnNewsCommentPage);
            }
        }

        var seoSettings = _settingService.LoadSetting<SeoSettings>();
        if (!seoSettings.ReservedUrlRecordSlugs.Contains(NewsDefaults.ReservedUrlRecord))
        {
            seoSettings.ReservedUrlRecordSlugs.Add(NewsDefaults.ReservedUrlRecord);
            _settingService.SaveSetting(seoSettings, settings => seoSettings.ReservedUrlRecordSlugs);
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
            ["Admin.ContentManagement.MessageTemplates.Description.News.NewsComment"] = "This message template is used when a new comment to the certain news item is created. The message is received by a store owner. You can set up this option by ticking the checkbox <strong>Notify about new news comments</strong> in Configuration - Settings - News settings.",
            ["Plugins.Misc.News.ActivityLog.AddNewNews"] = "Added a new news (ID = {0})",
            ["Plugins.Misc.News.ActivityLog.DeleteNews"] = "Deleted a news (ID = {0})",
            ["Plugins.Misc.News.ActivityLog.DeleteNewsComment"] = "Deleted a news comment (ID = {0})",
            ["Plugins.Misc.News.ActivityLog.EditNews"] = "Edited a news (ID = {0})",
            ["Plugins.Misc.News.ActivityLog.EditNewsComment"] = "Edited a news comment (ID = {0})",
            ["Plugins.Misc.News.ActivityLog.PublicStore.AddNewsComment"] = "Added a news comment",
            ["Plugins.Misc.News.Admin.Documentation.Reference.News"] = "Learn more about <a target=\"_blank\" href=\"{0}\">news</a>",
            ["Plugins.Misc.News.Configuration.AllowNotRegisteredUsersToLeaveComments"] = "Allow guests to leave comments",
            ["Plugins.Misc.News.Configuration.AllowNotRegisteredUsersToLeaveComments.Hint"] = "Check to allow guests to leave comments.",
            ["Plugins.Misc.News.Configuration.BlockTitle.Common"] = "Common",
            ["Plugins.Misc.News.Configuration.BlockTitle.NewsComments"] = "News comments",
            ["Plugins.Misc.News.Configuration.ShowCaptchaOnNewsCommentPage"] = "Show CAPTCHA on news page (comments)",
            ["Plugins.Misc.News.Configuration.ShowCaptchaOnNewsCommentPage.Hint"] = "Check to show CAPTCHA on news page when writing a comment.",
            ["Plugins.Misc.News.Configuration.ShowCaptchaOnNewsCommentPage.Warning"] = "Don't forget to enable CAPTCHA in the <a href=\"{0}\" target=\"_blank\">General settings</a> for correct working.",
            ["Plugins.Misc.News.Configuration.Enabled"] = "News enabled",
            ["Plugins.Misc.News.Configuration.Enabled.Hint"] = "Check to enable the news in your store.",
            ["Plugins.Misc.News.Configuration.MainPageNewsCount"] = "Number of items to display",
            ["Plugins.Misc.News.Configuration.MainPageNewsCount.Hint"] = "The number of news items to display on your home page.",
            ["Plugins.Misc.News.Configuration.NewsArchivePageSize"] = "News archive page size",
            ["Plugins.Misc.News.Configuration.NewsArchivePageSize.Hint"] = "A number of news displayed on one page.",
            ["Plugins.Misc.News.Configuration.NewsCommentsMustBeApproved"] = "News comments must be approved",
            ["Plugins.Misc.News.Configuration.NewsCommentsMustBeApproved.Hint"] = "Check if news comments must be approved by administrator.",
            ["Plugins.Misc.News.Configuration.NotifyAboutNewNewsComments"] = "Notify about new news comments",
            ["Plugins.Misc.News.Configuration.NotifyAboutNewNewsComments.Hint"] = "Check to notify store owner about new news comments.",
            ["Plugins.Misc.News.Configuration.ShowHeaderRSSUrl"] = "Display news RSS feed link in the browser address bar",
            ["Plugins.Misc.News.Configuration.ShowHeaderRSSUrl.Hint"] = "Check to enable the news RSS feed link in customers browser address bar.",
            ["Plugins.Misc.News.Configuration.ShowNewsCommentsPerStore"] = "News comments per store",
            ["Plugins.Misc.News.Configuration.ShowNewsCommentsPerStore.Hint"] = "Check to display news comments written in the current store only.",
            ["Plugins.Misc.News.Configuration.ShowNewsOnMainPage"] = "Show on home page",
            ["Plugins.Misc.News.Configuration.ShowNewsOnMainPage.Hint"] = "Check to display your news items on your store home page.",
            ["Plugins.Misc.News.Configuration.SitemapIncludeNews"] = "Sitemap includes news items",
            ["Plugins.Misc.News.Configuration.SitemapIncludeNews.Hint"] = "Check if you want to include news items in sitemap.",
            ["Plugins.Misc.News.Configuration.SitemapXmlIncludeNews"] = "sitemap.xml includes news items",
            ["Plugins.Misc.News.Configuration.SitemapXmlIncludeNews.Hint"] = "Check if you want to include news items in sitemap.xml file.",
            ["Plugins.Misc.News.Comments"] = "News comments",
            ["Plugins.Misc.News.Comments.CommentText"] = "Comment",
            ["Plugins.Misc.News.Comments.CommentText.Required"] = "Enter comment text",
            ["Plugins.Misc.News.Comments.CommentTitle"] = "Title",
            ["Plugins.Misc.News.Comments.CommentTitle.MaxLengthValidation"] = "Max length of comment title is {0} chars",
            ["Plugins.Misc.News.Comments.CommentTitle.Required"] = "Enter comment title",
            ["Plugins.Misc.News.Comments.CreatedOn"] = "Created on",
            ["Plugins.Misc.News.Comments.LeaveYourComment"] = "Leave your comment",
            ["Plugins.Misc.News.Comments.OnlyRegisteredUsersLeaveComments"] = "Only registered users can leave comments.",
            ["Plugins.Misc.News.Comments.SeeAfterApproving"] = "News comment is successfully added. You will see it after approving by a store administrator.",
            ["Plugins.Misc.News.Comments.SubmitButton"] = "New comment",
            ["Plugins.Misc.News.Comments.SuccessfullyAdded"] = "News comment is successfully added.",
            ["Plugins.Misc.News.Comments.ApproveSelected"] = "Approve selected",
            ["Plugins.Misc.News.Comments.DeleteSelected"] = "Delete selected",
            ["Plugins.Misc.News.Comments.DisapproveSelected"] = "Disapprove selected",
            ["Plugins.Misc.News.Comments.Fields.CommentText"] = "Comment text",
            ["Plugins.Misc.News.Comments.Fields.CommentTitle"] = "Comment title",
            ["Plugins.Misc.News.Comments.Fields.CreatedOn"] = "Created on",
            ["Plugins.Misc.News.Comments.Fields.Customer"] = "Customer",
            ["Plugins.Misc.News.Comments.Fields.IsApproved"] = "Is approved",
            ["Plugins.Misc.News.Comments.Fields.NewsItem"] = "News item",
            ["Plugins.Misc.News.Comments.Fields.StoreName"] = "Store name",
            ["Plugins.Misc.News.Comments.List.CreatedOnFrom"] = "Created from",
            ["Plugins.Misc.News.Comments.List.CreatedOnFrom.Hint"] = "The creation from date for the search.",
            ["Plugins.Misc.News.Comments.List.CreatedOnTo"] = "Created to",
            ["Plugins.Misc.News.Comments.List.CreatedOnTo.Hint"] = "The creation to date for the search.",
            ["Plugins.Misc.News.Comments.List.SearchApproved"] = "Approved",
            ["Plugins.Misc.News.Comments.List.SearchApproved.All"] = "All",
            ["Plugins.Misc.News.Comments.List.SearchApproved.ApprovedOnly"] = "Approved only",
            ["Plugins.Misc.News.Comments.List.SearchApproved.DisapprovedOnly"] = "Disapproved only",
            ["Plugins.Misc.News.Comments.List.SearchApproved.Hint"] = "Search by a \"Approved\" property.",
            ["Plugins.Misc.News.Comments.List.SearchText"] = "Message",
            ["Plugins.Misc.News.Comments.List.SearchText.Hint"] = "Search in title and comment text.",
            ["Plugins.Misc.News.NewsItems"] = "News items",
            ["Plugins.Misc.News.NewsArchive"] = "News archive",
            ["Plugins.Misc.News.NewsItems.Added"] = "The new news item has been added successfully.",
            ["Plugins.Misc.News.NewsItems.AddNew"] = "Add a new news item",
            ["Plugins.Misc.News.NewsItems.BackToList"] = "back to news item list",
            ["Plugins.Misc.News.NewsItems.Deleted"] = "The news item has been deleted successfully.",
            ["Plugins.Misc.News.NewsItems.EditNewsItemDetails"] = "Edit news item details",
            ["Plugins.Misc.News.NewsItems.Fields.AllowComments"] = "Allow comments",
            ["Plugins.Misc.News.NewsItems.Fields.AllowComments.Hint"] = "When checked, customers can leave comments about your news entry.",
            ["Plugins.Misc.News.NewsItems.Fields.Comments"] = "View comments",
            ["Plugins.Misc.News.NewsItems.Fields.CreatedOn"] = "Created on",
            ["Plugins.Misc.News.NewsItems.Fields.EndDate"] = "End date",
            ["Plugins.Misc.News.NewsItems.Fields.EndDate.Hint"] = "Set the news item end date in Coordinated Universal Time (UTC). You can also leave it empty.",
            ["Plugins.Misc.News.NewsItems.Fields.Full"] = "Full description",
            ["Plugins.Misc.News.NewsItems.Fields.Full.Hint"] = "The full description of this news entry.",
            ["Plugins.Misc.News.NewsItems.Fields.Full.Required"] = "Full description is required.",
            ["Plugins.Misc.News.NewsItems.Fields.Language"] = "Language",
            ["Plugins.Misc.News.NewsItems.Fields.Language.Hint"] = "The language of this news entry. A customer will only see the news entries for their selected language.",
            ["Plugins.Misc.News.NewsItems.Fields.LimitedToStores"] = "Limited to stores",
            ["Plugins.Misc.News.NewsItems.Fields.LimitedToStores.Hint"] = "Option to limit this news item to a certain store. If you have multiple stores, choose one or several from the list. If you don't use this option just leave this field empty.",
            ["Plugins.Misc.News.NewsItems.Fields.MetaDescription"] = "Meta description",
            ["Plugins.Misc.News.NewsItems.Fields.MetaDescription.Hint"] = "Meta description to be added to news page header.",
            ["Plugins.Misc.News.NewsItems.Fields.MetaKeywords"] = "Meta keywords",
            ["Plugins.Misc.News.NewsItems.Fields.MetaKeywords.Hint"] = "Meta keywords to be added to news page header.",
            ["Plugins.Misc.News.NewsItems.Fields.MetaTitle"] = "Meta title",
            ["Plugins.Misc.News.NewsItems.Fields.MetaTitle.Hint"] = "Override the page title. The default is the title of the news.",
            ["Plugins.Misc.News.NewsItems.Fields.Published"] = "Published",
            ["Plugins.Misc.News.NewsItems.Fields.Published.Hint"] = "Determines whether the news item is published (visible) in your store.",
            ["Plugins.Misc.News.NewsItems.Fields.SeName"] = "Search engine friendly page name",
            ["Plugins.Misc.News.NewsItems.Fields.SeName.Hint"] = "Set a search engine friendly page name e.g. 'the-best-news' to make your page URL 'http://www.yourStore.com/the-best-news'. Leave empty to generate it automatically based on the title of the news.",
            ["Plugins.Misc.News.NewsItems.Fields.Short"] = "Short description",
            ["Plugins.Misc.News.NewsItems.Fields.Short.Hint"] = "The short description of this news entry.",
            ["Plugins.Misc.News.NewsItems.Fields.Short.Required"] = "Short description is required.",
            ["Plugins.Misc.News.NewsItems.Fields.StartDate"] = "Start date",
            ["Plugins.Misc.News.NewsItems.Fields.StartDate.Hint"] = "Set the news item start date in Coordinated Universal Time (UTC). You can also leave it empty.",
            ["Plugins.Misc.News.NewsItems.Fields.Title"] = "Title",
            ["Plugins.Misc.News.NewsItems.Fields.Title.Hint"] = "The title of this news entry.",
            ["Plugins.Misc.News.NewsItems.Fields.Title.Required"] = "Title is required.",
            ["Plugins.Misc.News.NewsItems.Info"] = "Info",
            ["Plugins.Misc.News.NewsItems.List.SearchStore"] = "Store",
            ["Plugins.Misc.News.NewsItems.List.SearchStore.Hint"] = "Search by a specific store.",
            ["Plugins.Misc.News.NewsItems.List.SearchTitle"] = "Title",
            ["Plugins.Misc.News.NewsItems.List.SearchTitle.Hint"] = "Search by a news items title.",
            ["Plugins.Misc.News.NewsItems.Updated"] = "The news item has been updated successfully.",
            ["Plugins.Misc.News.Title"] = "News",
            ["Plugins.Misc.News.MoreInfo"] = "details",
            ["Plugins.Misc.News.RSS"] = "RSS",
            ["Plugins.Misc.News.RSS.Hint"] = "Click here to be informed automatically when we add new items to our site.",
            ["Plugins.Misc.News.ViewAll"] = "View News Archive",
            ["Plugins.Misc.News.PageTitle.NewsArchive"] = "News Archive",
            ["Security.Permission.News.CommentsManage"] = "Admin area. News comments. Create, edit, delete",
            ["Security.Permission.News.CommentsView"] = "Admin area. News comments. View",
            ["Security.Permission.News.Manage"] = "Admin area. News. Create, edit, delete",
            ["Security.Permission.News.View"] = "Admin area. News. View",
            ["Plugins.Misc.News.Sitemap.News"] = "News",
        });
    }

    /// <summary>
    /// Add a message template for default email account (if it doesn't exist)
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task InsertMessageTemplateAsync()
    {
        if ((await _messageTemplateService.GetMessageTemplatesByNameAsync(NewsDefaults.NewsCommentStoreOwnerNotification)).Any())
            return;

        var eaGeneral = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
            ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();

        if (eaGeneral is null)
            return;

        await _messageTemplateService.InsertMessageTemplateAsync(new()
        {
            Name = NewsDefaults.NewsCommentStoreOwnerNotification,
            Subject = "%Store.Name%. New news comment.",
            Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new news comment has been created for news \"%NewsComment.NewsTitle%\".{Environment.NewLine}</p>{Environment.NewLine}",
            IsActive = true,
            EmailAccountId = eaGeneral.Id
        });
    }

    /// <summary>
    /// Add activity log types
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task InserActivityLogTypesAsync()
    {
        var types = new List<string>
        {
            NewsDefaults.ActivityLogTypeSystemNames.AddNewNews,
            NewsDefaults.ActivityLogTypeSystemNames.DeleteNews,
            NewsDefaults.ActivityLogTypeSystemNames.EditNews,
            NewsDefaults.ActivityLogTypeSystemNames.DeleteNewsComment,
            NewsDefaults.ActivityLogTypeSystemNames.EditNewsComment,
            NewsDefaults.ActivityLogTypeSystemNames.PublicStoreAddNewsComment,
        };
        var existingActivityLogTypes = await _activityLogTypeRepository
            .GetAllAsync(query => query.Where(alt => types.Contains(alt.SystemKeyword, StringComparer.InvariantCultureIgnoreCase)));

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, NewsDefaults.ActivityLogTypeSystemNames.AddNewNews, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = NewsDefaults.ActivityLogTypeSystemNames.AddNewNews,
                Enabled = true,
                Name = "Add a new news"
            });
        }

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, NewsDefaults.ActivityLogTypeSystemNames.DeleteNews, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = NewsDefaults.ActivityLogTypeSystemNames.DeleteNews,
                Enabled = true,
                Name = "Delete a news"
            });
        }

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, NewsDefaults.ActivityLogTypeSystemNames.EditNews, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = NewsDefaults.ActivityLogTypeSystemNames.EditNews,
                Enabled = true,
                Name = "Edit a news"
            });
        }

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, NewsDefaults.ActivityLogTypeSystemNames.DeleteNewsComment, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = NewsDefaults.ActivityLogTypeSystemNames.DeleteNewsComment,
                Enabled = true,
                Name = "Delete a news comment"
            });
        }

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, NewsDefaults.ActivityLogTypeSystemNames.EditNewsComment, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = NewsDefaults.ActivityLogTypeSystemNames.EditNewsComment,
                Enabled = true,
                Name = "Edit a news comment"
            });
        }

        if (!existingActivityLogTypes.Any(alt => string.Equals(alt.SystemKeyword, NewsDefaults.ActivityLogTypeSystemNames.PublicStoreAddNewsComment, StringComparison.InvariantCultureIgnoreCase)))
        {
            await _activityLogTypeRepository.InsertAsync(new ActivityLogType
            {
                SystemKeyword = NewsDefaults.ActivityLogTypeSystemNames.PublicStoreAddNewsComment,
                Enabled = false,
                Name = "Public store. Add news comment"
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

                _permissionMappingRepository.Delete(pr => pr.PermissionRecordId == oldRec.Id);
                _permissionRepository.Delete(pr => pr.Id == oldRec.Id);
            }
        }
    }

    /// <summary>
    /// Update permission record names
    /// </summary>
    /// <returns></returns>
    private async Task PreparePermissionMappingsAsync()
    {
        await UpdatePermissionMappingsAsync("ContentManagement.NewsView", NewsDefaults.Permissions.NEWS_VIEW);
        await UpdatePermissionMappingsAsync("ContentManagement.NewsCreateEditDelete", NewsDefaults.Permissions.NEWS_MANAGE);
        await UpdatePermissionMappingsAsync("ContentManagement.NewsCommentsView", NewsDefaults.Permissions.NEWS_COMMENTS_VIEW);
        await UpdatePermissionMappingsAsync("ContentManagement.NewsCommentsCreateEditDelete", NewsDefaults.Permissions.NEWS_COMMENTS_MANAGE);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds the necessary data for the plugin to work correctly
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InstallRequiredDataAsync()
    {
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
        await _settingService.DeleteSettingAsync<NewsSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.News.");
        await _localizationService.DeleteLocaleResourcesAsync("Security.Permission.News.");
        await _localizationService.DeleteLocaleResourcesAsync("Admin.ContentManagement.MessageTemplates.Description.News.NewsComment");

        //message template
        foreach (var mt in await _messageTemplateService.GetMessageTemplatesByNameAsync(NewsDefaults.NewsCommentStoreOwnerNotification))
            await _messageTemplateService.DeleteMessageTemplateAsync(mt);

        //activity log type
        _activityLogTypeRepository.Delete(at => at.SystemKeyword == NewsDefaults.ActivityLogTypeSystemNames.AddNewNews);
        _activityLogTypeRepository.Delete(at => at.SystemKeyword == NewsDefaults.ActivityLogTypeSystemNames.DeleteNews);
        _activityLogTypeRepository.Delete(at => at.SystemKeyword == NewsDefaults.ActivityLogTypeSystemNames.EditNews);
        _activityLogTypeRepository.Delete(at => at.SystemKeyword == NewsDefaults.ActivityLogTypeSystemNames.DeleteNewsComment);
        _activityLogTypeRepository.Delete(at => at.SystemKeyword == NewsDefaults.ActivityLogTypeSystemNames.EditNewsComment);
        _activityLogTypeRepository.Delete(at => at.SystemKeyword == NewsDefaults.ActivityLogTypeSystemNames.PublicStoreAddNewsComment);

        //permission
        await _permissionRepository.DeleteAsync(record => record.SystemName == NewsDefaults.Permissions.NEWS_VIEW
            || record.SystemName == NewsDefaults.Permissions.NEWS_MANAGE
            || record.SystemName == NewsDefaults.Permissions.NEWS_COMMENTS_VIEW
            || record.SystemName == NewsDefaults.Permissions.NEWS_COMMENTS_MANAGE);

    }

    /// <summary>
    /// Install sample news
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InstallSampleDataAsync()
    {
        var language = await _workContext.GetWorkingLanguageAsync();

        var adminRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName);
        var customer = (await _customerService.GetAllCustomersAsync(isActive: true, customerRoleIds: [adminRole.Id])).FirstOrDefault();

        var aboutNewsItem = new NewsItem
        {
            Title = "About nopCommerce",
            Short = "It's stable and highly usable. From downloads to documentation, www.nopCommerce.com offers a comprehensive base of information, resources, and support to the nopCommerce community.",
            Full = "<p>For full feature list go to <a href=\"https://www.nopCommerce.com\">nopCommerce.com</a></p><p>Providing outstanding custom search engine optimization, web development services and e-commerce development solutions to our clients at a fair price in a professional manner.</p>",
            LanguageId = language.Id,
            Published = true,
            AllowComments = true,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _newsService.InsertNewsAsync(aboutNewsItem);

        //search engine name
        var seName = await _urlRecordService.ValidateSeNameAsync(aboutNewsItem, null, aboutNewsItem.Title, true);
        await _urlRecordService.SaveSlugAsync(aboutNewsItem, seName, aboutNewsItem.LanguageId);

        await _newsService.InsertNewsCommentAsync(new()
        {
            NewsItemId = aboutNewsItem.Id,
            CustomerId = customer.Id,
            CommentTitle = "Sample comment title",
            CommentText = "This is a sample comment...",
            IsApproved = true,
            StoreId = customer.RegisteredInStoreId,
            CreatedOnUtc = DateTime.UtcNow
        });

        var releaseNewsItem = new NewsItem
        {
            Title = "nopCommerce new release!",
            Short = "nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included! nopCommerce is a fully customizable shopping cart",
            Full = "<p>nopCommerce includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!</p>",
            LanguageId = language.Id,
            Published = true,
            AllowComments = true,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _newsService.InsertNewsAsync(releaseNewsItem);

        //search engine name
        var releaseNewsItemSeName = await _urlRecordService.ValidateSeNameAsync(releaseNewsItem, null, releaseNewsItem.Title, true);
        await _urlRecordService.SaveSlugAsync(releaseNewsItem, releaseNewsItemSeName, releaseNewsItem.LanguageId);

        await _newsService.InsertNewsCommentAsync(new()
        {
            NewsItemId = releaseNewsItem.Id,
            CustomerId = customer.Id,
            CommentTitle = "Sample comment title",
            CommentText = "This is a sample comment...",
            IsApproved = true,
            StoreId = customer.RegisteredInStoreId,
            CreatedOnUtc = DateTime.UtcNow
        });

        var openingNewsItem = new NewsItem
        {
            Title = "New online store is open!",
            Short = "The new nopCommerce store is open now! We are very excited to offer our new range of products. We will be constantly adding to our range so please register on our site.",
            Full = "<p>Our online store is officially up and running. Stock up for the holiday season! We have a great selection of items. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new products.</p><p>All shipping is worldwide and will leave the same day an order is placed! Happy Shopping and spread the word!!</p>",
            LanguageId = language.Id,
            Published = true,
            AllowComments = true,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _newsService.InsertNewsAsync(openingNewsItem);

        //search engine name
        var openingNewsItemSeName = await _urlRecordService.ValidateSeNameAsync(openingNewsItem, null, openingNewsItem.Title, true);
        await _urlRecordService.SaveSlugAsync(openingNewsItem, openingNewsItemSeName, openingNewsItem.LanguageId);

        await _newsService.InsertNewsCommentAsync(new()
        {
            NewsItemId = openingNewsItem.Id,
            CustomerId = customer.Id,
            CommentTitle = "Sample comment title",
            CommentText = "This is a sample comment...",
            IsApproved = true,
            StoreId = customer.RegisteredInStoreId,
            CreatedOnUtc = DateTime.UtcNow
        });
    }

    #endregion
}