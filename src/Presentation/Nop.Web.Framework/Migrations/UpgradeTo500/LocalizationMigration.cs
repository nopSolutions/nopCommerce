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
            "Sitemap.News"
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

            //#2430
            ["Admin.Customers.Customers.Fields.PhoneSmsVerified"] = "Is phone verified",
            ["Admin.Customers.Customers.Fields.PhoneSmsVerified.Hint"] = "Indicates whether the customer's phone number has been verified via SMS.",

            ["Admin.Configuration.Settings.CustomerUser.LoginByPhoneEnabled"] = "'Login by phone' enabled",
            ["Admin.Configuration.Settings.CustomerUser.LoginByPhoneEnabled.Hint"] = "Check if 'Login by phone' is enabled.",
            ["Admin.Configuration.Settings.CustomerUser.OtpTimeLife"] = "OTP code time to live",
            ["Admin.Configuration.Settings.CustomerUser.OtpTimeLife.Hint"] = "The time (in seconds) during which the OTP code is valid.",
            ["Admin.Configuration.Settings.CustomerUser.OtpCountAttemptsToSendCode"] = "OTP code send attempts",
            ["Admin.Configuration.Settings.CustomerUser.OtpCountAttemptsToSendCode.Hint"] = "The number of attempts to send the OTP code.",
            ["Admin.Configuration.Settings.CustomerUser.OtpTimeToRepeat"] = "OTP code resend time",
            ["Admin.Configuration.Settings.CustomerUser.OtpTimeToRepeat.Hint"] = "The time (in minutes) before the OTP code can be resent.",
            ["Admin.Configuration.Settings.CustomerUser.OtpLength"] = "OTP code length",
            ["Admin.Configuration.Settings.CustomerUser.OtpLength.Hint"] = "The length of the OTP code.",

            ["Admin.Configuration.Settings.CustomerUser.LoginByPhoneEnabled.Warning"] = "Warning - Login by Phone requires the following prerequisites: Phone numbers enabled, Phone numbers required, and Phone number validation enabled. Please configure these settings accordingly.",
            ["Account.Login.Fields.Phone"] = "Phone",
            ["Account.Login.EmailMode"] = "Login with email",
            ["Account.Login.PhoneMode"] = "Login with phone",

            // Phone verification
            ["PageTitle.ChengePhone"] = "Change phone number",
            ["PageTitle.RegisterOtp"] = "Phone number verification",
            ["PageTitle.LoginOtp"] = "Phone number verification",

            ["Account.IsAlreadyExistsVerifiedPhoneNumber"] = "A customer with the specified verified phone number already exists.",
            ["Account.Register.OtpRegisterSmsText"] = "We'll send you a code via SMS to complete registration.",
            ["Account.OtpPhoneVerification.OtpUpdatePhoneSmsText"] = "We'll send you a code via SMS to confirm changing your phone number.",
            ["Account.Login.OtpLoginSmsText"] = "We'll send you a code via SMS to confirm your login.",

            ["PhoneVerification.SendSms"] = "Send SMS",
            ["PhoneVerification.OtpCodeExpires"] = "Code expires in",
            ["PhoneVerification.Fields.OtpCode"] = "SMS code",
            ["PhoneVerification.Fields.Phone"] = "Phone",
            ["PhoneVerification.OtpCode.Required"] = "SMS code is required.",
            ["PhoneVerification.OtpCode.Error.SendError"] = "Failed to send SMS code. Please try again.",
            ["PhoneVerification.OtpCode.Error.NotRequested"] = "You have not requested an SMS code. Please request a code and try again.",
            ["PhoneVerification.OtpCode.Error.Expired"] = "The SMS code has expired.",
            ["PhoneVerification.OtpCode.Error.Invalid"] = "The SMS code you entered is invalid. Please try again.",
            ["PhoneVerification.OtpCode.Error.Verification"] = "SMS verification error.",

            //menu
            ["Admin.Configuration.Sms.Providers"] = "Sms providers",
            ["Admin.Configuration.Sms.Providers.BackToList"] = "back to sms provider list",
            ["Admin.Configuration.Sms.Providers.Configure"] = "Configure",
            ["Admin.Configuration.Sms.Providers.DownloadMorePlugins"] = "You can download more plugins in our <a href=\"{0}\" target=\"_blank\">marketplace</a>",
            ["Admin.Configuration.Sms.Providers.Fields.FriendlyName"] = "Friendly name",
            ["Admin.Configuration.Sms.Providers.Fields.IsPrimaryProvider"] = "Is primary provider",
            ["Admin.Configuration.Sms.Providers.Fields.MarkAsPrimaryProvider"] = "Mark as primary provider",
            ["Admin.Configuration.Sms.Providers.Fields.SystemName"] = "System name",
            ["Admin.Documentation.Reference.SmsProviders"] = "Learn more about <a target=\"_blank\" href=\"{0}\">sms providers</a>.",

            //customer info
            ["Account.CustomerInfo.VerifyPhoneNumber"] = "Verify phone number",
            ["Account.Fields.Phone.Status.NotVerified"] = "Phone number is not verified",
            
        });

        #endregion
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}