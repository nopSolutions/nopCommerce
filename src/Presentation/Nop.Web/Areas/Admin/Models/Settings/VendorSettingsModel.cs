using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a vendor settings model
/// </summary>
public partial record VendorSettingsModel : BaseNopModel, ISettingsModel
{
    #region Ctor

    public VendorSettingsModel()
    {
        VendorAttributeSearchModel = new VendorAttributeSearchModel();
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Vendor.VendorsBlockItemsToDisplay")]
    public int VendorsBlockItemsToDisplay { get; set; }
    public bool VendorsBlockItemsToDisplay_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Vendor.ShowVendorOnProductDetailsPage")]
    public bool ShowVendorOnProductDetailsPage { get; set; }
    public bool ShowVendorOnProductDetailsPage_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Vendor.AllowCustomersToContactVendors")]
    public bool AllowCustomersToContactVendors { get; set; }
    public bool AllowCustomersToContactVendors_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Vendor.AllowCustomersToApplyForVendorAccount")]
    public bool AllowCustomersToApplyForVendorAccount { get; set; }
    public bool AllowCustomersToApplyForVendorAccount_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Vendor.TermsOfServiceEnabled")]
    public bool TermsOfServiceEnabled { get; set; }
    public bool TermsOfServiceEnabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Vendor.AllowSearchByVendor")]
    public bool AllowSearchByVendor { get; set; }
    public bool AllowSearchByVendor_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Vendor.AllowVendorsToEditInfo")]
    public bool AllowVendorsToEditInfo { get; set; }
    public bool AllowVendorsToEditInfo_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Vendor.NotifyStoreOwnerAboutVendorInformationChange")]
    public bool NotifyStoreOwnerAboutVendorInformationChange { get; set; }
    public bool NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Vendor.MaximumProductNumber")]
    public int MaximumProductNumber { get; set; }
    public bool MaximumProductNumber_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Vendor.AllowVendorsToImportProducts")]
    public bool AllowVendorsToImportProducts { get; set; }
    public bool AllowVendorsToImportProducts_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Vendor.ShowVendorOnOrderDetailsPage")]
    public bool ShowVendorOnOrderDetailsPage { get; set; }
    public bool ShowVendorOnOrderDetailsPage_OverrideForStore { get; set; }

    public VendorAttributeSearchModel VendorAttributeSearchModel { get; set; }

    #endregion
}