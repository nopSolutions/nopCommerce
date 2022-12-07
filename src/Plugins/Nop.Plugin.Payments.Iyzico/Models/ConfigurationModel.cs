using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.Iyzico.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.UseToPaymentPopup")]
        public bool UseToPaymentPopup { get; set; }
        public bool UseToPaymentPopup_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.ApiKey")]
        public string ApiKey { get; set; }
        public bool ApiKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.ApiSecret")]
        public string ApiSecret { get; set; }
        public bool ApiSecret_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.ApiUrl")]
        public string ApiUrl { get; set; }
        public bool ApiUrl_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.IsCardStorage")]
        public bool IsCardStorage { get; set; }
        public bool IsCardStorage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.PaymentSuccessUrl")]
        public string PaymentSuccessUrl { get; set; }
        public bool PaymentSuccessUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.PaymentErrorUrl")]
        public string PaymentErrorUrl { get; set; }
        public bool PaymentErrorUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.InstallmentNumbers")]
        public List<int> InstallmentNumbers { get; set; }
        public bool InstallmentNumbers_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.InstallmentNumber2")]
        public bool InstallmentNumber2 { get; set; }
        public bool InstallmentNumber2_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.InstallmentNumber3")]
        public bool InstallmentNumber3 { get; set; }
        public bool InstallmentNumber3_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.InstallmentNumber6")]
        public bool InstallmentNumber6 { get; set; }
        public bool InstallmentNumber6_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.InstallmentNumber9")]
        public bool InstallmentNumber9 { get; set; }
        public bool InstallmentNumber9_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.Iyzico.Fields.InstallmentNumber12")]
        public bool InstallmentNumber12 { get; set; }
        public bool InstallmentNumber12_OverrideForStore { get; set; }
    }
}