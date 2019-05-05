using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Qualpay.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public class ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            PaymentTransactionTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        public bool IsConfigured { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Fields.MerchantId")]
        public string MerchantId { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Fields.SecurityKey")]
        public string SecurityKey { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Fields.ProfileId")]
        public string ProfileId { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Fields.UseEmbeddedFields")]
        public bool UseEmbeddedFields { get; set; }
        public bool UseEmbeddedFields_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Fields.UseCustomerVault")]
        public bool UseCustomerVault { get; set; }
        public bool UseCustomerVault_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Fields.UseRecurringBilling")]
        public bool UseRecurringBilling { get; set; }
        public bool UseRecurringBilling_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Fields.PaymentTransactionType")]
        public int PaymentTransactionTypeId { get; set; }
        public bool PaymentTransactionTypeId_OverrideForStore { get; set; }
        public IList<SelectListItem> PaymentTransactionTypes { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Qualpay.Fields.MerchantEmail")]
        [DataType(DataType.EmailAddress)]
        public string MerchantEmail { get; set; }

        #endregion
    }
}