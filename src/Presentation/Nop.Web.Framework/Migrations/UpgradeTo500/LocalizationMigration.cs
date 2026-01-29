using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
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

        #region Delete locales

        this.DeleteLocaleResources(new List<string>
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
            "Sitemap.News",

            //#7368
            "Admin.ContentManagement.Polls.Polls",
            "Admin.ContentManagement.Polls.Added",
            "Admin.ContentManagement.Polls.AddNew",
            "Admin.ContentManagement.Polls.Answers",
            "Admin.ContentManagement.Polls.Answers.Fields.DisplayOrder",
            "Admin.ContentManagement.Polls.Answers.Fields.DisplayOrder.Hint",
            "Admin.ContentManagement.Polls.Answers.Fields.Name",
            "Admin.ContentManagement.Polls.Answers.Fields.Name.Hint",
            "Admin.ContentManagement.Polls.Answers.Fields.Name.Required",
            "Admin.ContentManagement.Polls.Answers.Fields.NumberOfVotes",
            "Admin.ContentManagement.Polls.Answers.SaveBeforeEdit",
            "Admin.ContentManagement.Polls.BackToList",
            "Admin.ContentManagement.Polls.Configuration.Enabled",
            "Admin.ContentManagement.Polls.Configuration.Enabled.Hint",
            "Admin.ContentManagement.Polls.Deleted",
            "Admin.ContentManagement.Polls.EditPollDetails",
            "Admin.ContentManagement.Polls.Fields.AllowGuestsToVote",
            "Admin.ContentManagement.Polls.Fields.AllowGuestsToVote.Hint",
            "Admin.ContentManagement.Polls.Fields.DisplayOrder",
            "Admin.ContentManagement.Polls.Fields.DisplayOrder.Hint",
            "Admin.ContentManagement.Polls.Fields.EndDate",
            "Admin.ContentManagement.Polls.Fields.EndDate.Hint",
            "Admin.ContentManagement.Polls.Fields.Language",
            "Admin.ContentManagement.Polls.Fields.Language.Hint",
            "Admin.ContentManagement.Polls.Fields.LimitedToStores",
            "Admin.ContentManagement.Polls.Fields.LimitedToStores.Hint",
            "Admin.ContentManagement.Polls.Fields.Name",
            "Admin.ContentManagement.Polls.Fields.Name.Hint",
            "Admin.ContentManagement.Polls.Fields.Name.Required",
            "Admin.ContentManagement.Polls.Fields.Published",
            "Admin.ContentManagement.Polls.Fields.Published.Hint",
            "Admin.ContentManagement.Polls.Fields.ShowOnHomepage",
            "Admin.ContentManagement.Polls.Fields.ShowOnHomepage.Hint",
            "Admin.ContentManagement.Polls.Fields.ShowInLeftSideColumn",
            "Admin.ContentManagement.Polls.Fields.ShowInLeftSideColumn.Hint",
            "Admin.ContentManagement.Polls.Fields.StartDate",
            "Admin.ContentManagement.Polls.Fields.StartDate.Hint",
            "Admin.ContentManagement.Polls.Info",
            "Admin.ContentManagement.Polls.List.SearchStore",
            "Admin.ContentManagement.Polls.List.SearchStore.Hint",
            "Admin.ContentManagement.Polls.Updated",
            "Admin.Documentation.Reference.Polls",
            "Polls.OnlyRegisteredUsersVote",
            "Polls.SelectAnswer",
            "Polls.Title",
            "Polls.TotalVotes",
            "Polls.Vote",
            "Polls.VotesResultLine",
            "Security.Permission.ContentManagement.PollsCreateEditDelete",
            "Security.Permission.ContentManagement.PollsView",

        });

        #endregion

        #region Rename locales

        this.RenameLocales(new Dictionary<string, string>());

        #endregion

        #region Add or update locales

        this.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            //#7898
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.LogRequests"] = "Log AI requests",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.LogRequests.Hint"] = "Check to enable logging of all requests to AI services.",

            //#7989
            ["Products.ProductHasBeenUpdatedInTheWishlist.Link"] = "The product has been updated in your <a href=\"{0}\">wishlist</a>",
            
            //#8021
            ["Admin.Catalog.Products.RelatedProducts.CyclicallyRelated"] = "Circular dependency is not allowed for required products (e.g. product A requires product B. And product B requires product A)",

            //#7906
            ["Wishlist.DuplicateName"] = "A wishlist with this name already exists.",

            //#7907
            ["Wishlist.RenameCustomWishlist"] = "Rename wishlist",

            //#7400
            ["Admin.Catalog.Manufacturers.Gpsr"] = "GPSR",
            ["Admin.Configuration.Settings.Catalog.BlockTitle.Gpsr"] = "GPSR",
            ["Admin.Configuration.Settings.Gpsr.Enabled"] = "Enabled",
            ["Admin.Configuration.Settings.Gpsr.Enabled.Hint"] = "Check to display GPSR information in public store. The General Product Safety Regulation (GPSR) is a European Union (EU) law that came into effect in December 2024. The GPSR regulates the advertising and sale of consumer products in the European Union",
            ["Admin.Catalog.Manufacturers.Gpsr.Information"] = "<p>The General Product Safety Regulation (GPSR) is a European Union (EU) law that came into effect in December 2024. The GPSR regulates the advertising and sale of consumer products in the European Union.</p><p>If you are in the EU, you have to fill in the manufacturer’s name, physical address and e-mail or other electronic contact information.</p><p>If the manufacturer is not located in the European Union, the EU Responsible Person and their contact details. The EU Responsible Person is a person or entity located in the European Union that the seller designates to act as a point of contact with the EU authorities.</p>",
            ["Admin.Catalog.Manufacturers.Fields.PhysicalAddress"] = "Manufacturer physical address",
            ["Admin.Catalog.Manufacturers.Fields.PhysicalAddress.Hint"] = "The physical address of the manufacturer.",
            ["Admin.Catalog.Manufacturers.Fields.ElectronicAddress"] = "Manufacturer electronic address",
            ["Admin.Catalog.Manufacturers.Fields.ElectronicAddress.Hint"] = "The e-mail or other electronic contact information.",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePerson"] = "Responsible person name",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePerson.Hint"] = "The name of the manufacturer's responsible person",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePersonPhysicalAddress"] = "Responsible person physical address",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePersonPhysicalAddress.Hint"] = "The physical address of the manufacturer's responsible person.",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePersonElectronicAddress"] = "Responsible person electronic address",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePersonElectronicAddress.Hint"] = "The e-mail or other electronic contact information of the manufacturer's responsible person.",
            ["Products.Manufacturers.PhysicalAddress"] = "Manufacturer physical address: {0}",
            ["Products.Manufacturers.ElectronicAddress"] = "Manufacturer electronic address: {0}",
            ["Products.Manufacturers.ResponsiblePerson"] = "Responsible person name: {0}",
            ["Products.Manufacturers.ResponsiblePersonPhysicalAddress"] = "Responsible person physical address: {0}",
            ["Products.Manufacturers.ResponsiblePersonElectronicAddress"] = "Responsible person electronic address: {0}",
        });

        #endregion
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}