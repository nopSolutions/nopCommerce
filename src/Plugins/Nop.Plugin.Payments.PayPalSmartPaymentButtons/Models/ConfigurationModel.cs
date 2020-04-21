using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public class ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            PaymentTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.ClientId")]
        public string ClientId { get; set; }
        public bool ClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.SecretKey")]
        [DataType(DataType.Password)]
        [NoTrim]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.PaymentType")]
        public int PaymentTypeId { get; set; }
        public bool PaymentTypeId_OverrideForStore { get; set; }
        public IList<SelectListItem> PaymentTypes { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayButtonsOnShoppingCart")]
        public bool DisplayButtonsOnShoppingCart { get; set; }
        public bool DisplayButtonsOnShoppingCart_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayButtonsOnProductDetails")]
        public bool DisplayButtonsOnProductDetails { get; set; }
        public bool DisplayButtonsOnProductDetails_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayLogoInHeaderLinks")]
        public bool DisplayLogoInHeaderLinks { get; set; }
        public bool DisplayLogoInHeaderLinks_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.LogoInHeaderLinks")]
        public string LogoInHeaderLinks { get; set; }
        public bool LogoInHeaderLinks_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.DisplayLogoInFooter")]
        public bool DisplayLogoInFooter { get; set; }
        public bool DisplayLogoInFooter_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalSmartPaymentButtons.Fields.LogoInFooter")]
        public string LogoInFooter { get; set; }
        public bool LogoInFooter_OverrideForStore { get; set; }

        #endregion
    }
}