using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.News.Services;

public class NewsInstallService
{
    #region Fields

    private readonly EmailAccountSettings _emailAccountSettings;
    private readonly IEmailAccountService _emailAccountService;
    private readonly ILocalizationService _localizationService;
    private readonly IMessageTemplateService _messageTemplateService;
    private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
    private readonly IRepository<PermissionRecord> _permissionRepository;
    private readonly IRepository<PermissionRecordCustomerRoleMapping> _permissionMappingRepository;
    private readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public NewsInstallService(
        EmailAccountSettings emailAccountSettings,
        IEmailAccountService emailAccountService,
        ILocalizationService localizationService,
        IMessageTemplateService messageTemplateService,
        IRepository<ActivityLogType> activityLogTypeRepository,
        IRepository<PermissionRecord> permissionRepository,
        IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository,
        ISettingService settingService)
    {
        _emailAccountSettings = emailAccountSettings;
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
                //DisplayNewsFooterItem = true,
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
                await _settingService.DeleteSettingAsync(sitemapIncludeNews); //<- clear cache
            }

            var sitemapXmlIncludeNews = await _settingService.GetSettingAsync($"{nameof(SitemapSettings)}.SitemapXmlIncludeNews");
            if (sitemapXmlIncludeNews is not null)
            {
                newsSettings.SitemapXmlIncludeNews = CommonHelper.To<bool>(sitemapXmlIncludeNews.Value);
                await _settingService.SaveSettingAsync(newsSettings, settings => settings.SitemapXmlIncludeNews, clearCache: false);
                await _settingService.DeleteSettingAsync(sitemapXmlIncludeNews); //<- clear cache
            }

            var showCaptchaOnNewsCommentPage = await _settingService.GetSettingAsync($"{nameof(CaptchaSettings)}.ShowOnNewsCommentPage");
            if (showCaptchaOnNewsCommentPage is not null)
            {
                newsSettings.ShowCaptchaOnNewsCommentPage = CommonHelper.To<bool>(showCaptchaOnNewsCommentPage.Value);
                ;
                await _settingService.SaveSettingAsync(newsSettings, settings => settings.ShowCaptchaOnNewsCommentPage, clearCache: false);
                await _settingService.DeleteSettingAsync(showCaptchaOnNewsCommentPage); //<- clear cache
            }
        }
    }

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
            //["Plugins.Misc.News.Configuration.DisplayNewsFooterItem"] = "Display \"News\"",
            //["Plugins.Misc.News.Configuration.DisplayNewsFooterItem.Hint"] = "Check if \"News\" menu item should be displayed in the footer. News functionality should be also enabled in this case.",
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
            ["Plugins.Misc.News.Comments"] = "Comments",
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
            ["Plugins.Misc.News.NewsArchive"] = "News",
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

    private async Task InsertMessageTemplatesAsync()
    {
        if (await _messageTemplateService.GetMessageTemplatesByNameAsync(NewsDefaults.NewsCommentStoreOwnerNotification) is not null)
            return;

        var eaGeneral = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId) ??
                (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();

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

    private Task InserActivityLogTypesAsync()
    {
        return _activityLogTypeRepository.InsertAsync(new ActivityLogType()
        {
            SystemKeyword = NewsDefaults.ActivityLogTypeSystemName,
            Enabled = false,
            Name = "Public store. Add news comment"
        });
    }

    private async Task UpdatePermissionMappingsAsync(string oldSystemName, string newSystemName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(oldSystemName);
        ArgumentNullException.ThrowIfNullOrEmpty(newSystemName);

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
    public virtual async Task InstallRequiredDataAsync()
    {
        await InsertSettingsAsync();

        await InsertLocalesAsync();

        await InsertMessageTemplatesAsync();

        await InserActivityLogTypesAsync();

        await PreparePermissionMappingsAsync();
    }

    /// <summary>
    /// Removes the data inserted in <see cref="InstallRequiredDataAsync"/>
    /// </summary>
    public virtual async Task UnInstallRequiredDataAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<NewsSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.News.");
        await _localizationService.DeleteLocaleResourcesAsync("Admin.ContentManagement.MessageTemplates.Description.News.NewsComment");

        //message template
        if (await _messageTemplateService.GetMessageTemplatesByNameAsync(NewsDefaults.NewsCommentStoreOwnerNotification) is MessageTemplate mt)
            await _messageTemplateService.DeleteMessageTemplateAsync(mt);

        //activity log type
        _activityLogTypeRepository.Delete(at => at.SystemKeyword == NewsDefaults.ActivityLogTypeSystemName);
    }

    #endregion
}
