using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

[NopUpdateMigration("2025-10-29 00:00:00", "5.00", UpdateMigrationType.Localization)]
public class LocalizationMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        var (languageId, _) = this.GetLanguageData();

        #region Delete locales

        localizationService.DeleteLocaleResources(new List<string>
        {
            //#7340
            "ActivityLog.AddNewNews",
            "ActivityLog.DeleteNews",
            "ActivityLog.DeleteNewsComment",
            "ActivityLog.EditNews",
            "ActivityLog.EditNewsComment",
            "ActivityLog.PublicStore.AddNewsComment",
            "Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsCommentPage",
            "Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsCommentPage.Hint",
            "Admin.Configuration.Settings.GeneralCommon.SitemapIncludeNews",
            "Admin.Configuration.Settings.GeneralCommon.SitemapIncludeNews.Hint",
            "Admin.Configuration.Settings.News",
            "Admin.Configuration.Settings.News.AllowNotRegisteredUsersToLeaveComments",
            "Admin.Configuration.Settings.News.AllowNotRegisteredUsersToLeaveComments.Hint",
            "Admin.Configuration.Settings.News.BlockTitle.Common",
            "Admin.Configuration.Settings.News.BlockTitle.NewsComments",
            "Admin.Configuration.Settings.News.Enabled",
            "Admin.Configuration.Settings.News.Enabled.Hint",
            "Admin.Configuration.Settings.News.MainPageNewsCount",
            "Admin.Configuration.Settings.News.MainPageNewsCount.Hint",
            "Admin.Configuration.Settings.News.NewsArchivePageSize",
            "Admin.Configuration.Settings.News.NewsArchivePageSize.Hint",
            "Admin.Configuration.Settings.News.NewsCommentsMustBeApproved",
            "Admin.Configuration.Settings.News.NewsCommentsMustBeApproved.Hint",
            "Admin.Configuration.Settings.News.NotifyAboutNewNewsComments",
            "Admin.Configuration.Settings.News.NotifyAboutNewNewsComments.Hint",
            "Admin.Configuration.Settings.News.ShowHeaderRSSUrl",
            "Admin.Configuration.Settings.News.ShowHeaderRSSUrl.Hint",
            "Admin.Configuration.Settings.News.ShowNewsCommentsPerStore",
            "Admin.Configuration.Settings.News.ShowNewsCommentsPerStore.Hint",
            "Admin.Configuration.Settings.News.ShowNewsOnMainPage",
            "Admin.Configuration.Settings.News.ShowNewsOnMainPage.Hint",
            "Admin.ContentManagement.MessageTemplates.Description.News.NewsComment",
            "Admin.ContentManagement.News",
            "Admin.ContentManagement.News.Comments",
            "Admin.ContentManagement.News.Comments.ApproveSelected",
            "Admin.ContentManagement.News.Comments.DeleteSelected",
            "Admin.ContentManagement.News.Comments.DisapproveSelected",
            "Admin.ContentManagement.News.Comments.Fields.CommentText",
            "Admin.ContentManagement.News.Comments.Fields.CommentTitle",
            "Admin.ContentManagement.News.Comments.Fields.CreatedOn",
            "Admin.ContentManagement.News.Comments.Fields.Customer",
            "Admin.ContentManagement.News.Comments.Fields.IsApproved",
            "Admin.ContentManagement.News.Comments.Fields.NewsItem",
            "Admin.ContentManagement.News.Comments.Fields.StoreName",
            "Admin.ContentManagement.News.Comments.List.CreatedOnFrom",
            "Admin.ContentManagement.News.Comments.List.CreatedOnFrom.Hint",
            "Admin.ContentManagement.News.Comments.List.CreatedOnTo",
            "Admin.ContentManagement.News.Comments.List.CreatedOnTo.Hint",
            "Admin.ContentManagement.News.Comments.List.SearchApproved",
            "Admin.ContentManagement.News.Comments.List.SearchApproved.All",
            "Admin.ContentManagement.News.Comments.List.SearchApproved.ApprovedOnly",
            "Admin.ContentManagement.News.Comments.List.SearchApproved.DisapprovedOnly",
            "Admin.ContentManagement.News.Comments.List.SearchApproved.Hint",
            "Admin.ContentManagement.News.Comments.List.SearchText",
            "Admin.ContentManagement.News.Comments.List.SearchText.Hint",
            "Admin.ContentManagement.News.NewsItems",
            "Admin.ContentManagement.News.NewsItems.Added",
            "Admin.ContentManagement.News.NewsItems.AddNew",
            "Admin.ContentManagement.News.NewsItems.BackToList",
            "Admin.ContentManagement.News.NewsItems.Deleted",
            "Admin.ContentManagement.News.NewsItems.EditNewsItemDetails",
            "Admin.ContentManagement.News.NewsItems.Fields.AllowComments",
            "Admin.ContentManagement.News.NewsItems.Fields.AllowComments.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.Comments",
            "Admin.ContentManagement.News.NewsItems.Fields.CreatedOn",
            "Admin.ContentManagement.News.NewsItems.Fields.EndDate",
            "Admin.ContentManagement.News.NewsItems.Fields.EndDate.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.Full",
            "Admin.ContentManagement.News.NewsItems.Fields.Full.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.Full.Required",
            "Admin.ContentManagement.News.NewsItems.Fields.Language",
            "Admin.ContentManagement.News.NewsItems.Fields.Language.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.LimitedToStores",
            "Admin.ContentManagement.News.NewsItems.Fields.LimitedToStores.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.MetaDescription",
            "Admin.ContentManagement.News.NewsItems.Fields.MetaDescription.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.MetaKeywords",
            "Admin.ContentManagement.News.NewsItems.Fields.MetaKeywords.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.MetaTitle",
            "Admin.ContentManagement.News.NewsItems.Fields.MetaTitle.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.Published",
            "Admin.ContentManagement.News.NewsItems.Fields.Published.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.SeName",
            "Admin.ContentManagement.News.NewsItems.Fields.SeName.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.Short",
            "Admin.ContentManagement.News.NewsItems.Fields.Short.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.Short.Required",
            "Admin.ContentManagement.News.NewsItems.Fields.StartDate",
            "Admin.ContentManagement.News.NewsItems.Fields.StartDate.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.Title",
            "Admin.ContentManagement.News.NewsItems.Fields.Title.Hint",
            "Admin.ContentManagement.News.NewsItems.Fields.Title.Required",
            "Admin.ContentManagement.News.NewsItems.Info",
            "Admin.ContentManagement.News.NewsItems.List.SearchStore",
            "Admin.ContentManagement.News.NewsItems.List.SearchStore.Hint",
            "Admin.ContentManagement.News.NewsItems.List.SearchTitle",
            "Admin.ContentManagement.News.NewsItems.List.SearchTitle.Hint",
            "Admin.ContentManagement.News.NewsItems.Updated",
            "Admin.Documentation.Reference.News",
            "News",
            "News.Comments",
            "News.Comments.CommentText",
            "News.Comments.CommentText.Required",
            "News.Comments.CommentTitle",
            "News.Comments.CommentTitle.MaxLengthValidation",
            "News.Comments.CommentTitle.Required",
            "News.Comments.CreatedOn",
            "News.Comments.LeaveYourComment",
            "News.Comments.OnlyRegisteredUsersLeaveComments",
            "News.Comments.SeeAfterApproving",
            "News.Comments.SubmitButton",
            "News.Comments.SuccessfullyAdded",
            "News.MoreInfo",
            "News.RSS",
            "News.RSS.Hint",
            "News.ViewAll",
            "PageTitle.NewsArchive",
            "Security.Permission.ContentManagement.NewsCommentsCreateEditDelete",
            "Security.Permission.ContentManagement.NewsCommentsView",
            "Security.Permission.ContentManagement.NewsCreateEditDelete",
            "Security.Permission.ContentManagement.NewsView",
            "Sitemap.News"
        });

        #endregion

        #region Rename locales

        #endregion

        #region Add or update locales

        #endregion
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}