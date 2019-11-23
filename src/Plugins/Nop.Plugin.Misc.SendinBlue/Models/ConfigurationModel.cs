using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.SendinBlue.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public class ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            AvailableLists = new List<SelectListItem>();
            AvailableSenders = new List<SelectListItem>();
            AvailableMessageTemplates = new List<SelectListItem>();
            MessageTemplateSearchModel = new SendinBlueMessageTemplateSearchModel();
            SmsSearchModel = new SmsSearchModel();
            AddSms = new SmsModel();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.ApiKey")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.List")]
        public int ListId { get; set; }
        public bool ListId_OverrideForStore { get; set; }
        public IList<SelectListItem> AvailableLists { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.SmtpKey")]
        public string SmtpKey { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.UseSmtp")]
        public bool UseSmtp { get; set; }
        public bool UseSmtp_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.Sender")]
        public string SenderId { get; set; }
        public bool SenderId_OverrideForStore { get; set; }
        public IList<SelectListItem> AvailableSenders { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.UseSmsNotifications")]
        public bool UseSmsNotifications { get; set; }
        public bool UseSmsNotifications_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.SmsSenderName")]
        public string SmsSenderName { get; set; }
        public bool SmsSenderName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.StoreOwnerPhoneNumber")]
        public string StoreOwnerPhoneNumber { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.CampaignList")]
        public int CampaignListId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.CampaignSenderName")]
        public string CampaignSenderName { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.CampaignText")]
        public string CampaignText { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.MaKey")]
        public string MarketingAutomationKey { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.UseMarketingAutomation")]
        public bool UseMarketingAutomation { get; set; }
        public bool UseMarketingAutomation_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.AccountInfo")]
        public string AccountInfo { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        public IList<SelectListItem> AvailableMessageTemplates { get; set; }

        public bool MarketingAutomationDisabled { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.Fields.TrackingScript")]
        public string TrackingScript { get; set; }

        public bool HideGeneralBlock { get; set; }

        public bool HideSynchronizationBlock { get; set; }

        public bool HideTransactionalBlock { get; set; }

        public bool HideSmsBlock { get; set; }

        public bool HideMarketingAutomationBlock { get; set; }

        public SendinBlueMessageTemplateSearchModel MessageTemplateSearchModel { get; set; }

        public SmsSearchModel SmsSearchModel { get; set; }

        public SmsModel AddSms { get; set; }

        #endregion
    }
}