using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.CyberSource.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.MerchantId")]
        public string MerchantId { get; set; }
        public bool MerchantId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.KeyId")]
        public string KeyId { get; set; }
        public bool KeyId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.SecretKey")]
        [DataType(DataType.Password)]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.TokenizationEnabled")]
        public bool TokenizationEnabled { get; set; }
        public bool TokenizationEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.PaymentConnectionMethod")]
        public int PaymentConnectionMethodId { get; set; }
        public bool PaymentConnectionMethodId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.PayerAuthenticationEnabled")]
        public bool PayerAuthenticationEnabled { get; set; }
        public bool PayerAuthenticationEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.PayerAuthenticationRequired")]
        public bool PayerAuthenticationRequired { get; set; }
        public bool PayerAuthenticationRequired_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.TransactionType")]
        public int TransactionTypeId { get; set; }
        public bool TransactionTypeId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.CvvRequired")]
        public bool CvvRequired { get; set; }
        public bool CvvRequired_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.AvsActionType")]
        public int AvsActionTypeId { get; set; }
        public bool AvsActionTypeId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.CvnActionType")]
        public int CvnActionTypeId { get; set; }
        public bool CvnActionTypeId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.DecisionManagerEnabled")]
        public bool DecisionManagerEnabled { get; set; }
        public bool DecisionManagerEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.ConversionDetailReportingEnabled")]
        public bool ConversionDetailReportingEnabled { get; set; }

        [NopResourceDisplayName("Plugins.Payments.CyberSource.Fields.ConversionDetailReportingFrequency")]
        public int ConversionDetailReportingFrequency { get; set; }

        #endregion
    }
}