using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.PayPalCommerce.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            PaymentTypes = new List<SelectListItem>();
            OnboardingModel = new OnboardingModel();
        }

        #endregion

        #region Properties

        public bool IsConfigured { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.Email")]
        [EmailAddress]
        public string Email { get; set; }
        public bool Email_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.SetCredentialsManually")]
        public bool SetCredentialsManually { get; set; }
        public bool SetCredentialsManually_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.ClientId")]
        public string ClientId { get; set; }
        public bool ClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.SecretKey")]
        [DataType(DataType.Password)]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.PaymentType")]
        public int PaymentTypeId { get; set; }
        public bool PaymentTypeId_OverrideForStore { get; set; }
        public IList<SelectListItem> PaymentTypes { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.DisplayButtonsOnShoppingCart")]
        public bool DisplayButtonsOnShoppingCart { get; set; }
        public bool DisplayButtonsOnShoppingCart_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.DisplayButtonsOnProductDetails")]
        public bool DisplayButtonsOnProductDetails { get; set; }
        public bool DisplayButtonsOnProductDetails_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.DisplayLogoInHeaderLinks")]
        public bool DisplayLogoInHeaderLinks { get; set; }
        public bool DisplayLogoInHeaderLinks_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.LogoInHeaderLinks")]
        public string LogoInHeaderLinks { get; set; }
        public bool LogoInHeaderLinks_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.DisplayLogoInFooter")]
        public bool DisplayLogoInFooter { get; set; }
        public bool DisplayLogoInFooter_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.LogoInFooter")]
        public string LogoInFooter { get; set; }
        public bool LogoInFooter_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.DisplayPayLaterMessages")]
        public bool DisplayPayLaterMessages { get; set; }
        public bool DisplayPayLaterMessages_OverrideForStore { get; set; }

        public OnboardingModel OnboardingModel { get; set; }

        #endregion
    }
}