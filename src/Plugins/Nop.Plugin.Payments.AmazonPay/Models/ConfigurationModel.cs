using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.AmazonPay.Models;

public record ConfigurationModel : BaseNopModel
{
    #region Ctor

    public ConfigurationModel()
    {
        ButtonPlacement = new List<int>();
    }

    #endregion

    #region Credentials

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.SetCredentialsManually")]
    public bool SetCredentialsManually { get; set; }
    public bool SetCredentialsManually_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.PaymentRegion")]
    public int Region { get; set; }
    public bool Region_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.Payload")]
    public string Payload { get; set; }
    public bool Payload_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.PaymentRegion")]
    public int PaymentRegion { get; set; }
    public bool PaymentRegion_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.PrivateKey")]
    [DataType(DataType.Password)]
    [NoTrim]
    public string PrivateKey { get; set; }
    public bool PrivateKey_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.PublicKeyId")]
    public string PublicKeyId { get; set; }
    public bool PublicKeyId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.MerchantId")]
    public string MerchantId { get; set; }
    public bool MerchantId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.StoreId")]
    public string StoreId { get; set; }
    public bool StoreId_OverrideForStore { get; set; }

    #endregion

    #region Configuration

    public bool IsConnected { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.UseSandbox")]
    public bool UseSandbox { get; set; }
    public bool UseSandbox_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.PaymentType")]
    public int PaymentType { get; set; }
    public bool PaymentType_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.ButtonPlacement")]
    public IList<int> ButtonPlacement { get; set; }
    public bool ButtonPlacement_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.ButtonColor")]
    public int ButtonColor { get; set; }
    public bool ButtonColor_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.EnableLogging")]
    public bool EnableLogging { get; set; }
    public bool EnableLogging_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.AmazonPay.Settings.IpnUrl")]
    public string IpnUrl { get; set; }

    public bool HideCredentialsBlock { get; set; }
    public bool HideConfigurationBlock { get; set; }
    public int ActiveStoreScopeConfiguration { get; set; }

    #endregion
}