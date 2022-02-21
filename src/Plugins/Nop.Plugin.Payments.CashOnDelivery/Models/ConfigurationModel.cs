using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.CashOnDelivery.Models
{
    public record ConfigurationModel : BaseNopModel, ILocalizedModel<ConfigurationLocalizedModel>
    {
        public ConfigurationModel()
        {
            Locales = new List<ConfigurationLocalizedModel>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payment.CashOnDelivery.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.CashOnDelivery.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }

        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.CashOnDelivery.DescriptionText")]
        public string DescriptionText { get; set; }

        public bool DescriptionText_OverrideForStore { get; set; }

        public IList<ConfigurationLocalizedModel> Locales { get; set; }

        [NopResourceDisplayName("Plugins.Payment.CashOnDelivery.ShippableProductRequired")]
        public bool ShippableProductRequired { get; set; }

        public bool ShippableProductRequired_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.CashOnDelivery.SkipPaymentInfo")]
        public bool SkipPaymentInfo { get; set; }

        public bool SkipPaymentInfo_OverrideForStore { get; set; }
    }
}