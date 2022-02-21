using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.PaytrIframe.Models
{
    public record ConfigurationModel : BaseNopModel, ILocalizedModel<ConfigurationModel.ConfigurationLocalizedModel>
    {
        public ConfigurationModel()
        {
            Locales = new List<ConfigurationLocalizedModel>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.PaymentInfo")]
        public string PaymentInfo { get; set; }
        public bool PaymentInfo_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.PaymentMethodDescription")]
        public string PaymentMethodDescription { get; set; }
        public bool PaymentMethodDescription_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.MerchantId")]
        public string MerchantId { get; set; }
        public bool MerchantId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.MerchantKey")]
        public string MerchantKey { get; set; }
        public bool MerchantKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.MerchantSalt")]
        public string MerchantSalt { get; set; }
        public bool MerchantSalt_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.Language")]
        public int Language { get; set; }
        public bool Language_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.Installment")]
        public int Installment { get; set; }
        public bool Installment_OverrideForStore { get; set; }
        public string InstallmentOptions { get; set; }
        public bool InstallmentOptions_OverrideForStore { get; set; }

        /// <summary>
        /// Widget
        /// </summary>

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTitle")]
        public string InstallmentTableTitle { get; set; }
        public bool InstallmentTableTitle_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.InstallmentTableToken")]
        public string InstallmentTableToken { get; set; }
        public bool InstallmentTableToken_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.InstallmentTableMax")]
        public int InstallmentTableMax { get; set; }
        public bool InstallmentTableMax_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.InstallmentTableAdvanced")]
        public int InstallmentTableAdvanced { get; set; }
        public bool InstallmentTableAdvanced_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTopDesc")]
        public string InstallmentTableTopDescription { get; set; }
        public bool InstallmentTableTopDescription_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.InstallmentTableBottomDesc")]
        public string InstallmentTableBottomDescription { get; set; }
        public bool InstallmentTableBottomDescription_OverrideForStore { get; set; }

        public IList<ConfigurationLocalizedModel> Locales { get; set; }

        #region Nested Class

        public partial class ConfigurationLocalizedModel : ILocalizedLocaleModel
        {
            public int LanguageId { get; set; }

            [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.PaymentInfo")]
            public string PaymentInfo { get; set; }

            [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.PaymentMethodDescription")]
            public string PaymentMethodDescription { get; set; }

            //widget

            [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTitle")]
            public string InstallmentTableTitle { get; set; }

            [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.InstallmentTableTopDesc")]
            public string InstallmentTableTopDescription { get; set; }

            [NopResourceDisplayName("Plugins.Payments.PaytrIframe.Fields.InstallmentTableBottomDesc")]
            public string InstallmentTableBottomDescription { get; set; }
        }

        #endregion
    }
}
