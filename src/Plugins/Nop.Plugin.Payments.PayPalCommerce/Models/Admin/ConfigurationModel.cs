using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Admin;

/// <summary>
/// Represents the configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    #region Ctor

    public ConfigurationModel()
    {
        PaymentTypes = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    public bool IsConfigured { get; set; }

    public int ActiveStoreScopeConfiguration { get; set; }

    public string SandboxSignUpUrl { get; set; }

    public string LiveSignUpUrl { get; set; }

    public string MerchantGuid { get; set; }

    public MerchantModel MerchantModel { get; set; } = new();

    [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.MerchantId")]
    public string MerchantId { get; set; }
    public bool MerchantId_OverrideForStore { get; set; }

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
    [NoTrim]
    [DataType(DataType.Password)]
    public string SecretKey { get; set; }
    public bool SecretKey_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.PaymentType")]
    public int PaymentTypeId { get; set; }
    public bool PaymentTypeId_OverrideForStore { get; set; }
    public IList<SelectListItem> PaymentTypes { get; set; }

    [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.UseCardFields")]
    public bool UseCardFields { get; set; }
    public bool UseCardFields_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.CustomerAuthenticationRequired")]
    public bool CustomerAuthenticationRequired { get; set; }
    public bool CustomerAuthenticationRequired_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.UseApplePay")]
    public bool UseApplePay { get; set; }
    public bool UseApplePay_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.UseGooglePay")]
    public bool UseGooglePay { get; set; }
    public bool UseGooglePay_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.UseAlternativePayments")]
    public bool UseAlternativePayments { get; set; }
    public bool UseAlternativePayments_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.UseVault")]
    public bool UseVault { get; set; }
    public bool UseVault_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.UseShipmentTracking")]
    public bool UseShipmentTracking { get; set; }

    [NopResourceDisplayName("Plugins.Payments.PayPalCommerce.Fields.SkipOrderConfirmPage")]
    public bool SkipOrderConfirmPage { get; set; }
    public bool SkipOrderConfirmPage_OverrideForStore { get; set; }

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

    #endregion
}