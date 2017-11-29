using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Payments.Worldpay.Models
{
    /// <summary>
    /// Represents the Worldpay configuration model
    /// </summary>
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.Worldpay.Fields.SecureNetId")]
        public string SecureNetId { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Worldpay.Fields.SecureKey")]
        [DataType(DataType.Password)]
        [NoTrim]
        public string SecureKey { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Worldpay.Fields.PublicKey")]
        public string PublicKey { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Worldpay.Fields.TransactionMode")]
        public int TransactionModeId { get; set; }
        public SelectList TransactionModes { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Worldpay.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Worldpay.Fields.ValidateAddress")]
        public bool ValidateAddress { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Worldpay.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Worldpay.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
    }
}